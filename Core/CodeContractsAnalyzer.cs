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
            FalseEnsures,
            SuggestedRequires,
            MissingRequires
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

        public override ActionAnalysisResults AnalyzeActions(State source, IMethodDefinition action, List<IMethodDefinition> actions)
        {
            Contract.Requires(source != null && action != null && actions != null);
            Contract.Requires(actions.Count > 0);

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
                        LogManager.Log(LogLevel.Warn, string.Format("Clousot exited with code {0}", errorCode));
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

        public Dictionary<string, List<string>> executeCheckerConditions(string queryAssemblyName)
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
                //cccheckArgs += " -infer=requires -suggest=requires";
                //cccheckArgs += " -nologo -nopex -remote  -suggest=!! -premode combined -suggest codefixes -framework:v4.5 -warninglevel full  -maxwarnings 10000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false  -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires";
                cccheckArgs += " -nobox -nologo -nopex -remote  -suggest=!! -premode combined -suggest codefixes -framework:v4.0 -warninglevel full  -maxwarnings 100000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false  -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -missingPublicEnsuresAreErrors  -suggest calleeassumes -suggest assumes -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires -infer methodensures -infer objectinvariants";
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
                        LogManager.Log(LogLevel.Warn, string.Format("Clousot exited with code {0}", errorCode));
                    }

                    var analysisDuration = cccheckTime.Elapsed;
                    this.TotalAnalysisDuration += analysisDuration;
                    this.ExecutionsCount++;
                }

                outputString = output.ToString();
            }

            var result = new Dictionary<string, List<string>>();
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
                            result.Add(currentMethod, new List<string>());

                        var message = ccMessage.Substring(ccMessage.IndexOf(':') + 1).Trim();
                        var condition = CSGenerator.parseCondition(message);

                        if (!condition.Equals(""))
                            result[currentMethod].Add(condition);
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
            
            else if (message.Contains("Suggested requires: "))
                return ResultKind.SuggestedRequires;

            else if (message.Contains("Missing precondition in an externally visible method"))
                return ResultKind.MissingRequires;

            else return ResultKind.None;
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
            TransitionAnalysisResult evalResult = null;

            if (Configuration.TransitionsWithConditions)
            {
                var resultConditions = executeCheckerConditions(queryAssemblyName);
                evalResult = evaluateQueries(source, action, targets, result, resultConditions);
            }
            else
            {
                evalResult = evaluateQueries(source, action, targets, result);
            }

            return evalResult;
        }

        /*private Dictionary<string, List<string>> testConditions()
        {
            var result = new Dictionary<string, List<string>>();
            result.Add("ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid", new List<string>());
            result["ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid"].Add("C1");
            result["ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid"].Add("C2");
            
            result.Add("ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid$nextSystemInt32SystemObject", new List<string>());
            result["ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid$nextSystemInt32SystemObject"].Add("C3");

            result.Add("Examples.ICSE2011.ListItr_mut2145.ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid$nextSystemInt32SystemObject", new List<string>());
            result["Examples.ICSE2011.ListItr_mut2145.ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~ctorExamplesICSE2011ArrayListJavaSystemInt32SystemVoid~addSystemObjectSystemVoid$nextSystemInt32SystemObject"].Add("C4");
            return result;
        }*/

        private TransitionAnalysisResult evaluateQueries(State source, IMethodDefinition action, List<State> targets, Dictionary<string, List<ResultKind>> result,Dictionary<string, List<string>> conditions)
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
                        var cs = CSGenerator.generateCS(conditions, entry.Key);
                        var transition = new Transition(action, source, target, isUnproven,cs);
                        analysisResult.Transitions.Add(transition);
                    }

                    if (isUnproven)
                        this.UnprovenQueriesCount++;
                }
            }

            return analysisResult;
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

                        var transition = new Transition(action, source, target, isUnproven,"SIN CONDICIONES");
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