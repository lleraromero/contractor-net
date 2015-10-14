//using Contractor.Core.Model;
//using Contractor.Utils;
//using Microsoft.Cci;
//using Microsoft.Cci.Contracts;
//using Microsoft.Cci.MutableCodeModel;
//using Microsoft.Cci.MutableContracts;
//using System;
//using System.Collections.Generic;
//using System.Diagnostics.Contracts;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;
//using Action = Contractor.Core.Model.Action;

//namespace Contractor.Core
//{
//    internal class CciQueryGenerator
//    {
//        protected const string notPrefix = "_Not_";
//        protected const string methodNameDelimiter = "~";

//        protected AssemblyXXX inputAssembly;
//        protected IContractAwareHost host;

//        public CciQueryGenerator()
//        {
//            this.host = new CodeContractAwareHostEnvironment();
//        }

//        public IEnumerable<Tuple<MethodDefinition, MethodContract>> CreateQueries(State state, CciAction action, IEnumerable<CciAction> actions)
//        {
//            var queries = new List<Tuple<MethodDefinition, MethodContract>>();
//            foreach (var target in actions)
//            {
//                // Add positive query
//                queries.Add(GenerateQuery(state, action.Method, target.Method));
//                // Add negative query
//                queries.Add(GenerateQuery(state, action.Method, target.Method, true));
//            }

//            return queries;
//        }

//        public IEnumerable<Tuple<MethodDefinition, MethodContract>> CreateQueries(State state, CciAction action, IEnumerable<State> actions)
//        {
//            var queries = new List<Tuple<MethodDefinition, MethodContract>>();
//            foreach (var target in actions)
//            {
//                queries.Add(GenerateQuery(state, action.Method, target));
//            }

//            return queries;
//        }

//        private Tuple<MethodDefinition, MethodContract> GenerateQuery(State state, IMethodDefinition action, IMethodDefinition target, bool negate = false)
//        {
//            Contract.Requires(state != null && action != null && target != null);

//            var prefix = negate ? notPrefix : string.Empty;
//            var actionName = action.GetUniqueName();
//            var stateName = state.Name;
//            var targetName = target.GetUniqueName();
//            var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);

//            var method = CreateQueryMethod<IMethodDefinition>(state, methodName, action, target);
//            var queryContract = CreateQueryContract(state, target, negate);

//            return new Tuple<MethodDefinition, MethodContract>(method, queryContract);
//        }

//        private MethodContract CreateQueryContract(State state, IMethodDefinition target, bool negate)
//        {
//            var queryContract = new MethodContract();
//            var targetContract = inputAssembly.GetContractFor(target);

//            // Add preconditions of enabled actions
//            foreach (var a in state.EnabledActions)
//            {
//                var actionContract = inputAssembly.GetContractFor(a.Method);
//                if (actionContract == null) continue;

//                var preconditions = from p in actionContract.Preconditions
//                                    select new Precondition()
//                                    {
//                                        Condition = p.Condition,
//                                        Description = new CompileTimeConstant()
//                                        {
//                                            Value = string.Format("Enabled action ({0})", a.Name),
//                                            Type = this.host.PlatformType.SystemString
//                                        },
//                                        OriginalSource = Helper.PrintExpression(p.Condition)
//                                    };
//                queryContract.Preconditions.AddRange(preconditions);
//            }

//            // Add negated preconditions of disabled actions
//            foreach (var a in state.DisabledActions)
//            {
//                var actionContract = inputAssembly.GetContractFor(a.Method);
//                if (actionContract == null || actionContract.Preconditions.Count() == 0) continue;

//                var preconditions = from p in actionContract.Preconditions
//                                    select p.Condition;
//                var joinedPreconditions = new LogicalNot()
//                {
//                    Type = this.host.PlatformType.SystemBoolean,
//                    Operand = Helper.JoinWithLogicalAnd(this.host, preconditions.ToList(), true)
//                };
//                var compactPrecondition = new Precondition()
//                {
//                    Condition = joinedPreconditions,
//                    // Add the user message to identify easily each precondition
//                    Description = new CompileTimeConstant()
//                    {
//                        Value = string.Format("Disabled action ({0})", a.Name),
//                        Type = this.host.PlatformType.SystemString
//                    },
//                    // Add the string-ified version of the condition to help debugging
//                    OriginalSource = Helper.PrintExpression(joinedPreconditions)
//                };
//                queryContract.Preconditions.Add(compactPrecondition);
//            }

