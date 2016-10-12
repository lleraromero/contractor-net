using System;
using System.Collections.Generic;
using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using Action = Contractor.Core.Model.Action;

namespace Analysis.Cci
{
    public class CciQueryGenerator
    {
        protected const string NotPrefix = "_Not_";
        protected const string MethodNameDelimiter = "~";

        public IExpression exitCode_eq_expected;

        private IStatement localDefExitCode;

        private IStatement localDefExpectedExitCode; 

        protected IContractAwareHost host;

        public CciQueryGenerator()
        {
            host = CciHostEnvironment.GetInstance();
        }

        public IReadOnlyCollection<ActionQuery> CreatePositiveQueries(State state, Action action, IEnumerable<Action> actions)
        {
            var queryGenerator = new CciPositiveActionQueryGenerator(host);
            return CreateQueries(state, action, actions, queryGenerator);
        }

        public IReadOnlyCollection<ActionQuery> CreateNegativeQueries(State state, Action action, IEnumerable<Action> actions)
        {
            var queryGenerator = new CciNegativeActionQueryGenerator(host);
            return CreateQueries(state, action, actions, queryGenerator);
        }

        protected IReadOnlyCollection<ActionQuery> CreateQueries(State state, Action action, IEnumerable<Action> actions,
            CciActionQueryGenerator queryGenerator)
        {
            var queries = new List<ActionQuery>();
            foreach (var actionUnderTest in actions)
            {
                queries.Add(queryGenerator.CreateQuery(state, action, actionUnderTest));
            }
            return queries;
        }

        public IReadOnlyCollection<TransitionQuery> CreateTransitionQueries(State sourceState, Action action, IEnumerable<State> targetStates)
        {
            return
                targetStates.Select(
                    targetState => new TransitionQuery(GenerateQuery(sourceState, action, targetState), sourceState, action, targetState)).ToList();
        }

        private Action GenerateQuery(State state, Action action, State target)
        {
            var actionName = action.Name;
            var stateName = state.Name;
            var targetName = target.Name;
            var methodName = string.Format("{1}{0}{2}{0}{3}", MethodNameDelimiter, stateName, actionName, targetName);

            var method = CreateQueryMethod(state, methodName, action, target);
            var queryContract = CreateQueryContract(state, target);

            return new CciAction(method, queryContract);
        }

        private MethodContract CreateQueryContract(State state, State target)
        {
            var contracts = new MethodContract();

            // Source state invariant as a precondition
            var stateInv = Helper.GenerateStateInvariant(host, state);

            var preconditions = from condition in stateInv
                                select new Precondition
                                {
                                    Condition = condition,
                                    OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(condition),
                                    Description = new CompileTimeConstant { Value = "Source state invariant", Type = host.PlatformType.SystemString }
                                };
            contracts.Preconditions.AddRange(preconditions);

            // Add a redundant postcondition for only those conditions that predicate ONLY about parameters and not the instance. 
            // These postconditions will be translated as assumes in the ContractRewriter.cs
            var contractDependencyAnalyzer = new CciContractDependenciesAnalyzer(new ContractProvider(new ContractMethods(host), null));
            foreach (var action in target.EnabledActions.Union(target.DisabledActions))
            {
                if (action.Contract != null)
                {
                    foreach (var pre in action.Contract.Preconditions)
                    {
                        if (contractDependencyAnalyzer.PredicatesAboutInstance(pre) ||
                            !contractDependencyAnalyzer.PredicatesAboutParameter(pre))
                        {
                            continue;
                        }

                        var post = new Postcondition
                        {
                            Condition = pre.Condition,
                            OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(pre.Condition),
                            Description =
                                new CompileTimeConstant
                                {
                                    Value = "Conditions over parameters are assumed satisfiable",
                                    Type = host.PlatformType.SystemString
                                }
                        };
                        contracts.Postconditions.Add(post);
                    }
                }
            }
            /*
            IExpression exitCode_eq_expected = new Equality()
            {
                LeftOperand = new ExpressionStatement()
                {
                    Expression = new FieldReferenceS()
                },
                RightOperand = null,
                Type = host.PlatformType.SystemString

            };*/

            // Negated target state invariant as a postcondition
            var targetInv = Helper.GenerateStateInvariant(host, target);

            IExpression joinedTargetInv = new LogicalNot
            {
                Type = host.PlatformType.SystemBoolean,
                Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
            };
            Postcondition postcondition = null;
            if (exitCode_eq_expected != null)
            {
                var unit = this.host.LoadedUnits.First();
                var coreAssembly = this.host.FindAssembly(unit.CoreAssemblySymbolicIdentity);

                var arg= new List<IExpression>();

                //-------
                exitCode_eq_expected = new Equality
                {
                    Type = host.PlatformType.SystemBoolean,
                    //LeftOperand = new OldValue
                    //{
                    //    Type = coreAssembly.PlatformType.SystemInt32,
                    //    Expression = new BoundExpression
                    //    {
                    //        Definition = field,
                    //        Instance = new ThisReference(),
                    //        Type = field.Type
                    //    }
                    //},
                    //RightOperand = new CompileTimeConstant
                    //{
                    //    Type = coreAssembly.PlatformType.SystemInt32,
                    //    Value = fromStateId
                    //}
                    LeftOperand = ((IExpressionStatement)localDefExitCode).Expression,
                    RightOperand = ((IExpressionStatement)localDefExpectedExitCode).Expression
                };
                //------

                //exitCode_eq_expected = new MethodCall()
                //{
                //    MethodToCall = coreAssembly.PlatformType.,
                //    Type = coreAssembly.PlatformType.SystemBoolean,
                //    Arguments = arg
                //};

                IExpression notExitCode = new LogicalNot
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = exitCode_eq_expected
                };

                List<IExpression> listWithNotExitCodeAndPost = new List<IExpression>();
                listWithNotExitCodeAndPost.Add(notExitCode);
                listWithNotExitCodeAndPost.Add(joinedTargetInv);

                IExpression notExitCodeOrNotPost = Helper.JoinWithLogicalOr(host, listWithNotExitCodeAndPost, false);

                postcondition = new Postcondition
                {
                    Condition = notExitCodeOrNotPost,
                    OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(notExitCodeOrNotPost),
                    Description = new CompileTimeConstant { Value = "Negated target state invariant", Type = host.PlatformType.SystemString }
                };
            }
            else
            {
                postcondition = new Postcondition
                {
                    Condition = joinedTargetInv,
                    OriginalSource = new CciExpressionPrettyPrinter().PrintExpression(joinedTargetInv),
                    Description = new CompileTimeConstant { Value = "Negated target state invariant", Type = host.PlatformType.SystemString }
                };
            }

