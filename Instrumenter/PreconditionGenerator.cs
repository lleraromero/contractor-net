using System.Collections.Generic;
using Analysis.Cci;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;

namespace Instrumenter
{
    public class PreconditionGenerator
    {
        public Precondition GeneratePrecondition(FieldDefinition field, IReadOnlyCollection<int> fromStateIds)
        {
            var host = CciHostEnvironment.GetInstance();
            var conditions = new List<IExpression>();

            foreach (var fromId in fromStateIds)
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
                        Value = fromId
                    }
                };

                conditions.Add(cond);
            }

            return new Precondition
            {
                Condition = Helper.JoinWithLogicalOr(host, conditions, false),
                OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(Helper.JoinWithLogicalOr(host, conditions, false))
            };
        }
    }
}