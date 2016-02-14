using System;
using System.Collections.Generic;
using System.Linq;
using Contractor.Core;
using Contractor.Core.Model;
using Contractor.Utils;
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
                    OriginalSource = Helper.PrintExpression(condition),
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
                            OriginalSource = Helper.PrintExpression(pre.Condition),
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

            // Negated target state invariant as a postcondition
            var targetInv = Helper.GenerateStateInvariant(host, target);

            IExpression joinedTargetInv = new LogicalNot
            {
                Type = host.PlatformType.SystemBoolean,
                Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
            };

            var postcondition = new Postcondition
            {
                Condition = joinedTargetInv,
                OriginalSource = Helper.PrintExpression(joinedTargetInv),
                Description = new CompileTimeConstant { Value = "Negated target state invariant", Type = host.PlatformType.SystemString }
            };
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
            throw new NotSupportedException();
            //var block = new BlockStatement();
            //var args = new List<IExpression>();

            //foreach (var arg in action.Parameters)
            //{
            //    args.Add(new BoundExpression()
            //    {
            //        Definition = arg,
            //        Instance = null,
            //        Locations = new List<ILocation>(arg.Locations),
            //        Type = arg.Type
            //    });
            //}

            //IMethodReference methodReference = action;

            //if (typeToAnalyze.IsGeneric)
            //{
            //    methodReference = specializedInputType.SpecializeMember(action, host.InternFactory) as IMethodReference;
            //}

            //var callExpr = new MethodCall()
            //{
            //    Arguments = args,
            //    IsStaticCall = false,
            //    MethodToCall = methodReference,
            //    Type = action.Type,
            //    ThisArgument = new ThisReference(),
            //    Locations = new List<ILocation>(action.Locations)
            //};

            //if (action.Type.TypeCode == PrimitiveTypeCode.Void)
            //{
            //    var call = new ExpressionStatement()
            //    {
            //        Expression = callExpr
            //    };

            //    block.Statements.Add(call);
            //}
            //else
            //{
            //    var ret = new ReturnStatement()
            //    {
            //        Expression = callExpr
            //    };

            //    block.Statements.Add(ret);
            //}

            //return block;
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

            //Por tratarse de un constructor skipeamos
            //el primer statement porque es base..ctor();
            var skipCount = action.Method.IsConstructor ? 1 : 0;
            block.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));

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
                if (block.Statements.Count > 0 && block.Statements.Last() is IReturnStatement)
                {
                    block.Statements.InsertRange(block.Statements.Count - 1, assumes);
                }
                else
                {
                    block.Statements.AddRange(assumes);
                }
            }

            return block;
        }
    }
}