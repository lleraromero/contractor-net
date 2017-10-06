using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;
using System.Threading.Tasks;

namespace Analyzer.Corral
{
    public class CorralAnalyzer : IAnalyzer
    {
        protected string defaultArgs;
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected ITypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;
        // shared between all analyzers to reuse information
        protected static Dictionary<Tuple<State, Action, IEnumerable<Action>>, ActionAnalysisResults> map;
        protected static Dictionary<Action, ISet<Action>> enabled_dependencies;
        protected static Dictionary<Action, ISet<Action>> disabled_dependencies;
        //---
        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;
        private int maxDegreeOfParallelism;
        //protected static BoggieHardcoderForExceptionSupport exceptionHarcoder;

        public CorralAnalyzer(string defaultArgs, DirectoryInfo workingDir, CciQueryGenerator queryGenerator, CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze,
            CancellationToken token, List<string> errorList,int maxDegreeOfParallelism)
        {
            this.defaultArgs = defaultArgs;
            this.workingDir = workingDir;
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;
            this.maxDegreeOfParallelism=maxDegreeOfParallelism;

            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
        }

        public void ComputeDependencies(ISet<Action> actions)
        {
            map = new Dictionary<Tuple<State, Action, IEnumerable<Action>>, ActionAnalysisResults>();
            enabled_dependencies = new Dictionary<Action, ISet<Action>>();
            disabled_dependencies = new Dictionary<Action, ISet<Action>>();
            
            //int i=1;
            //foreach (var action in actions.Where(a=>!a.IsPure))
            var opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = maxDegreeOfParallelism;
            Parallel.ForEach(actions.Where(a => !a.IsPure), opt, action =>
            {
                ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
                var enabledActions = GetMustBeEnabledActionsDependencies(action, actions, corralRunner);
                var disabledActions = GetMustBeDisabledActionsDependencies(action, actions, corralRunner);
                enabled_dependencies.Add(action, enabledActions);
                disabled_dependencies.Add(action, disabledActions);
                /*Console.WriteLine("DEP ACT " + i + ": " + action.Name + " ENA:" + enabledActions.Count + " DIS:" + disabledActions.Count);
                Console.Write("enabled={ ");
                foreach (var act in enabledActions)
                {
                    Console.Write(act.Name+" ");
                }
                Console.WriteLine("}");
                Console.Write("disabled={");
                foreach (var act in disabledActions)
                {
                    Console.Write(act.Name + " ");
                }
                Console.WriteLine("}");
                i++;*/
            });
        }

        private ISet<Action> GetMustBeDisabledActionsDependencies(Action action, ISet<Action> actions, ISolver corralRunner)
        {
            var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(action, actions);
            generatedQueriesCount += targetNegatedPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetNegatedPreconditionQueries, CreateQueriesContextStringForPath(action, actions, "NOSE"));
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return disabledActions;
        }

