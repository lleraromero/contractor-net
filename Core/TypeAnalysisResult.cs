using System;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public class TypeAnalysisResult
    {
        protected Epa epa;
        protected string statistics;
        protected TimeSpan totalTime;
        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public TypeAnalysisResult(Epa epa, TimeSpan totalTime, int generatedQueriesCount, int unprovenQueriesCount)
        {
            Contract.Requires(epa != null);
            Contract.Requires(totalTime != null);
            Contract.Requires(generatedQueriesCount >= 0);
            Contract.Requires(unprovenQueriesCount >= 0);
            Contract.Requires(unprovenQueriesCount <= generatedQueriesCount);

            this.epa = epa;
            this.totalTime = totalTime;
            this.generatedQueriesCount = generatedQueriesCount;
            this.unprovenQueriesCount = unprovenQueriesCount;
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
            var statesCount = epa.States.Count;
            var initialStatesCount = epa.Transitions.Count(t => t.SourceState.Equals(epa.Initial));
            var transitionsCount = epa.Transitions.Count;
            var unprovenTransitionsCount = epa.Transitions.Count(t => t.IsUnproven);
            
            var sb = new StringBuilder();
            sb.AppendFormat(@"Total time:          {0:%m} minutes {0:%s} seconds", totalTime).AppendLine();
            sb.AppendFormat(@"States:              {0} ({1} initial)", statesCount, initialStatesCount).AppendLine();
            sb.AppendFormat(@"Transitions:         {0} ({1} unproven)", transitionsCount, unprovenTransitionsCount).AppendLine();

            sb.AppendFormat(@"Generated queries:   {0} ({1} unproven)", generatedQueriesCount, unprovenQueriesCount).AppendLine();
            var precision = 100 - Math.Ceiling((double)unprovenQueriesCount * 100 / generatedQueriesCount);
            sb.AppendFormat(@"Analysis precision:  {0}%", precision).AppendLine();
            sb.AppendLine(statistics);

            return sb.ToString();
        }
    }
}