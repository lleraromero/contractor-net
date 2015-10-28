using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public interface IAnalyzer
    {
        TimeSpan TotalAnalysisDuration { get; }
        int ExecutionsCount { get; }
        int TotalGeneratedQueriesCount { get; }
        int UnprovenQueriesCount { get; }

        ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions);

        TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets);
    }
}