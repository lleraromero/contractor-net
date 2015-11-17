﻿using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading.Tasks;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class EpaGenerator
    {
        protected IAnalyzer analyzer;

        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

        public EpaGenerator(IAnalyzer analyzer)
        {
            Contract.Requires(analyzer != null);
            this.analyzer = analyzer;
        }

        public Task<TypeAnalysisResult> GenerateEpa(TypeDefinition typeToAnalyze)
        {
            Contract.Requires(typeToAnalyze != null);

            var constructors = typeToAnalyze.Constructors();
            var actions = typeToAnalyze.Actions();
            var epaBuilder = new ObservableEpaBuilder(new EpaBuilder(typeToAnalyze));
            epaBuilder.StateAdded += StateAdded;
            epaBuilder.TransitionAdded += TransitionAdded;

            return Task.Run(() => GenerateEpa(constructors, actions, epaBuilder));
        }

        public Task<TypeAnalysisResult> GenerateEpa(TypeDefinition typeToAnalyze, IEnumerable<string> selectedMethods)
        {
            Contract.Requires(typeToAnalyze != null);
            Contract.Requires(selectedMethods != null);

            var constructors = new HashSet<Action>(typeToAnalyze.Constructors().Where(a => selectedMethods.Contains(a.ToString())));
            var actions = new HashSet<Action>(typeToAnalyze.Actions().Where(a => selectedMethods.Contains(a.ToString())));
            var epaBuilder = new ObservableEpaBuilder(new EpaBuilder(typeToAnalyze));
            epaBuilder.StateAdded += StateAdded;
            epaBuilder.TransitionAdded += TransitionAdded;

            return Task.Run(() => GenerateEpa(constructors, actions, epaBuilder));
        }

        /// <summary>
        /// Method to create an EPA of a particular type considering only the subset 'methods'
        /// </summary>
        /// <see cref="http://publicaciones.dc.uba.ar/Publications/2011/DBGU11/paper-icse-2011.pdf">Algorithm 1</see>
        private TypeAnalysisResult GenerateEpa(ISet<Action> constructors, ISet<Action> actions, IEpaBuilder epaBuilder)
        {
            Contract.Requires(constructors != null);
            Contract.Requires(actions != null);
            Contract.Requires(epaBuilder != null);

            var analysisTimer = Stopwatch.StartNew();

            var initialState = new State(constructors, new HashSet<Action>());
            epaBuilder.SetStateAsInitial(initialState);

            var statesToVisit = new Queue<State>();
            statesToVisit.Enqueue(initialState);

            //try
            //{
                while (statesToVisit.Count > 0)
                {
                    var source = statesToVisit.Dequeue();
                    foreach (var action in source.EnabledActions)
                    {
                        // Which actions are enabled or disabled if 'action' is called from 'source'?
                        var actionsResult = analyzer.AnalyzeActions(source, action, actions);
                        Contract.Assert(!actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).Any(), "Results are consistent");

                        var possibleTargets = GeneratePossibleStates(actions, actionsResult, epaBuilder.States);
                        Contract.Assert(possibleTargets.Any(), "There is always at least one target to reach");

                        // Which states are reachable from the current state (aka source) using 'action'?
                        var transitionsResults = analyzer.AnalyzeTransitions(source, action, possibleTargets);
                        Contract.Assert(transitionsResults.Transitions.Count > 0, "There is always at least one transition to traverse");

                        foreach (var transition in transitionsResults.Transitions)
                        {
                            var target = transition.TargetState;
                            // Do I have to add a new state to the EPA?
                            if (!epaBuilder.States.Contains(target))
                            {
                                statesToVisit.Enqueue(target);
                                epaBuilder.Add(target);
                            }
                            epaBuilder.Add(transition);
                        }
                    }
                }
            //}
            //catch (OperationCanceledException)
            //{
            //    // The user aborted the generation process
            //}

            analysisTimer.Stop();

            var statistics = new Dictionary<string, object>();
            statistics["TotalAnalyzerDuration"] = analyzer.TotalAnalysisDuration;
            statistics["ExecutionsCount"] = analyzer.ExecutionsCount;
            statistics["TotalGeneratedQueriesCount"] = analyzer.TotalGeneratedQueriesCount;
            statistics["UnprovenQueriesCount"] = analyzer.UnprovenQueriesCount;

            var analysisResult = new TypeAnalysisResult(epaBuilder.Build(), analysisTimer.Elapsed, statistics);

            return analysisResult;
        }

        private IEnumerable<State> GeneratePossibleStates(ISet<Action> actions, ActionAnalysisResults actionsResult, IReadOnlyCollection<State> knownStates)
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