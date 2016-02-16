using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analysis.Cci
{
    public class CciQueryAssembly : CciAssembly
    {
        public CciQueryAssembly(CciAssembly inputAssembly, ITypeDefinition typeToAnalyze, IReadOnlyCollection<Query> queries)
            : base(inputAssembly.Module, inputAssembly.ContractProvider)
        {
            Contract.Requires(inputAssembly != null);
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(queries.Any());

            // Clone module
            var host = CciHostEnvironment.GetInstance();
            module = new CodeAndContractDeepCopier(host).Copy(inputAssembly.Module);
            var cciQueryType = FindType(module, typeToAnalyze.Name);
            Contract.Assert(cciQueryType != null);

            // Create contract provider for the cloned module
            contractProvider = new ContractProvider(new ContractMethods(host), host.FindUnit(module.UnitIdentity));

            var cciInputType = FindType(inputAssembly.Module, typeToAnalyze.Name);
            Contract.Assert(cciInputType != null);
            var queryTypeContract = inputAssembly.ContractProvider.GetTypeContractFor(cciInputType);

            contractProvider.AssociateTypeWithContract(cciQueryType, queryTypeContract);

            // Add queries
            cciQueryType.Methods.AddRange(from a in queries select a.Method.Method);

            foreach (var query in queries)
            {
                // Find the query in the query assembly
                var method = cciQueryType.Methods.Find(m => m.GetUniqueName().Equals(query.Method.Name)) as MethodDefinition;
                Contract.Assert(method != null);

                method.ContainingTypeDefinition = cciQueryType;

                // Asociate query with its contract
                contractProvider.AssociateMethodWithContract(query.Method.Method, query.Method.Contract);
            }
        }

        protected NamedTypeDefinition FindType(Module module, string typeName)
        {
            Contract.Ensures(Contract.Result<NamedTypeDefinition>() != null);
            var types = GetAnalyzableTypes(module);
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.UseGenericTypeNameSuffix).Equals(typeName));
            return type as NamedTypeDefinition;
        }
    }
}