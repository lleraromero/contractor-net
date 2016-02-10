using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using IAssembly = Contractor.Core.Model.IAssembly;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analysis.Cci
{
    public class CciAssembly : IAssembly
    {
        protected IContractProvider contractProvider;
        protected Module decompiledModule;
        protected IContractAwareHost host;

        public CciAssembly(string fileName, string contractsFileName, IContractAwareHost host)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName) && File.Exists(fileName));

            this.host = host;
            decompiledModule = new CodeAndContractDeepCopier(this.host).Copy(DecompileModule(fileName));
            var contractExtractor = this.host.GetContractExtractor(decompiledModule.UnitIdentity);
            contractProvider = new AggregatingContractProvider(contractExtractor);
        }

        protected CciAssembly(CciAssembly anotherAssembly)
        {
            host = anotherAssembly.host;
            decompiledModule = new CodeAndContractDeepCopier(host).Copy(anotherAssembly.decompiledModule);
            var contractExtractor = host.GetContractExtractor(decompiledModule.UnitIdentity);
            contractProvider = new AggregatingContractProvider(contractExtractor);
        }

        public IReadOnlyCollection<NamespaceDefinition> Namespaces()
        {
            var namespaces = new Dictionary<string, List<ITypeDefinition>>();
            var analyzableTypes = GetAnalyzableTypes(decompiledModule);
            foreach (var type in analyzableTypes)
            {
                var typeNamespace = TypeHelper.GetDefiningNamespace(type).Name.Value;
                ;

                if (!namespaces.ContainsKey(typeNamespace))
                {
                    namespaces.Add(typeNamespace, new List<ITypeDefinition>());
                }

                namespaces[typeNamespace].Add(new CciTypeDefinition(type, contractProvider));
            }

            return new ReadOnlyCollection<NamespaceDefinition>(namespaces.Select(kvp => new NamespaceDefinition(kvp.Key, kvp.Value)).ToList());
        }

        public IReadOnlyCollection<ITypeDefinition> Types()
        {
            return
                new ReadOnlyCollection<ITypeDefinition>(Namespaces()
                    .Aggregate(new List<ITypeDefinition>(), (l, n) => new List<ITypeDefinition>(l.Union(n.Types()))));
        }

        public IMethodContract GetContractFor(IMethodDefinition method)
        {
            return contractProvider.GetMethodContractFor(method);
        }

        protected Module DecompileModule(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename) && File.Exists(filename));

            var module = LoadModule(filename);

            using (var pdbReader = GetPdbReader(module))
            {
                return Decompiler.GetCodeModelFromMetadataModel(host, module, pdbReader);
            }
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

        protected PdbReader GetPdbReader(IModule module)
        {
            Contract.Requires(module != null);

            PdbReader pdbReader = null;
            var pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
            {
                using (var pdbStream = File.OpenRead(pdbFile))
                {
                    pdbReader = new PdbReader(pdbStream, host);
                }
            }
            return pdbReader;
        }

        protected IReadOnlyCollection<INamedTypeDefinition> GetAnalyzableTypes(IModule module)
        {
            var types = from t in module.GetAllTypes()
                where (t.IsClass || t.IsStruct) &&
                      !t.IsStatic &&
                      !t.IsEnum &&
                      !t.IsInterface
                select t;
            return new List<INamedTypeDefinition>(types);
        }
    }
}