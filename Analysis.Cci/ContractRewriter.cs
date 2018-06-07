using System.Collections.Generic;
using System.Linq;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using MethodReference = Microsoft.Cci.MethodReference;
using NamespaceTypeReference = Microsoft.Cci.Immutable.NamespaceTypeReference;

namespace Analysis.Cci
{
    public class ContractRewriter : CodeAndContractRewriter
    {
        protected readonly ITypeReference systemVoid;
        protected readonly ISourceLocationProvider sourceLocationProvider;
        //public string expectedExitCode;
        public ContractRewriter(IMetadataHost host, ContractProvider contractProvider, ISourceLocationProvider sourceLocationProvider)
            : base(host, contractProvider)
        {
            systemVoid = host.PlatformType.SystemVoid;
            this.sourceLocationProvider = sourceLocationProvider;
        }

        protected IMethodReference AssumeReference
        {
            get
            {
                return new MethodReference(host, host.PlatformType.SystemDiagnosticsContractsContract, CallingConvention.Default,
                    host.PlatformType.SystemVoid, host.NameTable.GetNameFor("Assume"), 0, host.PlatformType.SystemBoolean,
                    host.PlatformType.SystemString);
            }
        }

        protected IMethodReference AssertReference
        {
            get
            {
                return new MethodReference(host, host.PlatformType.SystemDiagnosticsContractsContract, CallingConvention.Default,
                    host.PlatformType.SystemVoid, host.NameTable.GetNameFor("Assert"), 0, host.PlatformType.SystemBoolean,
                    host.PlatformType.SystemString);
            }
        }

        public override INamespaceTypeDefinition Rewrite(INamespaceTypeDefinition namespaceTypeDefinition)
        {
            var typeContract = contractProvider.GetTypeContractFor(namespaceTypeDefinition);
            VisitTypeDefinition(namespaceTypeDefinition, typeContract);
            return base.Rewrite(namespaceTypeDefinition);
        }

        public override INestedTypeDefinition Rewrite(INestedTypeDefinition nestedTypeDefinition)
        {
            var typeContract = contractProvider.GetTypeContractFor(nestedTypeDefinition);
            VisitTypeDefinition(nestedTypeDefinition, typeContract);
            return base.Rewrite(nestedTypeDefinition);
        }

        /// <summary>
        ///     If the <paramref name="typeDefinition" /> has a type contract, generate a
        ///     contract invariant method and add it to the Methods of the <paramref name="typeDefinition" />.
        /// </summary>
        protected void VisitTypeDefinition(ITypeDefinition typeDefinition, ITypeContract typeContract)
        {
            if (typeContract == null)
            {
                return;
            }

            #region Define the method

            var statements = new List<IStatement>();
            var methodBody = new SourceMethodBody(host)
            {
                LocalsAreZeroed = true,
                Block = new BlockStatement { Statements = statements }
            };
            var attributes = new List<ICustomAttribute>();
            var m = new MethodDefinition
            {
                Attributes = attributes,
                Body = methodBody,
                CallingConvention = CallingConvention.HasThis,
                ContainingTypeDefinition = typeDefinition,
                InternFactory = host.InternFactory,
                IsStatic = false,
                Name = host.NameTable.GetNameFor("$InvariantMethod$"),
                Type = systemVoid,
                Visibility = TypeMemberVisibility.Private
            };
            methodBody.MethodDefinition = m;

            #region Add calls to Contract.Invariant

            foreach (var inv in typeContract.Invariants)
            {
                var methodCall = new MethodCall
                {
                    Arguments = new List<IExpression> { inv.Condition },
                    IsStaticCall = true,
                    MethodToCall = contractProvider.ContractMethods.Invariant,
                    Type = systemVoid,
                    Locations = new List<ILocation>(inv.Locations)
                };
                var es = new ExpressionStatement
                {
                    Expression = methodCall
                };
                statements.Add(es);
            }
            statements.Add(new ReturnStatement());

            #endregion Add calls to Contract.Invariant

            #region Add [ContractInvariantMethod]

            var contractInvariantMethodType = new NamespaceTypeReference(
                host,
                host.PlatformType.SystemDiagnosticsContractsContract.ContainingUnitNamespace,
                host.NameTable.GetNameFor("ContractInvariantMethodAttribute"),
                0,
                false,
                false,
                true,
                PrimitiveTypeCode.NotPrimitive
                );
            var contractInvariantMethodCtor = new Microsoft.Cci.MutableCodeModel.MethodReference
            {
                CallingConvention = CallingConvention.HasThis,
                ContainingType = contractInvariantMethodType,
                GenericParameterCount = 0,
                InternFactory = host.InternFactory,
                Name = host.NameTable.Ctor,
                Type = host.PlatformType.SystemVoid
            };
            var contractInvariantMethodAttribute = new CustomAttribute();
            contractInvariantMethodAttribute.Constructor = contractInvariantMethodCtor;
            attributes.Add(contractInvariantMethodAttribute);

            #endregion Add [ContractInvariantMethod]

            var namedTypeDefinition = (NamedTypeDefinition) typeDefinition;

            var newMethods = new List<IMethodDefinition>(namedTypeDefinition.Methods == null ? 1 : namedTypeDefinition.Methods.Count() + 1);
            if (namedTypeDefinition.Methods != null)
            {
                foreach (var meth in namedTypeDefinition.Methods)
                {
                    if (!ContractHelper.IsInvariantMethod(host, meth))
                        newMethods.Add(meth);
                }
            }
            namedTypeDefinition.Methods = newMethods;
            namedTypeDefinition.Methods.Add(m);

            #endregion Define the method
        }

