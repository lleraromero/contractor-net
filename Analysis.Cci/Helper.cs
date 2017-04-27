using System.Collections.Generic;
using System.Linq;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using Microsoft.Cci.Contracts;

namespace Analysis.Cci
{
    public static class Helper
    {
        public static IExpression LogicalNotAfterJoinWithLogicalAnd(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
        {
            List<IExpression> negatedExpressions = new List<IExpression>();
            for (var i = 0; i < expressions.Count; ++i)
            {
                var notExpr = new LogicalNot
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = expressions[i]
                }; 
                negatedExpressions.Add(notExpr);
            }
            return JoinWithLogicalOr(host, negatedExpressions, !defaultValue);

        }

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

        // A || B = A ? true : B
        public static IExpression JoinWithLogicalOr(IMetadataHost host, List<IExpression> expressions, bool defaultValue)
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
                    ResultIfTrue = new CompileTimeConstant
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = true
                    },
                    ResultIfFalse = expressions[i]
                };
            }

            return result;
        }

        // Do not include the type invariant
        public static List<IExpression> GenerateStateInvariant(IMetadataHost host, State s)
        {
            var exprs = new List<IExpression>();
            var contractDependencyAnalyzer = new CciContractDependenciesAnalyzer(new ContractProvider(new ContractMethods(host), null));

            foreach (var action in s.EnabledActions)
            {
                if (action.Contract != null)
                {
                    var conditions = from pre in action.Contract.Preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x))
                        select pre.Condition;

                    exprs.AddRange(conditions);
                }
            }

            foreach (var action in s.DisabledActions)
            {
                if (action.Contract == null || !action.Contract.Preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x)).Any())
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
                    var conditions = (from pre in action.Contract.Preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x))
                        select pre.Condition).ToList();

                    var condition = Helper.LogicalNotAfterJoinWithLogicalAnd(host, conditions, true);
                    /*var condition = new LogicalNot
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Operand = JoinWithLogicalAnd(host, conditions, true)
                    };*/

                    exprs.Add(condition);
                }
            }

            return exprs;
        }
    }
}