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
using System.Text.RegularExpressions;
using System.Threading;

namespace Contractor.Core
{
    class CorralAnalyzer : Analyzer
    {
        private enum ResultKind { TrueBug, NoBugs, RecursionBoundReached }

        public CorralAnalyzer(IContractAwareHost host, IModule module, NamespaceTypeDefinition type, CancellationToken token)
            : base(host, module, type, token)
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
            TransitionAnalysisResult resultAnalysis;
            if (Configuration.TransitionsWithConditions)
            {
                // TODO: WITH CONDITIONS
                var result = AnalyzeWithConditions<State>(source, action, targets);
                resultAnalysis = EvaluateQueriesWithConditions(source, action, targets, result.Item1,result.Item2);
            }
            else
            {
                var result = Analyze<State>(source, action, targets);
                resultAnalysis = EvaluateQueries(source, action, targets, result);
            }
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

        private Tuple<Dictionary<MethodDefinition, ResultKind>, Dictionary<string, List<string>>> AnalyzeWithConditions<T>(State source, IMethodDefinition action, List<T> target)
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

            queryAssembly.Save(GetQueryAssemblyPath());

            //generate new queries from result
            queries = new List<MethodDefinition>();
            foreach(T targetState in target){
                queries.Add(GenerateQueryCS((State)source, action, (State)(object)targetState));
            }
            //queries.Add(GenerateQueryCS(state, action, (State)(object)target));

            this.typeToAnalyze.Methods.AddRange(queries);

            ContractHelper.InjectContractCalls(host, queryAssembly.DecompiledModule, queryContractProvider, sourceLocationProvider);

            queryAssembly.Save(GetQueryAssemblyPath());

            //execute CCCheck to get CS
            var checkerForConditions = CSGenerator.Instance(null);
            var resultConditions = checkerForConditions.executeCheckerConditions(GetQueryAssemblyPath());

            // I don't need the queries anymore
            this.typeToAnalyze.Methods.RemoveAll(m => queries.Contains(m));

            Tuple<Dictionary<MethodDefinition, ResultKind>, Dictionary<string, List<string>>> t = new Tuple<Dictionary<MethodDefinition, ResultKind>, Dictionary<string, List<string>>>(result, resultConditions);

            return t;
        }

        private MethodDefinition GenerateQueryCS(State source, IMethodDefinition action, State target)
        {
            var actionName = action.GetUniqueName();
            var stateName = source.UniqueName;
            var targetName = target.UniqueName;
            var methodName = string.Format("{1}{0}{2}{0}{3}", methodNameDelimiter, stateName, actionName, targetName);
            var method = CreateQueryMethod<State>(source, methodName, action, target);

            var queryContract = CreateQueryContractCS(source, target);
            queryContractProvider.AssociateMethodWithContract(method, queryContract);
            return method;
        }

        private MethodContract CreateQueryContractCS(State state, State target)
        {
            var contracts = new MethodContract();

            // Source state invariant as a precondition
            var stateInv = Helper.GenerateStateInvariant(host, inputContractProvider, typeToAnalyze, state);

            var preconditions = from condition in stateInv
                                select new Precondition()
                                {
                                    Condition = condition,
                                    OriginalSource = Helper.PrintExpression(condition),
                                    Description = new CompileTimeConstant() { Value = "Source state invariant", Type = this.host.PlatformType.SystemString }
                                };
            contracts.Preconditions.AddRange(preconditions);

            //  target state invariant as a postcondition
            var targetInv = Helper.GenerateStateInvariant(host, inputContractProvider, typeToAnalyze, target);

            IExpression joinedTargetInv = Helper.JoinWithLogicalAnd(host, targetInv, true);

            //IExpression joinedTargetInv = new LogicalNot()
            //{
            //    Type = host.PlatformType.SystemBoolean,
            //    Operand = Helper.JoinWithLogicalAnd(host, targetInv, true)
            //};

            var postcondition = new Postcondition()
            {
                Condition = joinedTargetInv,
                OriginalSource = Helper.PrintExpression(joinedTargetInv),
                Description = new CompileTimeConstant() { Value = "Target state invariant", Type = this.host.PlatformType.SystemString }
            };
            contracts.Postconditions.Add(postcondition);

            return contracts;
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
                // Check if the user stopped the analysis
                if (base.token.IsCancellationRequested)
                {
                    break;
                }

                var queryName = CreateUniqueMethodName(query);

                result[query] = RunCorral(queryName);
            }

