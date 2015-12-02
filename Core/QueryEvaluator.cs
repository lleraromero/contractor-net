using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class QueryEvaluator
    {
        protected ISolver solver;
        protected FileInfo queryAssembly;
        protected int unprovenQueries;

        public QueryEvaluator(ISolver solver, FileInfo queryAssembly)
        {
            Contract.Requires(solver != null);
            Contract.Requires(queryAssembly != null && queryAssembly.Exists);

            this.solver = solver;
            this.queryAssembly = queryAssembly;
            unprovenQueries = 0;
        }

        public IReadOnlyCollection<Action> GetEnabledActions(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            Contract.Requires(actionQueries.All(query => query.Type.Equals(QueryType.Negative)), "queries must be negative");

            var posiblyReachableQueries = ExecuteQueriesAndGetPosiblyReachableOnes(actionQueries);
            return posiblyReachableQueries.Select(query => query.ActionUnderTest).ToList();
        }

        public IReadOnlyCollection<Action> GetDisabledActions(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            Contract.Requires(actionQueries.All(query => query.Type.Equals(QueryType.Positive)), "queries must be positive");

            var posiblyReachableQueries = ExecuteQueriesAndGetPosiblyReachableOnes(actionQueries);
            return posiblyReachableQueries.Select(query => query.ActionUnderTest).ToList();
        }

        public IReadOnlyCollection<Transition> GetFeasibleTransitions(IReadOnlyCollection<TransitionQuery> transitionQueries)
        {
            Contract.Requires(transitionQueries != null);

            var feasibleTransitions = new List<Transition>();

            foreach(var query in transitionQueries)
            {
                var result = solver.Execute(queryAssembly, query);
                switch (result)
                {
                    case QueryResult.Reachable:
                        feasibleTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, false));
                        break;
                    case QueryResult.MaybeReachable:
                        feasibleTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, true));
                        unprovenQueries++;
                        break;
                    case QueryResult.Unreachable:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return feasibleTransitions;
        }

        protected IReadOnlyCollection<ActionQuery> ExecuteQueriesAndGetPosiblyReachableOnes(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            var reachableQueries = new List<ActionQuery>();

            foreach(var query in actionQueries)
            {
                var result = solver.Execute(queryAssembly, query);
                switch (result)
                {
                    case QueryResult.Reachable:
                        reachableQueries.Add(query);
                        break;
                    case QueryResult.MaybeReachable:
                        reachableQueries.Add(query);
                        unprovenQueries++;
                        break;
                    case QueryResult.Unreachable:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            }

            return reachableQueries;
        }

        public int UnprovenQueries
        {
            get { return unprovenQueries; }
        }
    }
}