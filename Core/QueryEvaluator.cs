using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
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
                        unproven++;
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
            var unproven = 0;
            foreach (var query in actionQueries)
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
            }

            return reachableQueries;
        }

        /*
         private ActionAnalysisResults EvaluateQueries(IEnumerable<Action> actions, IEnumerable<Query> result)
        {
            var enabledActions = new HashSet<Action>(actions);
            var disabledActions = new HashSet<Action>(actions);

            foreach (var evaluatedQuery in result)
            {
                if (evaluatedQuery.GetType() == typeof (ReachableQuery))
                {
                    EnableOrDisable(enabledActions, disabledActions, evaluatedQuery);
                }
                else if (evaluatedQuery.GetType() == typeof (MayBeReachableQuery))
                {
                    EnableOrDisable(enabledActions, disabledActions, evaluatedQuery);
                    unprovenQueriesCount++;
                }
            }

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        private void EnableOrDisable(HashSet<Action> enabledActions, HashSet<Action> disabledActions, Query query)
        {
            var actionName = query.Action.Method.Name.Value;
            var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
            actionName = actionName.Substring(actionNameStart);
            var isNegative = actionName.StartsWith(notPrefix);

            if (isNegative)
            {
                actionName = actionName.Remove(0, notPrefix.Length);
                if (disabledActions.Any(a => a.Name.Equals(actionName)))
                {
                    disabledActions.Remove(disabledActions.First(a => a.Name.Equals(actionName)));
                }
            }
            else
            {
                if (enabledActions.Any(a => a.Name.Equals(actionName)))
                {
                    enabledActions.Remove(enabledActions.First(a => a.Name.Equals(actionName)));
                }
            }
        }

        private ICollection<Transition> EvaluateQueries(State source, Action action, IEnumerable<State> targets, IEnumerable<Query> result)
        {
            var transitions = new HashSet<Transition>();

            foreach (var evaluatedQuery in result)
            {
                if (evaluatedQuery.GetType() == typeof (ReachableQuery))
                {
                    var actionName = evaluatedQuery.Action.Method.Name.Value;
                    var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.First(s => s.Name == targetName);
                    var isUnproven = false;

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }
                }
                else if (evaluatedQuery.GetType() == typeof (MayBeReachableQuery))
                {
                    var actionName = evaluatedQuery.Action.Method.Name.Value;
                    var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.First(s => s.Name == targetName);
                    var isUnproven = true;

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }

                    unprovenQueriesCount++;
                }
            }

            return new List<Transition>(transitions);
        }
         */
    }
}