//            // Now the postconditions
//            // Having no preconditions is the same as having the 'true' precondition
//            if (targetContract == null || targetContract.Preconditions.Count() == 0)
//            {
//                if (negate)
//                {
//                    var post = new Postcondition()
//                    {
//                        Condition = new CompileTimeConstant()
//                        {
//                            Type = this.host.PlatformType.SystemBoolean,
//                            Value = false
//                        },
//                        OriginalSource = "false",
//                        Description = new CompileTimeConstant() { Value = "Target negated precondition", Type = this.host.PlatformType.SystemString }
//                    };

//                    queryContract.Postconditions.Add(post);
//                }
//            }
//            else
//            {
//                if (negate)
//                {
//                    var exprs = (from pre in targetContract.Preconditions
//                                 select pre.Condition).ToList();

//                    var post = new Postcondition()
//                    {
//                        Condition = new LogicalNot()
//                        {
//                            Type = this.host.PlatformType.SystemBoolean,
//                            Operand = Helper.JoinWithLogicalAnd(this.host, exprs, true)
//                        },
//                        OriginalSource = Helper.PrintExpression(Helper.JoinWithLogicalAnd(this.host, exprs, true)),
//                        Description = new CompileTimeConstant() { Value = "Target negated precondition", Type = this.host.PlatformType.SystemString }
//                    };
//                    queryContract.Postconditions.Add(post);
//                }
//                else
//                {
//                    var targetPreconditions = from pre in targetContract.Preconditions
//                                              select new Postcondition()
//                                              {
//                                                  Condition = pre.Condition,
//                                                  OriginalSource = Helper.PrintExpression(pre.Condition),
//                                                  Description = new CompileTimeConstant() { Value = "Target precondition", Type = this.host.PlatformType.SystemString }
//                                              };
//                    queryContract.Postconditions.AddRange(targetPreconditions);
//                }
//            }
//            return queryContract;
//        }

//        private Tuple<MethodDefinition, MethodContract> GenerateQuery(State state, IMethodDefinition action, State target)
//        {
//            var actionName = action.GetUniqueName();
//            var stateName = state.Name;
//            var targetName = target.Name;
//            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);

//            var method = CreateQueryMethod<State>(state, methodName, action, target);
//            var queryContract = CreateQueryContract(state, target);

//            return new Tuple<MethodDefinition, MethodContract>(method, queryContract);
//        }

//        private MethodContract CreateQueryContract(State state, State target)
//        {
//            var contracts = new MethodContract();

//            // Source state invariant as a precondition
//            var stateInv = Helper.GenerateStateInvariant(inputContractProvider, typeToAnalyze, state);

//            var preconditions = from condition in stateInv
//                                select new Precondition()
//                                {
//                                    Condition = condition,
//                                    OriginalSource = Helper.PrintExpression(condition),
//                                    Description = new CompileTimeConstant() { Value = "Source state invariant", Type = this.host.PlatformType.SystemString }
//                                };
//            contracts.Preconditions.AddRange(preconditions);

//            // Negated target state invariant as a postcondition
//            var targetInv = Helper.GenerateStateInvariant(inputContractProvider, typeToAnalyze, target);

//            IExpression joinedTargetInv = new LogicalNot()
//            {
//                Type = this.host.PlatformType.SystemBoolean,
//                Operand = Helper.JoinWithLogicalAnd(targetInv, true)
//            };

//            var postcondition = new Postcondition()
//            {
//                Condition = joinedTargetInv,
//                OriginalSource = Helper.PrintExpression(joinedTargetInv),
//                Description = new CompileTimeConstant() { Value = "Negated target state invariant", Type = this.host.PlatformType.SystemString }
//            };
//            contracts.Postconditions.Add(postcondition);

//            return contracts;
//        }

//        protected MethodDefinition CreateQueryMethod<T>(State state, string name, IMethodDefinition action, T target)
//        {
//            Contract.Requires(target is IMethodDefinition || target is State);

//            // Get all the parameters that the query might need
//            var parameters = new HashSet<IParameterDefinition>();
//            foreach (var a in state.EnabledActions)
//                parameters.UnionWith(a.Method.Parameters);
//            foreach (var a in state.DisabledActions)
//                parameters.UnionWith(a.Method.Parameters);

