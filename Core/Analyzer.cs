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

namespace Contractor.Core
{
    abstract class Analyzer : IAnalyzer
    {
        #region IAnalyzer interface

        public TimeSpan TotalAnalysisDuration { get; protected set; }
        public int ExecutionsCount { get; protected set; }
        public int TotalGeneratedQueriesCount { get; protected set; }
        public int UnprovenQueriesCount { get; protected set; }

        public abstract ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions);

        public abstract TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets);

        #endregion IAnalyzer interface

        protected readonly IContractAwareHost host;
        protected readonly AssemblyInfo inputAssembly;
        protected readonly ContractProvider inputContractProvider;
        protected AssemblyInfo queryAssembly;
        protected ContractProvider queryContractProvider;
        protected readonly NamespaceTypeDefinition typeToAnalyze;

        protected string notPrefix = ".Not.";
        protected string methodNameDelimiter = "~";

        protected Microsoft.Cci.Immutable.GenericTypeInstance specializedInputType;

        protected Analyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type)
        {
            Contract.Requires(host != null && module != null && type != null);

            this.host = host;
            this.inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(module.Location);

            this.typeToAnalyze = type;
            this.inputContractProvider = inputAssembly.ExtractContracts();

            // Create a clone of the module as a working copy.
            CreateQueryAssembly(type);
            this.queryContractProvider = new ContractProvider(new ContractMethods(this.host), this.host.FindUnit(this.queryAssembly.Module.UnitIdentity));
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

        private void CreateQueryAssembly(NamespaceTypeDefinition type)
        {
            // Load original module
            IModule module = this.host.LoadUnitFrom(inputAssembly.Module.Location) as IModule;
            // Make a editable copy
            Module queryModule = new MetadataDeepCopier(this.host).Copy(module);
            
            // Remove types that we don't need to analyse in the query assembly.
            var types = queryModule.GetAnalyzableTypes().ToList();
            foreach (var t in types)
            {
                var tMutable = t as NamespaceTypeDefinition;
                if (tMutable != null && tMutable.ContainingUnitNamespace.Name == type.ContainingUnitNamespace.Name && tMutable.Name != type.Name)
                {
                    queryModule.AllTypes.Remove(t);
                }
            }

            // TODO: removed types are still present as RootNamespace members, remove them. 
            // How do we recognize useless types?
            this.queryAssembly = new AssemblyInfo(this.host, queryModule);
        }

        protected string GetQueryAssemblyPath()
        {
            Contract.Requires(this.inputAssembly != null);

            return Path.Combine(Configuration.TempPath, this.inputAssembly.Module.ModuleName.Value + ".tmp");
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
            var stateName = state.Id;
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

            // Add the invariant as a precondition and postcondition of the query
            queryContract.Preconditions.AddRange(from i in inputContractProvider.GetTypeContractFor(typeToAnalyze).Invariants
                                                 select new Precondition()
                                                 {
                                                     Condition = i.Condition,
                                                     OriginalSource = i.OriginalSource,
                                                     Locations = new List<ILocation>(i.Locations),
                                                 });
            queryContract.Postconditions.AddRange(from i in inputContractProvider.GetTypeContractFor(typeToAnalyze).Invariants
                                                  select new Postcondition()
                                                  {
                                                      Condition = i.Condition,
                                                      OriginalSource = i.OriginalSource,
                                                      Locations = new List<ILocation>(i.Locations),
                                                  });

            var targetContract = inputContractProvider.GetMethodContractFor(target);

            // Add preconditions of enabled actions
            foreach (var a in state.EnabledActions)
            {
                var actionContract = inputContractProvider.GetMethodContractFor(a);
                if (actionContract == null) continue;

                queryContract.Preconditions.AddRange(actionContract.Preconditions);
            }

            // Add negated preconditions of disabled actions
            foreach (var a in state.DisabledActions)
            {
                var actionContract = inputContractProvider.GetMethodContractFor(a);
                if (actionContract == null || actionContract.Preconditions.Count() == 0) continue;

                var pres = from pre in actionContract.Preconditions
                           select new Precondition()
                           {
                               Condition = new LogicalNot()
                               {
                                   Type = host.PlatformType.SystemBoolean,
                                   Operand = pre.Condition
                               },
                               OriginalSource = pre.OriginalSource
                           };

                queryContract.Preconditions.AddRange(pres);
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
                        OriginalSource = "false"
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
                        OriginalSource = Helper.PrintExpression(Helper.JoinWithLogicalAnd(host, exprs, true))
                    };

                    queryContract.Postconditions.Add(post);
                }
                else
                {
                    var posts = from pre in targetContract.Preconditions
                                select new Postcondition()
                                {
                                    Condition = pre.Condition,
                                    OriginalSource = pre.OriginalSource
                                };

                    queryContract.Postconditions.AddRange(posts);
                }
            }
            return queryContract;
        }
               
        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, State target)
        {
            var actionName = action.GetUniqueName();
            var stateName = state.UniqueName;
            var targetName = target.UniqueName;
            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);
            var method = CreateQueryMethod<State>(state, methodName, action, target);

            var queryContract = CreateQueryContract(state, target);
            queryContractProvider.AssociateMethodWithContract(method, queryContract);
            return method;
        }
               
        private MethodContract CreateQueryContract(State state, State target)
        {
            var contracts = new MethodContract();

            var stateInv = Helper.GenerateStateInvariant(host, inputContractProvider, typeToAnalyze, state);

            var precondition = from expr in stateInv
                               select new Precondition()
                               {
                                   Condition = expr,
                                   OriginalSource = Helper.PrintExpression(expr)
                               };

            contracts.Preconditions.AddRange(precondition);

            var targetInv = Helper.GenerateStateInvariant(host, inputContractProvider, typeToAnalyze, target);

            var postcondition = new Postcondition()
            {
                Condition = new LogicalNot()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
                },
                OriginalSource = string.Join(" AND ", from a in targetInv select Helper.PrintExpression(a))
            };

            contracts.Postconditions.Add(postcondition);
            return contracts;
        }
               
        private MethodDefinition CreateQueryMethod<T>(State state, string name, IMethodDefinition action, T target)
        {
            Contract.Requires(target is IMethodDefinition || target is State);

            // I need to assign the queries to the type that I'm processing
            var type = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == typeToAnalyze.Name) as NamespaceTypeDefinition;

            // Get all the parameters that the query might need
            var parameters = new HashSet<IParameterDefinition>();
            foreach (var a in state.EnabledActions)
                parameters.UnionWith(a.Parameters);
            foreach (var a in state.DisabledActions)
                parameters.UnionWith(a.Parameters);

            var method = new MethodDefinition()
            {
                Attributes = new List<ICustomAttribute>(action.Attributes),
                CallingConvention = Microsoft.Cci.CallingConvention.HasThis,
                ContainingTypeDefinition = type,
                InternFactory = host.InternFactory,
                IsStatic = false,
                Name = host.NameTable.GetNameFor(name),
                Type = action.Type,
                Visibility = TypeMemberVisibility.Private,
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

            var assumeSelfNotNull = new AssumeStatement()
            {
                Condition = new LogicalNot()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = new Equality()
                    {
                        Type = host.PlatformType.SystemBoolean,
                        LeftOperand = new ThisReference(),
                        RightOperand = new CompileTimeConstant()
                    }
                }
            };
            assumeSelfNotNull.OriginalSource = Helper.PrintExpression(assumeSelfNotNull.Condition);

            block.Statements.Insert(0, assumeSelfNotNull);

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
            throw new NotSupportedException(); //TODO: do the proper modifications
            var block = new BlockStatement();
            var args = new List<IExpression>();

            foreach (var arg in action.Parameters)
            {
                var defaultValue = new DefaultValue()
                {
                    DefaultValueType = arg.Type,
                    Type = arg.Type
                };

                args.Add(defaultValue);
            }

            IMethodReference methodReference = action;

            if (typeToAnalyze.IsGeneric)
            {
                methodReference = specializedInputType.SpecializeMember(action, host.InternFactory) as IMethodReference;
            }

            var callExpr = new MethodCall()
            {
                IsStaticCall = false,
                MethodToCall = methodReference,
                Type = action.Type,
                ThisArgument = new ThisReference(),
                Arguments = args
            };

            if (action.Type.TypeCode == PrimitiveTypeCode.Void)
            {
                var call = new ExpressionStatement()
                {
                    Expression = callExpr
                };

                block.Statements.Add(call);
                block.Statements.Add(new ReturnStatement());
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
                if (block.Statements.Last() is IReturnStatement)
                {
                    block.Statements.InsertRange(block.Statements.Count - 1, assumes);
                }
                else
                {
                    block.Statements.InsertRange(block.Statements.Count, assumes);
                }
            }

            return block;
        }
    }
}