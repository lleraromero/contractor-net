using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Contractor.Utils;
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


            var newContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(this.decompiledModule.UnitIdentity));
            newContractProvider.AssociateTypeWithContract(cciType, this.contractProvider.GetTypeContractFor(cciType));
            this.contractProvider = newContractProvider;

            foreach (var query in queries)
            {
                var method = cciType.Methods.Find(m => m.GetUniqueName() == query.Method.Name) as MethodDefinition;
                method.ContainingTypeDefinition = cciType;
                ((ContractProvider)this.contractProvider).AssociateMethodWithContract(query.Method.Method, query.Method.Contract);
            }
        }

        public void Save(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path) && !File.Exists(path));

            ISourceLocationProvider sourceLocationProvider = GetPdbReader(this.decompiledModule);            
            ContractHelper.InjectContractCalls(this.host, this.decompiledModule, (ContractProvider) this.contractProvider, sourceLocationProvider);

            // Save the query assembly to run Corral
            using (var peStream = File.Create(path))
            {
                PeWriter.WritePeToStream(this.decompiledModule, this.host, peStream);
            }
        }

        protected NamespaceTypeDefinition FindType(string typeName)
        {
            var types = this.decompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.None).Equals(typeName));
            return type;
        }
    }
}
