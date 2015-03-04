using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Text;
using System.Linq;

namespace Contractor.Core
{
    class ActionAnalysisResults
    {
        public List<IMethodDefinition> EnabledActions { get; private set; }
        public List<IMethodDefinition> DisabledActions { get; private set; }

        public ActionAnalysisResults()
        {
            this.EnabledActions = new List<IMethodDefinition>();
            this.DisabledActions = new List<IMethodDefinition>();
        }
    }

    class TransitionAnalysisResult
    {
        public List<Transition> Transitions { get; private set; }

        public TransitionAnalysisResult()
        {
            this.Transitions = new List<Transition>();
        }
    }

    public class TypeAnalysisResult
    {
        public Epa EPA { get; internal set; }
        public EpaGenerator.Backend Backend { get; internal set; }
        public TimeSpan TotalDuration { get; internal set; }
        public Dictionary<string, object> Statistics { get; internal set; }        

        public TypeAnalysisResult()
        {
            EPA = new Epa();
            TotalDuration = TimeSpan.Zero;
            Statistics = new Dictionary<string, object>();
        }

        public override string ToString()
        {
            var statesCount = this.EPA.States.Count;
            var transitionsCount = this.EPA.Transitions.Count;
            var initialStatesCount = (from s in this.EPA.States where s.IsInitial select s).Count();
            var unprovenQueriesCount = Convert.ToInt32(this.Statistics["UnprovenQueriesCount"]);
            var totalGeneratedQueriesCount = Convert.ToInt32(this.Statistics["TotalGeneratedQueriesCount"]);
            var precision = 100 - Math.Ceiling((double)unprovenQueriesCount * 100 / totalGeneratedQueriesCount);
            var unprovenTransitionsCount = this.EPA.Transitions.Count(t => t.IsUnproven);

            var sb = new StringBuilder();
            sb.AppendFormat(@"Total duration:             {0}"                 , this.TotalDuration).AppendLine();
            sb.AppendFormat(@"Engine:                     {0}"                 , this.Backend).AppendLine();
            sb.AppendFormat(@"Analysis total duration:    {0}"                 , this.Statistics["TotalAnalyzerDuration"]).AppendLine();
            sb.AppendFormat(@"Analysis precision:         {0}%"                , precision).AppendLine();
            sb.AppendFormat(@"Executions:                 {0}"                 , this.Statistics["ExecutionsCount"]).AppendLine();
            sb.AppendFormat(@"Generated queries:          {0} ({1} unproven)"  , totalGeneratedQueriesCount, unprovenQueriesCount).AppendLine();
            sb.AppendFormat(@"States:                     {0} ({1} initial)"   , statesCount, initialStatesCount).AppendLine();
            sb.AppendFormat(@"Transitions:                {0} ({1} unproven)"  , transitionsCount, unprovenTransitionsCount).AppendLine();
            //sb.AppendFormat(@"Propagation phase duration: {0}", this.Statistics["PropagationPhaseDuration"]).AppendLine();
            //sb.AppendFormat(@"Transitions removed:        {0}", this.Statistics["PropagationPhaseRemovedTransitions"]).AppendLine();
                                 
            return sb.ToString();
        }
    }
}