            contracts.Postconditions.Clear();
            contracts.Postconditions.Add(postcondition);

            return contracts;
        }

        private MethodDefinition CreateQueryMethod(State state, string name, Action action, State target)
        {
            var parameters = GetStateParameters(state);

            parameters.UnionWith(action.Method.Parameters);

            parameters.UnionWith(GetStateParameters(target));

            return CreateMethod(name, action, parameters);
        }

        private HashSet<IParameterDefinition> GetStateParameters(State state)
        {
            // Get all the parameters that the query might need
            var parameters = new HashSet<IParameterDefinition>();
            foreach (var a in state.EnabledActions)
            {
                parameters.UnionWith(a.Method.Parameters);
            }
            foreach (var a in state.DisabledActions)
            {
                parameters.UnionWith(a.Method.Parameters);
            }
            return parameters;
        }

        private MethodDefinition CreateMethod(string name, Action action, HashSet<IParameterDefinition> parameters)
        {
            var method = new MethodDefinition
            {
                CallingConvention = CallingConvention.HasThis,
                InternFactory = host.InternFactory,
                IsStatic = false,
                Name = host.NameTable.GetNameFor(name),
                Type = action.Method.Type,
                Visibility = TypeMemberVisibility.Public,
                Parameters = parameters.ToList()
            };

            BlockStatement block = null;

            // TODO (lleraromero): Por el momento hacemos solo inlining porque sirve para CC / Corral. Realmente deberiamos tener un creador del cuerpo polimorfico

            //if (Configuration.InlineMethodsBody)
            //{
            //block = CallMethod(action);
            block = InlineMethodBody(action);
            //}
            //else
            //{
            //    block = CallMethod(action);
            //}

            method.Body = new SourceMethodBody(host)
            {
                MethodDefinition = method,
                Block = block,
                LocalsAreZeroed = true
            };
            return method;
        }

        private BlockStatement CallMethod(Action action)
        {
            //throw new NotSupportedException();
            var block = new BlockStatement();
            var args = new List<IExpression>();

            foreach (var arg in action.Method.ExtraParameters) //action.Parameters)
            {
                args.Add(new BoundExpression()
                {
                    Definition = arg,
                    Instance = null,
                    Locations = new List<ILocation>(action.Method.Locations), //arg.Locations),
                    Type = arg.Type
                });
            }

            IMethodReference methodReference = action.Method;

            //if (typeToAnalyze.IsGeneric)
            //{
            //    methodReference = specializedInputType.SpecializeMember(action, host.InternFactory) as IMethodReference;
            //}

            var callExpr = new MethodCall()
            {
                Arguments = args,
                IsStaticCall = false,
                MethodToCall = methodReference,
                Type = action.Method.Type,
                ThisArgument = new ThisReference(),
                Locations = new List<ILocation>(action.Method.Locations)
            };

            if (action.Method.Type.TypeCode == PrimitiveTypeCode.Void)
            {
                var call = new ExpressionStatement()
                {
                    Expression = callExpr
                };

                block.Statements.Add(call);
            }
            else
            {
                var ret = new ReturnStatement()
                {
                    Expression = callExpr
                };

                block.Statements.Add(ret);
            }

            return block;
        }

