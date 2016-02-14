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
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Analyzer.Corral
{
    internal class CciQueryAssembly
    {
        protected CodeContractAwareHostEnvironment host;
        protected Module module;
        protected ContractProvider contractProvider;
        public CciQueryAssembly(CciAssembly inputAssembly, ITypeDefinition typeToAnalyze, IEnumerable<Query> queries)
        {
            Contract.Requires(inputAssembly != null);
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(queries.Any());
            host = CciHostEnvironment.GetInstance();

            var cciInputType = FindType(inputAssembly.Module, typeToAnalyze.Name) as NamedTypeDefinition;
            Contract.Assert(cciInputType != null);
            var queryTypeContract = inputAssembly.ContractProvider.GetTypeContractFor(cciInputType);

            var module = new CodeAndContractDeepCopier(host).Copy(inputAssembly.Module);
            var cciQueryType = FindType(module, typeToAnalyze.Name) as NamedTypeDefinition;
            Contract.Assert(cciQueryType != null);

            cciQueryType.Methods.AddRange(from a in queries select a.Method.Method);

            var queryContractProvider = new ContractProvider(new ContractMethods(host), host.FindUnit(module.UnitIdentity));
            queryContractProvider.AssociateTypeWithContract(cciQueryType, queryTypeContract);

            foreach (var query in queries)
            {
                var method = cciQueryType.Methods.Find(m => m.GetUniqueName().Equals(query.Method.Name)) as MethodDefinition;
                Contract.Assert(method != null);

                method.ContainingTypeDefinition = cciQueryType;
                queryContractProvider.AssociateMethodWithContract(query.Method.Method, query.Method.Contract);
            }

            this.module = module;
            contractProvider = queryContractProvider;
        }

        public void Save(string path)
        {
            Contract.Requires(!string.IsNullOrEmpty(path) && !File.Exists(path));

            var pdbReader = GetPdbReader(module);
            // I need to replace Pre/Post with Assume/Assert
            ILocalScopeProvider localScopeProvider = new Decompiler.LocalScopeProvider(pdbReader);
            ISourceLocationProvider sourceLocationProvider = pdbReader;
            var trans = new ContractRewriter(host, (ContractProvider) contractProvider, sourceLocationProvider);
            module = trans.Rewrite(module) as Module;

            pdbReader = GetPdbReader(module);
            // Save the query assembly to run Corral
            using (var peStream = File.Create(path))
            {
                if (GetPdbReader(module) == null)
                {
                    PeWriter.WritePeToStream(module, host, peStream);
                }
                else
                {
                    var pdbName = Path.ChangeExtension(path, "pdb");
                    using (var pdbWriter = new PdbWriter(pdbName, pdbReader))
                    {
                        PeWriter.WritePeToStream(module, host, peStream, pdbReader, pdbReader, pdbWriter);
                    }
                }
            }
        }

        protected NamespaceTypeDefinition FindType(Module module, string typeName)
        {
            var types = GetAnalyzableTypes(module).Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.UseGenericTypeNameSuffix).Equals(typeName));
            return type;
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