        public override void RewriteChildren(MethodDefinition methodDefinition)
        {
            var methodContract = contractProvider.GetMethodContractFor(methodDefinition);
            if (methodContract == null) return;
            var sourceMethodBody = methodDefinition.Body as ISourceMethodBody;
            if (sourceMethodBody == null) return;

            var newStatements = new List<IStatement>();

            // add the preconditions as assumes
            foreach (var precondition in methodContract.Preconditions)
            {
                var methodCall = new MethodCall
                {
                    Arguments =
                        new List<IExpression>
                        {
                            Rewrite(precondition.Condition),
                            precondition.Description ?? new CompileTimeConstant { Type = host.PlatformType.SystemString, Value = "Precondition" }
                        },
                    IsStaticCall = true,
                    MethodToCall = AssumeReference,
                    Type = systemVoid,
                    Locations = new List<ILocation>(precondition.Locations)
                };
                var es = new ExpressionStatement
                {
                    Expression = methodCall
                };
                newStatements.Add(es);
            }

            var existingStatements = new List<IStatement>(sourceMethodBody.Block.Statements);
            existingStatements = Rewrite(existingStatements);

            // keep the call to the base constructor at the top
            if (methodDefinition.IsConstructor && existingStatements.Count > 0)
            {
                newStatements.Add(existingStatements[0]);
                existingStatements.RemoveAt(0);
            }


            /* if the query returns a value, instead of copying the postconditions at the end of each
             * single return in the code, we assign the value at that point to a local variable and jump
             * to the beginning of the postcondition block.
             */

            // add a new local variable to store the value at return
            LocalDeclarationStatement retLocal = null;
            if (TypeHelper.GetTypeName(methodDefinition.Type) != TypeHelper.GetTypeName(host.PlatformType.SystemVoid))
            {
                var typeCode = TypeHelper.GetSytemTypeCodeFor(methodDefinition.Type.ResolvedType);
                object value;
                switch (typeCode)
                {
                    case System.TypeCode.Boolean:
                        value = false;
                        break;

                    case System.TypeCode.Int16:
                    case System.TypeCode.UInt16:
                    case System.TypeCode.Int32:
                    case System.TypeCode.UInt32:
                    case System.TypeCode.Int64:
                    case System.TypeCode.UInt64:
                        value = 0;
                        break;

                    case System.TypeCode.Single:
                    case System.TypeCode.Double:
                    case System.TypeCode.Decimal:
                        value = 0.0;
                        break;

                    case System.TypeCode.Object:
                    case System.TypeCode.String:
                    case System.TypeCode.DateTime:
                        value = null;
                        break;

                    case System.TypeCode.Byte:
                    case System.TypeCode.SByte:
                        value = default(byte);
                        break;

                    case System.TypeCode.Char:
                        value = default(char);
                        break;

                    default:
                        throw new System.Exception("Default case");
                }
        /*
         * DEFAULT CASE COULD INCLUDE:
        Empty = 0,
        
        //
        // Resumen:
        //     Un valor de null (columna) de la base de datos.
        DBNull = 2,
 
        */      
             
                retLocal = new LocalDeclarationStatement
                {
                    LocalVariable = new LocalDefinition
                    {
                        Name = host.NameTable.GetNameFor("retLocal"),
                        Type = methodDefinition.Type
                    },
                    InitialValue = new CompileTimeConstant { Type = methodDefinition.Type, Value = value }
                }; 
                newStatements.Add(retLocal);
            }

            // Add the invariant as a precondition as well
            var typeContract = contractProvider.GetTypeContractFor(methodDefinition.ContainingTypeDefinition);
            if (typeContract != null)
            {
                foreach (var inv in typeContract.Invariants)
                {
                    var methodCall = new MethodCall
                    {
                        Arguments =
                            new List<IExpression>
                            {
                                Rewrite(inv.Condition),
                                new CompileTimeConstant { Value = "Type invariant", Type = host.PlatformType.SystemString }
                            },
                        IsStaticCall = true,
                        MethodToCall = AssumeReference,
                        Type = systemVoid,
                        Locations = new List<ILocation>(inv.Locations)
                    };
                    var es = new ExpressionStatement
                    {
                        Expression = methodCall
                    };
                    newStatements.Add(es);
                }
            }

            // the dummy statement is going to indicate the beginning of the postcondition block
            var dummyPostconditionStatement = new LabeledStatement
            {
                Label = host.NameTable.GetNameFor("dummyPostconditionStatement"),
                Statement = new EmptyStatement()
            };

            var retRewriter = new ReturnRewriter(host, dummyPostconditionStatement, retLocal);
            //retRewriter.expectedExitCode = expectedExitCode;

            //replace (nested) ReturnStatements with the GoTo to a single return at the end
            newStatements.AddRange(existingStatements.Select(stmt => retRewriter.Rewrite(stmt)));

            // now, that all the existing statements were added it is time for the postcondition block
            newStatements.Add(dummyPostconditionStatement);
            //newStatements.Insert(newStatements.Count - 1,dummyPostconditionStatement);

            //****************************
            // move the localDeclarations to this point
            bool foundLocals=false;
            var localDeclStatements = new List<IStatement>();
            int i = 0;
            while (!foundLocals && i < newStatements.Count)
            {
                if (newStatements[i] is LabeledStatement)
                {
                    var labeled=(newStatements[i] as LabeledStatement);
                    if (labeled.Label.Value.Equals("begin"))
                        foundLocals = true;
                    else
                        i++;
                }
                else {
                    i++;
                }
                
            }
            foundLocals=false;
            while (!foundLocals && i<newStatements.Count)
            {
                if (newStatements[i] is LabeledStatement)
                {
                    if ((newStatements[i] as LabeledStatement).Label.Value.Equals("end"))
                    {
                        foundLocals = true;
                    }
                    newStatements.RemoveAt(i);
                }
                else
                {
                    localDeclStatements.Add(newStatements[i]);
                    newStatements.RemoveAt(i);
                }
            }
            newStatements.AddRange(localDeclStatements);

            // Add assume statements for each postcondition that predicates ONLY about parameters (ie. not about the instance)
            var contractDependencyAnalyzer = new CciContractDependenciesAnalyzer(contractProvider);
            foreach (var postcondition in methodContract.Postconditions)
            {
                if (contractDependencyAnalyzer.PredicatesAboutInstance(postcondition) ||
                    !contractDependencyAnalyzer.PredicatesAboutParameter(postcondition))
                {
                    continue;
                }

                var methodCall = new MethodCall
                {
                    Arguments =
                        new List<IExpression>
                        {
                            Rewrite(postcondition.Condition),
                            new CompileTimeConstant { Type = host.PlatformType.SystemString, Value = "Conditions over parameters are assumed satisfiable" }
                        },
                    IsStaticCall = true,
                    MethodToCall = AssumeReference,
                    Type = systemVoid,
                    Locations = new List<ILocation>(postcondition.Locations)
                };
                var es = new ExpressionStatement
                {
                    Expression = methodCall
                };
                newStatements.Add(es);
            }

            // the postcondition block. Add each postcondition as an assert
            foreach (var postcondition in methodContract.Postconditions)
            {
                var methodCall = new MethodCall
                {
                    Arguments =
                        new List<IExpression>
                        {
                            Rewrite(postcondition.Condition),
                            postcondition.Description ?? new CompileTimeConstant { Type = host.PlatformType.SystemString, Value = "Postcondition" }
                        },
                    IsStaticCall = true,
                    MethodToCall = AssertReference,
                    Type = systemVoid,
                    Locations = new List<ILocation>(postcondition.Locations)
                };
                var es = new ExpressionStatement
                {
                    Expression = methodCall
                };
                newStatements.Add(es);
            }

            // add the invariant as a postcondition as well
            //if (typeContract != null)
            //{
            //    foreach (var inv in typeContract.Invariants)
            //    {
            //        var methodCall = new MethodCall
            //        {
            //            Arguments =
            //                new List<IExpression>
            //                {
            //                    Rewrite(inv.Condition),
            //                    new CompileTimeConstant { Value = "Type invariant", Type = host.PlatformType.SystemString }
            //                },
            //            IsStaticCall = true,
            //            MethodToCall = AssertReference,
            //            Type = systemVoid,
            //            Locations = new List<ILocation>(inv.Locations)
            //        };
            //        var es = new ExpressionStatement
            //        {
            //            Expression = methodCall
            //        };
            //        newStatements.Add(es);
            //    }
            //}

            IReturnStatement returnStatement;
            if (TypeHelper.GetTypeName(methodDefinition.Type) != TypeHelper.GetTypeName(host.PlatformType.SystemVoid))
            {
                // If the method is not void, the return statement have to include the local variable
                returnStatement = new ReturnStatement
                {
                    Expression = new BoundExpression { Definition = retLocal.LocalVariable, Instance = null, Type = retLocal.LocalVariable.Type }
                };
            }
            else
            {
                returnStatement = new ReturnStatement();
            }
            newStatements.Add(returnStatement);

            var newSourceMethodBody = new SourceMethodBody(host, sourceLocationProvider)
            {
                Block = new BlockStatement
                {
                    Statements = newStatements
                },
                IsNormalized = false,
                LocalsAreZeroed = sourceMethodBody.LocalsAreZeroed,
                MethodDefinition = methodDefinition
            };
            methodDefinition.Body = newSourceMethodBody;
        }

