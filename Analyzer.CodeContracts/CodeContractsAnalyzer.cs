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

namespace Analyzer.CodeContracts
{
    public class CodeContractsAnalyzer : IAnalyzer
    {
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected ITypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;
        protected string ccCheckDefaultArgs;
        protected string libPaths;

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public CodeContractsAnalyzer(DirectoryInfo workingDir, string ccCheckDefaultArgs, string libPaths, CciQueryGenerator queryGenerator,
            CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze, CancellationToken token)
        {
            this.workingDir = workingDir;
            this.ccCheckDefaultArgs = ccCheckDefaultArgs;
            this.libPaths = libPaths;
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
            var codeContractsRunner = new CodeContractsRunner(workingDir, ccCheckDefaultArgs, libPaths, typeToAnalyze);

            var enabledActions = GetMustBeEnabledActions(source, action, actions, codeContractsRunner);
            var disabledActions = GetMustBeDisabledActions(source, action, actions, codeContractsRunner);

            Contract.Assert(!enabledActions.Intersect(disabledActions).Any(), "An action cannot be enabled and disabled at the same time");

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var codeContractsRunner = new CodeContractsRunner(workingDir, ccCheckDefaultArgs, libPaths, typeToAnalyze);

            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets);
            var queryAssembly = CreateQueryAssembly(transitionQueries);
            var evaluator = new QueryEvaluator(codeContractsRunner, queryAssembly);
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
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);

            var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
            generatedQueriesCount += targetNegatedPreconditionQueries.Count;
            var queryAssembly = CreateQueryAssembly(targetNegatedPreconditionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return disabledActions;
        }

        protected ISet<Action> GetMustBeEnabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);

            var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
            generatedQueriesCount += targetPreconditionQueries.Count;
            var queryAssembly = CreateQueryAssembly(targetPreconditionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return enabledActions;
        }

        protected FileInfo CreateQueryAssembly(IReadOnlyCollection<Query> queries)
        {
            Contract.Requires(queries.Any());

            // CciAttributeAdder will tell cccheck to analyze only the query methods. The rest will be assumed as already verified.
            var queryAssembly = new CciAttributeAdder(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));
            queryAssembly.Save(queryFilePath);

            return new FileInfo(queryFilePath);
        }
    }
}