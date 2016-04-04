using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using Analyzer.Corral;
using Action = Contractor.Core.Model.Action;

namespace Analyzer.CSGenerator
{
    class AnalyzerWithConditions : IAnalyzer
    {
        protected string defaultArgs;
        protected CciQueryGenerator queryGenerator;
        protected CciAssembly inputAssembly;
        protected string inputFileName;
        protected ITypeDefinition typeToAnalyze;
        protected CancellationToken token;
        protected DirectoryInfo workingDir;

        protected int generatedQueriesCount;
        protected int unprovenQueriesCount;

        public AnalyzerWithConditions(string defaultArgs, DirectoryInfo workingDir, CciQueryGenerator queryGenerator, CciAssembly inputAssembly,
            string inputFileName, ITypeDefinition typeToAnalyze,
            CancellationToken token)
        {
            this.defaultArgs = defaultArgs;
            this.workingDir = workingDir;
            this.queryGenerator = queryGenerator;
            this.inputAssembly = inputAssembly;
            this.typeToAnalyze = typeToAnalyze;
            this.inputFileName = inputFileName;
            this.token = token;

            generatedQueriesCount = 0;
            unprovenQueriesCount = 0;
        }
       
        public ActionAnalysisResults AnalyzeActions(State source, Action action, IEnumerable<Action> actions)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);

            if (action.IsPure)
            {
                return new ActionAnalysisResults(new HashSet<Action>(source.EnabledActions), new HashSet<Action>(source.DisabledActions));
            }

            var enabledActions = GetMustBeEnabledActions(source, action, actions, corralRunner);
            var disabledActions = GetMustBeDisabledActions(source, action, actions, corralRunner);

            Contract.Assert(!enabledActions.Intersect(disabledActions).Any());

            return new ActionAnalysisResults(enabledActions, disabledActions);
        }

        public IReadOnlyCollection<Transition> AnalyzeTransitions(State source, Action action, IEnumerable<State> targets)
        {
            ISolver corralRunner = new CorralRunner(defaultArgs, workingDir);
            var transitionQueries = queryGenerator.CreateTransitionQueries(source, action, targets);
            var queryAssembly = CreateBoogieQueryAssembly(transitionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var feasibleTransitions = evaluator.GetFeasibleTransitions(transitionQueries);
            unprovenQueriesCount += evaluator.UnprovenQueries;

            return feasibleTransitions;
        }

        public string GetUsageStatistics()
        {
            var statisticsBuilder = new StringBuilder();

            statisticsBuilder.AppendFormat(@"Generated queries: {0} ({1} unproven)", generatedQueriesCount, unprovenQueriesCount).AppendLine();

            var precision = 100 - Math.Ceiling((double) unprovenQueriesCount*100/generatedQueriesCount);
            statisticsBuilder.AppendFormat(@"Analysis precision: {0}%", precision).AppendLine();

            return statisticsBuilder.ToString();
        }

        protected ISet<Action> GetMustBeDisabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);

            var targetNegatedPreconditionQueries = queryGenerator.CreateNegativeQueries(source, action, actions);
            generatedQueriesCount += targetNegatedPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetNegatedPreconditionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var disabledActions = new HashSet<Action>(evaluator.GetDisabledActions(targetNegatedPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return disabledActions;
        }

        protected ISet<Action> GetMustBeEnabledActions(State source, Action action, IEnumerable<Action> actions, ISolver corralRunner)
        {
            Contract.Requires(source != null);
            Contract.Requires(action != null);
            Contract.Requires(actions.Any());
            Contract.Requires(corralRunner != null);

            var targetPreconditionQueries = queryGenerator.CreatePositiveQueries(source, action, actions);
            generatedQueriesCount += targetPreconditionQueries.Count;
            var queryAssembly = CreateBoogieQueryAssembly(targetPreconditionQueries);
            var evaluator = new QueryEvaluator(corralRunner, queryAssembly);
            var enabledActions = new HashSet<Action>(evaluator.GetEnabledActions(targetPreconditionQueries));
            unprovenQueriesCount += evaluator.UnprovenQueries;
            return enabledActions;
        }

        protected FileInfo CreateBoogieQueryAssembly(IReadOnlyCollection<Query> queries)
        {
            Contract.Requires(queries.Any());

            var queryAssembly = new CciQueryAssembly(inputAssembly, typeToAnalyze, queries);

            var queryFilePath = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString(), Path.GetFileName(inputFileName));
            Contract.Assert(!Directory.Exists(Path.GetDirectoryName(queryFilePath)));
            Directory.CreateDirectory(Path.GetDirectoryName(queryFilePath));

            // Mutex to avoid race-conditions in CCI static classes
            lock (CciAssemblyPersister.turnstile)
            {
                new CciContractRewriter().Rewrite(queryAssembly);
            }
            new CciAssemblyPersister().Save(queryAssembly, queryFilePath);

            return TranslateCSharpToBoogie(queryFilePath);
        }

        protected FileInfo TranslateCSharpToBoogie(string queryAssemblyPath)
        {
            Contract.Requires(!string.IsNullOrEmpty(queryAssemblyPath));

            var bctRunner = new BctRunner();
            var args = new[] { queryAssemblyPath, "/lib:" + Path.GetDirectoryName(inputFileName) };

            token.ThrowIfCancellationRequested();
            bctRunner.Run(args);

            return new FileInfo(queryAssemblyPath.Replace("dll", "bpl"));
        }
    }
                
        /*
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
            foreach (T targetState in target)
            {
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

                            while (name.Contains(' '))
                            {
                                //delete parameter name and withespace
                                var nextParamNameStart = name.IndexOf(' ');
                                var nextParamNameFinish = name.IndexOf(',', nextParamNameStart) - 1;
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
         */
}
