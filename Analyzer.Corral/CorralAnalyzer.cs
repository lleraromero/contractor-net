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
        protected TypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public CorralAnalyzer(string defaultArgs, DirectoryInfo workingDir, CciQueryGenerator queryGenerator, CciAssembly inputAssembly,
            string inputFileName, TypeDefinition typeToAnalyze,
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

            var enabledActions = GetMustBeEnabledActions(source, action, actions, corralRunner);
            var disabledActions = GetMustBeDisabledActions(source, action, actions, corralRunner);

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

        public string GetUsageStatistics()
        {
            var statisticsBuilder = new StringBuilder();

            statisticsBuilder.AppendFormat(@"Generated queries: {0} ({1} unproven)", generatedQueriesCount, unprovenQueriesCount).AppendLine();

            var precision = 100 - Math.Ceiling((double) unprovenQueriesCount*100/generatedQueriesCount);
            statisticsBuilder.AppendFormat(@"Analysis precision: {0}%", precision).AppendLine();

            return statisticsBuilder.ToString();
        }

        protected ISet<Action> GetMustBeDisabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions != null && actions.Any());
            Contract.Requires(corralRunner != null);

            var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
            generatedQueriesCount += targetNegatedPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetNegatedPreconditionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return disabledActions;
        }

        protected ISet<Action> GetMustBeEnabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions != null && actions.Any());
            Contract.Requires(corralRunner != null);

            var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
            generatedQueriesCount += targetPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return enabledActions;
        }

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries)
        {
            Contract.Requires(queries != null && queries.Any());

            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));
            queryAssembly.Save(queryFilePath);

            return TranslateCSharpToBoogie(queryFilePath);
        }

        protected FileInfo TranslateCSharpToBoogie(string queryAssemblyPath)
        {
            Contract.Requires(!string.IsNullOrEmpty(queryAssemblyPath));

            var bctRunner = new BctRunner();
            var args = new[] {queryAssemblyPath, "/lib:" + Path.GetDirectoryName(inputFileName)};

            token.ThrowIfCancellationRequested();
            bctRunner.Run(args);

            return new FileInfo(queryAssemblyPath.Replace("dll", "bpl"));
        }
    }
}