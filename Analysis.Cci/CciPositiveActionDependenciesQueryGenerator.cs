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
    public class CciPositiveActionDependenciesQueryGenerator : CciActionDependenciesQueryGenerator
    {
        protected const string MethodNameDelimiter = "~";
        public CciPositiveActionDependenciesQueryGenerator(Microsoft.Cci.Contracts.IContractAwareHost host, List<string> listOfExceptions, bool considerExitCode)
            : base(host, listOfExceptions, considerExitCode)
        {
        }

        public override ActionQuery CreateQuery( Action action, Action actionUnderTest)
        {
            return new ActionQuery(GenerateQuery(action, actionUnderTest), QueryType.Positive, actionUnderTest);
        }

        protected override string CreateQueryName(Action action, Action actionUnderTest)
        {
            var actionName = action.Name;
            var targetName = actionUnderTest.Name;
            return string.Format("{1}{0}{2}", MethodNameDelimiter, actionName, targetName);
        }

        protected override IMethodContract CreateQueryContract(Action action, Action target)
        {
            var queryContract = new MethodContract();
            var targetContract = target.Contract;
            var contractDependencyAnalyzer = new CciContractDependenciesAnalyzer(new ContractProvider(new ContractMethods(host), null));

            var actionContract = action.Contract;
            if (actionContract != null){
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
            if (targetContract != null && targetContract.Preconditions.Any())
            {
                var targetPreconditions = from pre in targetContract.Preconditions.Where(x => !contractDependencyAnalyzer.PredicatesAboutParameter(x))
                                          select new Postcondition
                                          {
                                              Condition = pre.Condition,
                                              OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(pre.Condition),
                                              Description = new CompileTimeConstant { Value = "Target precondition", Type = host.PlatformType.SystemString }
                                          };
                queryContract.Postconditions.AddRange(targetPreconditions);
            }

            return queryContract;
        }
    }
}
