using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public interface IAnalyzer
    {
        ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions);
        IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets);
        string GetUsageStatistics();
    }
}