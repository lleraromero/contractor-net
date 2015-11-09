using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class EpaGenerator
    {
        protected IAssemblyXXX assembly;
        protected IAnalyzer analyzer;

        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

        public EpaGenerator(IAssemblyXXX inputAssembly, IAnalyzer analyzer)
        {
            Contract.Requires(inputAssembly != null);
            Contract.Requires(analyzer != null);

            this.assembly = inputAssembly;
            this.analyzer = analyzer;
        }

        public TypeAnalysisResult GenerateEpa(TypeDefinition typeToAnalyze)
        {
            Contract.Requires(typeToAnalyze != null);

            var constructors = this.assembly.Constructors(typeToAnalyze);
            var actions = this.assembly.Actions(typeToAnalyze);
            return GenerateEpa(typeToAnalyze, constructors, actions);
        }

        public TypeAnalysisResult GenerateEpa(TypeDefinition typeToAnalyze, IEnumerable<string> selectedMethods)
        {
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(selectedMethods != null && selectedMethods.Any());

            var constructors = new HashSet<Action>(this.assembly.Constructors(typeToAnalyze).Where(a => selectedMethods.Contains(a.ToString())));
            var actions = new HashSet<Action>(this.assembly.Actions(typeToAnalyze).Where(a => selectedMethods.Contains(a.ToString())));
            return GenerateEpa(typeToAnalyze, constructors, actions);
        }

        /// <summary>
        /// Method to create an EPA of a particular type considering only the subset 'methods'
        /// </summary>
        /// <see cref="http://publicaciones.dc.uba.ar/Publications/2011/DBGU11/paper-icse-2011.pdf">Algorithm 1</see>
        private TypeAnalysisResult GenerateEpa(TypeDefinition typeToAnalyze, ISet<Action> constructors, ISet<Action> actions)
        {
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(analyzer != null);
            Contract.Requires(constructors != null);
            Contract.Requires(actions != null);

            var analysisTimer = Stopwatch.StartNew();

            var dummy = new State(constructors, new HashSet<Action>());

            var epaBuilder = new EpaBuilder(typeToAnalyze, dummy);

            var discoveredStates = new HashSet<State>();
            discoveredStates.Add(dummy);

            if (this.StateAdded != null)
            {
                this.StateAdded(this, new StateAddedEventArgs(typeToAnalyze, new Tuple<EpaBuilder, State>(epaBuilder, dummy)));
            }

            var statesToVisit = new Queue<State>();
            statesToVisit.Enqueue(dummy);

            try
            {
                while (statesToVisit.Count > 0)
                {
                    var source = statesToVisit.Dequeue();
                    foreach (var action in source.EnabledActions)
                    {
                        // Which actions are enabled or disabled if 'action' is called from 'source'?
                        var actionsResult = analyzer.AnalyzeActions(source, action, actions.ToList<Action>());

                        // Remove any inconsistency
                        var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();
                        foreach (var act in inconsistentActions)
                        {
                            actionsResult.EnabledActions.Remove(act);
                            actionsResult.DisabledActions.Remove(act);
                        }

                        var possibleTargets = GeneratePossibleStates(actions, actionsResult, epaBuilder.States);
                        // Which states are reachable from the current state (aka source) using 'action'?
                        var transitionsResults = analyzer.AnalyzeTransitions(source, action, new List<State>(possibleTargets));

                        foreach (var transition in transitionsResults.Transitions)
                        {
                            var target = transition.TargetState;
                            // Do I have to add a new state to the EPA?
                            if (!discoveredStates.Contains(target))
                            {
                                statesToVisit.Enqueue(target);

                                discoveredStates.Add(target);
                                epaBuilder.Add(target);

                                if (this.StateAdded != null)
                                {
                                    var eventArgs = new StateAddedEventArgs(typeToAnalyze, new Tuple<EpaBuilder, State>(epaBuilder, target as State));
                                    this.StateAdded(this, eventArgs);
                                }
                            }

                            epaBuilder.Add(transition);

                            if (this.TransitionAdded != null)
                            {
                                var eventArgs = new TransitionAddedEventArgs(typeToAnalyze, transition as Transition, source as State, epaBuilder);
                                this.TransitionAdded(this, eventArgs);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
                // The user aborted the generation process
            }

            analysisTimer.Stop();

            var statistics = new Dictionary<string, object>();
            statistics["TotalAnalyzerDuration"] = analyzer.TotalAnalysisDuration;
            statistics["ExecutionsCount"] = analyzer.ExecutionsCount;
            statistics["TotalGeneratedQueriesCount"] = analyzer.TotalGeneratedQueriesCount;
            statistics["UnprovenQueriesCount"] = analyzer.UnprovenQueriesCount;

            var analysisResult = new TypeAnalysisResult(epaBuilder.Build(), analysisTimer.Elapsed, statistics);

            return analysisResult;
        }

        private IEnumerable<State> GeneratePossibleStates(ISet<Action> actions, ActionAnalysisResults actionsResult, ISet<State> knownStates)
        {
            Contract.Requires(actions != null);
            Contract.Requires(actionsResult != null);
            Contract.Requires(knownStates != null);

            var unknownActions = new HashSet<Action>(actions);
            unknownActions.ExceptWith(actionsResult.EnabledActions);
            unknownActions.ExceptWith(actionsResult.DisabledActions);

            var states = new List<State>();

            var enabledActions = new HashSet<Action>(actionsResult.EnabledActions);
            var disabledActions = new HashSet<Action>(actionsResult.DisabledActions);
            disabledActions.UnionWith(unknownActions);
            var v = new State(enabledActions, disabledActions);

            if (knownStates.Contains(v))
            {
                v = knownStates.Single(s => s.Equals(v));
            }
            states.Add(v);

            while (unknownActions.Count > 0)
            {
                var action = unknownActions.First();
                unknownActions.Remove(action);

                var count = states.Count;

                for (int i = 0; i < count; ++i)
                {
                    enabledActions = new HashSet<Action>();
                    enabledActions.Add(action);
                    enabledActions.UnionWith(states[i].EnabledActions);
                    disabledActions = new HashSet<Action>();
                    disabledActions.UnionWith(states[i].DisabledActions);
                    disabledActions.Remove(action);

                    var w = new State(enabledActions, disabledActions);

                    if (knownStates.Contains(w))
                    {
                        w = knownStates.Single(s => s.Equals(w));
                    }

                    states.Add(w);
                }
            }

            return states;
        }
    }
}