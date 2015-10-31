using Contractor.Core.Model;
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Threading;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    public class EpaGenerator : IDisposable
    {
        public enum Backend { CodeContracts, Corral };

        private AssemblyInfo inputAssembly;
        private HashSet<string> instrumentedEpas;
        private CodeContractAwareHostEnvironment host;
        private Backend backend;

        protected CciAssembly assembly;

        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

        public EpaGenerator(Backend backend, string inputFileName, string contractsFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputFileName));
            Contract.Ensures(host != null);
            Contract.Ensures(inputAssembly != null);

            host = new CodeContractAwareHostEnvironment(true);
            inputAssembly = new AssemblyInfo(host);
            inputAssembly.Load(inputFileName);

            if (!string.IsNullOrEmpty(contractsFileName))
            {
                // Loads the contract reference assembly in the host.
                var contractsAssembly = new AssemblyInfo(host);
                contractsAssembly.Load(inputFileName);
            }

            this.backend = backend;

            instrumentedEpas = new HashSet<string>();

            assembly = new CciAssembly(inputFileName, contractsFileName, host);
        }

        public EpaGenerator()
        {
            //TODO: sacar cuando se desacoplen las cosas de esta clase
        }

        public void Dispose()
        {
            host.Dispose();
        }

        public TypeAnalysisResult GenerateEpa(string typeToAnalyze, CancellationToken token)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeToAnalyze));
            Contract.Requires(token != null);

            var constructors = this.assembly.Constructors(typeToAnalyze);
            var actions = this.assembly.Actions(typeToAnalyze);
            var methods = constructors.Union(actions).Select(a => a.ToString());

            return GenerateEpa(typeToAnalyze, methods, token);
        }

        public TypeAnalysisResult GenerateEpa(string typeToAnalyze, IEnumerable<string> selectedMethods, CancellationToken token)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeToAnalyze));
            Contract.Requires(selectedMethods != null && selectedMethods.Count() > 0);
            Contract.Requires(token != null);

            var constructors = new HashSet<Action>(this.assembly.Constructors(typeToAnalyze).Where(a => selectedMethods.Contains(a.ToString())));
            var actions = new HashSet<Action>(this.assembly.Actions(typeToAnalyze).Where(a => selectedMethods.Contains(a.ToString())));

            return GenerateEpa(typeToAnalyze, constructors, actions, token);
        }

        private TypeAnalysisResult GenerateEpa(string typeToAnalyze, ISet<Action> constructors,
            ISet<Action> actions, CancellationToken token)
        {
            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => TypeHelper.GetTypeName(t, NameFormattingOptions.None).Equals(typeToAnalyze));

            IAnalyzer checker;
            switch (this.backend)
            {
                case Backend.CodeContracts:
                    checker = new CodeContractsAnalyzer(host, inputAssembly, type, token);
                    break;
                case Backend.Corral:
                    checker = new CorralAnalyzer(new CciQueryGenerator(this.host), this.assembly, this.inputAssembly.Module.Location, typeToAnalyze, token);
                    break;
                default:
                    throw new NotImplementedException("Unknown backend");
            }

            return GenerateEpa(typeToAnalyze, checker, constructors, actions);
        }

        /// <summary>
        /// Method to create an EPA of a particular type considering only the subset 'methods'
        /// </summary>
        /// <see cref="http://publicaciones.dc.uba.ar/Publications/2011/DBGU11/paper-icse-2011.pdf">Algorithm 1</see>
        //TODO: es necesario que sea public?
        public TypeAnalysisResult GenerateEpa(string typeToAnalyze, IAnalyzer analyzer, ISet<Action> constructors,
            ISet<Action> actions)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeToAnalyze));
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
            }

            analysisTimer.Stop();

            var statistics = new Dictionary<string, object>();
            statistics["TotalAnalyzerDuration"] = analyzer.TotalAnalysisDuration;
            statistics["ExecutionsCount"] = analyzer.ExecutionsCount;
            statistics["TotalGeneratedQueriesCount"] = analyzer.TotalGeneratedQueriesCount;
            statistics["UnprovenQueriesCount"] = analyzer.UnprovenQueriesCount;

            var analysisResult = new TypeAnalysisResult(epaBuilder.Build(), this.backend, analysisTimer.Elapsed, statistics);

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