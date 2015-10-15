using Contractor.Core.Model;
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
    abstract class Analyzer : IAnalyzer
    {
        #region IAnalyzer interface

        public TimeSpan TotalAnalysisDuration { get; protected set; }
        public int ExecutionsCount { get; protected set; }
        public int TotalGeneratedQueriesCount { get; protected set; }
        public int UnprovenQueriesCount { get; protected set; }

        // Token that allows the user to stop the analysis
        protected CancellationToken token;

        public abstract ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions);

        public abstract TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets);

        #endregion IAnalyzer interface

        protected readonly IContractAwareHost host;
        protected readonly AssemblyInfo inputAssembly;
        protected readonly ContractProvider inputContractProvider;
        protected AssemblyInfo queryAssembly;
        protected ContractProvider queryContractProvider;
        protected readonly NamespaceTypeDefinition typeToAnalyze;

        protected string notPrefix = "_Not_";
        protected string methodNameDelimiter = "~";

        protected Microsoft.Cci.Immutable.GenericTypeInstance specializedInputType;

        protected CciQueryGenerator queryGenerator;

        protected Analyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type, CancellationToken token)
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

        protected virtual void CreateQueryAssembly(NamespaceTypeDefinition type)
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

        protected string GetQueryAssemblyPath()
        {
            Contract.Requires(this.inputAssembly != null);

            return Path.Combine(Configuration.TempPath, this.queryAssembly.Module.ModuleName.Value);
        }

        protected PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null && host != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }

        protected virtual List<MethodDefinition> GenerateQueries<T>(State state, Action action, List<T> actions /*states*/)
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
            else
            {
                throw new NotSupportedException();
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