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
using System.Threading;

namespace Contractor.Core
{
    #region EPAGenerator EventArgs

    public abstract class TypeEventArgs : EventArgs
    {
        public string TypeFullName { get; private set; }

        public TypeEventArgs(string typeFullName)
        {
            this.TypeFullName = typeFullName;
        }
    }

    public class TypeAnalysisStartedEventArgs : TypeEventArgs
    {
        public TypeAnalysisStartedEventArgs(string typeFullName)
            : base(typeFullName)
        {
        }
    }

    public class TypeAnalysisDoneEventArgs : TypeEventArgs
    {
        public TypeAnalysisResult AnalysisResult { get; private set; }

        public TypeAnalysisDoneEventArgs(string typeFullName, TypeAnalysisResult analysisResult)
            : base(typeFullName)
        {
            this.AnalysisResult = analysisResult;
        }
    }

    public class StateAddedEventArgs : TypeEventArgs
    {
        public IState State { get; private set; }

        public StateAddedEventArgs(string typeFullName, IState state)
            : base(typeFullName)
        {
            this.State = state;
        }
    }

    public class TransitionAddedEventArgs : TypeEventArgs
    {
        public ITransition Transition { get; private set; }

        public IState SourceState { get; private set; }

        public TransitionAddedEventArgs(string typeFullName, ITransition transition, IState sourceState)
            : base(typeFullName)
        {
            this.Transition = transition;
            this.SourceState = sourceState;
        }
    }

    #endregion EPAGenerator EventArgs

    public class EpaGenerator : IDisposable
    {
        public enum Backend { CodeContracts, Corral };

        private AssemblyInfo inputAssembly;
        private static Dictionary<string, TypeAnalysisResult> epas = new Dictionary<string,TypeAnalysisResult>();
        private CodeContractAwareHostEnvironment host;
        private Backend backend;

        public event EventHandler<TypeAnalysisStartedEventArgs> TypeAnalysisStarted;
        public event EventHandler<TypeAnalysisDoneEventArgs> TypeAnalysisDone;
        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

        public EpaGenerator(Backend backend)
        {
            Contract.Ensures(host != null);
            Contract.Ensures(inputAssembly != null);

            host = new CodeContractAwareHostEnvironment(true);
            inputAssembly = new AssemblyInfo(host);
            this.backend = backend;
        }

        public void Dispose()
        {
            host.Dispose();
        }

        public void LoadAssembly(string inputFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputFileName));

