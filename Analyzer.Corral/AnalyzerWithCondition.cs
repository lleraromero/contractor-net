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
            CancellationToken token, List<string> errorList, int maxDegreeOfParallelism)
            : base(defaultArgs, workingDir, queryGenerator, inputAssembly, inputFileName,
                typeToAnalyze, token, errorList, maxDegreeOfParallelism)
        {
        }

        /*public override ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            return base.AnalyzeActions(source, action, actions);
        }*/

        public override IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var transitions = base.AnalyzeTransitions(source, action, targets);
            if (transitions.Count > 0)
            {
                CSGenerator checker = CSGenerator.Instance(null);
                return checker.generateConditions(transitions);
            }
            else
            {
                return transitions;
            }
        }

        public override IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets, string exitCode)
        {
            var transitions = base.AnalyzeTransitions(source, action, targets,exitCode);
            if (transitions.Count > 0)
            {
                CSGenerator checker = CSGenerator.Instance(null);
                var transitionsWithConditions = checker.generateConditions(transitions,exitCode);
                var result = new List<Transition>();
                foreach (var t in transitionsWithConditions)
                {
                    result.Add(new Transition(t.Action, t.SourceState, t.TargetState, t.IsUnproven, t.Condition, exitCode, "NOSE"));
                }
                return result;
            }
            else
            {
                return transitions;
            }
        }
    }
}