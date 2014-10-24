using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Contractor.Core.Properties;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;

namespace Contractor.Core
{
    internal class CodeContractsAnalyzer : IAnalyzer
    {
        private enum ResultKind
        {
            None,
            UnsatisfiableRequires,
            FalseRequires,
            UnprovenEnsures,
            FalseEnsures
        }

        private const string notPrefix = ".Not.";
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

        public CodeContractsAnalyzer(IContractAwareHost host, AssemblyInfo assembly, NamespaceTypeDefinition type)
        {
            this.outputParser = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);
            this.host = host;
            this.inputAssembly = assembly;
            this.inputType = type;

            this.inputContractProvider = assembly.ExtractContracts();
            this.generateQueryAssembly();
        }

        public TimeSpan TotalAnalysisDuration { get; private set; }

        public int ExecutionsCount { get; private set; }

        public int TotalGeneratedQueriesCount { get; private set; }

        public int UnprovenQueriesCount { get; private set; }

        private void generateQueryAssembly()
        {
            var coreAssembly = host.LoadAssembly(host.CoreAssemblySymbolicIdentity);

            var assembly = new Assembly()
            {
                Name = host.NameTable.GetNameFor("Query"),
                ModuleName = host.NameTable.GetNameFor("query.dll"),
                Kind = ModuleKind.DynamicallyLinkedLibrary,
                TargetRuntimeVersion = coreAssembly.TargetRuntimeVersion,
            };

            assembly.AssemblyReferences.Add(coreAssembly);
            assembly.AssemblyReferences.Add(inputAssembly.Module.ContainingAssembly);

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

            this.queryType = new NamespaceTypeDefinition()
            {
                ContainingUnitNamespace = rootUnitNamespace,
                InternFactory = host.InternFactory,
                IsClass = true,
                IsPublic = true,
                Name = host.NameTable.GetNameFor("Query"),
                GenericParameters = (inputType.GenericParameters != null) ? inputType.GenericParameters.ToList() : new List<IGenericTypeParameter>()
            };

            queryType.BaseClasses = new List<ITypeReference>();
            queryType.BaseClasses.Add(host.PlatformType.SystemObject);

            rootUnitNamespace.Members.Add(queryType);
            assembly.AllTypes.Add(queryType);

            ITypeReference inputTypeReference = inputType;

            if (inputType.IsGeneric)
            {
                var typeReference = MutableModelHelper.GetGenericTypeInstanceReference(queryType.GenericParameters, inputType, host.InternFactory, null);
                this.specializedInputType = typeReference.ResolvedType as Microsoft.Cci.Immutable.GenericTypeInstance;
                inputTypeReference = typeReference;
            }

            var self = new FieldDefinition()
            {
                ContainingTypeDefinition = queryType,
                InternFactory = host.InternFactory,
                Name = host.NameTable.GetNameFor("self"),
                Type = inputTypeReference,
                Visibility = TypeMemberVisibility.Public
            };

            queryType.Methods = new List<IMethodDefinition>();
            queryType.Fields = new List<IFieldDefinition>();
            queryType.Fields.Add(self);

            this.queryAssembly = new AssemblyInfo(host, assembly);
            this.queryReplacer = new QueryReplacer(host, inputType, queryType);
        }

        public ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            var queryAssemblyName = Path.Combine(Configuration.TempPath, queryAssembly.Module.ModuleName.Value);
            var contractMethods = new ContractMethods(host);

            queryContractProvider = new ContractProvider(contractMethods, queryAssembly.Module);
            generateQueries(source, action, actions);
            queryAssembly.InjectContracts(queryContractProvider);

            queryReplacer.Rewrite(queryAssembly.DecompiledModule);
            queryAssembly.Save(queryAssemblyName);
            queryType.Methods.Clear();

            var result = executeChecker(queryAssemblyName);
            var evalResult = evaluateQueries(actions, result);

