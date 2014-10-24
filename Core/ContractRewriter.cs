﻿using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    public class ContractRewriter : CodeAndContractRewriter
    {
        private readonly ITypeReference systemVoid;
        private ISourceLocationProvider sourceLocationProvider;

        public ContractRewriter(IMetadataHost host, ContractProvider contractProvider, ISourceLocationProvider sourceLocationProvider)
            : base(host, contractProvider)
        {
            this.systemVoid = host.PlatformType.SystemVoid;
            this.sourceLocationProvider = sourceLocationProvider;
        }

        public override INamespaceTypeDefinition Rewrite(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            this.VisitTypeDefinition(namespaceTypeDefinition);
            return base.Rewrite(namespaceTypeDefinition);
        }

        public override INestedTypeDefinition Rewrite(INestedTypeDefinition nestedTypeDefinition)
        {
            this.VisitTypeDefinition(nestedTypeDefinition);
            return base.Rewrite(nestedTypeDefinition);
        }

        /// <summary>
        /// If the <paramref name="typeDefinition"/> has a type contract, generate a
        /// contract invariant method and add it to the Methods of the <paramref name="typeDefinition"/>.
        /// </summary>
        private void VisitTypeDefinition(ITypeDefinition typeDefinition)
        {
            ITypeContract typeContract = this.contractProvider.GetTypeContractFor(typeDefinition);
            if (typeContract != null)
            {
                #region Define the method

                List<IStatement> statements = new List<IStatement>();
                var methodBody = new SourceMethodBody(this.host)
                {
                    LocalsAreZeroed = true,
                    Block = new BlockStatement() { Statements = statements }
                };
                List<ICustomAttribute> attributes = new List<ICustomAttribute>();
                MethodDefinition m = new MethodDefinition()
                {
                    Attributes = attributes,
                    Body = methodBody,
                    CallingConvention = CallingConvention.HasThis,
                    ContainingTypeDefinition = typeDefinition,
                    InternFactory = this.host.InternFactory,
                    IsStatic = false,
                    Name = this.host.NameTable.GetNameFor("$InvariantMethod$"),
                    Type = systemVoid,
                    Visibility = TypeMemberVisibility.Private,
                };
                methodBody.MethodDefinition = m;

                #region Add calls to Contract.Invariant

                foreach (var inv in typeContract.Invariants)
                {
                    var methodCall = new MethodCall()
                    {
                        Arguments = new List<IExpression> { inv.Condition, },
                        IsStaticCall = true,
                        MethodToCall = this.contractProvider.ContractMethods.Invariant,
                        Type = systemVoid,
                        Locations = new List<ILocation>(inv.Locations),
                    };
                    ExpressionStatement es = new ExpressionStatement()
                    {
                        Expression = methodCall
                    };
                    statements.Add(es);
                }
                statements.Add(new ReturnStatement());

                #endregion Add calls to Contract.Invariant

                #region Add [ContractInvariantMethod]

                var contractInvariantMethodType = new Microsoft.Cci.Immutable.NamespaceTypeReference(
                  this.host,
                  this.host.PlatformType.SystemDiagnosticsContractsContract.ContainingUnitNamespace,
                  this.host.NameTable.GetNameFor("ContractInvariantMethodAttribute"),
                  0,
                  false,
                  false,
                  true,
                  PrimitiveTypeCode.NotPrimitive
                  );
                var contractInvariantMethodCtor = new Microsoft.Cci.MutableCodeModel.MethodReference()
                {
                    CallingConvention = CallingConvention.HasThis,
                    ContainingType = contractInvariantMethodType,
                    GenericParameterCount = 0,
                    InternFactory = this.host.InternFactory,
                    Name = host.NameTable.Ctor,
                    Type = host.PlatformType.SystemVoid,
                };
                var contractInvariantMethodAttribute = new CustomAttribute();
                contractInvariantMethodAttribute.Constructor = contractInvariantMethodCtor;
                attributes.Add(contractInvariantMethodAttribute);

                #endregion Add [ContractInvariantMethod]

                var namedTypeDefinition = (NamedTypeDefinition)typeDefinition;

                var newMethods = new List<IMethodDefinition>(namedTypeDefinition.Methods == null ? 1 : namedTypeDefinition.Methods.Count() + 1);
                if (namedTypeDefinition.Methods != null)
                {
                    foreach (var meth in namedTypeDefinition.Methods)
                    {
                        if (!ContractHelper.IsInvariantMethod(this.host, meth))
                            newMethods.Add(meth);
                    }
                }
                namedTypeDefinition.Methods = newMethods;
                namedTypeDefinition.Methods.Add(m);

                #endregion Define the method
            }
        }

        public override void RewriteChildren(MethodDefinition methodDefinition)
        {
            IMethodContract methodContract = this.contractProvider.GetMethodContractFor(methodDefinition);
            if (methodContract == null) return;
            ISourceMethodBody sourceMethodBody = methodDefinition.Body as ISourceMethodBody;
            if (sourceMethodBody == null) return;

            var newStatements = new List<IStatement>();

            List<IStatement> existingStatements = new List<IStatement>(sourceMethodBody.Block.Statements);
            existingStatements = this.Rewrite(existingStatements);

            // keep the call to the base constructor at the top
            if (methodDefinition.IsConstructor && existingStatements.Count > 0)
            {
                newStatements.Add(existingStatements[0]);
                existingStatements.RemoveAt(0);
            }

            foreach (var precondition in methodContract.Preconditions)
            {
                var methodCall = new MethodCall()
                {
                    Arguments = new List<IExpression> { precondition.Condition },
                    IsStaticCall = true,
                    MethodToCall = this.contractProvider.ContractMethods.Assume,
                    Type = systemVoid,
                    Locations = new List<ILocation>(precondition.Locations),
                };
                ExpressionStatement es = new ExpressionStatement()
                {
                    Expression = methodCall
                };
                newStatements.Add(es);
            }

            newStatements.AddRange(existingStatements); // replaces assert/assume?

            foreach (var postcondition in methodContract.Postconditions)
            {
                var methodCall = new MethodCall()
                {
                    Arguments = new List<IExpression> { postcondition.Condition, },
                    IsStaticCall = true,
                    MethodToCall = this.contractProvider.ContractMethods.Assert,
                    Type = systemVoid,
                    Locations = new List<ILocation>(postcondition.Locations),
                };
                ExpressionStatement es = new ExpressionStatement()
                {
                    Expression = methodCall
                };
                newStatements.Add(es);
            }

            var newSourceMethodBody = new SourceMethodBody(this.host, this.sourceLocationProvider)
            {
                Block = new BlockStatement()
                {
                    Statements = newStatements,
                },
                IsNormalized = false,
                LocalsAreZeroed = sourceMethodBody.LocalsAreZeroed,
                MethodDefinition = methodDefinition,
            };
            methodDefinition.Body = newSourceMethodBody;
            return;
        }

        /// <summary>
        /// Converts the assert statement into a call to Contract.Assert
        /// </summary>
        public override IStatement Rewrite(IAssertStatement assertStatement)
        {
            var methodCall = new MethodCall()
            {
                Arguments = new List<IExpression> { assertStatement.Condition, },
                IsStaticCall = true,
                MethodToCall = this.contractProvider.ContractMethods.Assert,
                Type = systemVoid,
                Locations = new List<ILocation>(assertStatement.Locations),
            };
            ExpressionStatement es = new ExpressionStatement()
            {
                Expression = methodCall
            };
            return es;
        }

        /// <summary>
        /// Converts the assume statement into a call to Contract.Assume
        /// </summary>
        public override IStatement Rewrite(IAssumeStatement assumeStatement)
        {
            var methodCall = new MethodCall()
            {
                Arguments = new List<IExpression> { assumeStatement.Condition, },
                IsStaticCall = true,
                MethodToCall = this.contractProvider.ContractMethods.Assume,
                Type = systemVoid,
                Locations = new List<ILocation>(assumeStatement.Locations),
            };
            ExpressionStatement es = new ExpressionStatement()
            {
                Expression = methodCall
            };
            return es;
        }

        /// <summary>
        /// Converts the old value into a call to Contract.OldValue
        /// </summary>
        public override IExpression Rewrite(IOldValue oldValue)
        {
            var mref = this.contractProvider.ContractMethods.Old;
            var methodToCall = new Microsoft.Cci.MutableCodeModel.GenericMethodInstanceReference()
            {
                CallingConvention = CallingConvention.Generic,
                ContainingType = mref.ContainingType,
                GenericArguments = new List<ITypeReference> { oldValue.Type },
                GenericMethod = mref,
                InternFactory = this.host.InternFactory,
                Name = mref.Name,
                Parameters = new List<IParameterTypeInformation> { new ParameterTypeInformation { Type = oldValue.Type, } },
                Type = oldValue.Type,
            };
            var methodCall = new MethodCall()
            {
                Arguments = new List<IExpression> { oldValue.Expression, },
                IsStaticCall = true,
                MethodToCall = methodToCall,
                Type = oldValue.Type,
                Locations = new List<ILocation>(oldValue.Locations),
            };
            return methodCall;
        }

        /// <summary>
        /// Converts the return value into a call to Contract.Result
        /// </summary>
        public override IExpression Rewrite(IReturnValue returnValue)
        {
            var mref = this.contractProvider.ContractMethods.Result;
            var methodToCall = new Microsoft.Cci.MutableCodeModel.GenericMethodInstanceReference()
            {
                CallingConvention = CallingConvention.Generic,
                ContainingType = mref.ContainingType,
                GenericArguments = new List<ITypeReference> { returnValue.Type },
                GenericMethod = mref,
                InternFactory = this.host.InternFactory,
                Name = mref.Name,
                Type = returnValue.Type,
            };
            var methodCall = new MethodCall()
            {
                IsStaticCall = true,
                MethodToCall = methodToCall,
                Type = returnValue.Type,
                Locations = new List<ILocation>(returnValue.Locations),
            };
            return methodCall;
        }
    }
}