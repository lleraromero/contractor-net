using Contractor.Core.Properties;
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
        private NamespaceTypeDefinition queryType;
        private StringBuilder output;

        public CodeContractsAnalyzer(IContractAwareHost host, AssemblyInfo assembly, NamespaceTypeDefinition type)
            : base(host, assembly.Module, type)
        {
            Contract.Requires(assembly != null && host != null && type != null);

            ITypeContract typeContract = this.inputContractProvider.GetTypeContractFor(type);
            this.queryContractProvider.AssociateTypeWithContract(this.typeToAnalyze, typeContract);

            this.queryType = this.queryAssembly.DecompiledModule.AllTypes.Find(t => t.Name == type.Name) as NamespaceTypeDefinition;
            this.outputParser = new Regex(pattern, RegexOptions.ExplicitCapture | RegexOptions.IgnorePatternWhitespace | RegexOptions.Multiline | RegexOptions.Compiled);
        }

        public override ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            var queries = base.GenerateQueries<IMethodDefinition>(source, action, actions);
            queryType.Methods.AddRange(queries);
            queryAssembly.InjectContracts(this.queryContractProvider);

            var queryAssemblyName = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyName);
            queryType.Methods.RemoveAll(m => queries.Contains(m));

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
                        var method = typeToAnalyze.Methods.Find(m => m.GetUniqueName() == actionName);
                        analysisResult.DisabledActions.Remove(method);
                    }
                    else
                    {
                        var method = typeToAnalyze.Methods.Find(m => m.GetUniqueName() == actionName);
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
            queryType.Methods.AddRange(queries);
            queryAssembly.InjectContracts(this.queryContractProvider);

            var queryAssemblyName = GetQueryAssemblyPath();
            queryAssembly.Save(queryAssemblyName);
            queryType.Methods.RemoveAll(m => queries.Contains(m));

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
}