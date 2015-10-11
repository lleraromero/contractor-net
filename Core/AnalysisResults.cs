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
    class ActionAnalysisResults
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

    class TransitionAnalysisResult
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
        public Epa EPA { get; protected set; }
        public EpaGenerator.Backend Backend { get; protected set; }
        public TimeSpan TotalDuration { get; protected set; }
        public Dictionary<string, object> Statistics { get; protected set; }        

        public TypeAnalysisResult(Epa epa, EpaGenerator.Backend backend, TimeSpan totalTime, Dictionary<string, object> statistics)
        {
            this.EPA = epa;
            this.Backend = backend;
            this.TotalDuration = totalTime;
            this.Statistics = statistics;
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
            sb.AppendFormat(@"Total duration:             {0}"                 , this.TotalDuration).AppendLine();
            sb.AppendFormat(@"Engine:                     {0}"                 , this.Backend).AppendLine();
            sb.AppendFormat(@"Analysis total duration:    {0}"                 , this.Statistics["TotalAnalyzerDuration"]).AppendLine();
            sb.AppendFormat(@"Analysis precision:         {0}%"                , precision).AppendLine();
            sb.AppendFormat(@"Executions:                 {0}"                 , this.Statistics["ExecutionsCount"]).AppendLine();
            sb.AppendFormat(@"Generated queries:          {0} ({1} unproven)"  , totalGeneratedQueriesCount, unprovenQueriesCount).AppendLine();
            sb.AppendFormat(@"States:                     {0} (1 initial)"   , statesCount).AppendLine();
            sb.AppendFormat(@"Transitions:                {0} ({1} unproven)"  , transitionsCount, unprovenTransitionsCount).AppendLine();
                                 
            return sb.ToString();
        }
    }
}
