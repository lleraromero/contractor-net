using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;

namespace Analysis.Cci
{
    public class CciNegativeActionQueryGenerator : CciActionQueryGenerator
    {
        protected const string MethodNameDelimiter = "~";
        protected const string NotPrefix = "_Not_";

        public CciNegativeActionQueryGenerator(IContractAwareHost host, List<string> listOfExceptions)
            : base(host, listOfExceptions)
        {
        }

        public override ActionQuery CreateQuery(State state, Action action, Action actionUnderTest, string expectedExitCode)
        {
            return new ActionQuery(GenerateQuery(state, action, actionUnderTest,expectedExitCode), QueryType.Negative, actionUnderTest);
        }

        protected override string CreateQueryName(State state, Action action, Action actionUnderTest)
        {
            var actionName = action.Name;
            var stateName = state.Name;
            var targetName = NotPrefix + actionUnderTest.Name;
            return string.Format("{1}{0}{2}{0}{3}", MethodNameDelimiter, stateName, actionName, targetName);
        }

        protected override IMethodContract CreateQueryContract(State state, Action target)
        {
            var queryContract = new MethodContract();
            var targetContract = target.Contract;
            var contractDependencyAnalyzer = new CciContractDependenciesAnalyzer(new ContractProvider(new ContractMethods(host), null));

            // Add preconditions of enabled actions
            foreach (var a in state.EnabledActions)
            {
                var actionContract = a.Contract;
                if (actionContract == null) continue;

                var preconditions = from p in actionContract.Preconditions
                    select new Precondition
                    {
                        Condition = p.Condition,
                        Description = new CompileTimeConstant
                        {
                            Value = string.Format("Enabled action ({0})", a.Name),
                            Type = host.PlatformType.SystemString
                        },
                        OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(p.Condition)
                    };
                queryContract.Preconditions.AddRange(preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x)));
            }

            // Add negated preconditions of disabled actions
            foreach (var a in state.DisabledActions)
            {
                var actionContract = a.Contract;
                if (actionContract == null || !actionContract.Preconditions.Any()) continue;

                var preconditions = from p in actionContract.Preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x))
                    select p.Condition;
                var joinedPreconditions = Helper.LogicalNotAfterJoinWithLogicalAnd(host, preconditions.ToList(), true);
                /*var joinedPreconditions = new LogicalNot
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = Helper.JoinWithLogicalAnd(host, preconditions.ToList(), true)
                };*/
                var compactPrecondition = new Precondition
                {
                    Condition = joinedPreconditions,
                    // Add the user message to identify easily each precondition
                    Description = new CompileTimeConstant
                    {
                        Value = string.Format("Disabled action ({0})", a.Name),
                        Type = host.PlatformType.SystemString
                    },
                    // Add the string-ified version of the condition to help debugging
                    OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(joinedPreconditions)
                };
                queryContract.Preconditions.Add(compactPrecondition);
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