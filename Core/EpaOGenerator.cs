using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Contractor.Core.Model;
using Log;
using Action = Contractor.Core.Model.Action;
using ErrorInstrumentator;
namespace Contractor.Core
{
    public class EpaOGenerator
    {
        protected IAnalyzerFactory analyzerFactory;
        protected int cutter;
        protected List<string> errorList;
        private bool computeDependencies;
        private int maxDegreeOfParallelism;

        public EpaOGenerator(IAnalyzerFactory analyzerFactory, int cutter, List<string> errorList, bool computeDependencies,int maxDegreeOfParallelism)
        {
            Contract.Requires(analyzerFactory != null);
            this.analyzerFactory = analyzerFactory;
            this.cutter = cutter;
            this.errorList = errorList;
            this.computeDependencies = computeDependencies;
            this.maxDegreeOfParallelism = maxDegreeOfParallelism;
        }

        public Task<TypeAnalysisResult> GenerateEpa(ITypeDefinition typeToAnalyze, IEpaBuilder epaBuilder)
        {
            Contract.Requires(typeToAnalyze != null);

            var constructors = typeToAnalyze.Constructors();
            var actions = typeToAnalyze.Actions();

            return GenerateEpaAndStatistics(constructors, actions, epaBuilder);
        }

        public Task<TypeAnalysisResult> GenerateEpa(ITypeDefinition typeToAnalyze, IEnumerable<string> selectedMethods, IEpaBuilder epaBuilder)
        {
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(selectedMethods != null);
            selectedMethods = selectedMethods.Select(a => a.Replace(" ", ""));
            var constructors = new HashSet<Action>(typeToAnalyze.Constructors().Where(a => selectedMethods.Contains(a.ToString().Replace(" ", ""))));
            var actions = new HashSet<Action>(typeToAnalyze.Actions().Where(a => selectedMethods.Contains(a.ToString().Replace(" ",""))));

            return GenerateEpaAndStatistics(constructors, actions, epaBuilder);
        }

        protected async Task<TypeAnalysisResult> GenerateEpaAndStatistics(ISet<Action> constructors, ISet<Action> actions, IEpaBuilder epaBuilder)
        {
            var analysisTimer = Stopwatch.StartNew();

            await Task.Run(() => GenerateEpa(constructors, actions, epaBuilder));

            analysisTimer.Stop();

            var analysisResult = new TypeAnalysisResult(epaBuilder.Build(), analysisTimer.Elapsed, analyzerFactory.GeneratedQueriesCount, analyzerFactory.UnprovenQueriesCount, analyzerFactory.DependencyQueriesCount);

            return analysisResult;
        }

