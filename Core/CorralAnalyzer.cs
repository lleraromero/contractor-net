using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Contractor.Core
{
    class CorralAnalyzer : Analyzer
    {
        private enum ResultKind { TrueBug, NoBugs, RecursionBoundReached }

        private const string notPrefix = "_Not_";
        private const string methodNameDelimiter = "~";

        private Microsoft.Cci.Immutable.GenericTypeInstance specializedInputType;

        public CorralAnalyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type)
            : base(host, module, type)
        {
            Contract.Requires(module != null && host != null && type != null);
        }

        public override ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            var result = Analyze<IMethodDefinition>(source, action, actions);
            var analysisResult = EvaluateQueries(actions, result);

            return analysisResult;
        }

        public override TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets)
        {
            var result = Analyze<State>(source, action, targets);
            var resultAnalysis = EvaluateQueries(source, action, targets, result);

            return resultAnalysis;
        }

        private Dictionary<MethodDefinition, ResultKind> Analyze<T>(State source, IMethodDefinition action, List<T> target)
        {
            List<MethodDefinition> queries = GenerateQueries<T>(source, action, target);

            // Add queries to the working assembly
            var type = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == typeToAnalyze.Name) as NamespaceTypeDefinition;
            type.Methods.AddRange(queries);

            // I need to replace Pre/Post with Assume/Assert
            ILocalScopeProvider localScopeProvider = new Microsoft.Cci.ILToCodeModel.Decompiler.LocalScopeProvider(GetPDBReader(queryAssembly.Module, host));
            ISourceLocationProvider sourceLocationProvider = GetPDBReader(queryAssembly.Module, host);
            var trans = new ContractRewriter(host, queryContractProvider, sourceLocationProvider);
            trans.Rewrite(queryAssembly.DecompiledModule);

            // Save the query assembly to run Corral
            queryAssembly.Save(GetQueryAssemblyPath());

            var result = ExecuteChecker(queries);

            // I don't need the queries anymore
            type.Methods.RemoveAll(m => queries.Contains(m));

            return result;
        }

        private List<MethodDefinition> GenerateQueries<T>(State state, IMethodDefinition action, List<T> actions /*states*/)
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

            base.TotalGeneratedQueriesCount += queries.Count;

            return queries;
        }

        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, IMethodDefinition target, bool negate = false)
        {
            Contract.Requires(state != null && action != null && target != null);

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

            var prefix = negate ? notPrefix : string.Empty;
            var actionName = action.GetUniqueName();
            var stateName = state.Id;
            var targetName = target.GetUniqueName();
            var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);
            var method = GenerateQuery<IMethodDefinition>(methodName, action, target);

            // Adding the parameters to the query
            var parameters = new HashSet<IParameterDefinition>();
            foreach (var a in state.EnabledActions)
            {
                parameters.UnionWith(a.Parameters);
            }
            foreach (var a in state.DisabledActions)
            {
                parameters.UnionWith(a.Parameters);
            }
            method.Parameters = parameters.ToList();

            queryContractProvider.AssociateMethodWithContract(method, queryContract);
            return method;
        }

        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, State target)
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

            var actionName = action.GetUniqueName();
            var stateName = state.UniqueName;
            var targetName = target.UniqueName;
            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);
            var method = GenerateQuery<State>(methodName, action, target);

            // Adding the parameters to the query
            var parameters = new HashSet<IParameterDefinition>();
            foreach (var a in state.EnabledActions)
            {
                parameters.UnionWith(a.Parameters);
            }
            foreach (var a in state.DisabledActions)
            {
                parameters.UnionWith(a.Parameters);
            }
            method.Parameters = parameters.ToList();

            queryContractProvider.AssociateMethodWithContract(method, contracts);
            return method;
        }

        private MethodDefinition GenerateQuery<T>(string name, IMethodDefinition action, T target)
        {
            Contract.Requires(target is IMethodDefinition || target is State);

            // I need to assign the queries to the type that I'm processing
            var type = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == typeToAnalyze.Name) as NamespaceTypeDefinition;

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
                //block.Statements.InsertRange(block.Statements.Count - 1, assumes);
                block.Statements.InsertRange(block.Statements.Count, assumes);
            }

            return block;
        }

        private ActionAnalysisResults EvaluateQueries(List<IMethodDefinition> actions, Dictionary<MethodDefinition, ResultKind> result)
        {
            var analysisResult = new ActionAnalysisResults();
            analysisResult.EnabledActions.AddRange(actions);
            analysisResult.DisabledActions.AddRange(actions);

            var queryType = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == typeToAnalyze.Name) as NamespaceTypeDefinition;
            foreach (var entry in result)
            {
                switch (entry.Value)
                {
                    case ResultKind.NoBugs:
                        break;
                    case ResultKind.TrueBug:
                    case ResultKind.RecursionBoundReached:
                        var query = entry.Key;
                        
                        var actionName = query.Name.Value;
                        var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                        actionName = actionName.Substring(actionNameStart);
                        var isNegative = actionName.StartsWith(notPrefix);

                        if (isNegative)
                        {
                            actionName = actionName.Remove(0, notPrefix.Length);
                            var method = typeToAnalyze.Methods.Find(m => m.GetUniqueName() == actionName);
                            analysisResult.DisabledActions.Remove(method);
                        }
                        else
                        {
                            var method = typeToAnalyze.Methods.Find(m => m.GetUniqueName() == actionName);
                            analysisResult.EnabledActions.Remove(method);
                        }

                        if (entry.Value == ResultKind.RecursionBoundReached)
                            base.UnprovenQueriesCount++;

                        break;
                    default:
                        throw new NotImplementedException("Unknown result");
                }
            }

            return analysisResult;
        }

        private Dictionary<MethodDefinition, ResultKind> ExecuteChecker(List<MethodDefinition> queries)
        {
            var result = new Dictionary<MethodDefinition, ResultKind>();

            RunBCT();

            var queryType = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == typeToAnalyze.Name) as NamespaceTypeDefinition;
            foreach (var query in queries)
            {
                StringBuilder queryName = new StringBuilder();
                // Method name
                queryName.Append(string.Concat(queryType.ResolvedType.ToString(), ".", query.Name.Value));
                // If it has parameters their types also appear in the name of the query in boogie
                foreach (var p in query.Parameters)
                    queryName.Append("$").Append(p.Type.ResolvedType.ToString());

                string output = RunCorral(queryName.ToString());
                const string pattern = @"(true bug)|(reached recursion bound)|(has no bugs)";
                Regex outputParser = new Regex(pattern, RegexOptions.ExplicitCapture |
                                                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matches = outputParser.Matches(output);

                if (matches.Count == 1)
                    result[query] = ParseResultKind(matches[0].Value);
                else
                    throw new NotSupportedException("Unknown result");
            }

            return result;
        }

        private void RunBCT()
        {
            //TODO: Use BCT as a library instead of an external process
            using (var bct = new Process())
            {
                bct.StartInfo = new ProcessStartInfo()
                {
                    FileName = Configuration.BCTPath,
                    Arguments = GetQueryAssemblyPath(),
                    WorkingDirectory = Configuration.TempPath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                LogManager.Log(LogLevel.Info, "=============== BCT ===============");
                bct.OutputDataReceived += (sender, e) => { LogManager.Log(LogLevel.Debug, e.Data); };
                bct.ErrorDataReceived += (sender, e) => { LogManager.Log(LogLevel.Fatal, e.Data); };
                bct.Start();
                bct.BeginErrorReadLine();
                bct.BeginOutputReadLine();
                bct.WaitForExit();

                if (bct.ExitCode != 0)
                    throw new Exception("Error translating the query assembly to boogie");

                base.TotalAnalysisDuration += bct.ExitTime - bct.StartTime;
            }
        }

        private string RunCorral(string method)
        {
            Contract.Requires(!string.IsNullOrEmpty(method));

            var args = string.Format("{0} /main:{1} /recursionBound:{2}", GetQueryAssemblyPath().Replace("tmp", "bpl"), method, 3);    // recursionBound 3 es absolutamente arbitrario :)

            var timer = new Stopwatch();
            timer.Start();

            try
            {
                LogManager.Log(LogLevel.Info, "=============== Corral ===============");
                LogManager.Log(LogLevel.Info, method);
                if (cba.Driver.run(args.Split(' ')) != 0)
                    throw new Exception("Error executing corral");
            }
            catch (Exception ex)
            {
                LogManager.Log(LogLevel.Fatal, ex.Message);
                throw ex;
            }
            
            timer.Stop();

            base.TotalAnalysisDuration += new TimeSpan(timer.ElapsedTicks);
            base.ExecutionsCount++;

            // TODO: Improve Corral as a library instead of a console application.
            switch (cba.Driver.Result)
            {
                case cba.CorralResult.BugFound:
                    return "true bug";

                case cba.CorralResult.NoBugs:
                    return "has no bugs";

                case cba.CorralResult.RecursionBoundReached:
                    return "reached recursion bound";
            }

            throw new NotImplementedException("bug");
        }

        private ResultKind ParseResultKind(string message)
        {
            message = message.ToLower();
            if (message.Contains("true bug"))
                return ResultKind.TrueBug;
            else if (message.Contains("has no bugs"))
                return ResultKind.NoBugs;
            else if (message.Contains("reached recursion bound"))
                return ResultKind.RecursionBoundReached;
            else
                throw new NotImplementedException("The result was not understood");
        }

        private TransitionAnalysisResult EvaluateQueries(State source, IMethodDefinition action, List<State> targets, Dictionary<MethodDefinition, ResultKind> result)
        {
            var analysisResult = new TransitionAnalysisResult();

            foreach (var entry in result)
            {
                switch (entry.Value)
                {
                    case ResultKind.NoBugs:
                        break;
                    case ResultKind.TrueBug:
                    case ResultKind.RecursionBoundReached:
                        var query = entry.Key;
                        
                        var actionName = query.Name.Value;
                        var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                        actionName = actionName.Substring(actionNameStart);

                        var targetNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                        var targetName = actionName.Substring(targetNameStart);
                        var target = targets.Find(s => s.UniqueName == targetName);
                        var isUnproven = entry.Value == ResultKind.RecursionBoundReached;

                        if (target != null)
                        {
                            var transition = new Transition(source, action, target, isUnproven);
                            analysisResult.Transitions.Add(transition);
                        }

                        if (isUnproven)
                            base.UnprovenQueriesCount++;
                        break;
                    default:
                        throw new NotImplementedException("Unknown result");
                }
            }

            return analysisResult;
        }
    }
}