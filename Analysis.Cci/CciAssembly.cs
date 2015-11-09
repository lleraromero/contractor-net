using Contractor.Core;
using Contractor.Core.Model;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Action = Contractor.Core.Model.Action;

namespace Analysis.Cci
{
    public class CciAssembly : IAssemblyXXX
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

        protected CciAssembly(CciAssembly anotherAssembly)
        {
            this.host = anotherAssembly.host;
            this.module = new CodeAndContractDeepCopier(this.host).Copy(anotherAssembly.module);
            var contractExtractor = this.host.GetContractExtractor(this.module.UnitIdentity);
            this.contractProvider = new AggregatingContractProvider(contractExtractor);
        }

        public IReadOnlyCollection<NamespaceDefinition> Namespaces()
        {
            //TODO: arreglar
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<TypeDefinition> Types()
        {
            var types = module.GetAnalyzableTypes();
            return new ReadOnlyCollection<TypeDefinition>((from t in types select (TypeDefinition) new CciTypeDefinition(t, contractProvider)).ToList());
        }

        public ISet<Action> Constructors(TypeDefinition type)
        {
            var cciType = FindType(type.Name);
            return new HashSet<Action>(from m in cciType.Methods
                                       where m.IsConstructor
                                       select new CciAction(m, this.contractProvider.GetMethodContractFor(m)));
        }

        public ISet<Action> Actions(TypeDefinition type)
        {
            var cciType = FindType(type.Name);
            return new HashSet<Action>(from m in cciType.Methods
                                       where !m.IsConstructor && m.Visibility == TypeMemberVisibility.Public && 
                                       !m.IsStatic && !m.IsStaticConstructor
                                       select new CciAction(m, this.contractProvider.GetMethodContractFor(m)));
        }

        public IMethodContract GetContractFor(IMethodDefinition method)
        {
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
