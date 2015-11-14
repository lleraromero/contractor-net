using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class ActionAnalysisResults
    {
        protected HashSet<Action> enabledActions;
        protected HashSet<Action> disabledActions;
        public IImmutableSet<Action> EnabledActions { get { return enabledActions.ToImmutableHashSet(); } }
        public IImmutableSet<Action> DisabledActions { get { return disabledActions.ToImmutableHashSet(); } }

        public ActionAnalysisResults(ISet<Action> enabledActions, ISet<Action> disabledActions)
        {
            Contract.Requires(enabledActions != null && !enabledActions.Contains(null));
            Contract.Requires(disabledActions != null && !disabledActions.Contains(null));

            this.enabledActions = new HashSet<Action>(enabledActions);
            this.disabledActions = new HashSet<Action>(disabledActions);
        }
    }

    public class TransitionAnalysisResult
    {
        protected HashSet<Transition> transitions;
        public IImmutableSet<Transition> Transitions { get { return transitions.ToImmutableHashSet(); } }

        public TransitionAnalysisResult(ISet<Transition> transitions)
        {
            Contract.Requires(transitions != null && !transitions.Contains(null));

            this.transitions = new HashSet<Transition>(transitions);
        }
    }

    public class TypeAnalysisResult
    {
        protected Epa epa;
        protected TimeSpan totalDuration;
        protected Dictionary<string, object> statistics;

        public Epa EPA { get { return epa; } }
        public TimeSpan TotalDuration { get { return totalDuration; } }
        public Dictionary<string, object> Statistics { get { return statistics; } }

        public TypeAnalysisResult(Epa epa, TimeSpan totalTime, Dictionary<string, object> statistics)
        {
            Contract.Requires(epa != null);
            Contract.Requires(totalTime != null);
            Contract.Requires(statistics != null);

            this.epa = epa;
            this.totalDuration = totalTime;
            this.statistics = statistics;
        }

        public override string ToString()
        {
            var statesCount = this.EPA.States.Count;
            var transitionsCount = this.EPA.Transitions.Count;
            var unprovenQueriesCount = Convert.ToInt32(this.Statistics["UnprovenQueriesCount"]);
            var totalGeneratedQueriesCount = Convert.ToInt32(this.Statistics["TotalGeneratedQueriesCount"]);
            var precision = 100 - Math.Ceiling((double)unprovenQueriesCount * 100 / totalGeneratedQueriesCount);
            var unprovenTransitionsCount = this.EPA.Transitions.Count<Transition>(t => t.IsUnproven);

            var sb = new StringBuilder();
            sb.AppendFormat(@"Total time:          {0:%m} minutes {0:%s} seconds", this.TotalDuration).AppendLine();
            sb.AppendFormat(@"Analysis total time: {0}", this.Statistics["TotalAnalyzerDuration"]).AppendLine();
            sb.AppendFormat(@"Analysis precision:  {0}%", precision).AppendLine();
            sb.AppendFormat(@"Executions:          {0}", this.Statistics["ExecutionsCount"]).AppendLine();
            sb.AppendFormat(@"Generated queries:   {0} ({1} unproven)", totalGeneratedQueriesCount, unprovenQueriesCount).AppendLine();
            sb.AppendFormat(@"States:              {0} (1 initial)", statesCount).AppendLine();
            sb.AppendFormat(@"Transitions:         {0} ({1} unproven)", transitionsCount, unprovenTransitionsCount).AppendLine();

            return sb.ToString();
        }
    }
}
