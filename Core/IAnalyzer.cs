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

        ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions);

        TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets);
    }
}