using Microsoft.Cci;
using System;
using System.Collections.Generic;

namespace Contractor.Core
{
    interface IAnalyzer
    {
        TimeSpan TotalAnalysisDuration { get; }
        int ExecutionsCount { get; }
        int TotalGeneratedQueriesCount { get; }
        int UnprovenQueriesCount { get; }

        ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions);

        TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets);
    }
}