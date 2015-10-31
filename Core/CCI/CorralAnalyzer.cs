using Contractor.Core.Model;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class CorralAnalyzer : IAnalyzer
    {
        protected Analyzer analyzer;

        public CorralAnalyzer(Analyzer analyzer, NamespaceTypeDefinition type)
        {
            this.analyzer = analyzer;

            ITypeContract typeContract = this.analyzer.inputContractProvider.GetTypeContractFor(type);
            this.analyzer.queryContractProvider.AssociateTypeWithContract(this.analyzer.typeToAnalyze, typeContract);
        }

        public TimeSpan TotalAnalysisDuration { get { return this.analyzer.TotalAnalysisDuration; } }
        public int ExecutionsCount { get { return this.analyzer.ExecutionsCount; } }
        public int TotalGeneratedQueriesCount { get { return this.analyzer.TotalGeneratedQueriesCount; } }
        public int UnprovenQueriesCount { get { return this.analyzer.UnprovenQueriesCount; } }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, List<Action> actions)
        {
            var queries = this.analyzer.GenerateQueries<Action>(source, action, actions);
            var result = Analyze(queries);
            var analysisResult = EvaluateQueries(actions, result);

            return analysisResult;
        }

        public TransitionAnalysisResult AnalyzeTransitions(State source, Action action, List<State> targets)
        {
            var queries = this.analyzer.GenerateQueries<State>(source, action, targets);
            var result = Analyze(queries);
            var resultAnalysis = EvaluateQueries(source, action, targets, result);

            return resultAnalysis;
        }

        private Dictionary<MethodDefinition, ResultKind> Analyze(List<MethodDefinition> queries)
        {
            // Add queries to the working assembly
            this.analyzer.typeToAnalyze.Methods.AddRange(queries);

            // I need to replace Pre/Post with Assume/Assert
            ILocalScopeProvider localScopeProvider = new Microsoft.Cci.ILToCodeModel.Decompiler.LocalScopeProvider(this.analyzer.GetPDBReader(this.analyzer.queryAssembly.Module, this.analyzer.host));
            ISourceLocationProvider sourceLocationProvider = this.analyzer.GetPDBReader(this.analyzer.queryAssembly.Module, this.analyzer.host);
            var trans = new ContractRewriter(this.analyzer.host, this.analyzer.queryContractProvider, sourceLocationProvider);
            trans.Rewrite(this.analyzer.queryAssembly.DecompiledModule);

            // Save the query assembly to run Corral
            this.analyzer.queryAssembly.Save(this.analyzer.GetQueryAssemblyPath());

            var result = ExecuteChecker(queries);

            // I don't need the queries anymore
            this.analyzer.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            return result;
        }

        private Dictionary<MethodDefinition, ResultKind> ExecuteChecker(List<MethodDefinition> queries)
        {
            var result = new Dictionary<MethodDefinition, ResultKind>();

            var bctRunner = new BctRunner(this.analyzer.token);
            var args = new string[] { this.analyzer.GetQueryAssemblyPath(), "/lib:" + Path.GetDirectoryName(this.analyzer.inputAssembly.Module.ContainingAssembly.Location) };

            this.analyzer.TotalAnalysisDuration += bctRunner.Run(args);

            foreach (var query in queries)
            {
                // Check if the user stopped the analysis
                if (this.analyzer.token.IsCancellationRequested)
                {
                    break;
                }

                var queryName = CreateUniqueMethodName(query);

                var corralRunner = new CorralRunner(this.analyzer.token);
                var corralArgs = string.Format("{0} /main:{1} {2}", this.analyzer.GetQueryAssemblyPath().Replace("dll", "bpl"), queryName, Configuration.CorralArguments);
                corralRunner.Run(corralArgs);

                //using (var writer = new System.IO.StreamWriter(System.IO.Path.Combine(Configuration.TempPath, "log-vending.txt"), true))
                //{
                //    writer.WriteLine(string.Format("{0} {1}", CreateUniqueMethodName(query.Name.Value), corralRunner.Result));
                //}

                result[query] = corralRunner.Result;
            }

            return result;
        }

        private ActionAnalysisResults EvaluateQueries(List<Action> actions, Dictionary<MethodDefinition, ResultKind> result)
        {
            var enabledActions = new HashSet<Action>(actions);
            var disabledActions = new HashSet<Action>(actions);

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
                        var actionNameStart = actionName.LastIndexOf(this.analyzer.methodNameDelimiter) + 1;
                        actionName = actionName.Substring(actionNameStart);
                        var isNegative = actionName.StartsWith(this.analyzer.notPrefix);

                        if (isNegative)
                        {
                            actionName = actionName.Remove(0, this.analyzer.notPrefix.Length);
                            if (disabledActions.Any(a => a.Name.Equals(actionName)))
                            {
                                disabledActions.Remove(disabledActions.First(a => a.Name.Equals(actionName)));
                            }
                        }
                        else
                        {
                            if (enabledActions.Any(a => a.Name.Equals(actionName)))
                            {
                                enabledActions.Remove(enabledActions.First(a => a.Name.Equals(actionName)));
                            }
                        }

                        if (entry.Value == ResultKind.RecursionBoundReached)
                            this.analyzer.UnprovenQueriesCount++;

                        break;
                    default:
                        throw new NotImplementedException("Unknown result");
                }
            }

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        private TransitionAnalysisResult EvaluateQueries(State source, Action action, List<State> targets, Dictionary<MethodDefinition, ResultKind> result)
        {
            var transitions = new HashSet<Transition>();

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
                        var actionNameStart = actionName.LastIndexOf(this.analyzer.methodNameDelimiter) + 1;
                        actionName = actionName.Substring(actionNameStart);

                        var targetNameStart = actionName.LastIndexOf(this.analyzer.methodNameDelimiter) + 1;
                        var targetName = actionName.Substring(targetNameStart);
                        var target = targets.Find(s => s.Name == targetName);
                        var isUnproven = entry.Value == ResultKind.RecursionBoundReached;

                        if (target != null)
                        {
                            var transition = new Transition(action, source, target, isUnproven);
                            transitions.Add(transition);
                        }

                        if (isUnproven)
                            this.analyzer.UnprovenQueriesCount++;
                        break;
                    default:
                        throw new NotImplementedException("Unknown result");
                }
            }

            return new TransitionAnalysisResult(transitions);
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

        private static string CreateUniqueMethodName(string method)
        {
            var s = method;
            //s = s.Substring(2);
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