        /// <summary>
        ///     Converts the assert statement into a call to Contract.Assert
        /// </summary>
        public override IStatement Rewrite(IAssertStatement assertStatement)
        {
            var methodCall = new MethodCall
            {
                Arguments =
                    new List<IExpression>
                    {
                        Rewrite(assertStatement.Condition),
                        assertStatement.Description ?? new CompileTimeConstant { Type = host.PlatformType.SystemString, Value = "Assert" }
                    },
                IsStaticCall = true,
                MethodToCall = AssertReference,
                Type = systemVoid,
                Locations = new List<ILocation>(assertStatement.Locations)
            };
            var es = new ExpressionStatement
            {
                Expression = methodCall
            };
            return es;
        }

        /// <summary>
        ///     Converts the assume statement into a call to Contract.Assume
        /// </summary>
        public override IStatement Rewrite(IAssumeStatement assumeStatement)
        {
            var methodCall = new MethodCall
            {
                Arguments =
                    new List<IExpression>
                    {
                        Rewrite(assumeStatement.Condition),
                        assumeStatement.Description ?? new CompileTimeConstant { Type = host.PlatformType.SystemString, Value = "Assume" }
                    },
                IsStaticCall = true,
                MethodToCall = AssumeReference,
                Type = systemVoid,
                Locations = new List<ILocation>(assumeStatement.Locations)
            };
            var es = new ExpressionStatement
            {
                Expression = methodCall
            };
            return es;
        }