//            parameters.UnionWith(action.Parameters);

//            if (target is IMethodDefinition)
//                parameters.UnionWith(((IMethodDefinition)target).Parameters);
//            else
//            {
//                var stateTarget = target as State;
//                foreach (var a in stateTarget.EnabledActions)
//                    parameters.UnionWith(a.Method.Parameters);
//                foreach (var a in stateTarget.DisabledActions)
//                    parameters.UnionWith(a.Method.Parameters);
//            }

//            var method = new MethodDefinition()
//            {
//                CallingConvention = Microsoft.Cci.CallingConvention.HasThis,
//                ContainingTypeDefinition = this.typeToAnalyze,
//                InternFactory = host.InternFactory,
//                IsStatic = false,
//                Name = host.NameTable.GetNameFor(name),
//                Type = action.Type,
//                Visibility = TypeMemberVisibility.Public,
//                Parameters = parameters.ToList()
//            };

//            BlockStatement block = null;

//            if (Configuration.InlineMethodsBody)
//            {
//                block = InlineMethodBody(action);
//            }
//            else
//            {
//                block = CallMethod(action);
//            }

//            method.Body = new SourceMethodBody(host)
//            {
//                MethodDefinition = method,
//                Block = block,
//                LocalsAreZeroed = true
//            };

//            return method;
//        }

//        private BlockStatement CallMethod(IMethodDefinition action)
//        {
//            var block = new BlockStatement();
//            var args = new List<IExpression>();

//            foreach (var arg in action.Parameters)
//            {
//                args.Add(new BoundExpression()
//                {
//                    Definition = arg,
//                    Instance = null,
//                    Locations = new List<ILocation>(arg.Locations),
//                    Type = arg.Type
//                });
//            }

//            IMethodReference methodReference = action;

//            if (typeToAnalyze.IsGeneric)
//            {
//                methodReference = specializedInputType.SpecializeMember(action, host.InternFactory) as IMethodReference;
//            }

//            var callExpr = new MethodCall()
//            {
//                Arguments = args,
//                IsStaticCall = false,
//                MethodToCall = methodReference,
//                Type = action.Type,
//                ThisArgument = new ThisReference(),
//                Locations = new List<ILocation>(action.Locations)
//            };

//            if (action.Type.TypeCode == PrimitiveTypeCode.Void)
//            {
//                var call = new ExpressionStatement()
//                {
//                    Expression = callExpr
//                };

//                block.Statements.Add(call);
//            }
//            else
//            {
//                var ret = new ReturnStatement()
//                {
//                    Expression = callExpr
//                };

//                block.Statements.Add(ret);
//            }

//            return block;
//        }

//        private BlockStatement InlineMethodBody(IMethodDefinition action)
//        {
//            var block = new BlockStatement();

//            var mc = inputContractProvider.GetMethodContractFor(action);

//            if (mc != null && mc.Preconditions.Count() > 0)
//            {
//                var asserts = from pre in mc.Preconditions
//                              select new AssertStatement()
//                              {
//                                  Condition = pre.Condition,
//                                  OriginalSource = pre.OriginalSource
//                              };

//                block.Statements.AddRange(asserts);
//            }

//            IBlockStatement actionBodyBlock = null;
//            if (action.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
//            {
//                var actionBody = action.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
//                actionBodyBlock = actionBody.Block;
//            }
//            else if (action.Body is SourceMethodBody)
//            {
//                var actionBody = action.Body as SourceMethodBody;
//                actionBodyBlock = actionBody.Block;
//            }

//            //Por tratarse de un constructor skipeamos
//            //el primer statement porque es base..ctor();
//            var skipCount = action.IsConstructor ? 1 : 0;
//            block.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));

//            if (mc != null && mc.Postconditions.Count() > 0)
//            {
//                var assumes = from post in mc.Postconditions
//                              select new AssumeStatement()
//                              {
//                                  Condition = post.Condition,
//                                  OriginalSource = post.OriginalSource
//                              };
//                //Ponemos los assume antes del return
//                if (block.Statements.Count > 0 && block.Statements.Last() is IReturnStatement)
//                {
//                    block.Statements.InsertRange(block.Statements.Count - 1, assumes);
//                }
//                else
//                {
//                    block.Statements.AddRange(assumes);
//                }
//            }

//            return block;
//        }
//    }

//}
