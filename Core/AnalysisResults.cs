using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class ActionAnalysisResults
    {
        protected HashSet<Action> enabledActions;
        protected HashSet<Action> disabledActions;

        public ActionAnalysisResults(ISet<Action> enabledActions, ISet<Action> disabledActions)
        {
            Contract.Requires(enabledActions != null && !enabledActions.Contains(null));
            Contract.Requires(disabledActions != null && !disabledActions.Contains(null));

            this.enabledActions = new HashSet<Action>(enabledActions);
            this.disabledActions = new HashSet<Action>(disabledActions);
        }

        public IImmutableSet<Action> EnabledActions
        {
            get { return enabledActions.ToImmutableHashSet(); }
        }

        public IImmutableSet<Action> DisabledActions
        {
            get { return disabledActions.ToImmutableHashSet(); }
        }
    }

    public class TypeAnalysisResult
    {
        protected Epa epa;
        protected string statistics;
        protected TimeSpan totalTime;

        public TypeAnalysisResult(Epa epa, TimeSpan totalTime, string statistics)
        {
            Contract.Requires(epa != null);
            Contract.Requires(totalTime != null);
            Contract.Requires(statistics != null);

            this.epa = epa;
            this.totalTime = totalTime;
            this.statistics = statistics;
        }

        public Epa Epa
        {
            get { return epa; }
        }

        public TimeSpan TotalTime
        {
            get { return totalTime; }
        }

        public override string ToString()
        {
            var statesCount = Epa.States.Count;
            var transitionsCount = Epa.Transitions.Count;
            var unprovenTransitionsCount = Epa.Transitions.Count(t => t.IsUnproven);

            var sb = new StringBuilder();
            sb.AppendFormat(@"Total time:          {0:%m} minutes {0:%s} seconds", totalTime).AppendLine();
            sb.AppendFormat(@"States:              {0} (1 initial)", statesCount).AppendLine();
            sb.AppendFormat(@"Transitions:         {0} ({1} unproven)", transitionsCount, unprovenTransitionsCount).AppendLine();
            sb.AppendLine(statistics);

            return sb.ToString();
        }
    }
}