            inputAssembly.Load(inputFileName);
            // Cleaning the EPAs that were generated with another assembly
            epas.Clear();
        }

        // Loads the contract reference assembly in the host.
        public void LoadContractReferenceAssembly(string inputFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(inputFileName));

            var contractsAssembly = new AssemblyInfo(host);
            contractsAssembly.Load(inputFileName);
        }

        public Dictionary<string, TypeAnalysisResult> GenerateEpas(CancellationToken token)
        {
            Contract.Requires(token != null);

            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var analysisResults = new Dictionary<string, TypeAnalysisResult>();

            foreach (var type in types)
            {
                var typeUniqueName = type.GetUniqueName();

                var result = GenerateEpa(type.ToString(), token);
                analysisResults.Add(typeUniqueName, result);
            }

            return analysisResults;
        }

        public TypeAnalysisResult GenerateEpa(string typeFullName, CancellationToken token)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeFullName));
            Contract.Requires(token != null);

            //Borramos del nombre los parametros de generics
            int start = typeFullName.IndexOf('<');

            if (start != -1)
                typeFullName = typeFullName.Remove(start);

            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => t.ToString() == typeFullName);

            var methods = from m in type.GetPublicInstanceMethods()
                          select m.GetDisplayName();
            return GenerateEpa(typeFullName, methods, token);
        }

        public TypeAnalysisResult GenerateEpa(string typeFullName, IEnumerable<string> selectedMethods, CancellationToken token)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeFullName));
            Contract.Requires(selectedMethods != null && selectedMethods.Count() > 0);
            Contract.Requires(token != null);

            //Borramos del nombre los parametros de generics
            int start = typeFullName.IndexOf('<');

            if (start != -1)
                typeFullName = typeFullName.Remove(start);

            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => t.ToString() == typeFullName);
            var typeUniqueName = type.GetUniqueName();

            if (!epas.ContainsKey(typeUniqueName))
                epas.Add(typeUniqueName, new TypeAnalysisResult());

            var typeAnalysis = epas[typeUniqueName];

            if (!typeAnalysis.EPA.GenerationCompleted)
            {
                var methods = from name in selectedMethods
                              join m in type.Methods
                              on name equals m.GetDisplayName()
                              select m;
                GenerateEpa(type, methods, token);
            }

            return typeAnalysis;
        }

        /// <summary>
        /// Method to create an EPA of a particular type considering only the subset 'methods'
        /// </summary>
        /// <see cref="http://publicaciones.dc.uba.ar/Publications/2011/DBGU11/paper-icse-2011.pdf">Algorithm 1</see>
        private void GenerateEpa(NamespaceTypeDefinition type, IEnumerable<IMethodDefinition> methods, CancellationToken token)
        {
            Contract.Requires(type != null);
            Contract.Requires(methods != null && methods.Count() > 0);
            Contract.Requires(token != null);

            var typeDisplayName = type.GetDisplayName();
            var typeUniqueName = type.GetUniqueName();
            var analysisTimer = Stopwatch.StartNew();

            if (this.TypeAnalysisStarted != null)
                this.TypeAnalysisStarted(this, new TypeAnalysisStartedEventArgs(typeDisplayName));

            var constructors = (from m in methods
                                where m.IsConstructor
                                select m)
                                .ToList();

            var actions = (from m in methods
                           where !m.IsConstructor
                           select m)
                           .ToList();

            var epa = epas[typeUniqueName].EPA;
            epa.Type = typeUniqueName;

            IAnalyzer checker;
            switch (this.backend)
            {
                case Backend.CodeContracts:
                    checker = new CodeContractsAnalyzer(host, inputAssembly, type, token);
                    break;
                case Backend.Corral:
                    checker = new CorralAnalyzer(host, inputAssembly.Module, type, token);
                    break;
                default:
                    throw new NotImplementedException("Unknown backend");
            }

            var states = new Dictionary<string, State>();

            var dummy = new State();
            dummy.EnabledActions.UnionWith(constructors);
            dummy.IsInitial = true;
            dummy.Id = 0;

            states.Add(dummy.UniqueName, dummy);
            epa.AddState(dummy);

            if (this.StateAdded != null)
                this.StateAdded(this, new StateAddedEventArgs(typeDisplayName, dummy));

            var newStates = new Queue<State>();
            newStates.Enqueue(dummy);

            while (newStates.Count > 0 && !token.IsCancellationRequested)
            {
                var source = newStates.Dequeue();
                foreach (var action in source.EnabledActions)
                {
                    var actionUniqueName = action.GetUniqueName();
                    // Which actions are enabled or disabled if 'action' is called from 'source'?
                    var actionsResult = checker.AnalyzeActions(source, action, actions);

                    // Remove any inconsistency
                    var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();
                    foreach (var act in inconsistentActions)
                    {
                        actionsResult.EnabledActions.Remove(act);
                        actionsResult.DisabledActions.Remove(act);
                    }

                    var possibleTargets = generatePossibleStates(actions, actionsResult, epa.States);
                    // Which states are reachable from the current state (aka source) using 'action'?
                    var transitionsResults = checker.AnalyzeTransitions(source, action, possibleTargets);

                    foreach (var transition in transitionsResults.Transitions)
                    {
                        var target = transition.TargetState;
                        // Do I have to add a new state to the EPA?
                        if (!states.ContainsKey(target.UniqueName))
                        {
                            target.Id = (uint)states.Keys.Count;
                            target.IsInitial = false;
                            newStates.Enqueue(target);

                            states.Add(target.UniqueName, target);
                            epa.AddState(target);

                            if (this.StateAdded != null)
                            {
                                var eventArgs = new StateAddedEventArgs(typeDisplayName, target as IState);
                                this.StateAdded(this, eventArgs);
                            }
                        }

                        epa.AddTransition(transition);

                        if (this.TransitionAdded != null)
                        {
                            var eventArgs = new TransitionAddedEventArgs(typeDisplayName, transition as ITransition, source as IState);
                            this.TransitionAdded(this, eventArgs);
                        }
                    }
                }
            }

            analysisTimer.Stop();

            epa.GenerationCompleted = true;

            //#region PropagationExperiment
            //var pngSerializer = new EpaBinarySerializer();
            //using (var epa1 = File.Create(string.Format("{0}\\{1}.png", Configuration.TempPath, TypeHelper.GetTypeName(type, NameFormattingOptions.OmitContainingNamespace))))
            //{
            //    pngSerializer.Serialize(epa1, epa);
            //}

            //var transitionsCount = epa.Transitions.Count;
            //var propagationAnalysis = Stopwatch.StartNew();
            //new FeasiblePathsPass(this.host).Run(epa, checker, inputAssembly);
            //propagationAnalysis.Stop();

            //analysisResult.Statistics["PropagationPhaseDuration"] = propagationAnalysis.Elapsed;
            //analysisResult.Statistics["PropagationPhaseRemovedTransitions"] = transitionsCount - epa.Transitions.Count;

            //using (var epa2 = File.Create(string.Format("{0}\\{1}_post.png", Configuration.TempPath, TypeHelper.GetTypeName(type, NameFormattingOptions.OmitContainingNamespace))))
            //{
            //    epa = FeasiblePathsPass.GetLineEpa(epa);
            //    pngSerializer.Serialize(epa2, epa);
            //}
            //#endregion

            if (this.TypeAnalysisDone != null)
            {
                var analysisResult = new TypeAnalysisResult();
                analysisResult.EPA = epa;
                analysisResult.TotalDuration = analysisTimer.Elapsed;
                analysisResult.Statistics["TotalAnalyzerDuration"] = checker.TotalAnalysisDuration;
                analysisResult.Statistics["ExecutionsCount"] = checker.ExecutionsCount;
                analysisResult.Statistics["TotalGeneratedQueriesCount"] = checker.TotalGeneratedQueriesCount;
                analysisResult.Statistics["UnprovenQueriesCount"] = checker.UnprovenQueriesCount;
                analysisResult.Backend = this.backend;

                var eventArgs = new TypeAnalysisDoneEventArgs(typeDisplayName, analysisResult);
                this.TypeAnalysisDone(this, eventArgs);
            }
        }

        private List<State> generatePossibleStates(List<IMethodDefinition> actions, ActionAnalysisResults actionsResult, HashSet<IState> knownStates)
        {
            Contract.Requires(actions != null && actions.Count > 0);
            Contract.Requires(actionsResult != null);
            Contract.Requires(knownStates != null);

            var unknownActions = new HashSet<IMethodDefinition>(actions);

            unknownActions.ExceptWith(actionsResult.EnabledActions);
            unknownActions.ExceptWith(actionsResult.DisabledActions);

            var states = new List<State>();

            var v = new State();
            v.EnabledActions.UnionWith(actionsResult.EnabledActions);
            v.DisabledActions.UnionWith(actionsResult.DisabledActions);
            v.DisabledActions.UnionWith(unknownActions);
            if (knownStates.Contains(v))
            {
                v = knownStates.Single(s => s.Equals(v)) as State;
            }
            states.Add(v);

            while (unknownActions.Count > 0)
            {
                var m = unknownActions.First();
                unknownActions.Remove(m);

                var count = states.Count;

                for (int i = 0; i < count; ++i)
                {
                    var w = new State();

                    w.EnabledActions.Add(m);
                    w.EnabledActions.UnionWith(states[i].EnabledActions);
                    w.DisabledActions.UnionWith(states[i].DisabledActions);
                    w.DisabledActions.Remove(m);

                    if (knownStates.Contains(w))
                    {
                        w = knownStates.Single(s => s.Equals(w)) as State;
                    }

                    states.Add(w);
                }
            }

            return states;
        }

        public void GenerateOutputAssembly(string outputFileName)
        {
            Contract.Requires(!string.IsNullOrEmpty(outputFileName));

            var contractProvider = inputAssembly.ExtractContracts();
            var instrumenter = new Instrumenter(host, contractProvider);

            foreach (var typeUniqueName in epas.Keys)
            {
                var typeAnalysis = epas[typeUniqueName];

                if (!typeAnalysis.EPA.Instrumented)
                {
                    var type = (from t in inputAssembly.DecompiledModule.AllTypes
                                where typeUniqueName == t.GetUniqueName()
                                select t as NamespaceTypeDefinition)
                                .First();

                    instrumenter.InstrumentType(type, typeAnalysis.EPA);
                    typeAnalysis.EPA.Instrumented = true;
                }
            }

            inputAssembly.InjectContracts(contractProvider);
            inputAssembly.Save(outputFileName);
        }
    }
}