        private BlockStatement InlineMethodBody(Action action)
        {
            var block = new BlockStatement();

            var mc = action.Contract;

            if (mc != null && mc.Preconditions.Any())
            {
                var asserts = from pre in mc.Preconditions
                    select new AssertStatement
                    {
                        Condition = pre.Condition,
                        OriginalSource = pre.OriginalSource,
                        Description = new CompileTimeConstant { Value = "Inlined method precondition", Type = host.PlatformType.SystemString }
                    };

                block.Statements.AddRange(asserts);
            }

            IBlockStatement actionBodyBlock = null;
            if (action.Method.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
            {
                var actionBody = action.Method.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
                actionBodyBlock = actionBody.Block;
            }
            else if (action.Method.Body is SourceMethodBody)
            {
                var actionBody = action.Method.Body as SourceMethodBody;
                actionBodyBlock = actionBody.Block;
            }

            //***************************************************** armamos el TRY-CATCH
            //AGREGAR LOS STATEMENT DEL ACTION AL TRYBLOCK EN VEZ DE AL BLOCK
            
            var tryBlock = new BlockStatement();

            //Por tratarse de un constructor skipeamos
            //el primer statement porque es base..ctor();
            var skipCount = action.Method.IsConstructor ? 1 : 0;
            tryBlock.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));

            localDefExitCode = tryBlock.Statements.First(x => x is LocalDeclarationStatement && (x as LocalDeclarationStatement).LocalVariable.Name.Value == "exitCode");
            localDefExpectedExitCode = tryBlock.Statements.First(x => x is LocalDeclarationStatement && (x as LocalDeclarationStatement).LocalVariable.Name.Value == "expectedExitCode");

            var unit = this.host.LoadedUnits.First();
            var coreAssembly = this.host.FindAssembly(unit.CoreAssemblySymbolicIdentity);
            
            var catchClauses = new List<ICatchClause>();

            var intType = coreAssembly.PlatformType.SystemInt32;
            var variable = new LocalDefinition()
            {
                Name = Dummy.Name,
                Type = coreAssembly.PlatformType.SystemString,
                MethodDefinition = action.Method
            };

            // var variable = action.Method.Body.LocalVariables.FirstOrDefault( v => v.Name.Value == "exitCode");

            var listOfExceptions = new List<String>();
            listOfExceptions.Add("NullReferenceException");
            listOfExceptions.Add("IndexOutOfRangeException");
            listOfExceptions.Add("DivideByZeroException");
            listOfExceptions.Add("OverflowException");
            listOfExceptions.Add("Exception");

            foreach(var exception in listOfExceptions){
                var excType = coreAssembly.GetAllTypes().Single(t => t.Name.Value == exception);
                var catchExc = GenerateCatchClauseFor(coreAssembly, variable, excType);
                catchClauses.Add(catchExc);
            }
            
            var tryStmt = new TryCatchFinallyStatement
                {
                    TryBody = tryBlock,
	                CatchClauses = catchClauses
                };

            block.Statements.Add(tryStmt);
            //*****************************************************
            
            if (mc != null && mc.Postconditions.Any())
            {
                var assumes = from post in mc.Postconditions
                    select new AssumeStatement
                    {
                        Condition = post.Condition,
                        OriginalSource = post.OriginalSource,
                        Description = new CompileTimeConstant { Value = "Inlined method postcondition", Type = host.PlatformType.SystemString }
                    };
                //Ponemos los assume antes del return
                //var assume=assumes.ElementAt(0);
                //this.exitCode_eq_expected = assume.Condition;
                List<AssumeStatement> finalAssumes = new List<AssumeStatement>(assumes);
                //finalAssumes.RemoveAt(0);

                if (block.Statements.Count > 0 && block.Statements.Last() is IReturnStatement)
                {
                    block.Statements.InsertRange(block.Statements.Count - 1, finalAssumes);
                }
                else
                {
                    block.Statements.AddRange(finalAssumes);
                }
            }

            return block;
        }

        private CatchClause GenerateCatchClauseFor(Microsoft.Cci.IAssembly coreAssembly, LocalDefinition variable, INamedTypeDefinition nullExcType)
        {
            var nullExcBody = new BlockStatement();
            var assign2 = new ExpressionStatement()
            {
                Expression = new Assignment()
                {
                    Source = new CompileTimeConstant
                    {
                        Type = coreAssembly.PlatformType.SystemInt32,
                        Value = 1
                    },
                    Target = new TargetExpression()
                    {
                        Definition = ((ILocalDeclarationStatement)localDefExitCode).LocalVariable,
                        Type = coreAssembly.PlatformType.SystemInt32
                    },
                    Type = coreAssembly.PlatformType.SystemInt32
                }
            };
            nullExcBody.Statements.Add(assign2);
            var catchNullExc = new CatchClause()
            {
                ExceptionType = nullExcType, // this.host.NameTable.GetNameFor("Exception"),
                Body = nullExcBody,
                ExceptionContainer = variable
            };
            return catchNullExc;
        }

        
    }
}