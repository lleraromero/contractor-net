﻿using Contractor.Core.Properties;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableCodeModel.Contracts;
using Microsoft.Cci.MutableContracts;
using System.Collections.Generic;
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

        private const string pattern = @"^ Method \W* \d+ \W* : (< [a-z ]+ >)? \W* (?<MethodName> [^(\r]+ (\( [^)]* \))?) \r | ^ [^( ]+ (\( [^)]* \))? \W* (\[ [^]]* \])? \W* : \W* ([^:]+ :)? \W* (?<Message> [^\r]+) \r";

        private readonly Regex outputParser;
        private StringBuilder output;

        public CodeContractsAnalyzer(IContractAwareHost host, AssemblyInfo assembly, NamespaceTypeDefinition type, CancellationToken token)
            : base(host, assembly.Module, type, token)
        {
            Contract.Requires(assembly != null && host != null && type != null);

            ITypeContract typeContract = this.inputContractProvider.GetTypeContractFor(type);
            this.queryContractProvider.AssociateTypeWithContract(this.typeToAnalyze, typeContract);

            // We assume that the methods were already proved by cccheck during the compilation
            AddVerifierAttribute();

            this.outputParser = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        private void AddVerifierAttribute()
        {
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

        public override ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            var queries = base.GenerateQueries<IMethodDefinition>(source, action, actions);
            this.typeToAnalyze.Methods.AddRange(queries);
            queryAssembly.InjectContracts(this.queryContractProvider);

            var queryAssemblyName = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyName);
            this.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            var result = executeChecker(queryAssemblyName);
            var evalResult = evaluateQueries(actions, result);

            return evalResult;
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

            output = new StringBuilder();
            var cccheckArgs = Configuration.CheckerArguments;
            cccheckArgs = cccheckArgs.Replace("@assemblyName", queryAssemblyName);
            cccheckArgs = cccheckArgs.Replace("@fullTypeName", typeName);
            cccheckArgs = cccheckArgs.Replace("@libPaths", libPaths);

            // If the user didn't stop the analysis, execute cccheck
            if (!base.token.IsCancellationRequested)
            {
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
                }
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

        public override TransitionAnalysisResult AnalyzeTransitions(State source, IMethodDefinition action, List<State> targets)
        {
            var queries = base.GenerateQueries<State>(source, action, targets);
            this.typeToAnalyze.Methods.AddRange(queries);
            queryAssembly.InjectContracts(this.queryContractProvider);

            var queryAssemblyName = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyName);
            this.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            var result = executeChecker(queryAssemblyName);
            var evalResult = evaluateQueries(source, action, targets, result);

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
                    var target = targets.Find(s => s.UniqueName == targetName);
                    var isUnproven = entry.Value.Contains(ResultKind.UnprovenEnsures);

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
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