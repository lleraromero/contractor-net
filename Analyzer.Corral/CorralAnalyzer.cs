using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
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

        public CorralAnalyzer(string defaultArgs, DirectoryInfo workingDir,  CciQueryGenerator queryGenerator, CciAssembly inputAssembly, string inputFileName, TypeDefinition typeToAnalyze,
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
            //TODO: necesito saber cuantas maybe hubieron
            unprovenQueriesCount = 0;
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);

            var enabledActions = GetEnabledActions(source, action, actions, corralRunner);
            var disabledActions = GetDisabledActions(source, action, actions, corralRunner);

            var enabledAndDisabledActions = new HashSet<Action>(enabledActions);
            enabledAndDisabledActions.IntersectWith(disabledActions);

            enabledActions.ExceptWith(enabledAndDisabledActions);
            disabledActions.ExceptWith(enabledAndDisabledActions);

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        protected ISet<Action> GetDisabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            var positiveQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
            generatedQueriesCount += positiveQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(positiveQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(positiveQueries));
            return disabledActions;
        }

        protected ISet<Action> GetEnabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            var negativeQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
            generatedQueriesCount += negativeQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(negativeQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(negativeQueries));
            return enabledActions;
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries);

            return feasibleTransitions;
        }

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries)
        {
            Contract.Ensures(Contract.Result<FileInfo>().Exists);

            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));
            queryAssembly.Save(queryFilePath);

            return TranslateCSharpToBoogie(queryFilePath);
        }

        protected FileInfo TranslateCSharpToBoogie(string queryAssemblyPath)
        {
            Contract.Ensures(Contract.Result<FileInfo>().Exists);

            var bctRunner = new BctRunner();
            var args = new[] {queryAssemblyPath, "/lib:" + Path.GetDirectoryName(inputFileName)};

            token.ThrowIfCancellationRequested();
            bctRunner.Run(args);

            return new FileInfo(queryAssemblyPath.Replace("dll", "bpl"));
        }

        public string GetUsageStatistics()
        {
            var statisticsBuilder = new StringBuilder();

            //TODO: habilitar cuando tengamos el conteo de maybes
            statisticsBuilder.AppendFormat(@"Generated queries: {0}" /*({1} unproven)"*/, generatedQueriesCount, unprovenQueriesCount).AppendLine();

            //TODO: habilitar cuando tengamos el conteo de maybes
            //var precision = 100 - Math.Ceiling((double)unprovenQueriesCount * 100 / generatedQueriesCount);
            //statisticsBuilder.AppendFormat(@"Analysis precision: {0}%", precision).AppendLine();

            return statisticsBuilder.ToString();
        }
    }
}