        /// <summary>
        ///     Converts the old value into a call to Contract.OldValue
        /// </summary>
        public override IExpression Rewrite(IOldValue oldValue)
        {
            var mref = contractProvider.ContractMethods.Old;
            var methodToCall = new GenericMethodInstanceReference
            {
                CallingConvention = CallingConvention.Generic,
                ContainingType = mref.ContainingType,
                GenericArguments = new List<ITypeReference> { oldValue.Type },
                GenericMethod = mref,
                InternFactory = host.InternFactory,
                Name = mref.Name,
                Parameters = new List<IParameterTypeInformation> { new ParameterTypeInformation { Type = oldValue.Type } },
                Type = oldValue.Type
            };
            var methodCall = new MethodCall
            {
                Arguments = new List<IExpression> { oldValue.Expression },
                IsStaticCall = true,
                MethodToCall = methodToCall,
                Type = oldValue.Type,
                Locations = new List<ILocation>(oldValue.Locations)
            };
            return methodCall;
        }

        /// <summary>
        ///     Converts the return value into a call to Contract.Result
        /// </summary>
        public override IExpression Rewrite(IReturnValue returnValue)
        {
            var mref = contractProvider.ContractMethods.Result;
            var methodToCall = new GenericMethodInstanceReference
            {
                CallingConvention = CallingConvention.Generic,
                ContainingType = mref.ContainingType,
                GenericArguments = new List<ITypeReference> { returnValue.Type },
                GenericMethod = mref,
                InternFactory = host.InternFactory,
                Name = mref.Name,
                Type = returnValue.Type
            };
            var methodCall = new MethodCall
            {
                IsStaticCall = true,
                MethodToCall = methodToCall,
                Type = returnValue.Type,
                Locations = new List<ILocation>(returnValue.Locations)
            };
            return methodCall;
        }
    }
}