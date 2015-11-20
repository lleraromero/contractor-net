using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Analyzer.Corral
{
    public class CorralAnalyzer : IAnalyzer
    {
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected TypeDefinition typeToAnalyze;
        protected CancellationToken token;

        protected TimeSpan totalAnalysisTime;
        protected int executionsCount;
        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        protected string notPrefix = "_Not_";
        protected string methodNameDelimiter = "~";

        public CorralAnalyzer(CciQueryGenerator queryGenerator, CciAssembly inputAssembly, string inputFileName, TypeDefinition typeToAnalyze,
            CancellationToken token)
        {
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;

            totalAnalysisTime = new TimeSpan();
            executionsCount = 0;
            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
        }

        public TimeSpan TotalAnalysisDuration
        {
            get { return totalAnalysisTime; }
        }

        public int ExecutionsCount
        {
            get { return executionsCount; }
        }

        public int TotalGeneratedQueriesCount
        {
            get { return generatedQueriesCount; }
        }

        public int UnprovenQueriesCount
        {
            get { return unprovenQueriesCount; }
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            var queries = queryGenerator.CreateQueries(source, action, actions);
            var result = Analyze(queries);
            return EvaluateQueries(actions, result);
        }

        public TransitionAnalysisResult AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var queries = queryGenerator.CreateQueries(source, action, targets);
            var result = Analyze(queries);
            return EvaluateQueries(source, action, targets, result);
        }

        private IEnumerable<Query> Analyze(IEnumerable<Action> queriesActions)
        {
            var queries = from a in queriesActions select new Query(a);
            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(Configuration.TempPath, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));
            queryAssembly.Save(queryFilePath);

            var boogieQueryFilePath = TranslateCSharpToBoogie(queryFilePath);

            return TestQueries(queries, boogieQueryFilePath);
        }

        protected IEnumerable<Query> TestQueries(IEnumerable<Query> queries, string boogieQueryFilePath)
        {
            var result = new List<Query>();
            var corralRunner = new CorralRunnerParallel(token, queries, new FileInfo(boogieQueryFilePath));
            var timer = Stopwatch.StartNew();
            result.AddRange(corralRunner.Run());
            timer.Stop();
            totalAnalysisTime += timer.Elapsed;

            return result;
        }

        protected string TranslateCSharpToBoogie(string queryAssemblyPath)
        {
            var bctRunner = new BctRunner(token);
            var args = new[] {queryAssemblyPath, "/lib:" + Path.GetDirectoryName(inputFileName)};

            totalAnalysisTime += bctRunner.Run(args);

            return queryAssemblyPath.Replace("dll", "bpl");
        }

        private ActionAnalysisResults EvaluateQueries(IEnumerable<Action> actions, IEnumerable<Query> result)
        {
            var enabledActions = new HashSet<Action>(actions);
            var disabledActions = new HashSet<Action>(actions);

            foreach (var evaluatedQuery in result)
            {
                if (evaluatedQuery.GetType() == typeof (ReachableQuery))
                {
                    EnableOrDisable(enabledActions, disabledActions, evaluatedQuery);
                }
                else if (evaluatedQuery.GetType() == typeof (MayBeReachableQuery))
                {
                    EnableOrDisable(enabledActions, disabledActions, evaluatedQuery);
                    unprovenQueriesCount++;
                }
            }

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        private void EnableOrDisable(HashSet<Action> enabledActions, HashSet<Action> disabledActions, Query query)
        {
            var actionName = query.Action.Method.Name.Value;
            var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
            actionName = actionName.Substring(actionNameStart);
            var isNegative = actionName.StartsWith(notPrefix);

            if (isNegative)
            {
                actionName = actionName.Remove(0, notPrefix.Length);
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

        private TransitionAnalysisResult EvaluateQueries(State source, Action action, IEnumerable<State> targets, IEnumerable<Query> result)
        {
            var transitions = new HashSet<Transition>();

            foreach (var evaluatedQuery in result)
            {
                if (evaluatedQuery.GetType() == typeof (ReachableQuery))
                {
                    var actionName = evaluatedQuery.Action.Method.Name.Value;
                    var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.First(s => s.Name == targetName);
                    var isUnproven = false;

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }
                }
                else if (evaluatedQuery.GetType() == typeof (MayBeReachableQuery))
                {
                    var actionName = evaluatedQuery.Action.Method.Name.Value;
                    var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.First(s => s.Name == targetName);
                    var isUnproven = true;

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }

                    unprovenQueriesCount++;
                }
            }

            return new TransitionAnalysisResult(transitions);
        }
    }
}