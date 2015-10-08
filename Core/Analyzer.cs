using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;

namespace Contractor.Core
{
    abstract class Analyzer : IAnalyzer
    {
        #region IAnalyzer interface

        public TimeSpan TotalAnalysisDuration { get; protected set; }
        public int ExecutionsCount { get; protected set; }
        public int TotalGeneratedQueriesCount { get; protected set; }
        public int UnprovenQueriesCount { get; protected set; }

        // Token that allows the user to stop the analysis
        protected CancellationToken token;

        public abstract ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions);

        public abstract TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets);

        #endregion IAnalyzer interface

        protected readonly IContractAwareHost host;
        protected readonly AssemblyInfo inputAssembly;
        protected readonly ContractProvider inputContractProvider;
        protected AssemblyInfo queryAssembly;
        protected ContractProvider queryContractProvider;
        protected readonly NamespaceTypeDefinition typeToAnalyze;

        protected string notPrefix = "_Not_";
        protected string methodNameDelimiter = "~";

        protected Microsoft.Cci.Immutable.GenericTypeInstance specializedInputType;

        protected Analyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type, CancellationToken token)
        {
            Contract.Requires(host != null && module != null && type != null && token != null);

            this.host = host;
            this.inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(module.Location);

            this.inputContractProvider = inputAssembly.ExtractContracts();

            if (type.IsGeneric)
            {
                var typeReference = MutableModelHelper.GetGenericTypeInstanceReference(type.GenericParameters, type, host.InternFactory, null);
                this.specializedInputType = typeReference.ResolvedType as Microsoft.Cci.Immutable.GenericTypeInstance;
            }

            // Create a clone of the module as a working copy.
            CreateQueryAssembly(type);

            this.typeToAnalyze = queryAssembly.DecompiledModule.AllTypes.Find(t => t.Name == type.Name) as NamespaceTypeDefinition;
            this.queryContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(this.queryAssembly.Module.UnitIdentity));

            this.token = token;
        }

        ~Analyzer()
        {
            // Delete the working copy of the module.
            try
            {
                File.Delete(GetQueryAssemblyPath());
            }
            catch
            { }
        }

        protected virtual void CreateQueryAssembly(NamespaceTypeDefinition type)
        {
            // TODO: fix the query assembly to include the class being analysed and all its dependencies
            this.queryAssembly = new AssemblyInfo(host, new MetadataDeepCopier(this.host).Copy(inputAssembly.DecompiledModule));
            return;

            var coreAssembly = host.LoadAssembly(host.CoreAssemblySymbolicIdentity);

            var assembly = new Assembly()
            {
                Name = host.NameTable.GetNameFor("Query"),
                ModuleName = host.NameTable.GetNameFor("query.dll"),
                Kind = ModuleKind.DynamicallyLinkedLibrary,
                TargetRuntimeVersion = coreAssembly.TargetRuntimeVersion,
            };

            assembly.AssemblyReferences.Add(coreAssembly);

            var rootUnitNamespace = new RootUnitNamespace();
            assembly.UnitNamespaceRoot = rootUnitNamespace;
            rootUnitNamespace.Unit = assembly;

            var moduleClass = new NamespaceTypeDefinition()
            {
                ContainingUnitNamespace = rootUnitNamespace,
                InternFactory = host.InternFactory,
                IsClass = true,
                Name = host.NameTable.GetNameFor("<Module>"),
            };


            assembly.AllTypes.Add(moduleClass);

            var queryType = new MetadataDeepCopier(this.host).Copy(type);
            rootUnitNamespace.Members.Add(queryType);
            assembly.AllTypes.Add(queryType);

            this.queryAssembly = new AssemblyInfo(host, assembly);
        }

        protected string GetQueryAssemblyPath()
        {
            Contract.Requires(this.inputAssembly != null);

            return Path.Combine(Configuration.TempPath, this.queryAssembly.Module.ModuleName.Value);
        }

        protected PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null && host != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }

        protected virtual List<MethodDefinition> GenerateQueries<T>(State state, IMethodDefinition action, List<T> actions /*states*/)
        {
            Contract.Requires(typeof(T) == typeof(IMethodDefinition) || typeof(T) == typeof(State));

            var queries = new List<MethodDefinition>();

            foreach (var target in actions)
            {
                if (typeof(T) == typeof(IMethodDefinition))
                {
                    // Add positive query
                    queries.Add(GenerateQuery(state, action, (IMethodDefinition)target));
                    // Add negative query
                    queries.Add(GenerateQuery(state, action, (IMethodDefinition)target, true));
                }
                else if (typeof(T) == typeof(State))
                {
                    queries.Add(GenerateQuery(state, action, (State)(object)target));
                }
            }

            TotalGeneratedQueriesCount += queries.Count;

            return queries;
        }

        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, IMethodDefinition target, bool negate = false)
        {
            Contract.Requires(state != null && action != null && target != null);

            var prefix = negate ? notPrefix : string.Empty;
            var actionName = action.GetUniqueName();
            var stateName = state.Name;
            var targetName = target.GetUniqueName();
            var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);
            var method = CreateQueryMethod<IMethodDefinition>(state, methodName, action, target);

            var queryContract = CreateQueryContract(state, target, negate);
            queryContractProvider.AssociateMethodWithContract(method, queryContract);
            return method;
        }

        private MethodContract CreateQueryContract(State state, IMethodDefinition target, bool negate)
        {
            var queryContract = new MethodContract();
            var targetContract = inputContractProvider.GetMethodContractFor(target);

            // Add preconditions of enabled actions
            foreach (var a in state.EnabledActions)
            {
                var actionContract = inputContractProvider.GetMethodContractFor(a.Method);
                if (actionContract == null) continue;

                var preconditions = from p in actionContract.Preconditions
                                    select new Precondition()
                                    {
                                        Condition = p.Condition,
                                        Description = new CompileTimeConstant()
                                        {
                                            Value = string.Format("Enabled action ({0})", a.Name),
                                            Type = this.host.PlatformType.SystemString
                                        },
                                        OriginalSource = Helper.PrintExpression(p.Condition)
                                    };
                queryContract.Preconditions.AddRange(preconditions);
            }

            // Add negated preconditions of disabled actions
            foreach (var a in state.DisabledActions)
            {
                var actionContract = inputContractProvider.GetMethodContractFor(a.Method);
                if (actionContract == null || actionContract.Preconditions.Count() == 0) continue;

                var preconditions = from p in actionContract.Preconditions
                                    select p.Condition;
                var joinedPreconditions = new LogicalNot()
                                          {
                                              Type = host.PlatformType.SystemBoolean,
                                              Operand = Helper.JoinWithLogicalAnd(this.host, preconditions.ToList(), true)
                                          };
                var compactPrecondition = new Precondition()
                {
                    Condition = joinedPreconditions,
                    // Add the user message to identify easily each precondition
                    Description = new CompileTimeConstant()
                    {
                        Value = string.Format("Disabled action ({0})", a.Name),
                        Type = this.host.PlatformType.SystemString
                    },
                    // Add the string-ified version of the condition to help debugging
                    OriginalSource = Helper.PrintExpression(joinedPreconditions)
                };
                queryContract.Preconditions.Add(compactPrecondition);
            }

            // Now the postconditions
            // Having no preconditions is the same as having the 'true' precondition
            if (targetContract == null || targetContract.Preconditions.Count() == 0)
            {
                if (negate)
                {
                    var post = new Postcondition()
                    {
                        Condition = new CompileTimeConstant()
                        {
                            Type = host.PlatformType.SystemBoolean,
                            Value = false
                        },
                        OriginalSource = "false",
                        Description = new CompileTimeConstant() { Value = "Target negated precondition", Type = this.host.PlatformType.SystemString }
                    };

                    queryContract.Postconditions.Add(post);
                }
            }
            else
            {
                if (negate)
                {
                    var exprs = (from pre in targetContract.Preconditions
                                 select pre.Condition).ToList();

                    var post = new Postcondition()
                    {
                        Condition = new LogicalNot()
                        {
                            Type = host.PlatformType.SystemBoolean,
                            Operand = Helper.JoinWithLogicalAnd(host, exprs, true)
                        },
                        OriginalSource = Helper.PrintExpression(Helper.JoinWithLogicalAnd(host, exprs, true)),
                        Description = new CompileTimeConstant() { Value = "Target negated precondition", Type = this.host.PlatformType.SystemString }
                    };
                    queryContract.Postconditions.Add(post);
                }
                else
                {
                    var targetPreconditions = from pre in targetContract.Preconditions
                                              select new Postcondition()
                    {
                        Condition = pre.Condition,
                        OriginalSource = Helper.PrintExpression(pre.Condition),
                        Description = new CompileTimeConstant() { Value = "Target precondition", Type = this.host.PlatformType.SystemString }
                    };
                    queryContract.Postconditions.AddRange(targetPreconditions);
                }
            }
            return queryContract;
        }

        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, State target)
        {
            var actionName = action.GetUniqueName();
            var stateName = state.Name;
            var targetName = target.Name;
            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);
            var method = CreateQueryMethod<State>(state, methodName, action, target);

            var queryContract = CreateQueryContract(state, target);
            queryContractProvider.AssociateMethodWithContract(method, queryContract);
            return method;
        }

        private MethodContract CreateQueryContract(State state, State target)
        {
            var contracts = new MethodContract();

            // Source state invariant as a precondition
            var stateInv = Helper.GenerateStateInvariant(host, inputContractProvider, typeToAnalyze, state);

            var preconditions = from condition in stateInv
                                select new Precondition()
                                {
                                    Condition = condition,
                                    OriginalSource = Helper.PrintExpression(condition),
                                    Description = new CompileTimeConstant() { Value = "Source state invariant", Type = this.host.PlatformType.SystemString }
                                };
            contracts.Preconditions.AddRange(preconditions);

            // Negated target state invariant as a postcondition
            var targetInv = Helper.GenerateStateInvariant(host, inputContractProvider, typeToAnalyze, target);

            IExpression joinedTargetInv = new LogicalNot()
            {
                Type = host.PlatformType.SystemBoolean,
                Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
            };

            var postcondition = new Postcondition()
            {
                Condition = joinedTargetInv,
                OriginalSource = Helper.PrintExpression(joinedTargetInv),
                Description = new CompileTimeConstant() { Value = "Negated target state invariant", Type = this.host.PlatformType.SystemString }
            };
            contracts.Postconditions.Add(postcondition);

            return contracts;
        }

        protected MethodDefinition CreateQueryMethod<T>(State state, string name, IMethodDefinition action, T target)
        {
            Contract.Requires(target is IMethodDefinition || target is State);

            // Get all the parameters that the query might need
            var parameters = new HashSet<IParameterDefinition>();
            foreach (var a in state.EnabledActions)
                parameters.UnionWith(a.Method.Parameters);
            foreach (var a in state.DisabledActions)
                parameters.UnionWith(a.Method.Parameters);

            parameters.UnionWith(action.Parameters);

            if (target is IMethodDefinition)
                parameters.UnionWith(((IMethodDefinition)target).Parameters);
            else
            {
                var stateTarget = target as State;
                foreach (var a in stateTarget.EnabledActions)
                    parameters.UnionWith(a.Method.Parameters);
                foreach (var a in stateTarget.DisabledActions)
                    parameters.UnionWith(a.Method.Parameters);
            }

            var method = new MethodDefinition()
            {
                CallingConvention = Microsoft.Cci.CallingConvention.HasThis,
                ContainingTypeDefinition = this.typeToAnalyze,
                InternFactory = host.InternFactory,
                IsStatic = false,
                Name = host.NameTable.GetNameFor(name),
                Type = action.Type,
                Visibility = TypeMemberVisibility.Public,
                Parameters = parameters.ToList()
            };

            BlockStatement block = null;

            if (Configuration.InlineMethodsBody)
            {
                block = InlineMethodBody(action);
            }
            else
            {
                block = CallMethod(action);
            }

            method.Body = new SourceMethodBody(host)
            {
                MethodDefinition = method,
                Block = block,
                LocalsAreZeroed = true
            };

            return method;
        }

        private BlockStatement CallMethod(IMethodDefinition action)
        {
            var block = new BlockStatement();
            var args = new List<IExpression>();

            foreach (var arg in action.Parameters)
            {
                args.Add(new BoundExpression()
                {
                    Definition = arg,
                    Instance = null,
                    Locations = new List<ILocation>(arg.Locations),
                    Type = arg.Type
                });
            }

            IMethodReference methodReference = action;

            if (typeToAnalyze.IsGeneric)
            {
                methodReference = specializedInputType.SpecializeMember(action, host.InternFactory) as IMethodReference;
            }

            var callExpr = new MethodCall()
            {
                Arguments = args,
                IsStaticCall = false,
                MethodToCall = methodReference,
                Type = action.Type,
                ThisArgument = new ThisReference(),
                Locations = new List<ILocation>(action.Locations)
            };

            if (action.Type.TypeCode == PrimitiveTypeCode.Void)
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

        private BlockStatement InlineMethodBody(IMethodDefinition action)
        {
            var block = new BlockStatement();

            var mc = inputContractProvider.GetMethodContractFor(action);

            if (mc != null && mc.Preconditions.Count() > 0)
            {
                var asserts = from pre in mc.Preconditions
                              select new AssertStatement()
                              {
                                  Condition = pre.Condition,
                                  OriginalSource = pre.OriginalSource
                              };

                block.Statements.AddRange(asserts);
            }

            IBlockStatement actionBodyBlock = null;
            if (action.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
            {
                var actionBody = action.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
                actionBodyBlock = actionBody.Block;
            }
            else if (action.Body is SourceMethodBody)
            {
                var actionBody = action.Body as SourceMethodBody;
                actionBodyBlock = actionBody.Block;
            }

            //Por tratarse de un constructor skipeamos
            //el primer statement porque es base..ctor();
            var skipCount = action.IsConstructor ? 1 : 0;
            block.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));

            if (mc != null && mc.Postconditions.Count() > 0)
            {
                var assumes = from post in mc.Postconditions
                              select new AssumeStatement()
                              {
                                  Condition = post.Condition,
                                  OriginalSource = post.OriginalSource
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