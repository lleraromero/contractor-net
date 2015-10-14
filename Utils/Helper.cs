using Contractor.Core.Model;
using CSharpSourceEmitter;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Utils
{
    public static class Helper
    {
        public static string PrintExpression(IExpression expression)
        {
            SourceEmitterOutputString sourceEmitterOutput = new SourceEmitterOutputString();
            var CSSourceEmitter = new CSharpSourceEmitter.SourceEmitter(sourceEmitterOutput);
            CSSourceEmitter.Traverse(expression);
            return sourceEmitterOutput.Data;
        }

        // A || B = A ? true : B
        public static IExpression JoinWithLogicalOr(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
        {
            if (expressions.Count == 0)
                return new CompileTimeConstant()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Value = defaultValue
                };

            IExpression result = expressions[0];

            for (int i = 1; i < expressions.Count; ++i)
            {
                result = new Conditional()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Condition = result,
                    ResultIfTrue = new CompileTimeConstant()
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = true
                    },
                    ResultIfFalse = expressions[i]
                };
            }

            return result;
        }

        // A && B = A ? B : false
        public static IExpression JoinWithLogicalAnd(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
        {
            if (expressions.Count == 0)
                return new CompileTimeConstant()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Value = defaultValue
                };

            IExpression result = expressions[0];

            for (int i = 1; i < expressions.Count; ++i)
            {
                result = new Conditional()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Condition = result,
                    ResultIfTrue = expressions[i],
                    ResultIfFalse = new CompileTimeConstant()
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = false
                    }
                };
            }

            return result;
        }

        public static List<IExpression> GenerateTypeInvariant(IMetadataHost host, ContractProvider cp, NamespaceTypeDefinition type)
        {
            var exprs = new List<IExpression>();
            ITypeContract mci = cp.GetTypeContractFor(type);

            if (mci != null && mci.Invariants.Count() > 0)
            {
                var conditions = from inv in mci.Invariants
                                 select inv.Condition;

                exprs.AddRange(conditions);
            };

            return exprs;
        }

        public static List<IExpression> GenerateStatesConditions(IMetadataHost host, Dictionary<string, List<IPrecondition>> preconditions,
            IEnumerable<State> states)
        {
            //Optimizacion: calculamos la interseccion de todas las acciones habilitadas
            //y desabilitadas de todos los estados y se la restamos a todos
            var firstState = states.First();
            var enabledIntersection = states.Aggregate(firstState.EnabledActions, (IEnumerable<Action> a, State s) => a.Intersect(s.EnabledActions));
            var disabledIntersection = states.Aggregate(firstState.DisabledActions, (IEnumerable<Action> a, State s) => a.Intersect(s.DisabledActions));
            var conditions = new List<IExpression>();

            foreach (var state in states)
            {
                var enabledActionsId = new HashSet<Action>(state.EnabledActions.Except(enabledIntersection));
                var disabledActionsId = new HashSet<Action>(state.DisabledActions.Except(disabledIntersection));

                var exprs = GenerateStateInvariant(host, new State(enabledActionsId, disabledActionsId));
                var condition = Helper.JoinWithLogicalAnd(host, exprs, true);
                conditions.Add(condition);
            }

            return conditions;
        }

        // Do not include the type invariant
        public static List<IExpression> GenerateStateInvariant(IMetadataHost host, State s)
        {
            var exprs = new List<IExpression>();

            foreach (var action in s.EnabledActions)
            {
                if (action.Contract != null)
                {
                    var conditions = from pre in action.Contract.Preconditions
                                     select pre.Condition;

                    exprs.AddRange(conditions);
                }
            }

            foreach (var action in s.DisabledActions)
            {
                if (action.Contract == null || action.Contract.Preconditions.Count() == 0)
                {
                    var literal = new CompileTimeConstant()
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = false
                    };

                    exprs.Add(literal);
                }
                else
                {
                    var conditions = (from pre in action.Contract.Preconditions
                                      select pre.Condition).ToList();

                    var condition = new LogicalNot()
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Operand = Helper.JoinWithLogicalAnd(host, conditions, true)
                    };

                    exprs.Add(condition);
                }
            }

            return exprs;
        }
    }
}
