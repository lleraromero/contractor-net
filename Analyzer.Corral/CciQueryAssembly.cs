﻿using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using Analysis.Cci;
using Contractor.Core;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
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

            var persister = new CciAssemblyPersister();
            var currentAssembly = new CciAssembly(module, contractProvider);
            new CciContractRewriter().Rewrite(currentAssembly);
            persister.Save(currentAssembly, path);
        }

        protected NamedTypeDefinition FindType(Module module, string typeName)
        {
            Contract.Ensures(Contract.Result<NamedTypeDefinition>() != null);
            var types = GetAnalyzableTypes(module);
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.UseGenericTypeNameSuffix).Equals(typeName));
            return type as NamedTypeDefinition;
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