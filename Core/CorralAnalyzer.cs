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
    internal class CorralAnalyzer : IAnalyzer
    {
        public TimeSpan TotalAnalysisDuration { get; private set; }
        public int ExecutionsCount { get; private set; }
        public int TotalGeneratedQueriesCount { get; private set; }
        public int UnprovenQueriesCount { get; private set; }

        private enum ResultKind
        {
            None,
            UnsatisfiableRequires,
            FalseRequires,
            UnprovenEnsures,
            FalseEnsures
        }

        private enum ResultKindCorral { None, BugFound, Correct, BoundReached }

        private const string notPrefix = "_Not_";
        private const string methodNameDelimiter = "~";
        private const string pattern = @"^ Method \W* \d+ \W* : (< [a-z ]+ >)? \W* (?<MethodName> [^(\r]+ (\( [^)]* \))?) \r | ^ [^( ]+ (\( [^)]* \))? \W* (\[ [^]]* \])? \W* : \W* ([^:]+ :)? \W* (?<Message> [^\r]+) \r";

        private readonly Regex outputParser;
        private readonly IContractAwareHost host;
        private readonly AssemblyInfo inputAssembly;
        private readonly NamespaceTypeDefinition inputType;
        private readonly ContractProvider inputContractProvider;
        private AssemblyInfo queryAssembly;
        private NamespaceTypeDefinition queryType;
        private Microsoft.Cci.Immutable.GenericTypeInstance specializedInputType;
        private ContractProvider queryContractProvider;
        private QueryReplacer queryReplacer;
        private StringBuilder output;

        public CorralAnalyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type)
        {
            Contract.Requires(module != null && host != null && type != null);

            this.host = host;
            this.inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(module.Location);
            inputAssembly.Decompile();

            // Create a clone of the module as a working copy.
            inputAssembly.Save(GetQueryAssemblyPath());

            this.queryAssembly = new AssemblyInfo(host);
            this.queryAssembly.Load(GetQueryAssemblyPath());
            this.queryAssembly.Decompile();
            this.inputType = type;

            this.inputContractProvider = inputAssembly.ExtractContracts();
            //this.queryContractProvider = queryAssembly.ExtractContracts();

            //this.queryReplacer = new QueryReplacer(host, inputType, type);
        }

        ~CorralAnalyzer()
        {
            // Delete the working copy of the module.
            File.Delete(GetQueryAssemblyPath());
        }

        public ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            Contract.Requires(source != null && action != null && actions != null && actions.Count > 0);

            // Extract contracts (side effect: removes them from the method bodies)
            queryContractProvider = queryAssembly.ExtractContracts();
            var queries = GenerateQueries(source, action, actions);

            // Add queries to the working assembly
            var type = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == inputType.Name) as NamespaceTypeDefinition;
            type.Methods.AddRange(queries);

            // Put contracts back to the assembly
            queryAssembly.InjectContracts(queryContractProvider);

            //TODO: para que el rewriter?
            //queryReplacer.Rewrite(queryAssembly.DecompiledModule);

            // I need to replace Pre/Post into Assume/Assert
            ILocalScopeProvider localScopeProvider = new Microsoft.Cci.ILToCodeModel.Decompiler.LocalScopeProvider(GetPDBReader(queryAssembly.Module, host));
            ISourceLocationProvider sourceLocationProvider = GetPDBReader(queryAssembly.Module, host);
            var trans = new ContractRewriter(host, queryContractProvider, sourceLocationProvider);
            trans.Rewrite(queryAssembly.DecompiledModule);

            // Save the query assembly to run Corral
            var queryAssemblyPath = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyPath);

            var result = ExecuteChecker(queryAssemblyPath);

            return EvaluateQueries(actions, result);
        }

        private List<MethodDefinition> GenerateQueries(State state, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            var queries = new List<MethodDefinition>();

            foreach (var target in actions)
            {
                // Add positive query
                queries.Add(GenerateQuery(state, action, target));
                // Add negative query
                queries.Add(GenerateQuery(state, action, target, true));
            }

            this.TotalGeneratedQueriesCount += queries.Count;

            return queries;
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

        private MethodDefinition GenerateQuery(string name, IMethodDefinition action)
        {
            // I need to assign the queries to the type that I'm processing
            var type = queryAssembly.DecompiledModule.AllTypes.Find(x => x.Name == inputType.Name) as NamespaceTypeDefinition;
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

            if (inputType.IsGeneric)
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

            //if (method.Parameters == null)
            //    method.Parameters = new List<IParameterDefinition>();

            //method.Parameters.AddRange(action.Parameters);
            //var mc = inputContractProvider.GetMethodContractFor(action);

            //if (mc != null && mc.Preconditions.Count() > 0)
            //{
            //    var asserts = from pre in mc.Preconditions
            //                  select new AssertStatement()
            //                  {
            //                      Condition = pre.Condition
            //                  };

            //    block.Statements.AddRange(asserts);
            //}

            //IBlockStatement actionBodyBlock = null;

            //if (action.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
            //{
            //    var actionBody = action.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
            //    actionBodyBlock = actionBody.Block;
            //}
            //else if (action.Body is SourceMethodBody)
            //{
            //    var actionBody = action.Body as SourceMethodBody;
            //    actionBodyBlock = actionBody.Block;
            //}

            ////Por tratarse de un constructor skipeamos
            ////el primer statement porque es base..ctor();
            //var skipCount = action.IsConstructor ? 1 : 0;
            //block.Statements.AddRange(actionBodyBlock.Statements.Skip(skipCount));

            //if (mc != null && mc.Postconditions.Count() > 0)
            //{
            //    var assumes = from post in mc.Postconditions
            //                  select new AssumeStatement()
            //                  {
            //                      Condition = post.Condition
            //                  };

            //    ////Ponemos los assume antes del return
            //    //block.Statements.InsertRange(block.Statements.Count - 1, assumes);
            //    block.Statements.InsertRange(block.Statements.Count, assumes);
            //}

            return block;
        }

        private ActionAnalysisResults EvaluateQueries(List<IMethodDefinition> actions, Dictionary<string, List<ResultKind>> result)
        {
            var analysisResult = new ActionAnalysisResults();
            analysisResult.EnabledActions.AddRange(actions);
            analysisResult.DisabledActions.AddRange(actions);

            foreach (var entry in result)
            {
                if (entry.Value.Contains(ResultKind.FalseEnsures) ||
                    entry.Value.Contains(ResultKind.FalseRequires) ||
                    entry.Value.Contains(ResultKind.UnsatisfiableRequires) ||
                    entry.Value.Contains(ResultKind.UnprovenEnsures))
                {
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
                        var method = inputType.Methods.Find(m => m.GetUniqueName() == actionName);
                        analysisResult.DisabledActions.Remove(method);
                    }
                    else
                    {
                        var method = inputType.Methods.Find(m => m.GetUniqueName() == actionName);
                        analysisResult.EnabledActions.Remove(method);
                    }

                    if (entry.Value.Contains(ResultKind.UnprovenEnsures))
                        this.UnprovenQueriesCount++;
                }
            }

            return analysisResult;
        }

        private Dictionary<string, List<ResultKind>> ExecuteChecker(string queryAssemblyName)
        {
            string libPaths;

            if (inputAssembly.Module.TargetRuntimeVersion.StartsWith("v4.0"))
                libPaths = Configuration.ExpandVariables(Resources.Netv40);
            else
                libPaths = Configuration.ExpandVariables(Resources.Netv35);

            var inputAssemblyPath = Path.GetDirectoryName(inputAssembly.FileName);
            libPaths = string.Format("{0};{1}", libPaths, inputAssemblyPath);

            var typeName = queryType.ToString();

            if (queryType.IsGeneric)
                typeName = string.Format("{0}`{1}", typeName, queryType.GenericParameterCount);

            output = new StringBuilder();
            var cccheckArgs = Configuration.CheckerArguments;
            cccheckArgs = cccheckArgs.Replace("@assemblyName", queryAssemblyName);
            cccheckArgs = cccheckArgs.Replace("@fullTypeName", typeName);
            cccheckArgs = cccheckArgs.Replace("@libPaths", libPaths);

            using (var cccheck = new Process())
            {
                cccheck.StartInfo = new ProcessStartInfo()
                {
                    FileName = Configuration.CheckerFileName,
                    Arguments = cccheckArgs,
                    WorkingDirectory = Directory.GetCurrentDirectory(),
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                cccheck.OutputDataReceived += cccheck_DataReceived;
                cccheck.ErrorDataReceived += cccheck_DataReceived;
                cccheck.Start();
                cccheck.BeginErrorReadLine();
                cccheck.BeginOutputReadLine();
                cccheck.WaitForExit();

                var analysisDuration = cccheck.ExitTime - cccheck.StartTime;
                this.TotalAnalysisDuration += analysisDuration;
                this.ExecutionsCount++;

                //#if DEBUG
                //                System.Console.WriteLine("\tCode Contracts analysis duration: {0}", analysisDuration);
                //#endif
            }

            var outputString = output.ToString();
            var matches = outputParser.Matches(outputString);
            output = null;

            var result = new Dictionary<string, List<ResultKind>>();
            string currentMethod = null;

            foreach (Match m in matches)
            {
                if (m.Groups["MethodName"].Success)
                {
                    currentMethod = m.Groups["MethodName"].Value;
                    //Para el caso de .#ctor
                    currentMethod = currentMethod.Replace("#", string.Empty);

                    result.Add(currentMethod, new List<ResultKind>());
                }
                else if (m.Groups["Message"].Success)
                {
                    var message = m.Groups["Message"].Value;
                    var resultKind = ParseResultKind(message);
                    result[currentMethod].Add(resultKind);
                }
            }

            return result;
        }

        private ResultKind ParseResultKind(string message)
        {
            if (message.Contains("Requires (including invariants) are unsatisfiable"))
                return ResultKind.UnsatisfiableRequires;
            else if (message.Contains("ensures unproven"))
                return ResultKind.UnprovenEnsures;
            else if (message.Contains("ensures is false"))
                return ResultKind.FalseEnsures;
            else if (message.Contains("ensures (always false) may be reachable"))
                return ResultKind.FalseEnsures;
            else if (message.Contains("requires is false"))
                return ResultKind.FalseRequires;
            else return ResultKind.None;
        }

        private void cccheck_DataReceived(object sender, DataReceivedEventArgs e)
        {
            output.AppendLine(e.Data);
        }

        public TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets)
        {
            var queryAssemblyName = Path.Combine(Configuration.TempPath, queryAssembly.Module.ModuleName.Value);
            var contractMethods = new ContractMethods(host);

            queryContractProvider = new ContractProvider(contractMethods, queryAssembly.Module);
            GenerateQueries(source, action, targets);
            queryAssembly.InjectContracts(queryContractProvider);

            queryReplacer.Rewrite(queryAssembly.DecompiledModule);
            queryAssembly.Save(queryAssemblyName);
            queryType.Methods.Clear();

            var result = ExecuteChecker(queryAssemblyName);
            var evalResult = EvaluateQueries(source, action, targets, result);

            return evalResult;
        }

        private void GenerateQueries(State state, IMethodDefinition action, List<State> states)
        {
            foreach (var target in states)
            {
                var query = GenerateQuery(state, action, target);

                query.ContainingTypeDefinition = queryType;
                queryType.Methods.Add(query);

                this.TotalGeneratedQueriesCount++;
            }
        }

        private MethodDefinition GenerateQuery(State state, IMethodDefinition action, State target)
        {
            var contracts = new MethodContract();
            //var typeInv = Helper.GenerateTypeInvariant(host, inputContractProvider, inputType);

            //var typeInvPre = from expr in typeInv
            //                 select new Precondition()
            //                 {
            //                     Condition = expr
            //                 };

            var stateInv = Helper.GenerateStateInvariant(host, inputContractProvider, inputType, state);

            var precondition = from expr in stateInv
                               select new Precondition()
                               {
                                   Condition = expr
                               };

            //contracts.Preconditions.AddRange(typeInvPre);
            contracts.Preconditions.AddRange(precondition);

            //var typeInvPost = from expr in typeInv
            //                  select new Postcondition()
            //                  {
            //                      Condition = expr
            //                  };

            var targetInv = Helper.GenerateStateInvariant(host, inputContractProvider, inputType, target);

            var postcondition = new Postcondition()
            {
                Condition = new LogicalNot()
                {
                    Type = host.PlatformType.SystemBoolean,
                    Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
                },
            };

            //contracts.Postconditions.AddRange(typeInvPost);
            contracts.Postconditions.Add(postcondition);

            var actionName = action.GetUniqueName();
            var stateName = state.UniqueName;
            var targetName = target.UniqueName;
            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);
            var method = GenerateQuery(methodName, action);

            queryContractProvider.AssociateMethodWithContract(method, contracts);
            return method;
        }

        private TransitionAnalysisResult EvaluateQueries(State source, IMethodDefinition action, List<State> targets, Dictionary<string, List<ResultKind>> result)
        {
            var analysisResult = new TransitionAnalysisResult();

            foreach (var entry in result)
            {
                if (entry.Value.Contains(ResultKind.FalseEnsures) ||
                    entry.Value.Contains(ResultKind.UnprovenEnsures))
                {
                    var query = entry.Key;
                    var queryParametersStart = query.LastIndexOf('(');

                    // Borramos los parametros del query
                    if (queryParametersStart != -1)
                        query = query.Remove(queryParametersStart);

                    var targetNameStart = query.LastIndexOf(methodNameDelimiter) + 1;
                    var targetName = query.Substring(targetNameStart);
                    var target = targets.Find(s => s.UniqueName == targetName);
                    var isUnproven = entry.Value.Contains(ResultKind.UnprovenEnsures);

                    if (target != null)
                    {
                        var transition = new Transition(source, action, target, isUnproven);
                        analysisResult.Transitions.Add(transition);
                    }

                    if (isUnproven)
                        this.UnprovenQueriesCount++;
                }
            }

            return analysisResult;
        }

        private string GetQueryAssemblyPath()
        {
            Contract.Requires(inputAssembly != null);

            return Path.Combine(Configuration.TempPath, inputAssembly.Module.ModuleName.Value + ".tmp");
        }

        private PdbReader GetPDBReader(IModule module, IContractAwareHost host)
        {
            Contract.Requires(module != null && host != null);

            PdbReader pdbReader = null;
            string pdbFile = Path.ChangeExtension(module.Location, "pdb");
            if (File.Exists(pdbFile))
                using (var pdbStream = File.OpenRead(pdbFile))
                    pdbReader = new PdbReader(pdbStream, host);
            return pdbReader;
        }
    }
}