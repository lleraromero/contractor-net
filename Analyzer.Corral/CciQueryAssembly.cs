using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Analysis.Cci;
using Contractor.Core;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.ILToCodeModel;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analyzer.Corral
{
    internal class CciQueryAssembly : CciAssembly
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
                (contractProvider as ContractProvider).AssociateMethodWithContract(query.Method.Method, query.Method.Contract);
            }
        }

        public void Save(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path) && !File.Exists(path));

            var pdbReader = GetPdbReader(decompiledModule);
            // I need to replace Pre/Post with Assume/Assert
            ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);
            ISourceLocationProvider sourceLocationProvider = pdbReader;
            var trans = new ContractRewriter(host, (ContractProvider) contractProvider, sourceLocationProvider);
            decompiledModule = trans.Rewrite(decompiledModule) as Module;

            pdbReader = GetPdbReader(decompiledModule);
            // Save the query assembly to run Corral
            using (var peStream = File.Create(path))
            {
                if (GetPdbReader(decompiledModule) == null)
                {
                    PeWriter.WritePeToStream(decompiledModule, host, peStream);
                }
                else
                {
                    var pdbName = Path.ChangeExtension(path, "pdb");
                    using (var pdbWriter = new PdbWriter(pdbName, pdbReader))
                    {
                        PeWriter.WritePeToStream(decompiledModule, host, peStream, pdbReader, pdbReader, pdbWriter);
                    }
                }
            }
        }

        protected NamespaceTypeDefinition FindType(string typeName)
        {
            var types = GetAnalyzableTypes(decompiledModule).Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.UseGenericTypeNameSuffix).Equals(typeName));
            return type;
        }
    }
}