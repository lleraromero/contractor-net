using Contractor.Core.Properties;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
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

        private Dictionary<string, ResultKind> Analyze<T>(State source, IMethodDefinition action, List<T> target)
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
                    queries.Add(GenerateQuery(state, action, (IMethodDefinition) target));
                    // Add negative query
                    queries.Add(GenerateQuery(state, action, (IMethodDefinition) target, true));
                }
                else if (typeof(T) == typeof(State))
                {
                    queries.Add(GenerateQuery(state, action, (State)(object) target));
                }
                else
                {
                    throw new NotImplementedException("Unknown type");
                }
            }

            base.TotalGeneratedQueriesCount += queries.Count;

            return queries;
        }

        private MethodDefinition GenerateQuery(string name, IMethodDefinition action)
        {
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
                GenericParameters = new List<IGenericMethodParameter>(action.GenericParameters)
            };

            BlockStatement block = null;

            if (Configuration.InlineMethodsBody)
            {
                block = InlineMethodBody(action, method);
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

            block.Statements.Insert(0, assumeSelfNotNull);

            method.Body = new SourceMethodBody(host)
            {
                MethodDefinition = method,
                Block = block,
                LocalsAreZeroed = true
            };

            return method;
        }

        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, IMethodDefinition target, bool negate = false)
        {
            Contract.Requires(state != null && action != null && target != null);

            var queryContract = new MethodContract();
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
                               }
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
                        }
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
                        }
                    };

                    queryContract.Postconditions.Add(post);
                }
                else
                {
                    var posts = from pre in targetContract.Preconditions
                                select new Postcondition()
                                {
                                    Condition = pre.Condition
                                };

                    queryContract.Postconditions.AddRange(posts);
                }
            }

            var prefix = negate ? notPrefix : string.Empty;
            var actionName = action.GetUniqueName();
            var stateName = state.Id;
            var targetName = target.GetUniqueName();
            var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);
            var method = GenerateQuery(methodName, action);

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
                                   Condition = expr
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
            };

            contracts.Postconditions.Add(postcondition);

            var actionName = action.GetUniqueName();
            var stateName = state.UniqueName;
            var targetName = target.UniqueName;
            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);
            var method = GenerateQuery(methodName, action);

            queryContractProvider.AssociateMethodWithContract(method, contracts);
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

        private BlockStatement InlineMethodBody(IMethodDefinition action, MethodDefinition method)
        {
            var block = new BlockStatement();

            if (method.Parameters == null)
                method.Parameters = new List<IParameterDefinition>();

            method.Parameters.AddRange(action.Parameters);
            var mc = inputContractProvider.GetMethodContractFor(action);

            if (mc != null && mc.Preconditions.Count() > 0)
            {
                var asserts = from pre in mc.Preconditions
                              select new AssertStatement()
                              {
                                  Condition = pre.Condition
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
                                  Condition = post.Condition
                              };

                //Ponemos los assume antes del return
                //block.Statements.InsertRange(block.Statements.Count - 1, assumes);
                block.Statements.InsertRange(block.Statements.Count, assumes);
            }

            return block;
        }

        private ActionAnalysisResults EvaluateQueries(List<IMethodDefinition> actions, Dictionary<string, ResultKind> result)
        {
            var analysisResult = new ActionAnalysisResults();
            analysisResult.EnabledActions.AddRange(actions);
            analysisResult.DisabledActions.AddRange(actions);

            foreach (var entry in result)
            {
                switch (entry.Value)
                {
                    case ResultKind.TrueBug:
                    case ResultKind.RecursionBoundReached:
                        var query = entry.Key;
                        var queryParametersStart = query.LastIndexOf('(');

                        // Borramos los parametros del query
                        if (queryParametersStart != -1)
                            query = query.Remove(queryParametersStart);

                        var actionNameStart = query.LastIndexOf(methodNameDelimiter) + 1;
                        var actionName = query.Substring(actionNameStart);
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
                    case ResultKind.NoBugs:
                        break;
                    default:
                        throw new NotImplementedException("Unknown result");
                }
            }

            return analysisResult;
        }

        private Dictionary<string, ResultKind> ExecuteChecker(List<MethodDefinition> queries)
        {
            var result = new Dictionary<string, ResultKind>();

            RunBCT();

            var queryType = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == typeToAnalyze.Name) as NamespaceTypeDefinition;
            foreach (var query in queries)
            {
                var queryName = string.Concat(queryType.ResolvedType.ToString(), ".", query.Name.Value);
                string output = RunCorral(queryName);
                const string pattern = @"(true bug)|(recursion bound reached)|(has no bugs)";
                Regex outputParser = new Regex(pattern, RegexOptions.ExplicitCapture |
                                                RegexOptions.Multiline | RegexOptions.Compiled | RegexOptions.IgnoreCase);
                var matches = outputParser.Matches(output);

                if (matches.Count == 1)
                    result[queryName] = ParseResultKind(matches[0].Value);
            }

            return result;
        }

        private void RunBCT()
        {
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

            var output = new StringBuilder();
            var args = string.Format("{0} /main:{1} /recursionBound:{2}", GetQueryAssemblyPath().Replace("tmp", "bpl"), method, 1);    // recursionBound 3 es absolutamente arbitrario :)

            using (var corral = new Process())
            {
                corral.StartInfo = new ProcessStartInfo()
                {
                    FileName = Configuration.CorralPath,
                    Arguments = args,
                    WorkingDirectory = Configuration.TempPath,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                LogManager.Log(LogLevel.Info, "=============== CORRAL ===============");
                corral.OutputDataReceived += (sender, e) => { output.AppendLine(e.Data); LogManager.Log(LogLevel.Debug, e.Data); };
                corral.ErrorDataReceived += (sender, e) => { LogManager.Log(LogLevel.Fatal, e.Data); };
                corral.Start();
                corral.BeginErrorReadLine();
                corral.BeginOutputReadLine();
                corral.WaitForExit();

                if (corral.ExitCode != 0)
                    throw new Exception("Error executing corral");

                base.TotalAnalysisDuration += corral.ExitTime - corral.StartTime;
                base.ExecutionsCount++;
            }

            return output.ToString();
        }

        private ResultKind ParseResultKind(string message)
        {
            message = message.ToLower();
            if (message.Contains("true bug"))
                return ResultKind.TrueBug;
            else if (message.Contains("has no bugs"))
                return ResultKind.NoBugs;
            else if (message.Contains("recursion bound reached"))
                return ResultKind.RecursionBoundReached;
            else
                throw new NotImplementedException("The result was not understood");
        }

        private TransitionAnalysisResult EvaluateQueries(State source, IMethodDefinition action, List<State> targets, Dictionary<string, ResultKind> result)
        {
            var analysisResult = new TransitionAnalysisResult();

            foreach (var entry in result)
            {
                switch (entry.Value)
                {
                    case ResultKind.TrueBug:
                    case ResultKind.RecursionBoundReached:
                        var query = entry.Key;
                        var queryParametersStart = query.LastIndexOf('(');

                        // Borramos los parametros del query
                        if (queryParametersStart != -1)
                            query = query.Remove(queryParametersStart);

                        var targetNameStart = query.LastIndexOf(methodNameDelimiter) + 1;
                        var targetName = query.Substring(targetNameStart);
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
                    case ResultKind.NoBugs:
                        break;
                    default:
                        throw new NotImplementedException("Unknown result");
                }
            }

            return analysisResult;
        }
    }
}