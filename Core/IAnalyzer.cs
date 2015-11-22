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

        ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions);

        ICollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets);
    }
}