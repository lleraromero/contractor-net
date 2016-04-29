using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;

namespace Analysis.Cci
{
    // TODO: Hacer que estas tres clases sean en realidad un solo visitor

    public class CciContractVisitor : CodeVisitor, ICodeAndContractVisitor
    {
        protected bool instanceIsUsed;

        public CciContractVisitor(IContractProvider contractProvider)
        {
            Contract.Requires(contractProvider != null);
            instanceIsUsed = false;
        }

        public bool InstanceIsUsed
        {
            get { return instanceIsUsed; }
        }

        #region CodeVisitor override
        public override void Visit(IBoundExpression boundExpression)
        {
            if (boundExpression.Instance is IThisReference)
            {
                instanceIsUsed = true;
                return;
            }

            base.Visit(boundExpression);
        }

        public override void Visit(IMethodCall methodCall)
        {
            // I'm going to assume that if it is not a static call then the method makes use of the instance.
            if (!methodCall.IsStaticCall)
            {
                instanceIsUsed = true;
                return;
            }

            base.Visit(methodCall);
        }
        #endregion

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

    public class CciContractElementDispatcher : CodeVisitor, ICodeAndContractVisitor
    {
        protected CciContractTraverser traverser;

        public CciContractElementDispatcher(CciContractTraverser traverser)
        {
            this.traverser = traverser;
        }

        public void Visit(ILoopInvariant loopInvariant)
        {
            traverser.Traverse(loopInvariant);
        }

        public void Visit(IPostcondition postCondition)
        {
            traverser.Traverse(postCondition);
        }

        public void Visit(IPrecondition precondition)
        {
            traverser.Traverse(precondition);
        }

        public void Visit(ITypeInvariant typeInvariant)
        {
            traverser.Traverse(typeInvariant);
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
    }

    /// <summary>
    ///     A class that traverses the contract, code and metadata models in depth first, left to right order to determine if
    ///     the contract predicates about the internal state of the object
    /// </summary>
    public class CciContractTraverser : CodeTraverser
    {
        protected readonly IContractProvider contractProvider;
        protected readonly CciContractElementDispatcher dispatchingVisitor;
        protected readonly ICodeAndContractVisitor preorderVisitor;

        /// <summary>
        ///     A class that traverses the contract, code and metadata models in depth first, left to right order.
        /// </summary>
        public CciContractTraverser(IContractProvider contractProvider, ICodeAndContractVisitor preorderVisitor)
        {
            Contract.Requires(contractProvider != null);
            Contract.Requires(preorderVisitor != null);

            this.contractProvider = contractProvider;

            this.preorderVisitor = preorderVisitor;
            PreorderVisitor = preorderVisitor;

            dispatchingVisitor = new CciContractElementDispatcher(this);
        }

        /// <summary>
        ///     Traverses the contract element.
        /// </summary>
        public void Traverse(IContractElement contractElement)
        {
            Contract.Requires(contractElement != null);
            contractElement.Dispatch(dispatchingVisitor);
        }

