using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contractor.Core;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Analyzer.Corral
{
    public class QueryEvaluator
    {
        protected ISolver solver;
        protected FileInfo queryAssembly;
        protected int unprovenQueries;
        private int maxDegreeOfParallelism;

        public QueryEvaluator(ISolver solver, FileInfo queryAssembly, int maxDegreeOfParallelism)
        {
            Contract.Requires(solver != null);
            Contract.Requires(queryAssembly.Exists);

            this.solver = solver;
            this.queryAssembly = queryAssembly;
            unprovenQueries = 0;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public int UnprovenQueries
        {
            get { return unprovenQueries; }
        }

        public IReadOnlyCollection<Action> GetEnabledActions(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            Contract.Requires(actionQueries.All(query => query.Type.Equals(QueryType.Positive)), "queries must be positive");

            var unreachableQueries = UnreachableQueries(actionQueries);
            return unreachableQueries.Select(query => query.ActionUnderTest).ToList();
        }

        protected IReadOnlyCollection<ActionQuery> UnreachableQueries(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            var unreachableQueries = new List<ActionQuery>();

            // TODO: Make ParallelOptions a singleton?
            // Change ParallelOptions.MaxDegreeOfParallelism to 1 to make the loop sequential.
            var opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = maxDegreeOfParallelism;
            Parallel.ForEach(actionQueries, opt, query =>
            {
                var result = solver.Execute(queryAssembly, query);
                switch (result)
                {
                    case QueryResult.Reachable:
                        break;
                    case QueryResult.MaybeReachable:
                        unprovenQueries++;
                        break;
                    case QueryResult.Unreachable:
                        unreachableQueries.Add(query);
                        break;
                    default:
                        throw new NotSupportedException();
                }
            });
            return unreachableQueries;
        }

        public IReadOnlyCollection<Action> GetDisabledActions(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            Contract.Requires(actionQueries.All(query => query.Type.Equals(QueryType.Negative)), "queries must be negative");

            var unreachableQueries = UnreachableQueries(actionQueries);
            return unreachableQueries.Select(query => query.ActionUnderTest).ToList();
        }

        public IReadOnlyCollection<Transition> GetFeasibleTransitions(IReadOnlyCollection<TransitionQuery> transitionQueries, string exitCode = "NOSE", string condition = "NOSE")
        {
            Contract.Requires(transitionQueries != null);

            var feasibleTransitions = new List<Transition>();

            // Change ParallelOptions.MaxDegreeOfParallelism to 1 to make the loop sequential.
            var opt = new ParallelOptions();
            opt.MaxDegreeOfParallelism = maxDegreeOfParallelism;
            Parallel.ForEach(transitionQueries, opt, query =>
            {
                var result = solver.Execute(queryAssembly, query);
                var resultInfo = "";
                /*if (!query.Method.Method.Type.ToString().Equals("System.Void") && !condition.Equals("NOSE"))
                    resultInfo = query.Method.Method.Type.ToString() + " && " + condition;
                else if (!query.Method.Method.Type.ToString().Equals("System.Void"))
                     resultInfo = query.Method.Method.Type.ToString();*/
                switch (result)
                {
                    case QueryResult.Reachable:
                        feasibleTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, false, exitCode, resultInfo));
                        break;
                    case QueryResult.MaybeReachable:
                        feasibleTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, true, exitCode, resultInfo));
                        unprovenQueries++;
                        break;
                    case QueryResult.Unreachable:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            });

            return feasibleTransitions;
        }
    }
}