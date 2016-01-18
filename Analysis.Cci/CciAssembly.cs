using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;

namespace Analysis.Cci
{
    public class CciAssembly : IAssemblyXXX
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
            var namespaces = new Dictionary<string, List<TypeDefinition>>();
            foreach (var type in decompiledModule.AllTypes)
            {
                var typeNamespace = FindNamespace(type);

                if (!namespaces.ContainsKey(typeNamespace))
                {
                    namespaces.Add(typeNamespace, new List<TypeDefinition>());
                }

                namespaces[typeNamespace].Add(new CciTypeDefinition(type, contractProvider));
            }

            return new ReadOnlyCollection<NamespaceDefinition>(namespaces.Select(kvp => new NamespaceDefinition(kvp.Key, kvp.Value)).ToList());
        }

        public IReadOnlyCollection<TypeDefinition> Types()
        {
            return
                new ReadOnlyCollection<TypeDefinition>(Namespaces()
                    .Aggregate(new List<TypeDefinition>(), (l, n) => new List<TypeDefinition>(l.Union(n.Types()))));
        }

        public IMethodContract GetContractFor(IMethodDefinition method)
        {
            return contractProvider.GetMethodContractFor(method);
        }

        /// <remarks>
        ///     There are only two kind of INamedTypeDefinitions: Namespace and Nested
        /// </remarks>
        protected string FindNamespace(INamedTypeDefinition typeDefinition)
        {
            ITypeDefinition currentType = typeDefinition;
            while (currentType is INestedTypeDefinition)
            {
                currentType = ((INestedTypeDefinition) typeDefinition).ContainingTypeDefinition;
            }

            return ((INamespaceTypeDefinition) currentType).ContainingUnitNamespace.Name.Value;
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
    }
}