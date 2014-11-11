using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Linq;

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

        public TransitionAddedEventArgs(string typeFullName, ITransition transition)
            : base(typeFullName)
        {
            this.Transition = transition;
        }
    }

    #endregion EPAGenerator EventArgs

    public class EpaGenerator : IDisposable
    {
        public enum Backend { CodeContracts, Corral };

        private AssemblyInfo inputAssembly;
        private Dictionary<string, Epa> epas;
        private CodeContractAwareHostEnvironment host;
        private Backend backend;

        public event EventHandler<TypeAnalysisStartedEventArgs> TypeAnalysisStarted;
        public event EventHandler<TypeAnalysisDoneEventArgs> TypeAnalysisDone;
        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

        public EpaGenerator(Backend backend)
        {
            host = new CodeContractAwareHostEnvironment(true);
            inputAssembly = new AssemblyInfo(host);
            epas = new Dictionary<string, Epa>();
            this.backend = backend;
        }

        public void Dispose()
        {
            host.Dispose();
        }

        public void LoadAssembly(string inputFileName)
        {
            inputAssembly.Load(inputFileName);
        }

        // Loads the contract reference assembly in the host.
        public void LoadContractReferenceAssembly(string inputFileName)
        {
            var contractsAssembly = new AssemblyInfo(host);
            contractsAssembly.Load(inputFileName);
        }

        public void UnloadAssembly()
        {
            foreach (var epa in epas.Values)
                epa.Instrumented = false;
        }

        public Dictionary<string, TypeAnalysisResult> GenerateEpas()
        {
            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var analysisResults = new Dictionary<string, TypeAnalysisResult>();

            foreach (var type in types)
            {
                var typeUniqueName = type.GetUniqueName();

                var result = GenerateEpa(type.ToString());
                analysisResults.Add(typeUniqueName, result);
            }

            return analysisResults;
        }

        public TypeAnalysisResult GenerateEpa(string typeFullName)
        {
            //Borramos del nombre los parametros de generics
            int start = typeFullName.IndexOf('<');

            if (start != -1)
                typeFullName = typeFullName.Remove(start);

            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => t.ToString() == typeFullName);

            var methods = from m in type.GetPublicInstanceMethods()
                          select m.GetDisplayName();
            return GenerateEpa(typeFullName, methods);
        }

        public TypeAnalysisResult GenerateEpa(string typeFullName, IEnumerable<string> selectedMethods)
        {
            //Borramos del nombre los parametros de generics
            int start = typeFullName.IndexOf('<');

            if (start != -1)
                typeFullName = typeFullName.Remove(start);

            var types = inputAssembly.DecompiledModule.GetAnalyzableTypes().Cast<NamespaceTypeDefinition>();
            var type = types.First(t => t.ToString() == typeFullName);
            var typeUniqueName = type.GetUniqueName();

            if (!epas.ContainsKey(typeUniqueName))
                epas.Add(typeUniqueName, new Epa());

            var epa = epas[typeUniqueName];

            if (!epa.GenerationCompleted)
            {
                var methods = from name in selectedMethods
                              join m in type.Methods
                              on name equals m.GetDisplayName()
                              select m;

                GenerateEpa(type, methods);
            }

            return epa.AnalysisResult;
        }

        private void GenerateEpa(NamespaceTypeDefinition type, IEnumerable<IMethodDefinition> methods)
        {
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

            var epa = epas[typeUniqueName];
            epa.Clear();

            IAnalyzer checker;
            switch (this.backend)
            {
                case Backend.CodeContracts:
                    checker = new CodeContractsAnalyzer(host, inputAssembly, type);
                    break;
                case Backend.Corral:
                    checker = new CorralAnalyzer(host, inputAssembly.Module, type);
                    break;
                default:
                    throw new NotImplementedException("Unknown backend");
            }

            var states = new Dictionary<string, State>();

            var dummy = new State();
            dummy.EnabledActions.UnionWith(constructors);
            dummy.IsInitial = true;

            states.Add(dummy.UniqueName, dummy);
            epa.States.Add(dummy.Id, dummy.EPAState);
            epa.AnalysisResult.States.Add(dummy.EPAState);
            if (this.StateAdded != null)
                this.StateAdded(this, new StateAddedEventArgs(typeDisplayName, dummy.EPAState));

            var newStates = new Queue<State>();
            newStates.Enqueue(dummy);

            while (newStates.Count > 0)
            {
                var source = newStates.Dequeue();
                var isDummySource = (source == dummy);

                foreach (var action in source.EnabledActions)
                {
                    var actionUniqueName = action.GetUniqueName();
                    var actionsResult = checker.AnalyzeActions(source, action, actions);
                    var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();

                    foreach (var act in inconsistentActions)
                    {
                        actionsResult.EnabledActions.Remove(act);
                        actionsResult.DisabledActions.Remove(act);
                    }

                    var possibleTargets = generatePossibleStates(actions, actionsResult);
                    var transitionsResults = checker.AnalyzeTransitions(source, action, possibleTargets);

                    foreach (var transition in transitionsResults.Transitions)
                    {
                        var target = transition.TargetState;

                        if (states.ContainsKey(target.UniqueName))
                        {
                            target = states[target.UniqueName].EPAState;
                        }
                        else
                        {
                            target.Id = (uint)states.Keys.Count;
                            target.IsInitial = false;
                            newStates.Enqueue(target);

                            states.Add(target.UniqueName, target);
                            epa.States.Add(target.Id, target.EPAState);
                            epa.AnalysisResult.States.Add(target.EPAState);

                            if (this.StateAdded != null)
                            {
                                var eventArgs = new StateAddedEventArgs(typeDisplayName, target.EPAState);
                                this.StateAdded(this, eventArgs);
                            }
                        }

                        if (!epa.ContainsKey(actionUniqueName))
                            epa.Add(actionUniqueName, new EpaTransitions());

                        var actionTransitions = epa[actionUniqueName];

                        if (!actionTransitions.ContainsKey(source.Id))
                            actionTransitions.Add(source.Id, new List<uint>());

                        Contract.Assert(epa.States.Any(s => s.Value.Id == source.Id));
                        Contract.Assert(epa.States.Any(s => s.Value.Id == target.Id));
                        Contract.Assert(epa.AnalysisResult.States.Count == epa.States.Count);

                        actionTransitions[source.Id].Add(target.Id);
                        epa.AnalysisResult.Transitions.Add(transition);

                        if (this.TransitionAdded != null)
                        {
                            var eventArgs = new TransitionAddedEventArgs(typeDisplayName, transition);
                            this.TransitionAdded(this, eventArgs);
                        }
                    }
                }
            }

            analysisTimer.Stop();
            epa.GenerationCompleted = true;
            epa.AnalysisResult.TotalDuration = analysisTimer.Elapsed;
            epa.AnalysisResult.TotalAnalyzerDuration = checker.TotalAnalysisDuration;
            epa.AnalysisResult.ExecutionsCount = checker.ExecutionsCount;
            epa.AnalysisResult.TotalGeneratedQueriesCount = checker.TotalGeneratedQueriesCount;
            epa.AnalysisResult.UnprovenQueriesCount = checker.UnprovenQueriesCount;

            if (this.TypeAnalysisDone != null)
            {
                var eventArgs = new TypeAnalysisDoneEventArgs(typeDisplayName, epa.AnalysisResult);
                this.TypeAnalysisDone(this, eventArgs);
            }
        }

        private List<State> generatePossibleStates(List<IMethodDefinition> actions, ActionAnalysisResults actionsResult)
        {
            var unknownActions = new HashSet<IMethodDefinition>(actions);

            unknownActions.ExceptWith(actionsResult.EnabledActions);
            unknownActions.ExceptWith(actionsResult.DisabledActions);

            var states = new List<State>();

            var v = new State();
            v.EnabledActions.UnionWith(actionsResult.EnabledActions);
            v.DisabledActions.UnionWith(actionsResult.DisabledActions);
            v.DisabledActions.UnionWith(unknownActions);
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

                    states.Add(w);
                }
            }

            return states;
        }

        public void GenerateOutputAssembly(string outputFileName)
        {
            var contractProvider = inputAssembly.ExtractContracts();
            var instrumenter = new Instrumenter(host, contractProvider);

            foreach (var typeUniqueName in epas.Keys)
            {
                var epa = epas[typeUniqueName];

                if (!epa.Instrumented)
                {
                    var type = (from t in inputAssembly.DecompiledModule.AllTypes
                                where typeUniqueName == t.GetUniqueName()
                                select t as NamespaceTypeDefinition)
                                .First();

                    instrumenter.InstrumentType(type, epa);
                    epa.Instrumented = true;
                }
            }

            inputAssembly.InjectContracts(contractProvider);
            inputAssembly.Save(outputFileName);
        }
    }
}