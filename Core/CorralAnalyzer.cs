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

        public CorralAnalyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type)
            : base(host, module, type)
        {
            Contract.Requires(module != null && host != null && type != null);

            ITypeContract typeContract = this.inputContractProvider.GetTypeContractFor(type);
            this.queryContractProvider.AssociateTypeWithContract(this.typeToAnalyze, typeContract);
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
            this.typeToAnalyze.Methods.AddRange(queries);

            // I need to replace Pre/Post with Assume/Assert
            ILocalScopeProvider localScopeProvider = new Microsoft.Cci.ILToCodeModel.Decompiler.LocalScopeProvider(GetPDBReader(queryAssembly.Module, host));
            ISourceLocationProvider sourceLocationProvider = GetPDBReader(queryAssembly.Module, host);
            var trans = new ContractRewriter(host, queryContractProvider, sourceLocationProvider);
            trans.Rewrite(queryAssembly.DecompiledModule);

            // Save the query assembly to run Corral
            queryAssembly.Save(GetQueryAssemblyPath());

            var result = ExecuteChecker(queries);

            // I don't need the queries anymore
            this.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            return result;
        }

        private ActionAnalysisResults EvaluateQueries(List<IMethodDefinition> actions, Dictionary<MethodDefinition, ResultKind> result)
        {
            var analysisResult = new ActionAnalysisResults();
            analysisResult.EnabledActions.AddRange(actions);
            analysisResult.DisabledActions.AddRange(actions);

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
                            analysisResult.DisabledActions.RemoveAll(m => m.GetUniqueName() == actionName);
                        }
                        else
                        {
                            analysisResult.EnabledActions.RemoveAll(m => m.GetUniqueName() == actionName);
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

            foreach (var query in queries)
            {
                var queryName = CreateUniqueMethodName(query);

                string output = RunCorral(queryName);
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

                bct.OutputDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        LogManager.Log(LogLevel.Debug, e.Data);
                };
                bct.ErrorDataReceived += (sender, e) =>
                {
                    if (!string.IsNullOrEmpty(e.Data))
                        LogManager.Log(LogLevel.Fatal, e.Data);
                };
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

            var args = string.Format("{0} /main:{1} {2}", GetQueryAssemblyPath().Replace("dll", "bpl"), method, Configuration.CorralArguments);    // recursionBound 3 es absolutamente arbitrario :)

            var timer = Stopwatch.StartNew();

            try
            {
                if (cba.Driver.run(args.Split(' ')) != 0)
                    throw new Exception("Error executing corral");
            }
            catch (Exception ex)
            {
                LogManager.Log(LogLevel.Fatal, ex);
                LogManager.Log(LogLevel.Info, args);
                throw;
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

        #region BCT-TranslationHelper.cs
        public static string CreateUniqueMethodName(IMethodReference method)
        {
            var containingTypeName = TypeHelper.GetTypeName(method.ContainingType, NameFormattingOptions.None);
            var s = MemberHelper.GetMethodSignature(method, NameFormattingOptions.DocumentationId);
            s = s.Substring(2);
            s = s.TrimEnd(')');
            s = TurnStringIntoValidIdentifier(s);
            return s;
        }

        public static string TurnStringIntoValidIdentifier(string s)
        {

            // Do this specially just to make the resulting string a little bit more readable.
            // REVIEW: Just let the main replacement take care of it?
            s = s.Replace("[0:,0:]", "2DArray"); // TODO: Do this programmatically to handle arbitrary arity
            s = s.Replace("[0:,0:,0:]", "3DArray");
            s = s.Replace("[0:,0:,0:,0:]", "4DArray");
            s = s.Replace("[0:,0:,0:,0:,0:]", "5DArray");
            s = s.Replace("[]", "array");

            // The definition of a Boogie identifier is from BoogiePL.atg.
            // Just negate that to get which characters should be replaced with a dollar sign.

            // letter = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz".
            // digit = "0123456789".
            // special = "'~#$^_.?`".
            // nondigit = letter + special.
            // ident =  [ '\\' ] nondigit {nondigit | digit}.

            s = Regex.Replace(s, "[^A-Za-z0-9'~#$^_.?`]", "$");

            s = GetRidOfSurrogateCharacters(s);
            return s;
        }

        /// <summary>
        /// Unicode surrogates cannot be handled by Boogie.
        /// http://msdn.microsoft.com/en-us/library/dd374069(v=VS.85).aspx
        /// </summary>
        private static string GetRidOfSurrogateCharacters(string s)
        {
            //  TODO this is not enough! Actually Boogie cannot support UTF8
            var cs = s.ToCharArray();
            var okayChars = new char[cs.Length];
            for (int i = 0, j = 0; i < cs.Length; i++)
            {
                if (Char.IsSurrogate(cs[i])) continue;
                okayChars[j++] = cs[i];
            }
            var raw = String.Concat(okayChars);
            return raw.Trim(new char[] { '\0' });
        }
        #endregion
    }
}