﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

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

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public CorralAnalyzer(string defaultArgs, DirectoryInfo workingDir, CciQueryGenerator queryGenerator, CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze,
            CancellationToken token)
        {
            this.defaultArgs = defaultArgs;
            this.workingDir = workingDir;
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;

            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);

            if (action.IsPure)
            {
                return new ActionAnalysisResults(new HashSet<Action>(source.EnabledActions), new HashSet<Action>(source.DisabledActions));
            }

            var enabledActions = GetMustBeEnabledActions(source, action, actions, corralRunner);
            var disabledActions = GetMustBeDisabledActions(source, action, actions, corralRunner);

            Contract.Assert(!enabledActions.Intersect(disabledActions).Any());

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions,string expectedExitCode)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);

            if (action.IsPure)
            {
                return new ActionAnalysisResults(new HashSet<Action>(source.EnabledActions), new HashSet<Action>(source.DisabledActions));
            }

            //rewrite(action.Method.Body);

            var enabledActions = GetMustBeEnabledActions(source, action, actions, corralRunner,expectedExitCode);
            var disabledActions = GetMustBeDisabledActions(source, action, actions, corralRunner,expectedExitCode);

            Contract.Assert(!enabledActions.Intersect(disabledActions).Any());

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries);
            unprovenQueriesCount += evaluator.UnprovenQueries;

            return feasibleTransitions;
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets, string expectedExitCode)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
            
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets,expectedExitCode);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries,expectedExitCode);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries,expectedExitCode);
            unprovenQueriesCount += evaluator.UnprovenQueries;

            return feasibleTransitions;
        }

        public Transition AnalyzeTransitionToNotInv(State source, Action action, string exitCode)
        {
            State notInvState = null;
            var targets = new List<State>();
            targets.Add(notInvState);
            return AnalyzeTransitions(source, action, targets,exitCode);
        }

        public string GetUsageStatistics()
        {
            var statisticsBuilder = new StringBuilder();

            statisticsBuilder.AppendFormat(@"Generated queries: {0} ({1} unproven)", generatedQueriesCount, unprovenQueriesCount).AppendLine();

            var precision = 100 - Math.Ceiling((double) unprovenQueriesCount*100/generatedQueriesCount);
            statisticsBuilder.AppendFormat(@"Analysis precision: {0}%", precision).AppendLine();

            return statisticsBuilder.ToString();
        }

        protected ISet<Action> GetMustBeDisabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner, string expectedExitCode="Ok")
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);

            var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
            generatedQueriesCount += targetNegatedPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetNegatedPreconditionQueries, expectedExitCode);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return disabledActions;
        }

        protected ISet<Action> GetMustBeEnabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner, string expectedExitCode="Ok")
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);
            if(expectedExitCode!="Ok"){
                var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions,expectedExitCode);
                generatedQueriesCount += targetPreconditionQueries.Count;
                var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries, expectedExitCode);
                var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
                var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
                unprovenQueriesCount += evaluator.UnprovenQueries;
                return enabledActions;
            }else{
                var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
                generatedQueriesCount += targetPreconditionQueries.Count;
                var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries, expectedExitCode);
                var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
                var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
                unprovenQueriesCount += evaluator.UnprovenQueries;
                return enabledActions;
            }
            
        }

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries)
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
        }

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries, string expectedExitCode="Ok")
        {
            Contract.Requires(queries.Any());

            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
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