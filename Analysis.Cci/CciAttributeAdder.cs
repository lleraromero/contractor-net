using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;
using MethodReference = Microsoft.Cci.MethodReference;

namespace Analysis.Cci
{
    // TODO (lleraromero): Sacar la herencia (poner una interfaz?). Reestructurar los QueryAssemblies.
    public class CciAttributeAdder : CciQueryAssembly
    {
        public CciAttributeAdder(CciAssembly inputAssembly, ITypeDefinition typeToAnalyze, IEnumerable<Query> queries)
            : base(inputAssembly, typeToAnalyze, queries)
        {
            Contract.Requires(inputAssembly != null);
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(queries.Any());

            AddVerifierAttribute(typeToAnalyze, queries);
        }

        protected void AddVerifierAttribute(ITypeDefinition typeToAnalyze, IEnumerable<Query> queries)
        {
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(queries.Any());

            var cciType = FindType(module, typeToAnalyze.Name);
            foreach (var m in cciType.Methods)
            {
                if (m.Visibility != TypeMemberVisibility.Public || queries.Any(query => query.Method.Method.Equals(m)))
                {
                    continue;
                }

                var disableVerifier = DisableVerifier();

                var tmp = m as MethodDefinition;
                Contract.Assert(tmp != null);

                if (tmp.Attributes == null)
                {
                    tmp.Attributes = new List<ICustomAttribute>();
                }
                tmp.Attributes.Add(disableVerifier);
            }
        }

        protected CustomAttribute DisableVerifier()
        {
            var disableVerifier = new CustomAttribute
            {
                Arguments = new List<IMetadataExpression> { new MetadataConstant { Value = false, Type = host.PlatformType.SystemBoolean } },
                Constructor =
                    new MethodReference(host,
                        host.PlatformType.SystemDiagnosticsContractsContract.ResolvedType.ContainingNamespace.GetMembersNamed(
                            host.NameTable.GetNameFor("ContractVerificationAttribute"), false).First() as INamespaceTypeReference,
                        CallingConvention.HasThis,
                        host.PlatformType.SystemVoid, host.NameTable.GetNameFor(".ctor"), 0, host.PlatformType.SystemBoolean)
            };
            return disableVerifier;
        }
    }
}