        private ISet<Action> GetMustBeEnabledActionsDependencies(Action action, ISet<Action> actions, ISolver corralRunner)
        {
            var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(action, actions);
            generatedQueriesCount += targetPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries, CreateQueriesContextStringForPath(action, actions, "NOSE"));
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
            var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return enabledActions;
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            return AnalyzeActions(source, action,actions,null);
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions,string expectedExitCode=null)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);

            if (action.IsPure)
            {
                return new ActionAnalysisResults(new HashSet<Action>(source.EnabledActions), new HashSet<Action>(source.DisabledActions));
            }

            //Cache optimization
            ActionAnalysisResults result;
            if (map.TryGetValue(new Tuple<State,Action,IEnumerable<Action>>(source,action,actions), out result))
            {
                return result;
            }

            var enabledActions = GetMustBeEnabledActions(source, action, actions, corralRunner,expectedExitCode);
            var disabledActions = GetMustBeDisabledActions(source, action, actions, corralRunner,expectedExitCode);

            Contract.Assert(!enabledActions.Intersect(disabledActions).Any());

            result = new ActionAnalysisResults(enabledActions, disabledActions);
            map.Add(new Tuple<State, Action, IEnumerable<Action>>(source, action, actions), result);

            return result;
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
            //Log.MyLogger.LogMsg("---- #TARGETS "+targets.Count()+"----");
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries, CreateQueriesContextStringForPath(source, action, targets, "NOSE"));
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries);
            unprovenQueriesCount += evaluator.UnprovenQueries;

            return feasibleTransitions;
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets, string expectedExitCode)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
            
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets,expectedExitCode);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries, CreateQueriesContextStringForPath(source, action, targets, expectedExitCode));
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries,expectedExitCode);
            unprovenQueriesCount += evaluator.UnprovenQueries;

            return feasibleTransitions;
        }

        private string CreateQueriesContextStringForPath(Action action, ISet<Action> actions, string expectedExitCode)
        {
            string name = !String.IsNullOrWhiteSpace(action.Name) && action.Name.Length >= 50 ? action.Name.Substring(0, 50) : action.Name;
            var result="ACTION_"+name + "_"+"ACTIONS_";
            //foreach (var act in actions)
            //{
            //    result += "_" + act.Name;
            //}
            result += "EXITCODE_"+expectedExitCode;
            return result;
        }

        private string CreateQueriesContextStringForPath(State source, Action action, IEnumerable<Action> actions, string expectedExitCode)
        {
            string name = !String.IsNullOrWhiteSpace(action.Name) && action.Name.Length >= 50 ? action.Name.Substring(0, 50) : action.Name;
            var result = "SOURCE_S"+source.Id+"_ACTION_" + name + "_" + "ACTIONS_";
            //foreach (var act in actions)
            //{
            //    result += "_" + act.Name;
            //}
            result += "EXITCODE_" + expectedExitCode;
            return result;
        }

        private string CreateQueriesContextStringForPath(State source, Action action, IEnumerable<State> targets, string expectedExitCode)
        {
            string name = !String.IsNullOrWhiteSpace(action.Name) && action.Name.Length >= 50 ? action.Name.Substring(0, 50) : action.Name;
            var result = "SOURCE_S"+source.Id+"_ACTION_" + name + "_" + "TARGETS_";
            foreach (var tar in targets)
            {
                result += "_S" + tar.Id;
            }
            result += "_EXITCODE_" + expectedExitCode;
            return result;
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets, string expectedExitCode, string condition)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);

            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets, expectedExitCode,condition);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries, CreateQueriesContextStringForPath(source, action, targets, expectedExitCode));
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries, expectedExitCode,condition);
            unprovenQueriesCount += evaluator.UnprovenQueries;

            return feasibleTransitions;
        }

        public int GeneratedQueriesCount()
        {
            return generatedQueriesCount;
        }

        public int UnprovenQueriesCount()
        {
            return unprovenQueriesCount;
        }

        protected ISet<Action> GetMustBeDisabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner, string expectedExitCode=null)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);
            if (disabled_dependencies.ContainsKey(action))
            {
                actions = new HashSet<Action>(actions.Except<Action>(disabled_dependencies[action]));
            }
            if (expectedExitCode != null){
                var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(source, action, actions,expectedExitCode);
                generatedQueriesCount += targetNegatedPreconditionQueries.Count;
                var queryAssembly = CreateBoogieQueryAssembly(targetNegatedPreconditionQueries, CreateQueriesContextStringForPath(source, action, actions, expectedExitCode));
                var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
                var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
                unprovenQueriesCount += evaluator.UnprovenQueries;
                if (disabled_dependencies.ContainsKey(action))
                {
                    disabledActions.UnionWith(disabled_dependencies[action]);
                }
                return disabledActions;
            
            }else{
                var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
                generatedQueriesCount += targetNegatedPreconditionQueries.Count;
                var queryAssembly = CreateBoogieQueryAssembly(targetNegatedPreconditionQueries, CreateQueriesContextStringForPath(source, action, actions, "NOSE"));
                var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
                var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
                unprovenQueriesCount += evaluator.UnprovenQueries;
                if (disabled_dependencies.ContainsKey(action))
                {
                    disabledActions.UnionWith(disabled_dependencies[action]);
                }
                return disabledActions;
            }
        }

        protected ISet<Action> GetMustBeEnabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner, string expectedExitCode=null)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);
            if (enabled_dependencies.ContainsKey(action))
            {
                actions = new HashSet<Action>(actions.Except<Action>(enabled_dependencies[action]));
            }
            if(expectedExitCode!=null){
                var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions,expectedExitCode);
                generatedQueriesCount += targetPreconditionQueries.Count;
                var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries, CreateQueriesContextStringForPath(source, action, actions, expectedExitCode));
                var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
                var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
                unprovenQueriesCount += evaluator.UnprovenQueries;
                if (enabled_dependencies.ContainsKey(action))
                {
                    enabledActions.UnionWith(enabled_dependencies[action]);
                }
                return enabledActions;
            }else{
                var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
                generatedQueriesCount += targetPreconditionQueries.Count;
                var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries, CreateQueriesContextStringForPath(source, action, actions, "NOSE"));
                var evaluator = new QueryEvaluator(corralRunner, queryAssembly, maxDegreeOfParallelism);
                var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
                unprovenQueriesCount += evaluator.UnprovenQueries;
                if (enabled_dependencies.ContainsKey(action))
                {
                    enabledActions.UnionWith(enabled_dependencies[action]);
                }
                return enabledActions;
            }
            
        }

        /*protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries)
        {
            Contract.Requires(queries.Any());

            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Contract.Assert(!Directory.Exists(Path.GetDirectoryName(queryFilePath)));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));

            // Mutex to avoid race-conditions in CCI static classes
            lock (CciAssemblyPersister.turnstile)
            {
                new CciContractRewriter().Rewrite(queryAssembly);
            }
            new CciAssemblyPersister().Save(queryAssembly, queryFilePath);

            return TranslateCSharpToBoogie(queryFilePath);
        }*/

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries, string path)
        {
            Contract.Requires(queries.Any());
            
            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, path, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Contract.Assert(!Directory.Exists(Path.GetDirectoryName(queryFilePath)));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));

            // Mutex to avoid race-conditions in CCI static classes
            lock (CciAssemblyPersister.turnstile)
            {
                var rewriter= new CciContractRewriter();
                //rewriter.expectedExitCode=expectedExitCode;
                rewriter.Rewrite(queryAssembly);
            }
            new CciAssemblyPersister().Save(queryAssembly, queryFilePath);
            
            return TranslateCSharpToBoogie(queryFilePath);
        }

        protected FileInfo TranslateCSharpToBoogie(string queryAssemblyPath)
        {
            Contract.Requires(!string.IsNullOrEmpty(queryAssemblyPath));

            var bctRunner = new BctRunner();
            var args = new[] { queryAssemblyPath, "/lib:" + Path.GetDirectoryName(inputFileName) };

            token.ThrowIfCancellationRequested();
            bctRunner.Run(args);

            return new FileInfo(queryAssemblyPath.Replace("dll", "bpl"));
        }
    }
}