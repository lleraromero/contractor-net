using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contractor.Utils;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public abstract class AssemblyXXX
    {
        public abstract ISet<string> Types();
        public abstract ISet<Action> Constructors(string type);
        public abstract ISet<Action> Actions(string type);
    }

    public class CciAssembly : AssemblyXXX
    {
        protected Module module;

        public CciAssembly(string fileName, string contractsFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName) && File.Exists(fileName));

            this.module = DecompileModule(fileName);
        }

        public override ISet<string> Types()
        {
            var types = this.module.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            return new HashSet<string>(from t in types
                                       select TypeHelper.GetTypeName(t, NameFormattingOptions.None));
        }

        public override ISet<Action> Constructors(string typeName)
        {
            var type = FindType(typeName);
            return new HashSet<Action>(from m in type.Methods
                                                        where m.IsConstructor
                                                        select new CciAction(m));
        }

        public override ISet<Action> Actions(string typeName)
        {
            var type = FindType(typeName);
            return new HashSet<Action>(from m in type.Methods
                                                        where !m.IsConstructor
                                                        select new CciAction(m));
        }

        private NamespaceTypeDefinition FindType(string typeName)
        {
            var types = this.module.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.None).Equals(typeName));
            return type;
        }

        private Module DecompileModule(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename) && File.Exists(filename));

            var host = new CodeContractAwareHostEnvironment(true);
            var module = LoadModule(filename, host);
            Module decompiledModule;
            using (var pdbReader = GetPDBReader(module, host))
            {
                decompiledModule = Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader);
            }
            return decompiledModule;
        }

        private IModule LoadModule(string filename, IContractAwareHost host)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename) && File.Exists(filename));
            Contract.Requires(host != null);

            var module = host.LoadUnitFrom(filename) as IModule;
            if (module == null || module is Dummy || module == Dummy.Assembly)
            {
                throw new Exception(string.Concat(filename, " is not a PE file containing a CLR module or assembly."));
            }
            return module;
        }

        private PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null);
            Contract.Requires(host != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
            {
                using (var pdbStream = File.OpenRead(pdbFile))
                {
                    pdbReader = new PdbReader(pdbStream, host);
                }
            }
            return pdbReader;
        }
    }
}
