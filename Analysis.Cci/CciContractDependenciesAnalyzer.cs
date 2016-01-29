using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;

namespace Analysis.Cci
{
    public class CciContractDependenciesAnalyzer
    {
        protected readonly IContractProvider contractProvider;

        public CciContractDependenciesAnalyzer(IContractProvider contractProvider)
        {
            this.contractProvider = contractProvider;
        }

        public bool PredicatesAboutInstance(IContractElement postcondition)
        {
            var instanceVisitor = new CciContractVisitor(contractProvider);
            var expressionTraverser = new CciContractTraverser(contractProvider, instanceVisitor);
            expressionTraverser.Traverse(postcondition);            
            return instanceVisitor.InstanceIsUsed;
        }

        public bool PredicatesAboutParameter(IContractElement postcondition)
        {
            var parameterVisitor = new CciContractParameterVisitor(contractProvider);
            var expressionTraverser = new CciContractTraverser(contractProvider, parameterVisitor);
            expressionTraverser.Traverse(postcondition);
            return parameterVisitor.UsesAParameter;
        }

        public class CciContractParameterVisitor : CodeVisitor, ICodeAndContractVisitor
        {
            protected bool usesAParameter;

            public CciContractParameterVisitor(IContractProvider contractProvider)
            {
                Contract.Requires(contractProvider != null);
                usesAParameter = false;
            }

            public bool UsesAParameter
            {
                get { return usesAParameter; }
            }

            public override void Visit(IBoundExpression boundExpression)
            {
                if (boundExpression.Definition is IParameterDefinition)
                {
                    usesAParameter = true;
                    return;
                }

                base.Visit(boundExpression);
            }

            #region ICodeAndContractVisitor interface
            public void Visit(IContractElement contractElement)
            {
                Contract.Assert(contractElement != null);
                Visit(contractElement.Condition);
                if (contractElement.Description != null)
                {
                    Visit(contractElement.Description);
                }
            }

            public void Visit(ILoopInvariant loopInvariant)
            {
                Contract.Assert(loopInvariant != null);
                Visit((IContractElement)loopInvariant);
            }

            public void Visit(IPostcondition postCondition)
            {
                Contract.Assert(postCondition != null);
                Visit((IContractElement)postCondition);
            }

            public void Visit(IPrecondition precondition)
            {
                Contract.Assert(precondition != null);
                Visit((IContractElement)precondition);
                if (precondition.ExceptionToThrow != null)
                {
                    Visit(precondition.ExceptionToThrow);
                }
            }

            public void Visit(ITypeInvariant typeInvariant)
            {
                Contract.Assert(typeInvariant != null);
                Visit((IContractElement)typeInvariant);
            }

            public void Visit(ILoopContract loopContract)
            {
                Contract.Assume(false);
            }

            public void Visit(IMethodContract methodContract)
            {
                Contract.Assume(false);
            }

            public void Visit(IThrownException thrownException)
            {
                Contract.Assume(false);
            }

            public void Visit(ITypeContract typeContract)
            {
                Contract.Assume(false);
            }
            #endregion
        }
    }
}