        /// <summary>
        ///     Traverses the enumeration of addressable expressions.
        /// </summary>
        public virtual void Traverse(IEnumerable<IAddressableExpression> addressableExpressions)
        {
            Contract.Requires(addressableExpressions != null);
            foreach (var addressableExpression in addressableExpressions)
            {
                Traverse(addressableExpression);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the enumeration of trigger expressions.
        /// </summary>
        public virtual void Traverse(IEnumerable<IEnumerable<IExpression>> triggers)
        {
            Contract.Requires(triggers != null);
            foreach (var trigs in triggers)
            {
                Traverse(trigs);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the enumeration of loop invariants.
        /// </summary>
        public virtual void Traverse(IEnumerable<ILoopInvariant> loopInvariants)
        {
            Contract.Requires(loopInvariants != null);
            foreach (var loopInvariant in loopInvariants)
            {
                Traverse(loopInvariant);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the enumeration of post conditions.
        /// </summary>
        public virtual void Traverse(IEnumerable<IPostcondition> postConditions)
        {
            Contract.Requires(postConditions != null);
            foreach (var postCondition in postConditions)
            {
                Traverse(postCondition);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the enumeration of pre conditions.
        /// </summary>
        public virtual void Traverse(IEnumerable<IPrecondition> preconditions)
        {
            Contract.Requires(preconditions != null);
            foreach (var precondition in preconditions)
            {
                Traverse(precondition);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the enumeration of thrown exceptions.
        /// </summary>
        public virtual void Traverse(IEnumerable<IThrownException> thrownExceptions)
        {
            Contract.Requires(thrownExceptions != null);
            foreach (var thrownException in thrownExceptions)
            {
                Traverse(thrownException);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the enumeration of addressable expressions.
        /// </summary>
        public virtual void Traverse(IEnumerable<ITypeInvariant> typeInvariants)
        {
            Contract.Requires(typeInvariants != null);
            foreach (var typeInvariant in typeInvariants)
            {
                Traverse(typeInvariant);
                if (StopTraversal) return;
            }
        }

        /// <summary>
        ///     Traverses the loop contract.
        /// </summary>
        public void Traverse(ILoopContract loopContract)
        {
            Contract.Requires(loopContract != null);
            preorderVisitor.Visit(loopContract);
            if (StopTraversal) return;
            TraverseChildren(loopContract);
        }

        /// <summary>
        ///     Traverses the loop invariant.
        /// </summary>
        public void Traverse(ILoopInvariant loopInvariant)
        {
            Contract.Requires(loopInvariant != null);
            preorderVisitor.Visit(loopInvariant);
            if (StopTraversal) return;
            TraverseChildren(loopInvariant);
        }

        /// <summary>
        ///     Traverses the method contract.
        /// </summary>
        public void Traverse(IMethodContract methodContract)
        {
            Contract.Requires(methodContract != null);
            preorderVisitor.Visit(methodContract);
            if (StopTraversal) return;
            TraverseChildren(methodContract);
        }

        /// <summary>
        ///     Traverses the postCondition.
        /// </summary>
        public void Traverse(IPostcondition postCondition)
        {
            Contract.Requires(postCondition != null);
            preorderVisitor.Visit(postCondition);
            if (StopTraversal) return;
            TraverseChildren(postCondition);
        }

        /// <summary>
        ///     Traverses the pre condition.
        /// </summary>
        public void Traverse(IPrecondition precondition)
        {
            Contract.Requires(precondition != null);
            preorderVisitor.Visit(precondition);
            if (StopTraversal) return;
            TraverseChildren(precondition);
        }

        /// <summary>
        ///     Traverses the thrown exception.
        /// </summary>
        public void Traverse(IThrownException thrownException)
        {
            Contract.Requires(thrownException != null);
            preorderVisitor.Visit(thrownException);
            if (StopTraversal) return;
            TraverseChildren(thrownException);
        }

        /// <summary>
        ///     Traverses the type contract.
        /// </summary>
        public void Traverse(ITypeContract typeContract)
        {
            Contract.Requires(typeContract != null);
            preorderVisitor.Visit(typeContract);
            if (StopTraversal) return;
            TraverseChildren(typeContract);
        }

        /// <summary>
        ///     Traverses the type invariant.
        /// </summary>
        public void Traverse(ITypeInvariant typeInvariant)
        {
            Contract.Requires(typeInvariant != null);
            preorderVisitor.Visit(typeInvariant);
            if (StopTraversal) return;
            TraverseChildren(typeInvariant);
        }

        /// <summary>
        ///     Called whenever a contract element is about to be traversed by a type specific routine.
        ///     This gives the traverser the opportunity to take some uniform action for all contract elements,
        ///     regardless of how the traversal gets to them.
        /// </summary>
        public virtual void TraverseChildren(IContractElement contractElement)
        {
            Contract.Requires(contractElement != null);
            Traverse(contractElement.Condition);
            if (StopTraversal) return;
            if (contractElement.Description != null)
            {
                Traverse(contractElement.Description);
            }
        }

        /// <summary>
        ///     Traverses the children of the loop contract.
        /// </summary>
        public virtual void TraverseChildren(ILoopContract loopContract)
        {
            Contract.Requires(loopContract != null);
            Traverse(loopContract.Invariants);
            if (StopTraversal) return;
            Traverse(loopContract.Variants);
            if (StopTraversal) return;
            Traverse(loopContract.Writes);
        }

        /// <summary>
        ///     Traverses the children of the loop invariant.
        /// </summary>
        public virtual void TraverseChildren(ILoopInvariant loopInvariant)
        {
            Contract.Requires(loopInvariant != null);
            TraverseChildren((IContractElement)loopInvariant);
        }

        /// <summary>
        ///     Traverses the children of the method call.
        /// </summary>
        public override void TraverseChildren(IMethodCall methodCall)
        {
            base.TraverseChildren(methodCall);
            if (StopTraversal) return;
            if (contractProvider == null) return;
            var /*?*/ triggers = contractProvider.GetTriggersFor(methodCall);
            if (triggers != null)
                Traverse(triggers);
        }

        /// <summary>
        ///     Traverses the children of the method contract.
        /// </summary>
        public virtual void TraverseChildren(IMethodContract methodContract)
        {
            Contract.Requires(methodContract != null);
            Traverse(methodContract.Allocates);
            if (StopTraversal) return;
            Traverse(methodContract.Frees);
            if (StopTraversal) return;
            Traverse(methodContract.ModifiedVariables);
            if (StopTraversal) return;
            Traverse(methodContract.Postconditions);
            if (StopTraversal) return;
            Traverse(methodContract.Preconditions);
            if (StopTraversal) return;
            Traverse(methodContract.Reads);
            if (StopTraversal) return;
            Traverse(methodContract.ThrownExceptions);
            if (StopTraversal) return;
            Traverse(methodContract.Writes);
        }

        /// <summary>
        ///     Traverses the children of the method definition.
        /// </summary>
        public override void TraverseChildren(IMethodDefinition method)
        {
            if (contractProvider != null)
            {
                var /*?*/ methodContract = contractProvider.GetMethodContractFor(method);
                if (methodContract != null)
                {
                    Traverse(methodContract);
                    if (StopTraversal) return;
                }
            }
            base.TraverseChildren(method);
        }

        /// <summary>
        ///     Traverses the children of the postCondition.
        /// </summary>
        public virtual void TraverseChildren(IPostcondition postCondition)
        {
            Contract.Requires(postCondition != null);
            TraverseChildren((IContractElement)postCondition);
        }

        /// <summary>
        ///     Traverses the children of the pre condition.
        /// </summary>
        public virtual void TraverseChildren(IPrecondition precondition)
        {
            Contract.Requires(precondition != null);
            TraverseChildren((IContractElement)precondition);
            if (StopTraversal) return;
            if (precondition.ExceptionToThrow != null)
                Traverse(precondition.ExceptionToThrow);
        }

        /// <summary>
        ///     Traverses the children of the statement.
        /// </summary>
        public override void TraverseChildren(IStatement statement)
        {
            base.TraverseChildren(statement);
            if (contractProvider == null) return;
            var /*?*/ loopContract = contractProvider.GetLoopContractFor(statement);
            if (loopContract != null)
                Traverse(loopContract);
        }

        /// <summary>
        ///     Traverses the children of the thrown exception.
        /// </summary>
        public virtual void TraverseChildren(IThrownException thrownException)
        {
            Contract.Requires(thrownException != null);
            Traverse(thrownException.ExceptionType);
            if (StopTraversal) return;
            Traverse(thrownException.Postcondition);
        }

        /// <summary>
        ///     Traverses the children of the type contract.
        /// </summary>
        public virtual void TraverseChildren(ITypeContract typeContract)
        {
            Contract.Requires(typeContract != null);
            Traverse(typeContract.ContractFields);
            if (StopTraversal) return;
            Traverse(typeContract.ContractMethods);
            if (StopTraversal) return;
            Traverse(typeContract.Invariants);
        }

        /// <summary>
        ///     Traverses the children of the type definition.
        /// </summary>
        public override void TraverseChildren(ITypeDefinition typeDefinition)
        {
            base.TraverseChildren(typeDefinition);
            if (StopTraversal) return;
            if (contractProvider == null) return;
            var /*?*/ typeContract = contractProvider.GetTypeContractFor(typeDefinition);
            if (typeContract != null)
                Traverse(typeContract);
        }

        /// <summary>
        ///     Traverses the children of the type invariant.
        /// </summary>
        public virtual void TraverseChildren(ITypeInvariant typeInvariant)
        {
            Contract.Requires(typeInvariant != null);
            TraverseChildren((IContractElement)typeInvariant);
        }
    }
}