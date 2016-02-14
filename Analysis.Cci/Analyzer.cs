using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.Immutable;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using Action = Contractor.Core.Model.Action;

namespace Analysis.Cci
{
    public class Analyzer : IAnalyzer
    {
        public readonly IContractAwareHost host;
        public readonly AssemblyInfo inputAssembly;
        public readonly ContractProvider inputContractProvider;
        public readonly NamespaceTypeDefinition typeToAnalyze;
        public string methodNameDelimiter = "~";

        public string notPrefix = "_Not_";
        public AssemblyInfo queryAssembly;
        public ContractProvider queryContractProvider;

        public CciQueryGenerator queryGenerator;
        protected string tempPath;
        public GenericTypeInstance specializedInputType;

        public Analyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type, CancellationToken token, string tempPath)
        {
            Contract.Requires(host != null && module != null && type != null && token != null);

            this.tempPath = tempPath;

            this.host = host;
            inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(module.Location);

            inputContractProvider = inputAssembly.ExtractContracts();

            if (type.IsGeneric)
            {
                var typeReference = MutableModelHelper.GetGenericTypeInstanceReference(type.GenericParameters, type, host.InternFactory, null);
                specializedInputType = typeReference.ResolvedType as GenericTypeInstance;
            }

            // Create a clone of the module as a working copy.
            CreateQueryAssembly(type);

            typeToAnalyze = queryAssembly.DecompiledModule.AllTypes.Find(t => t.Name == type.Name) as NamespaceTypeDefinition;
            queryContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(queryAssembly.Module.UnitIdentity));

            this.token = token;

            queryGenerator = new CciQueryGenerator();
        }

        ~Analyzer()
        {
            // Delete the working copy of the module.
            try
            {
                File.Delete(GetQueryAssemblyPath());
            }
            catch
            {
            }
        }

        public virtual void CreateQueryAssembly(NamespaceTypeDefinition type)
        {
            // TODO: fix the query assembly to include the class being analysed and all its dependencies
            queryAssembly = new AssemblyInfo(host, new MetadataDeepCopier(host).Copy(inputAssembly.DecompiledModule));

            //var coreAssembly = host.LoadAssembly(host.CoreAssemblySymbolicIdentity);

            //var assembly = new Assembly
            //{
            //    Name = host.NameTable.GetNameFor("Query"),
            //    ModuleName = host.NameTable.GetNameFor("query.dll"),
            //    Kind = ModuleKind.DynamicallyLinkedLibrary,
            //    TargetRuntimeVersion = coreAssembly.TargetRuntimeVersion
            //};

            //assembly.AssemblyReferences.Add(coreAssembly);

            //var rootUnitNamespace = new RootUnitNamespace();
            //assembly.UnitNamespaceRoot = rootUnitNamespace;
            //rootUnitNamespace.Unit = assembly;

            //var moduleClass = new NamespaceTypeDefinition
            //{
            //    ContainingUnitNamespace = rootUnitNamespace,
            //    InternFactory = host.InternFactory,
            //    IsClass = true,
            //    Name = host.NameTable.GetNameFor("<Module>")
            //};


            //assembly.AllTypes.Add(moduleClass);

            //var queryType = new MetadataDeepCopier(host).Copy(type);
            //rootUnitNamespace.Members.Add(queryType);
            //assembly.AllTypes.Add(queryType);

            //queryAssembly = new AssemblyInfo(host, assembly);
        }

        public string GetQueryAssemblyPath()
        {
            Contract.Requires(inputAssembly != null);

            return Path.Combine(tempPath, queryAssembly.Module.ModuleName.Value);
        }

        public PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null && host != null);

            PdbReader pdbReader = null;
            var pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }

        public virtual List<MethodDefinition> GenerateQueries<T>(State state, Action action, List<T> actions /*states*/)
        {
            Contract.Requires(typeof (T) == typeof (Action) || typeof (T) == typeof (State));

            var queries = new List<Query>();

            if (typeof (T) == typeof (Action))
            {
                queries.AddRange(queryGenerator.CreatePositiveQueries(state, action, actions as List<Action>));
                queries.AddRange(queryGenerator.CreateNegativeQueries(state, action, actions as List<Action>));

            }
            else if (typeof (T) == typeof (State))
            {
                queries.AddRange(queryGenerator.CreateTransitionQueries(state, action, actions as List<State>));
            }

            foreach (var query in queries)
            {
                (query.Method.Method as MethodDefinition).ContainingTypeDefinition = typeToAnalyze;
                queryContractProvider.AssociateMethodWithContract(query.Method.Method as MethodDefinition, query.Method.Contract);
            }

            return new List<MethodDefinition>(from a in queries select a.Method.Method as MethodDefinition);
        }

        #region IAnalyzer interface
        // Token that allows the user to stop the analysis
        protected CancellationToken token;

        public virtual ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            throw new NotImplementedException();
        }

        public virtual IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            throw new NotImplementedException();
        }

        public string GetUsageStatistics()
        {
            throw new NotImplementedException();
        }

        #endregion IAnalyzer interface   
    }
}