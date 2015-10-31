﻿using Contractor.Core.Model;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class Analyzer : IAnalyzer
    {
        #region IAnalyzer interface

        public TimeSpan TotalAnalysisDuration { get; set; }
        public int ExecutionsCount { get; set; }
        public int TotalGeneratedQueriesCount { get; set; }
        public int UnprovenQueriesCount { get; set; }

        // Token that allows the user to stop the analysis
        public CancellationToken token;

        public virtual ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions) { throw new NotImplementedException(); }

        public virtual TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets) { throw new NotImplementedException(); }

        #endregion IAnalyzer interface

        public readonly IContractAwareHost host;
        public readonly AssemblyInfo inputAssembly;
        public readonly ContractProvider inputContractProvider;
        public AssemblyInfo queryAssembly;
        public ContractProvider queryContractProvider;
        public readonly NamespaceTypeDefinition typeToAnalyze;

        public string notPrefix = "_Not_";
        public string methodNameDelimiter = "~";

        public Microsoft.Cci.Immutable.GenericTypeInstance specializedInputType;

        public CciQueryGenerator queryGenerator;

        public Analyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type, CancellationToken token)
        {
            Contract.Requires(host != null && module != null && type != null && token != null);

            this.host = host;
            this.inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(module.Location);

            this.inputContractProvider = inputAssembly.ExtractContracts();

            if (type.IsGeneric)
            {
                var typeReference = MutableModelHelper.GetGenericTypeInstanceReference(type.GenericParameters, type, host.InternFactory, null);
                this.specializedInputType = typeReference.ResolvedType as Microsoft.Cci.Immutable.GenericTypeInstance;
            }

            // Create a clone of the module as a working copy.
            CreateQueryAssembly(type);

            this.typeToAnalyze = queryAssembly.DecompiledModule.AllTypes.Find(t => t.Name == type.Name) as NamespaceTypeDefinition;
            this.queryContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(this.queryAssembly.Module.UnitIdentity));

            this.token = token;

            this.queryGenerator = new CciQueryGenerator(this.host, this.typeToAnalyze);
        }

        ~Analyzer()
        {
            // Delete the working copy of the module.
            try
            {
                File.Delete(GetQueryAssemblyPath());
            }
            catch
            { }
        }

        public virtual void CreateQueryAssembly(NamespaceTypeDefinition type)
        {
            // TODO: fix the query assembly to include the class being analysed and all its dependencies
            this.queryAssembly = new AssemblyInfo(host, new MetadataDeepCopier(this.host).Copy(inputAssembly.DecompiledModule));
            return;

            var coreAssembly = host.LoadAssembly(host.CoreAssemblySymbolicIdentity);

            var assembly = new Assembly()
            {
                Name = host.NameTable.GetNameFor("Query"),
                ModuleName = host.NameTable.GetNameFor("query.dll"),
                Kind = ModuleKind.DynamicallyLinkedLibrary,
                TargetRuntimeVersion = coreAssembly.TargetRuntimeVersion,
            };

            assembly.AssemblyReferences.Add(coreAssembly);

            var rootUnitNamespace = new RootUnitNamespace();
            assembly.UnitNamespaceRoot = rootUnitNamespace;
            rootUnitNamespace.Unit = assembly;

            var moduleClass = new NamespaceTypeDefinition()
            {
                ContainingUnitNamespace = rootUnitNamespace,
                InternFactory = host.InternFactory,
                IsClass = true,
                Name = host.NameTable.GetNameFor("<Module>"),
            };


            assembly.AllTypes.Add(moduleClass);

            var queryType = new MetadataDeepCopier(this.host).Copy(type);
            rootUnitNamespace.Members.Add(queryType);
            assembly.AllTypes.Add(queryType);

            this.queryAssembly = new AssemblyInfo(host, assembly);
        }

        public string GetQueryAssemblyPath()
        {
            Contract.Requires(this.inputAssembly != null);

            return Path.Combine(Configuration.TempPath, this.queryAssembly.Module.ModuleName.Value);
        }

        public PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null && host != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }

        public virtual List<MethodDefinition> GenerateQueries<T>(State state, Action action, List<T> actions /*states*/)
        {
            Contract.Requires(typeof(T) == typeof(Action) || typeof(T) == typeof(State));

            var queries = new List<Action>();

            if (typeof(T) == typeof(Action))
            {
                queries.AddRange(this.queryGenerator.CreateQueries(state, action, actions as List<Action>));
            }
            else if (typeof(T) == typeof(State))
            {
                queries.AddRange(this.queryGenerator.CreateQueries(state, action, actions as List<State>));
            }

            TotalGeneratedQueriesCount += queries.Count;

            foreach (var query in queries)
            {
                this.queryContractProvider.AssociateMethodWithContract(query.Method as MethodDefinition, query.Contract);
            }

            return new List<MethodDefinition>(from a in queries select a.Method as MethodDefinition);
        }
    }
}