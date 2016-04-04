using System;
using System.Collections.Generic;
using System.IO;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using Analyzer.CodeContracts;
using Action = Contractor.Core.Model.Action;

namespace Analyzer.Corral
{
    public class AnalyzerWithCondition : CorralAnalyzer
    {
        
        public AnalyzerWithCondition(string defaultArgs, DirectoryInfo workingDir, CciQueryGenerator queryGenerator, CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze,
            CancellationToken token)
            : base( defaultArgs, workingDir, queryGenerator, inputAssembly, inputFileName, typeToAnalyze, token)
        {
        }

        public override ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            return base.AnalyzeActions(source, action, actions);
        }

        public override IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var transitions = base.AnalyzeTransitions(source, action, targets);
            CSGenerator checker= CSGenerator.Instance(null);
            return checker.generateConditions(transitions);
        }

        public new string GetUsageStatistics()
        {
            return base.GetUsageStatistics();
        }
    }
}