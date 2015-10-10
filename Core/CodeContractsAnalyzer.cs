using Contractor.Core.Properties;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Research.CodeAnalysis;
using System.Collections;
using System.Collections.Generic;
using System.Compiler.Analysis;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace Contractor.Core
{
    class CodeContractsAnalyzer : Analyzer
    {
        private enum ResultKind
        {
            None,
            UnsatisfiableRequires,
            FalseRequires,
            UnprovenEnsures,
            FalseEnsures
        }

        public CodeContractsAnalyzer(IContractAwareHost host, AssemblyInfo assembly, NamespaceTypeDefinition type, CancellationToken token)
            : base(host, assembly.Module, type, token)
        {
            Contract.Requires(host != null && assembly != null && type != null && token != null);

            ITypeContract typeContract = this.inputContractProvider.GetTypeContractFor(type);
            this.queryContractProvider.AssociateTypeWithContract(this.typeToAnalyze, typeContract);

            // We assume that the methods were already proved by cccheck during the compilation
            AddVerifierAttribute();
        }

        private void AddVerifierAttribute()
        {
            Contract.Requires(this.typeToAnalyze != null);

            foreach (var m in this.typeToAnalyze.Methods)
            {
                if (m.Visibility != TypeMemberVisibility.Public)
                {
                    continue;
                }

                var disableVerifier = new CustomAttribute()
                {
                    Arguments = new List<IMetadataExpression>() { new MetadataConstant() { Value = false, Type = this.host.PlatformType.SystemBoolean } },
                    Constructor = new Microsoft.Cci.MethodReference(this.host, this.host.PlatformType.SystemDiagnosticsContractsContract.ResolvedType.ContainingNamespace.GetMembersNamed(this.host.NameTable.GetNameFor("ContractVerificationAttribute"), false).First() as INamespaceTypeReference, CallingConvention.HasThis,
                this.host.PlatformType.SystemVoid, this.host.NameTable.GetNameFor(".ctor"), 0, this.host.PlatformType.SystemBoolean),
                };

                var tmp = m as MethodDefinition;
                if (tmp.Attributes == null)
                {
                    tmp.Attributes = new List<ICustomAttribute>();
                }
                tmp.Attributes.Add(disableVerifier);
            }
        }

        public override ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions)
        {
            Contract.Requires(source != null && action != null && actions != null);
            Contract.Requires(actions.Count > 0);

            var queries = base.GenerateQueries<Action>(source, action, actions);
            this.typeToAnalyze.Methods.AddRange(queries);
            queryAssembly.InjectContracts(this.queryContractProvider);

            var queryAssemblyName = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyName);
            this.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            var result = executeChecker(queryAssemblyName);
            var evalResult = evaluateQueries(actions, result);

            return evalResult;
        }

        private ActionAnalysisResults evaluateQueries(List<Action> actions, Dictionary<string, List<ResultKind>> result)
        {
            var analysisResult = new ActionAnalysisResults();
            analysisResult.EnabledActions.AddRange(from a in actions select a.Method);
            analysisResult.DisabledActions.AddRange(from a in actions select a.Method);

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
                        analysisResult.DisabledActions.RemoveAll(m => m.GetUniqueName() == actionName);
                    }
                    else
                    {
                        analysisResult.EnabledActions.RemoveAll(m => m.GetUniqueName() == actionName);
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

            var typeName = this.typeToAnalyze.ToString();

            if (this.typeToAnalyze.IsGeneric)
                typeName = string.Format("{0}`{1}", typeName, this.typeToAnalyze.GenericParameterCount);

            string outputString;
            using (var output = new StringWriter())
            {
                var cccheckArgs = Configuration.CheckerArguments;
                cccheckArgs = cccheckArgs.Replace("\"@assemblyName\"", queryAssemblyName);
                cccheckArgs = cccheckArgs.Replace("@fullTypeName", typeName);

                // If the user didn't stop the analysis, execute cccheck
                if (!base.token.IsCancellationRequested)
                {
                    var cccheckTime = Stopwatch.StartNew();

                    var args = cccheckArgs.Split(' ');
                    args[args.Length - 2] = args[args.Length - 2].Replace("@libPaths", libPaths);

                    IOutputFullResultsFactory<System.Compiler.Method, System.Compiler.AssemblyNode> outputFactory =
                        new FullTextWriterOutputFactory<System.Compiler.Method, System.Compiler.AssemblyNode>(output);
                    var errorCode = Clousot.ClousotMain(args, CCIMDDecoder.Value, CCIContractDecoder.Value, new Hashtable(), outputFactory);

                    cccheckTime.Stop();

                    if (errorCode != 0)
                    {
                        Logger.Log(LogLevel.Warn, string.Format("Clousot exited with code {0}", errorCode));
                    }

                    var analysisDuration = cccheckTime.Elapsed;
                    this.TotalAnalysisDuration += analysisDuration;
                    this.ExecutionsCount++;
                }

                outputString = output.ToString();
            }

            var result = new Dictionary<string, List<ResultKind>>();
            using (var reader = new StringReader(outputString))
            {
                for (string ccMessage = reader.ReadLine(); ccMessage != null; ccMessage = reader.ReadLine())
                {
                    try
                    {
                        string currentMethod = ccMessage.Substring(0, ccMessage.IndexOf('['));
                        //Para el caso de .#ctor
                        currentMethod = currentMethod.Replace("#", string.Empty);

                        if (!result.ContainsKey(currentMethod))
                            result.Add(currentMethod, new List<ResultKind>());

                        var message = ccMessage.Substring(ccMessage.IndexOf(':') + 1).Trim();
                        var resultKind = parseResultKind(message);
                        result[currentMethod].Add(resultKind);
                    }
                    catch { }
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

            else if (message.Contains("ensures unreachable"))
                return ResultKind.UnprovenEnsures;

            else if (message.Contains("ensures is false"))
                return ResultKind.FalseEnsures;

            else if (message.Contains("ensures (always false) may be reachable"))
                return ResultKind.FalseEnsures;

            else if (message.Contains("requires is false"))
                return ResultKind.FalseRequires;

            else return ResultKind.None;
        }

        public override TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets)
        {
            var queries = base.GenerateQueries<State>(source, action, targets);
            this.typeToAnalyze.Methods.AddRange(queries);
            queryAssembly.InjectContracts(this.queryContractProvider);

            var queryAssemblyName = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyName);
            this.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            var result = executeChecker(queryAssemblyName);
            var evalResult = evaluateQueries(source, action.Method, targets, result);

            return evalResult;
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
                    var target = targets.Find(s => s.Name == targetName);
                    var isUnproven = entry.Value.Contains(ResultKind.UnprovenEnsures);

                    if (target != null)
                    {
                        var transition = new Transition(new CciAction(action), source, target, isUnproven);
                        analysisResult.Transitions.Add(transition);
                    }

                    if (isUnproven)
                        this.UnprovenQueriesCount++;
                }
            }

            return analysisResult;
        }
    }
}