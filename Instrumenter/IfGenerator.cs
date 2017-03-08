using System.Collections.Generic;
using System.Linq;
using Analysis.Cci;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;

namespace Instrumenter
{
    public class IfGenerator
    {
        public IStatement GenerateIf(FieldDefinition field, List<int> toStateIds, Epa epa, Dictionary<State, int> stateNumberMap)
        {
            var toStates = new List<State>();
            foreach (var kvp in stateNumberMap)
            {
                var id = kvp.Value;
                var state = kvp.Key;
                if (toStateIds.Contains(id))
                {
                    toStates.Add(state);
                }
            }

            var conditions = GenerateStatesConditions(toStates);

            var stmt = new AssignmentGenerator().GenerateAssign(field, toStateIds[0]);

            for (var i = 1; i < toStateIds.Count; ++i)
            {
                stmt = new ConditionalStatement
                {
                    Condition = conditions[i],
                    TrueBranch = new AssignmentGenerator().GenerateAssign(field, toStateIds[i]),
                    FalseBranch = stmt
                };
            }
            return stmt;
        }

        protected List<IExpression> GenerateStatesConditions(IReadOnlyCollection<State> states)
        {
            var host = CciHostEnvironment.GetInstance();

            //Optimizacion: calculamos la interseccion de todas las acciones habilitadas
            //y desabilitadas de todos los estados y se la restamos a todos
            var firstState = states.First();
            var enabledIntersection =
                new List<Action>(states.Aggregate(firstState.EnabledActions, (IEnumerable<Action> a, State s) => a.Intersect(s.EnabledActions)));
            var disabledIntersection =
                new List<Action>(states.Aggregate(firstState.DisabledActions, (IEnumerable<Action> a, State s) => a.Intersect(s.DisabledActions)));

            var conditions = new List<IExpression>();
            foreach (var state in states)
            {
                var enabledActions = state.EnabledActions.Except(enabledIntersection);
                var disabledActions = state.DisabledActions.Except(disabledIntersection);

                var exprs = GenerateStateInvariant(enabledActions, disabledActions);
                var condition = Helper.JoinWithLogicalAnd(host, exprs, true);
                conditions.Add(condition);
            }

            return conditions;
        }

        // Do not include the type invariant
        protected List<IExpression> GenerateStateInvariant(IReadOnlyCollection<Action> enabledActions, IReadOnlyCollection<Action> disabledActions)
        {
            var host = CciHostEnvironment.GetInstance();
            var exprs = new List<IExpression>();

            foreach (var action in enabledActions)
            {
                var conditions = from pre in action.Contract.Preconditions
                    select pre.Condition;
                exprs.AddRange(conditions);
            }

            foreach (var action in disabledActions)
            {
                if (action.Contract == null || !action.Contract.Preconditions.Any())
                {
                    var literal = new CompileTimeConstant
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = false
                    };

                    exprs.Add(literal);
                    continue;
                }

                var conditions = (from pre in action.Contract.Preconditions select pre.Condition).ToList();
                var condition = Helper.LogicalNotAfterJoinWithLogicalAnd(host, conditions, true);
                /*var condition = new LogicalNot
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = Helper.JoinWithLogicalAnd(host, conditions, true)
                };*/

                exprs.Add(condition);
            }

            return exprs;
        }
    }
}