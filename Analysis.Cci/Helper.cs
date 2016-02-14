using System.Collections.Generic;
using System.Linq;
using Contractor.Core.Model;
using CSharpSourceEmitter;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using SourceEmitter = CSharpSourceEmitter.SourceEmitter;

namespace Contractor.Utils
{
    public static class Helper
    {
        // A && B = A ? B : false
        public static IExpression JoinWithLogicalAnd(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
        {
            if (expressions.Count == 0)
                return new CompileTimeConstant
                {
                    Type = host.PlatformType.SystemBoolean,
                    Value = defaultValue
                };

            var result = expressions[0];

            for (var i = 1; i < expressions.Count; ++i)
            {
                result = new Conditional
                {
                    Type = host.PlatformType.SystemBoolean,
                    Condition = result,
                    ResultIfTrue = expressions[i],
                    ResultIfFalse = new CompileTimeConstant
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = false
                    }
                };
            }

            return result;
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
                if (action.Contract == null || !action.Contract.Preconditions.Any())
                {
                    var literal = new CompileTimeConstant
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

                    var condition = new LogicalNot
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Operand = JoinWithLogicalAnd(host, conditions, true)
                    };

                    exprs.Add(condition);
                }
            }

            return exprs;
        }
    }
}