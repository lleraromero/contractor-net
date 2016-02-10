using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Contractor.Core;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analysis.Cci
{
    public class CciQueryAssembly : CciAssembly
    {
        public CciQueryAssembly(CciAssembly inputAssembly, ITypeDefinition typeToAnalyze, IEnumerable<Query> queries)
            : base(inputAssembly)
        {
            var cciType = FindType(typeToAnalyze.Name);
            cciType.Methods.AddRange(from a in queries select a.Method.Method);

            var newContractProvider = new ContractProvider(new ContractMethods(host), host.FindUnit(decompiledModule.UnitIdentity));
            newContractProvider.AssociateTypeWithContract(cciType, contractProvider.GetTypeContractFor(cciType));
            contractProvider = newContractProvider;

            foreach (var query in queries)
            {
                var method = cciType.Methods.Find(m => m.GetUniqueName() == query.Method.Name) as MethodDefinition;
                method.ContainingTypeDefinition = cciType;
                ((ContractProvider) contractProvider).AssociateMethodWithContract(query.Method.Method, query.Method.Contract);
            }
        }

        public void Save(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path) && !File.Exists(path));

            ISourceLocationProvider sourceLocationProvider = GetPdbReader(decompiledModule);
            ContractHelper.InjectContractCalls(host, decompiledModule, (ContractProvider) contractProvider, sourceLocationProvider);

            // Save the query assembly to run Corral
            using (var peStream = File.Create(path))
            {
                PeWriter.WritePeToStream(decompiledModule, host, peStream);
            }
        }

        protected NamespaceTypeDefinition FindType(string typeName)
        {
            var types = decompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.UseGenericTypeNameSuffix).Equals(typeName));
            return type;
        }
    }
}