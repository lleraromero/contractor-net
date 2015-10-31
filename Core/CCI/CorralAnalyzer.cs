using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class CorralAnalyzer : IAnalyzer
    {
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected string typeToAnalyze;
        protected CancellationToken token;

        protected TimeSpan totalAnalysisTime;
        protected int executionsCount;
        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        protected string notPrefix = "_Not_";
        protected string methodNameDelimiter = "~";

        public CorralAnalyzer(CciQueryGenerator queryGenerator, CciAssembly inputAssembly, string inputFileName,  string typeToAnalyze, CancellationToken token)
        {
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;

            this.totalAnalysisTime = new TimeSpan();
            this.executionsCount = 0;
            this.generatedQueriesCount = 0;
            this.unprovenQueriesCount = 0;
        }

        public TimeSpan TotalAnalysisDuration { get { return totalAnalysisTime; } }
        public int ExecutionsCount { get { return executionsCount; } }
        public int TotalGeneratedQueriesCount { get { return generatedQueriesCount; } }
        public int UnprovenQueriesCount { get { return unprovenQueriesCount; } }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions)
        {
            var queries = this.queryGenerator.CreateQueries(source, action, actions);
            var result = Analyze(queries);
            var analysisResult = EvaluateQueries(actions, result);

            return analysisResult;
        }

        public TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets)
        {
            var queries = this.queryGenerator.CreateQueries(source, action, targets);
            var result = Analyze(queries);
            return EvaluateQueries(source, action, targets, result);
        }

        protected IEnumerable<Query> Analyze(IEnumerable<Action> queriesActions)
        {
            var queries = from a in queriesActions select new Query(a);
            var queryAssembly = new CciQueryAssembly(this.inputAssembly, this.typeToAnalyze, queries);

            string path = Path.Combine(Configuration.TempPath, Guid.NewGuid().ToString(), Path.GetFileName(this.inputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(path));
            queryAssembly.Save(path);

            return ExecuteChecker(queries, path);
        }

        protected IEnumerable<Query> ExecuteChecker(IEnumerable<Query> queries, string queryAssemblyPath)
        {
            var result = new List<Query>();

            var bctRunner = new BctRunner(this.token);
            var args = new string[] { queryAssemblyPath, "/lib:" + Path.GetDirectoryName(this.inputFileName) };

            this.totalAnalysisTime += bctRunner.Run(args);

            foreach (var query in queries)
            {
                var queryName = BctTranslator.CreateUniqueMethodName(query.Action.Method);

                var corralRunner = new CorralRunner(this.token);
                var corralArgs = string.Format("{0} /main:{1} {2}", queryAssemblyPath.Replace("dll", "bpl"), queryName, Configuration.CorralArguments);
                this.totalAnalysisTime += corralRunner.Run(corralArgs, query);
                result.Add(corralRunner.Result);
            }

            return result;
        }

        protected ActionAnalysisResults EvaluateQueries(IEnumerable<Action> actions, IEnumerable<Query> result)
        {
            var enabledActions = new HashSet<Action>(actions);
            var disabledActions = new HashSet<Action>(actions);

            foreach (var evaluatedQuery in result)
            {
                if (evaluatedQuery.GetType() == typeof(ReachableQuery))
                {
                    EnableOrDisable(enabledActions, disabledActions, evaluatedQuery);
                }
                else if (evaluatedQuery.GetType() == typeof(MayBeReachableQuery))
                {
                    EnableOrDisable(enabledActions, disabledActions, evaluatedQuery);
                    this.unprovenQueriesCount++;
                }
            }

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        protected void EnableOrDisable(HashSet<Action> enabledActions, HashSet<Action> disabledActions, Query query)
        {
            var actionName = query.Action.Method.Name.Value;
            var actionNameStart = actionName.LastIndexOf(this.methodNameDelimiter) + 1;
            actionName = actionName.Substring(actionNameStart);
            var isNegative = actionName.StartsWith(this.notPrefix);

            if (isNegative)
            {
                actionName = actionName.Remove(0, this.notPrefix.Length);
                if (disabledActions.Any(a => a.Name.Equals(actionName)))
                {
                    disabledActions.Remove(disabledActions.First(a => a.Name.Equals(actionName)));
                }
            }
            else
            {
                if (enabledActions.Any(a => a.Name.Equals(actionName)))
                {
                    enabledActions.Remove(enabledActions.First(a => a.Name.Equals(actionName)));
                }
            }
        }

        protected TransitionAnalysisResult EvaluateQueries(State source, Action action, List<State> targets, IEnumerable<Query> result)
        {
            var transitions = new HashSet<Transition>();

            foreach (var evaluatedQuery in result)
            {
                if (evaluatedQuery.GetType() == typeof(ReachableQuery))
                {
                    var actionName = evaluatedQuery.Action.Method.Name.Value;
                    var actionNameStart = actionName.LastIndexOf(this.methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(this.methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.Find(s => s.Name == targetName);
                    var isUnproven = false;

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }
                }
                else if (evaluatedQuery.GetType() == typeof(MayBeReachableQuery))
                {
                    var actionName = evaluatedQuery.Action.Method.Name.Value;
                    var actionNameStart = actionName.LastIndexOf(this.methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(this.methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.Find(s => s.Name == targetName);
                    var isUnproven = true;

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }

                    this.unprovenQueriesCount++;
                }
            }

            return new TransitionAnalysisResult(transitions);
        }
    }
}