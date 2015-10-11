using Contractor.Core.Model;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;

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

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            Contract.Invariant(!this.EnabledActions.Contains(null));
            Contract.Invariant(!this.DisabledActions.Contains(null));
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
