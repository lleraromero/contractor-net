using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using IAssembly = Contractor.Core.Model.IAssembly;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analysis.Cci
{
    public class CciAssembly : IAssembly
    {
        protected Module module;
        protected ContractProvider contractProvider;

        public CciAssembly(Module module, ContractProvider contractProvider)
        {
            Contract.Requires(module != null);
            Contract.Requires(contractProvider != null);

            this.module = module;
            this.contractProvider = contractProvider;
        }

        public Module Module
        {
            get { return module; }
        }

        public ContractProvider ContractProvider
        {
            get { return contractProvider; }
        }

        public IReadOnlyCollection<NamespaceDefinition> Namespaces()
        {
            var namespaces = new Dictionary<string, List<ITypeDefinition>>();
            var analyzableTypes = GetAnalyzableTypes(module);
            foreach (var type in analyzableTypes)
            {
                var typeNamespace = TypeHelper.GetDefiningNamespace(type).Name.Value;

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