        /// <summary>
        ///     Method to create an EPA of a particular type considering only the subset 'methods'
        /// </summary>
        /// <see cref="http://publicaciones.dc.uba.ar/Publications/2011/DBGU11/paper-icse-2011.pdf">Algorithm 1</see>
        protected Epa GenerateEpa(ISet<Action> constructors, ISet<Action> actions, IEpaBuilder epaBuilder)
        {
            Contract.Requires(constructors != null);
            Contract.Requires(actions != null);
            Contract.Requires(epaBuilder != null);

            if (this.computeDependencies)
            {
                var analyzer = analyzerFactory.CreateAnalyzer();
                analyzer.ComputeDependencies(actions);
                analyzerFactory.DependencyQueriesCount += analyzer.DependencyQueriesCount();
            }

            var initialState = new State(constructors, new HashSet<Action>());
            var statesToVisit = new Queue<State>();
            var visitedStates = new HashSet<State>();
            statesToVisit.Enqueue(initialState);

            while (statesToVisit.Count > 0)
            {
                var source = statesToVisit.Dequeue();
                visitedStates.Add(source);

                try
                {

                    // Change ParallelOptions.MaxDegreeOfParallelism to 1 to make the loop sequential.
                    var opt = new ParallelOptions();
                    opt.MaxDegreeOfParallelism = maxDegreeOfParallelism;
                    Parallel.ForEach(source.EnabledActions, opt, action =>
                    {

                        ActionAnalysisResults actionsResult;
                        var analyzer = analyzerFactory.CreateAnalyzer();

                        // PARA CADA Em:
                        foreach (string exitCode in errorList)
                        {
                            //MyLogger.LogAction(action.Name, exitCode, source.Name);                    

                            actionsResult = analyzer.AnalyzeActions(source, action, actions,exitCode);
                            if (actionsResult.EnabledActions.Count.Equals(actions.Count) && actionsResult.DisabledActions.Count.Equals(actions.Count()))
                            {
                                Logger.Log(LogLevel.Warn,
                                    "Suspicious state! Only a state with a unsatisfiable invariant can lead to every action being enabled and disabled at the same time. It can also mean a bug in our code.");
                                //continue;
                            }
                            /*
                            Contract.Assert(!actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).Any(), "Results should be consistent");
                            Contract.Assert(actionsResult.EnabledActions.Any() || actionsResult.DisabledActions.Any(),
                                "State explosion leads to a useless EPA");
                            */
                            var possibleTargets = GeneratePossibleStates(actions, actionsResult);
                            if (action.Method.IsConstructor && !exitCode.Equals("Ok"))
                            {
                                var states=new List<State>();
                                states.Add(initialState);
                                possibleTargets = states;
                            }

                            if (cutter > 0 && possibleTargets.Count > cutter)
                            {
                                throw new Exception("Number of states too big.");
                            }

                            Contract.Assert(possibleTargets.Any(), "There is always at least one target to reach");

                            var transitionsResults = analyzer.AnalyzeTransitions(source, action, possibleTargets,exitCode);

                            //Contract.Assert(transitionsResults.Count > 0, "There is always at least one transition to traverse");
                            //if (!(transitionsResults.Count > 0))
                            //    MyLogger.LogMsg("transitionsResults.Count == 0. ASSUMPTION FAILURE: There is always at least one transition to traverse. ASSUME: transitionsResults.Count > 0");

                            foreach (var transition in transitionsResults)
                            {
                                var target = transition.TargetState;
                                // Do I have to add a new state to the EPA?
                                if (!visitedStates.Contains(target) && !statesToVisit.Contains(target))
                                {
                                    statesToVisit.Enqueue(target);
                                }
                                epaBuilder.Add(transition);
                            }

                            analyzerFactory.GeneratedQueriesCount += analyzer.GeneratedQueriesCount();
                            analyzerFactory.UnprovenQueriesCount += analyzer.UnprovenQueriesCount();
                            //END PARA CADA Em
                        }

                    });
                }
                catch (AggregateException e)
                {
                    foreach (var ex in e.InnerExceptions)
                    {
                    //    if (ex.GetType() == typeof(OperationCanceledException))
                    //    {
                            throw ex;
                    //    }
                    }
                }
            }
            return epaBuilder.Build();
        }

        protected IReadOnlyCollection<State> GeneratePossibleStates(ISet<Action> actions, ActionAnalysisResults actionsResult)
        {
            Contract.Requires(actions != null);
            Contract.Requires(actionsResult != null);

            var unknownActions = new HashSet<Action>(actions);
            unknownActions.ExceptWith(actionsResult.EnabledActions);
            unknownActions.ExceptWith(actionsResult.DisabledActions);

            var states = new List<State>();

            var enabledActions = new HashSet<Action>(actionsResult.EnabledActions);
            var disabledActions = new HashSet<Action>(actionsResult.DisabledActions);
            disabledActions.UnionWith(unknownActions);
            var v = new State(enabledActions, disabledActions);
            states.Add(v);

            while (unknownActions.Count > 0)
            {
                var action = unknownActions.First();
                unknownActions.Remove(action);

                var count = states.Count;

                for (var i = 0; i < count; ++i)
                {
                    enabledActions = new HashSet<Action>();
                    enabledActions.Add(action);
                    enabledActions.UnionWith(states[i].EnabledActions);
                    disabledActions = new HashSet<Action>();
                    disabledActions.UnionWith(states[i].DisabledActions);
                    disabledActions.Remove(action);

                    var w = new State(enabledActions, disabledActions);
                    states.Add(w);
                }
            }

            return states;
        }
    }
}