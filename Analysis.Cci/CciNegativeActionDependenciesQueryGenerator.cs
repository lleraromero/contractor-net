using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Analysis.Cci
{
    public class CciNegativeActionDependenciesQueryGenerator : CciActionDependenciesQueryGenerator
    {
        protected const string MethodNameDelimiter = "~";
        protected const string NotPrefix = "_Not_";

        public CciNegativeActionDependenciesQueryGenerator(Microsoft.Cci.Contracts.IContractAwareHost host, List<string> listOfExceptions, bool considerExitCode)
            : base(host, listOfExceptions, considerExitCode)
        {
        }
        public override ActionQuery CreateQuery(Action action, Action actionUnderTest)
        {
            return new ActionQuery(GenerateQuery(action, actionUnderTest), QueryType.Negative, actionUnderTest);
        }

        protected override string CreateQueryName(Action action, Action actionUnderTest)
        {
            var actionName = action.Name;
            var targetName = NotPrefix + actionUnderTest.Name;
            return string.Format("{1}{0}{2}", MethodNameDelimiter, actionName, targetName);
        }
        protected override IMethodContract CreateQueryContract(Action action,Action target)
        {
            var queryContract = new MethodContract();
            var targetContract = target.Contract;
            var contractDependencyAnalyzer = new CciContractDependenciesAnalyzer(new ContractProvider(new ContractMethods(host), null));

            var actionContract = action.Contract;
            if (actionContract != null)
            {
                var preconditions = from p in actionContract.Preconditions
                                    select new Precondition
                                    {
                                        Condition = p.Condition,
                                        Description = new CompileTimeConstant
                                        {
                                            Value = string.Format("Enabled action ({0})", action.Name),
                                            Type = host.PlatformType.SystemString
                                        },
                                        OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(p.Condition)
                                    };
                queryContract.Preconditions.AddRange(preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x)));
            }
            
            // Now the postconditions
            // Having no preconditions is the same as having the 'true' precondition
            if (targetContract == null || !targetContract.Preconditions.Any())
            {
                var post = new Postcondition
                {
                    Condition = new CompileTimeConstant
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Value = false
                    },
                    OriginalSource = "false",
                    Description = new CompileTimeConstant { Value = "Target negated precondition", Type = host.PlatformType.SystemString }
                };

                queryContract.Postconditions.Add(post);
            }
            else
            {
                var exprs = (from pre in targetContract.Preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x))
                             select pre.Condition).ToList();

                var post = new Postcondition
                {
                    /*Condition = new LogicalNot
                    {
                        Type = host.PlatformType.SystemBoolean,
                        Operand = Helper.JoinWithLogicalAnd(host, exprs, true)
                    }*/
                    Condition = Helper.LogicalNotAfterJoinWithLogicalAnd(host, exprs, true),
                    OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(Helper.JoinWithLogicalAnd(host, exprs, true)),
                    Description = new CompileTimeConstant { Value = "Target negated precondition", Type = host.PlatformType.SystemString }
                };
                queryContract.Postconditions.Add(post);
            }
            return queryContract;
        }
    }
}
