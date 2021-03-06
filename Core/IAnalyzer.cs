﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    [ContractClass(typeof (IAnalyzerContracts))]
    public interface IAnalyzer
    {
        ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions);
        IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets);
        int GeneratedQueriesCount();
        int UnprovenQueriesCount();
    }

    #region IAnalyzer Contracts

    [ContractClassFor(typeof (IAnalyzer))]
    public abstract class IAnalyzerContracts : IAnalyzer
    {
        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Ensures(Contract.Result<ActionAnalysisResults>() != null);
            throw new NotImplementedException();
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(targets.Any());
            Contract.Ensures(Contract.Result<IReadOnlyCollection<Transition>>().Any());
            throw new NotImplementedException();
        }

        public int GeneratedQueriesCount()
        {
            throw new NotImplementedException();
        }

        public int UnprovenQueriesCount()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}