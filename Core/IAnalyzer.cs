using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    [ContractClass(typeof(IAnalyzerContracts))]
    public interface IAnalyzer
    {
        ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions);
        IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets);
        string GetUsageStatistics();
    }

    [ContractClassFor(typeof(IAnalyzer))]
    public abstract class IAnalyzerContracts : IAnalyzer
    {
        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions != null && actions.Any());
            Contract.Ensures(Contract.Result<ActionAnalysisResults>() != null);
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(targets != null && targets.Any());
            Contract.Ensures(Contract.Result<IReadOnlyCollection<Transition>>() != null);
            Contract.Ensures(Contract.Result<IReadOnlyCollection<Transition>>().Any());
            throw new NotImplementedException();
        }

        public string GetUsageStatistics()
        {
            throw new NotImplementedException();
        }
    }
}