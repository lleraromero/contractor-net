using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public abstract class AssemblyXXX
    {
        [Pure]
        public abstract ISet<string> Types();
        [Pure]
        public abstract ISet<Action> Constructors(string type);
        [Pure]
        public abstract ISet<Action> Actions(string type);
        [Pure]
        public abstract IMethodContract GetContractFor(IMethodDefinition method);
    }

    public class CciAssembly : AssemblyXXX
    {
        protected IContractAwareHost host;
        protected Module module;
        protected IContractProvider contractProvider;

        public CciAssembly(string fileName, string contractsFileName, IContractAwareHost host)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName) && File.Exists(fileName));

            this.host = host;
            this.module = new CodeAndContractDeepCopier(this.host).Copy(DecompileModule(fileName));
            var contractExtractor = this.host.GetContractExtractor(this.module.UnitIdentity);
            this.contractProvider = new AggregatingContractProvider(contractExtractor);
        }

        public override ISet<string> Types()
        {
            var types = this.module.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            return new HashSet<string>(from t in types
                                       select TypeHelper.GetTypeName(t, NameFormattingOptions.None));
        }

        public override ISet<Action> Constructors(string typeName)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeName));
            Contract.Requires(Types().Contains(typeName));

            var type = FindType(typeName);
            return new HashSet<Action>(from m in type.Methods
                                       where m.IsConstructor
                                       select new CciAction(m, this.contractProvider.GetMethodContractFor(m)));
        }

        public override ISet<Action> Actions(string typeName)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeName));
            Contract.Requires(Types().Contains(typeName));

            var type = FindType(typeName);
            return new HashSet<Action>(from m in type.Methods
                                       where !m.IsConstructor && m.Visibility == TypeMemberVisibility.Public && 
                                       !m.IsStatic && !m.IsStaticConstructor
                                       select new CciAction(m, this.contractProvider.GetMethodContractFor(m)));
        }

        public override IMethodContract GetContractFor(IMethodDefinition method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<IMethodContract>() != null);

            return this.contractProvider.GetMethodContractFor(method);
        }

        protected NamespaceTypeDefinition FindType(string typeName)
        {
            var types = this.module.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.None).Equals(typeName));
            return type;
        }

        protected Module DecompileModule(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename) && File.Exists(filename));

            var module = LoadModule(filename);
            Module decompiledModule;
            using (var pdbReader = GetPDBReader(module))
            {
                decompiledModule = Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader);
            }
            return decompiledModule;
        }

        protected IModule LoadModule(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename) && File.Exists(filename));

            var module = host.LoadUnitFrom(filename) as IModule;
            if (module == null || module is Dummy || module == Dummy.Assembly)
            {
                throw new Exception(string.Concat(filename, " is not a PE file containing a CLR module or assembly."));
            }
            return module;
        }

        protected PdbReader GetPDBReader(IModule module)
        {
            Contract.Requires(module != null);

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
