using System.Collections.Generic;
using System.Linq;
using Analysis.Cci;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;

namespace Instrumenter
{
    public class PostconditionGenerator
    {
        protected readonly int initialStateId;

        public PostconditionGenerator(int initialStateId)
        {
            this.initialStateId = initialStateId;
        }

        public List<Postcondition> GeneratePostconditions(FieldDefinition field, Dictionary<int, List<int>> transitions)
        {
            return transitions.Select(t => GeneratePostcondition(field, t.Key, t.Value)).ToList();
        }

        protected Postcondition GeneratePostcondition(FieldDefinition field, int fromStateId, IReadOnlyCollection<int> toStateIds)
        {
            var host = CciHostEnvironment.GetInstance();
            IExpression conditional;

            if (fromStateId.Equals(initialStateId))
            {
                //Initial state
                conditional = GenerateConditionPartTo(field, toStateIds);
            }
            else
            {
                conditional = new Conditional
                {
                    Type = host.PlatformType.SystemBoolean,
                    Condition = GenerateConditionPartFrom(field, fromStateId),
                    ResultIfTrue = GenerateConditionPartTo(field, toStateIds),
                    ResultIfFalse = new CompileTimeConstant
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = true
                    }
                };
            }

            return new Postcondition
            {
                Condition = conditional,
                OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(conditional)
            };
        }

        protected IExpression GenerateConditionPartFrom(FieldDefinition field, int fromStateId)
        {
            var host = CciHostEnvironment.GetInstance();
            return new Equality
            {
                Type = host.PlatformType.SystemBoolean,
                LeftOperand = new OldValue
                {
                    Type = field.Type,
                    Expression = new BoundExpression
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    }
                },
                RightOperand = new CompileTimeConstant
                {
                    Type = field.Type,
                    Value = fromStateId
                }
            };
        }

        protected IExpression GenerateConditionPartTo(FieldDefinition field, IReadOnlyCollection<int> toStateIds)
        {
            var host = CciHostEnvironment.GetInstance();
            var conditions = new List<IExpression>();

            foreach (var toStateId in toStateIds)
            {
                var cond = new Equality
                {
                    Type = host.PlatformType.SystemBoolean,
                    LeftOperand = new BoundExpression
                    {
                        Definition = field,
                        Instance = new ThisReference(),
                        Type = field.Type
                    },
                    RightOperand = new CompileTimeConstant
                    {
                        Type = field.Type,
                        Value = toStateId
                    }
                };

                conditions.Add(cond);
            }

            return Helper.JoinWithLogicalOr(host, conditions, false);
        }
    }
}