using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class EpaBuilderNotifier
    {
    }

    public class EpaGenerator
    {
        protected AssemblyXXX assembly;
        public AssemblyXXX Assembly { get { return assembly; } }

        public EpaGenerator(AssemblyXXX assembly)
        {
            Contract.Requires(assembly != null);

            this.assembly = assembly;
        }

        public Epa GenerateEpa(string typeToAnalyze, IAnalyzer analyzer)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeToAnalyze));
            Contract.Requires(Assembly.Types().Contains(typeToAnalyze));
            Contract.Requires(analyzer != null);

            var dummy = new State(this.assembly.Constructors(typeToAnalyze), new HashSet<Action>());
            
            var epaBuilder = new EpaBuilder(typeToAnalyze, dummy);
            
            var discoveredStates = new HashSet<State>();
            discoveredStates.Add(dummy);

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
                        var actionsResult = analyzer.AnalyzeActions(source, action, this.assembly.Actions(typeToAnalyze).ToList<Action>());

                        // Remove any inconsistency
                        var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();
                        foreach (var act in inconsistentActions)
                        {
                            actionsResult.EnabledActions.Remove(act);
                            actionsResult.DisabledActions.Remove(act);
                        }

                        var possibleTargets = GeneratePossibleStates(this.assembly.Actions(typeToAnalyze), actionsResult, epaBuilder.States);
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
                            }

                            epaBuilder.Add(transition);
                        }
                    }
                }
            }
            catch (OperationCanceledException)
            {
            }

            return epaBuilder.Build();
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
