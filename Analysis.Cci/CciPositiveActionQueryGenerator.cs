using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Contractor.Utils;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;

namespace Analysis.Cci
{
    public class CciPositiveActionQueryGenerator : CciActionQueryGenerator
    {
        protected const string MethodNameDelimiter = "~";

        public CciPositiveActionQueryGenerator(IContractAwareHost host) : base(host)
        {
        }

        public override ActionQuery CreateQuery(State state, Action action, Action actionUnderTest)
        {
            return new ActionQuery(GenerateQuery(state, action, actionUnderTest), QueryType.Positive, actionUnderTest);
        }

        protected override string CreateQueryName(State state, Action action, Action actionUnderTest)
        {
            var actionName = action.Name;
            var stateName = state.Name;
            var targetName = actionUnderTest.Name;
            return string.Format("{1}{0}{2}{0}{3}", MethodNameDelimiter, stateName, actionName, targetName);
        }

        protected override IMethodContract CreateQueryContract(State state, Action target)
        {
            var queryContract = new MethodContract();
            var targetContract = target.Contract;

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
                        OriginalSource = Helper.PrintExpression(p.Condition)
                    };
                queryContract.Preconditions.AddRange(preconditions);
            }

            // Add negated preconditions of disabled actions
            foreach (var a in state.DisabledActions)
            {
                var actionContract = a.Contract;
                if (actionContract == null || !actionContract.Preconditions.Any()) continue;

                var preconditions = from p in actionContract.Preconditions
                    select p.Condition;
                var joinedPreconditions = new LogicalNot
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = Helper.JoinWithLogicalAnd(host, preconditions.ToList(), true)
                };
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
                    OriginalSource = Helper.PrintExpression(joinedPreconditions)
                };
                queryContract.Preconditions.Add(compactPrecondition);
            }

            // Now the postconditions
            if (targetContract != null && targetContract.Preconditions.Any())
            {
                var targetPreconditions = from pre in targetContract.Preconditions
                    select new Postcondition
                    {
                        Condition = pre.Condition,
                        OriginalSource = Helper.PrintExpression(pre.Condition),
                        Description = new CompileTimeConstant { Value = "Target precondition", Type = host.PlatformType.SystemString }
                    };
                queryContract.Postconditions.AddRange(targetPreconditions);
            }

            return queryContract;
        }
    }
}