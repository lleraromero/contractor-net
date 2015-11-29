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
    public interface IQueryEvaluator
    {
        IReadOnlyCollection<Action> GetEnabledActions(IReadOnlyCollection<ActionQuery> actionQueries);
        IReadOnlyCollection<Action> GetDisabledActions(IReadOnlyCollection<ActionQuery> actionQueries);
        IReadOnlyCollection<Transition> GetFeasibleTransitions(IReadOnlyCollection<TransitionQuery> transitionQueries);
    }

    public class QueryEvaluator : IQueryEvaluator
    {
        protected ISolver solver;
        protected FileInfo queryAssembly;

        public QueryEvaluator(ISolver solver, FileInfo queryAssembly)
        {
            Contract.Requires(solver != null);
            Contract.Requires(queryAssembly != null && queryAssembly.Exists);

            this.solver = solver;
            this.queryAssembly = queryAssembly;
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
            var unproven = 0;
            Parallel.ForEach(transitionQueries, query =>
            {
                var result = solver.Execute(queryAssembly, query);
                switch (result)
                {
                    case QueryResult.Reachable:
                        feasibleTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, false));
                        break;
                    case QueryResult.MaybeReachable:
                        feasibleTransitions.Add(new Transition(query.Action, query.SourceState, query.TargetState, true));
                        unproven++;
                        break;
                    case QueryResult.Unreachable:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            });

            return feasibleTransitions;
        }

        protected IReadOnlyCollection<ActionQuery> ExecuteQueriesAndGetPosiblyReachableOnes(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            var reachableQueries = new List<ActionQuery>();
            var unproven = 0;
            Parallel.ForEach(actionQueries, query =>
            {
                var result = solver.Execute(queryAssembly, query);
                switch (result)
                {
                    case QueryResult.Reachable:
                        reachableQueries.Add(query);
                        break;
                    case QueryResult.MaybeReachable:
                        reachableQueries.Add(query);
                        unproven++;
                        break;
                    case QueryResult.Unreachable:
                        break;
                    default:
                        throw new NotSupportedException();
                }
            });

            return reachableQueries;
        }
    }
}