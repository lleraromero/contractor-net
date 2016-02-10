using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
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

        public QueryEvaluator(ISolver solver, FileInfo queryAssembly)
        {
            Contract.Requires(solver != null);
            Contract.Requires(queryAssembly.Exists);

            this.solver = solver;
            this.queryAssembly = queryAssembly;
            unprovenQueries = 0;
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

            foreach (var query in actionQueries)
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
            }
            return unreachableQueries;
        }

        public IReadOnlyCollection<Action> GetDisabledActions(IReadOnlyCollection<ActionQuery> actionQueries)
        {
            Contract.Requires(actionQueries.All(query => query.Type.Equals(QueryType.Negative)), "queries must be negative");

            var unreachableQueries = UnreachableQueries(actionQueries);
            return unreachableQueries.Select(query => query.ActionUnderTest).ToList();
        }

        public IReadOnlyCollection<Transition> GetFeasibleTransitions(IReadOnlyCollection<TransitionQuery> transitionQueries)
        {
            Contract.Requires(transitionQueries != null);

            var feasibleTransitions = new List<Transition>();

            foreach (var query in transitionQueries)
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
    }
}