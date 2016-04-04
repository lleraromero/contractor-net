using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Analyzer.CodeContracts
{
    public class AnalyzerWithCondition : CodeContractsAnalyzer
    {
        //protected IAnalyzer analyzer;
        public AnalyzerWithCondition(DirectoryInfo workingDir, string ccCheckDefaultArgs, string libPaths, CciQueryGenerator queryGenerator,
            CciAssembly inputAssembly, string inputFileName, ITypeDefinition typeToAnalyze, CancellationToken token)
            : base(workingDir, ccCheckDefaultArgs, libPaths, queryGenerator,
            inputAssembly, inputFileName, typeToAnalyze, token)
        {
        }

        public override ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            return base.AnalyzeActions(source, action, actions);
        }

        public override IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var transitions = base.AnalyzeTransitions(source, action, targets);
        
            return GenerateConditions(transitions);
        }

        public IReadOnlyCollection<Transition> GenerateConditions(IReadOnlyCollection<Transition> transitions)
        {
            var codeContractsRunner = new CodeContractsRunner(workingDir, ccCheckDefaultArgs, libPaths, typeToAnalyze);
            var transitionQueries = queryGenerator.CreateTransitionQueries(transitions);
            var queryAssembly = CreateQueryAssembly(transitionQueries);
            var evaluator = new ConditionQueryEvaluator(codeContractsRunner, queryAssembly);
            var transitionsWithConditions = evaluator.GetTransitionsWithConditions(transitions,transitionQueries);
            return transitionsWithConditions;
        }

        public new string GetUsageStatistics()
        {
            return base.GetUsageStatistics();
        }

        
    }
}