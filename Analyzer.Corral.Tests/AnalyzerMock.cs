using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text.RegularExpressions;
using Contractor.Core;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Analyzer.Corral.Tests
{
    public class AnalyzerMock : IAnalyzer
    {
        protected const string notPrefix = "_Not_";
        protected const string methodNameDelimiter = "~";

        public TimeSpan TotalAnalysisDuration
        {
            get { return new TimeSpan(); }
        }

        public int ExecutionsCount
        {
            get { return 0; }
        }

        public int TotalGeneratedQueriesCount
        {
            get { return 0; }
        }

        public int UnprovenQueriesCount
        {
            get { return 0; }
        }

        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            var queries = GenerateQueries(source, action, actions);
            var result = Analyze(queries);
            var analysisResult = EvaluateQueries(actions, result);

            return analysisResult;
        }

        public TransitionAnalysisResult AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            var queries = GenerateQueries(source, action, targets);
            var result = Analyze(queries);
            var resultAnalysis = EvaluateQueries(source, action, targets, result);

            return resultAnalysis;
        }

        private List<string> GenerateQueries(State source, Action action, IEnumerable<Action> actions)
        {
            var queries = new List<string>();
            foreach (var a in actions)
            {
                var prefix = notPrefix;
                var actionName = action.Name;
                var stateName = source.Name;
                var targetName = a.Name;
                var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);

                queries.Add(methodName);
            }

            foreach (var a in actions)
            {
                var prefix = string.Empty;
                var actionName = action.Name;
                var stateName = source.Name;
                var targetName = a.Name;
                var methodName = string.Format("{1}{0}{2}{0}{3}{4}", methodNameDelimiter, stateName, actionName, prefix, targetName);

                queries.Add(methodName);
            }

            return queries;
        }

        private Dictionary<string, Query> Analyze(List<string> queries)
        {
            var results = new Dictionary<string, Query>();
            foreach (var query in queries)
            {
                var queryName = CreateUniqueMethodName(query);

                var corralRunner = new CorralMock();
                var corralArgs = string.Format("{0} /main:{1} {2}", "Z:\\DummyPath\\Query.bpl", queryName, Configuration.CorralArguments);
                corralRunner.Run(corralArgs);

                results[query] = corralRunner.Result;
            }

            return results;
        }

        private ActionAnalysisResults EvaluateQueries(IEnumerable<Action> actions, Dictionary<string, Query> result)
        {
            var enabledActions = new HashSet<Action>(actions);
            var disabledActions = new HashSet<Action>(actions);

            foreach (var entry in result)
            {
                if (entry.Value.GetType() == typeof (ReachableQuery) || entry.Value.GetType() == typeof (MayBeReachableQuery))
                {
                    var actionName = entry.Key;
                    var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);
                    var isNegative = actionName.StartsWith(notPrefix);

                    if (isNegative)
                    {
                        actionName = actionName.Remove(0, notPrefix.Length);
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

                    //if (entry.Value == ResultKind.RecursionBoundReached)
                    //    base.UnprovenQueriesCount++;
                }
            }

            Contract.Assert(enabledActions.Count + disabledActions.Count <= actions.Count());
            Contract.Assert(!enabledActions.Intersect(disabledActions).Any());

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        private List<string> GenerateQueries(State source, Action action, IEnumerable<State> targets)
        {
            var queries = new List<string>();
            foreach (var s in targets)
            {
                var actionName = action.Name;
                var stateName = source.Name;
                var targetName = s.Name;
                var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);

                queries.Add(methodName);
            }

            return queries;
        }

        private TransitionAnalysisResult EvaluateQueries(State source, Action action, IEnumerable<State> targets, Dictionary<string, Query> result)
        {
            var transitions = new HashSet<Transition>();

            foreach (var entry in result)
            {
                if (entry.Value.GetType() == typeof (ReachableQuery) || entry.Value.GetType() == typeof (MayBeReachableQuery))
                {
                    var actionName = entry.Key;
                    var actionNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    actionName = actionName.Substring(actionNameStart);

                    var targetNameStart = actionName.LastIndexOf(methodNameDelimiter) + 1;
                    var targetName = actionName.Substring(targetNameStart);
                    var target = targets.First(s => s.Name == targetName);
                    var isUnproven = entry.Value.GetType() == typeof (MayBeReachableQuery);

                    if (target != null)
                    {
                        var transition = new Transition(action, source, target, isUnproven);
                        transitions.Add(transition);
                    }

                    //if (isUnproven)
                    //    base.UnprovenQueriesCount++;
                }
            }

            return new TransitionAnalysisResult(transitions);
        }

        #region BCT-TranslationHelper.cs

        public static string CreateUniqueMethodName(string method)
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
        ///     Unicode surrogates cannot be handled by Boogie.
        ///     http://msdn.microsoft.com/en-us/library/dd374069(v=VS.85).aspx
        /// </summary>
        private static string GetRidOfSurrogateCharacters(string s)
        {
            //  TODO this is not enough! Actually Boogie cannot support UTF8
            var cs = s.ToCharArray();
            var okayChars = new char[cs.Length];
            for (int i = 0, j = 0; i < cs.Length; i++)
            {
                if (char.IsSurrogate(cs[i])) continue;
                okayChars[j++] = cs[i];
            }
            var raw = string.Concat(okayChars);
            return raw.Trim('\0');
        }

        #endregion
    }
}