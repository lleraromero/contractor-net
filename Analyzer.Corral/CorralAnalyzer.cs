using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
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
            var timer = Stopwatch.StartNew();

            ISolver corralRunner = new CorralRunner();

            var negativeQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
            var queryAssembly = CreateBoogieQueryAssembly(negativeQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(negativeQueries));

            
            var positiveQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
            queryAssembly = CreateBoogieQueryAssembly(positiveQueries);
            evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(positiveQueries));

            timer.Stop();
            totalAnalysisTime += timer.Elapsed;

            var enabledAndDisabledActions = new HashSet<Action>(enabledActions);
            enabledAndDisabledActions.IntersectWith(disabledActions);

            enabledActions.ExceptWith(enabledAndDisabledActions);
            disabledActions.ExceptWith(enabledAndDisabledActions);

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var timer = Stopwatch.StartNew();

            ISolver corralRunner = new CorralRunner();
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries);

            timer.Stop();
            totalAnalysisTime += timer.Elapsed;

            return feasibleTransitions;
        }

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries)
        {
            Contract.Ensures(Contract.Result<FileInfo>().Exists);

            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(Configuration.TempPath, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));
            queryAssembly.Save(queryFilePath);

            return TranslateCSharpToBoogie(queryFilePath);
        }

        protected FileInfo TranslateCSharpToBoogie(string queryAssemblyPath)
        {
            Contract.Ensures(Contract.Result<FileInfo>().Exists);

            var bctRunner = new BctRunner(token);
            var args = new[] {queryAssemblyPath, "/lib:" + Path.GetDirectoryName(inputFileName)};

            totalAnalysisTime += bctRunner.Run(args);

            return new FileInfo(queryAssemblyPath.Replace("dll", "bpl"));
        }
    }
}