            return result;
        }

        private void RunBCT()
        {
            // Check if the user stopped the analysis
            if (base.token.IsCancellationRequested)
            {
                return;
            }

            var timer = Stopwatch.StartNew();

            // I need to change the current directory so BCT can write the output in the correct folder
            var tmp = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Configuration.TempPath;
            var boogieTranslator = new BytecodeTranslator.BCT();
            var args = new string[] { GetQueryAssemblyPath(), "/lib:" + Path.GetDirectoryName(inputAssembly.Module.ContainingAssembly.Location) };
            if (boogieTranslator.Main(args) != 0)
            {
                LogManager.Log(LogLevel.Fatal, "Error translating the query assembly to boogie");
                LogManager.Log(LogLevel.Info, string.Format("args: {0}, {1}", args));
                throw new Exception("Error translating the query assembly to boogie");
            }
            Environment.CurrentDirectory = tmp;

            timer.Stop();
            base.TotalAnalysisDuration += new TimeSpan(timer.ElapsedTicks);            
        }

        private ResultKind RunCorral(string method)
        {
            Contract.Requires(!string.IsNullOrEmpty(method));

            var args = string.Format("{0} /main:{1} {2}", GetQueryAssemblyPath().Replace("dll", "bpl"), method, Configuration.CorralArguments);

            var timer = Stopwatch.StartNew();

            var corral = new cba.Driver();
            try
            {
                if (corral.run(args.Split(' ')) != 0)
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

            switch (corral.Result)
            {
                case cba.CorralResult.BugFound:
                    return ResultKind.TrueBug;

                case cba.CorralResult.NoBugs:
                    return ResultKind.NoBugs;

                case cba.CorralResult.RecursionBoundReached:
                    return ResultKind.RecursionBoundReached;

                default:
                    throw new NotImplementedException("The result was not understood");
            }
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
                            var transition = new Transition(action, source, target, isUnproven,"SIN CONDICIONES");
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

        private TransitionAnalysisResult EvaluateQueriesWithConditions(State source, IMethodDefinition action, List<State> targets, Dictionary<MethodDefinition, ResultKind> result, Dictionary<string, List<string>> conditions)
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
                            //TODO:
                            string name = query.ToString();
                            var nameStart = name.IndexOf(' ') + 1;
                            name = name.Substring(nameStart);
                            //int nextParamNameFinish=0;

                            while(name.Contains(' ')){
                                //delete parameter name and withespace
                                var nextParamNameStart = name.IndexOf(' ');
                                var nextParamNameFinish = name.IndexOf(',',nextParamNameStart)-1;
                                if (nextParamNameFinish != -2)
                                {
                                    name = name.Remove(nextParamNameStart, nextParamNameFinish - nextParamNameStart + 1);
                                    //delete withespace after ','
                                    nextParamNameStart = name.IndexOf(' ');
                                    name = name.Remove(nextParamNameStart, 1);
                                }
                                else
                                {
                                    nextParamNameFinish = name.IndexOf(')', nextParamNameStart) - 1;
                                    name = name.Remove(nextParamNameStart, nextParamNameFinish - nextParamNameStart + 1);
                                }
                                
                            }

                            var cs = CSGenerator.generateCS(conditions, name);
                            var transition = new Transition(action, source, target, isUnproven, cs);
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