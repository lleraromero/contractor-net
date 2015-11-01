using Analysis.Cci;
using Contractor.Core;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;

namespace Analyzer.Corral
{
    class CciQueryAssembly : CciAssembly
    {
        public CciQueryAssembly(CciAssembly inputAssembly, string typeToAnalyze, IEnumerable<Query> queries)
            : base(inputAssembly)
        {
            var cciType = FindType(typeToAnalyze);
            cciType.Methods.AddRange(from a in queries select a.Action.Method);


            var newContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(this.module.UnitIdentity));
            newContractProvider.AssociateTypeWithContract(cciType, this.contractProvider.GetTypeContractFor(cciType));
            this.contractProvider = newContractProvider;

            foreach (var query in queries)
            {
                var method = cciType.Methods.Find(m => m.GetUniqueName() == query.Action.Name) as MethodDefinition;
                method.ContainingTypeDefinition = cciType;
                (this.contractProvider as ContractProvider).AssociateMethodWithContract(query.Action.Method, query.Action.Contract);
            }
        }

        public void Save(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path) && !File.Exists(path));

            var pdbReader = GetPDBReader(this.module);
            // I need to replace Pre/Post with Assume/Assert
            ILocalScopeProvider localScopeProvider = new Microsoft.Cci.ILToCodeModel.Decompiler.LocalScopeProvider(pdbReader);
            ISourceLocationProvider sourceLocationProvider = pdbReader;
            var trans = new ContractRewriter(this.host, (ContractProvider)this.contractProvider, sourceLocationProvider);
            this.module = trans.Rewrite(this.module) as Module;

            pdbReader = GetPDBReader(this.module);
            // Save the query assembly to run Corral
            using (var peStream = File.Create(path))
            {
                if (GetPDBReader(this.module) == null)
                {
                    PeWriter.WritePeToStream(this.module, this.host, peStream);
                }
                else
                {
                    var pdbName = Path.ChangeExtension(path, "pdb");
                    using (var pdbWriter = new PdbWriter(pdbName, pdbReader))
                    {
                        PeWriter.WritePeToStream(this.module, this.host, peStream, pdbReader, pdbReader, pdbWriter);
                    }
                }
            }
        }
    }
}