            return evalResult;
        }

        private void generateQueries(State state, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            foreach (var target in actions)
            {
                var positiveQuery = generatePositiveNegativeQuery(state, action, target, false);
                var negativeQuery = generatePositiveNegativeQuery(state, action, target, true);

                positiveQuery.ContainingTypeDefinition = queryType;
                negativeQuery.ContainingTypeDefinition = queryType;

                queryType.Methods.Add(positiveQuery);
                queryType.Methods.Add(negativeQuery);

                this.TotalGeneratedQueriesCount += 2;
            }
        }

        private MethodDefinition generatePositiveNegativeQuery(State state, IMethodDefinition action, IMethodDefinition target, bool negative)
        {
            var contracts = new MethodContract();
            //var mci = inputContractProvider.GetTypeContractFor(inputType);
            var mct = inputContractProvider.GetMethodContractFor(target);

            //if (mci != null && mci.Invariants.Count() > 0)
            //{
            //    var pres = from inv in mci.Invariants
            //               select new Precondition()
            //               {
            //                   Condition = inv.Condition
            //               };

            //    contracts.Preconditions.AddRange(pres);
            //}

            foreach (var c in state.EnabledActions)
            {
                var mc = inputContractProvider.GetMethodContractFor(c);
                if (mc == null) continue;

                contracts.Preconditions.AddRange(mc.Preconditions);
            }

            foreach (var c in state.DisabledActions)
            {
                var mc = inputContractProvider.GetMethodContractFor(c);

                if (mc == null || mc.Preconditions.Count() == 0)
                {
                    var pre = new Precondition()
                    {
                        Condition = new CompileTimeConstant()
                        {
                            Type = host.PlatformType.SystemBoolean,
                            Value = false
                        }
                    };

                    contracts.Preconditions.Add(pre);
                }
                else
                {
                    var pres = from pre in mc.Preconditions
                               select new Precondition()
                               {
                                   Condition = new LogicalNot()
                                   {
                                       Type = host.PlatformType.SystemBoolean,
                                       Operand = pre.Condition
                                   }
                               };

                    contracts.Preconditions.AddRange(pres);
                }
            }

            if (mct == null || mct.Preconditions.Count() == 0)
            {
                if (negative)
                {
                    var post = new Postcondition()
                    {
                        Condition = new CompileTimeConstant()
                        {
                            Type = host.PlatformType.SystemBoolean,
                            Value = false
                        }
                    };

                    contracts.Postconditions.Add(post);
                }
            }
            else
            {
                if (negative)
                {
                    var exprs = (from pre in mct.Preconditions
                                 select pre.Condition).ToList();

                    var post = new Postcondition()
                    {
                        Condition = new LogicalNot()
                        {
                            Type = host.PlatformType.SystemBoolean,
                            Operand = Helper.JoinWithLogicalAnd(host, exprs, true)
                        }
                    };

                    contracts.Postconditions.Add(post);
                }
                else
                {
                    var posts = from pre in mct.Preconditions
                                select new Postcondition()
                                {
                                    Condition = pre.Condition
                                };

                    contracts.Postconditions.AddRange(posts);
                }
            }

            var prefix = negative ? notPrefix : string.Empty;
            var actionName = action.GetUniqueName();
            var stateName = state.UniqueName;
            var targetName = target.GetUniqueName();
            var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);
            var method = generateQuery(methodName, action);

            queryContractProvider.AssociateMethodWithContract(method, contracts);
            return method;
        }

        private MethodDefinition generateQuery(string name, IMethodDefinition action)
        {
            var method = new MethodDefinition()
            {
                Attributes = new List<ICustomAttribute>(action.Attributes),
                CallingConvention = Microsoft.Cci.CallingConvention.HasThis,
                InternFactory = host.InternFactory,
                IsCil = true,
                IsStatic = false,
                Name = host.NameTable.GetNameFor(name),
                Type = action.Type,
                Visibility = TypeMemberVisibility.Public,
                GenericParameters = new List<IGenericMethodParameter>(action.GenericParameters)
            };

            BlockStatement block = null;

            if (Configuration.InlineMethodsBody)
            {
                block = inlineMethodBody(action, method);
            }
            else
            {
                block = callMethod(action);
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

        private BlockStatement callMethod(IMethodDefinition action)
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

        private BlockStatement inlineMethodBody(IMethodDefinition action, MethodDefinition method)
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

                ////Ponemos los assume antes del return
                //block.Statements.InsertRange(block.Statements.Count - 1, assumes);
                block.Statements.InsertRange(block.Statements.Count, assumes);
            }

            return block;
        }

        private ActionAnalysisResults evaluateQueries(List<IMethodDefinition> actions, Dictionary<string, List<ResultKind>> result)
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

        private Dictionary<string, List<ResultKind>> executeChecker(string queryAssemblyName)
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
                    var resultKind = parseResultKind(message);
                    result[currentMethod].Add(resultKind);
                }
            }

            return result;
        }

        private ResultKind parseResultKind(string message)
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
            generateQueries(source, action, targets);
            queryAssembly.InjectContracts(queryContractProvider);

            queryReplacer.Rewrite(queryAssembly.DecompiledModule);
            queryAssembly.Save(queryAssemblyName);
            queryType.Methods.Clear();

            var result = executeChecker(queryAssemblyName);
            var evalResult = evaluateQueries(source, action, targets, result);

            return evalResult;
        }

        private void generateQueries(State state, IMethodDefinition action, List<State> states)
        {
            foreach (var target in states)
            {
                var query = generateQuery(state, action, target);

                query.ContainingTypeDefinition = queryType;
                queryType.Methods.Add(query);

                this.TotalGeneratedQueriesCount++;
            }
        }

        private MethodDefinition generateQuery(State state, IMethodDefinition action, State target)
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
            var method = generateQuery(methodName, action);

            queryContractProvider.AssociateMethodWithContract(method, contracts);
            return method;
        }

        private TransitionAnalysisResult evaluateQueries(State source, IMethodDefinition action, List<State> targets, Dictionary<string, List<ResultKind>> result)
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
    }

    //Reemplaza los 'this' que apuntan al 'module' del input para que pasen a apuntar al nuevo 'module' creado para las queries
    internal class QueryReplacer : CodeAndContractRewriter
    {
        private NamespaceTypeDefinition inputType;
        private NamespaceTypeDefinition queryType;
        private IBoundExpression selfExpression;
        private IMethodDefinition currentMethod;

        public QueryReplacer(IMetadataHost host, NamespaceTypeDefinition inputType, NamespaceTypeDefinition queryType)
            : base(host)
        {
            this.inputType = inputType;
            this.queryType = queryType;

            var self = queryType.Fields.Single();

            this.selfExpression = new BoundExpression()
            {
                Definition = self,
                Instance = new ThisReference(),
                Type = self.Type
            };
        }

        public override IMethodDefinition Rewrite(IMethodDefinition method)
        {
            currentMethod = method;
            var newMethod = base.Rewrite(method);
            currentMethod = null;
            return newMethod;
        }

        public override IExpression Rewrite(IBoundExpression boundExpression)
        {
            if (boundExpression == selfExpression)
                return boundExpression;

            return base.Rewrite(boundExpression);
        }

        public override IExpression Rewrite(IThisReference thisReference)
        {
            return selfExpression;
        }

        public override IGenericTypeParameter Rewrite(IGenericTypeParameter genericTypeParameter)
        {
            var newGenericTypeParameter = new GenericTypeParameter();

            newGenericTypeParameter.Copy(genericTypeParameter, host.InternFactory);
            newGenericTypeParameter.DefiningType = queryType;

            return newGenericTypeParameter;
        }

        public override IGenericMethodParameter Rewrite(IGenericMethodParameter genericMethodParameter)
        {
            var newGenericMethodParameter = new GenericMethodParameter();

            newGenericMethodParameter.Copy(genericMethodParameter, host.InternFactory);
            newGenericMethodParameter.DefiningMethod = currentMethod;

            return newGenericMethodParameter;
        }
    }
}