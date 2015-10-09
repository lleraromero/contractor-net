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
        public Tuple<EpaBuilder,State> EpaAndState { get; private set; }

        public StateAddedEventArgs(string typeFullName, Tuple<EpaBuilder, State> epaAndState)
            : base(typeFullName)
        {
            this.EpaAndState = epaAndState;
        }
    }

    public class TransitionAddedEventArgs : TypeEventArgs
    {
        public Transition Transition { get; private set; }

        public State SourceState { get; private set; }

        public EpaBuilder epaBuilder { get; private set; }

        public TransitionAddedEventArgs(string typeFullName, Transition transition, State sourceState, EpaBuilder epaBuilder)
            : base(typeFullName)
        {
            this.Transition = transition;
            this.SourceState = sourceState;
            this.epaBuilder = epaBuilder;
        }
    }

    #endregion EPAGenerator EventArgs

    public class EpaGenerator : IDisposable
    {
        public enum Backend { CodeContracts, Corral };

        private AssemblyInfo inputAssembly;
        private static Dictionary<string, TypeAnalysisResult> epas = new Dictionary<string,TypeAnalysisResult>();
        private HashSet<string> instrumentedEpas;
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

            instrumentedEpas = new HashSet<string>();
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
            {
                var methods = from name in selectedMethods
                              join m in type.Methods
                              on name equals m.GetDisplayName()
                              select m;

                var analysisResult = GenerateEpa(type, methods, token);
                epas.Add(typeUniqueName, analysisResult);
            }

            return epas[typeUniqueName];
        }

        /// <summary>
        /// Method to create an EPA of a particular type considering only the subset 'methods'
        /// </summary>
        /// <see cref="http://publicaciones.dc.uba.ar/Publications/2011/DBGU11/paper-icse-2011.pdf">Algorithm 1</see>
        private TypeAnalysisResult GenerateEpa(NamespaceTypeDefinition type, IEnumerable<IMethodDefinition> methods, CancellationToken token)
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
                                select new CciAction(m))
                                .ToList();

            var actions = (from m in methods
                           where !m.IsConstructor
                           select m)
                           .ToList();

            var epaBuilder = new EpaBuilder(typeUniqueName);

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

            states.Add(dummy.Name, dummy);
            epaBuilder.Add(dummy);
            epaBuilder.Initial = dummy;

            if (this.StateAdded != null)
                this.StateAdded(this, new StateAddedEventArgs(typeDisplayName, new Tuple<EpaBuilder, State>(epaBuilder, dummy)));

            var newStates = new Queue<State>();
            newStates.Enqueue(dummy);

            while (newStates.Count > 0 && !token.IsCancellationRequested)
            {
                var source = newStates.Dequeue();
                foreach (var action in source.EnabledActions)
                {
                    var actionUniqueName = action.Method.GetUniqueName();
                    // Which actions are enabled or disabled if 'action' is called from 'source'?
                    var actionsResult = checker.AnalyzeActions(source, action.Method, actions);

                    // Remove any inconsistency
                    var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();
                    foreach (var act in inconsistentActions)
                    {
                        actionsResult.EnabledActions.Remove(act);
                        actionsResult.DisabledActions.Remove(act);
                    }

                    var possibleTargets = generatePossibleStates(actions, actionsResult, epaBuilder.States);
                    // Which states are reachable from the current state (aka source) using 'action'?
                    var transitionsResults = checker.AnalyzeTransitions(source, action.Method, possibleTargets);

                    foreach (var transition in transitionsResults.Transitions)
                    {
                        var target = transition.TargetState;
                        // Do I have to add a new state to the EPA?
                        if (!states.ContainsKey(target.Name))
                        {
                            newStates.Enqueue(target);

                            states.Add(target.Name, target);
                            epaBuilder.Add(target);

                            if (this.StateAdded != null)
                            {
                                var eventArgs = new StateAddedEventArgs(typeDisplayName, new Tuple<EpaBuilder, State>(epaBuilder, target as State));
                                this.StateAdded(this, eventArgs);
                            }
                        }

                        epaBuilder.Add(transition);

                        if (this.TransitionAdded != null)
                        {
                            var eventArgs = new TransitionAddedEventArgs(typeDisplayName, transition as Transition, source as State, epaBuilder);
                            this.TransitionAdded(this, eventArgs);
                        }
                    }
                }
            }

            analysisTimer.Stop();

            var statistics = new Dictionary<string, object>();
            statistics["TotalAnalyzerDuration"] = checker.TotalAnalysisDuration;
            statistics["ExecutionsCount"] = checker.ExecutionsCount;
            statistics["TotalGeneratedQueriesCount"] = checker.TotalGeneratedQueriesCount;
            statistics["UnprovenQueriesCount"] = checker.UnprovenQueriesCount;

            var analysisResult = new TypeAnalysisResult(epaBuilder.Build(), this.backend, analysisTimer.Elapsed, statistics);

            if (this.TypeAnalysisDone != null)
            {
                var eventArgs = new TypeAnalysisDoneEventArgs(typeDisplayName, analysisResult);
                this.TypeAnalysisDone(this, eventArgs);
            }

            return analysisResult;
        }

        private List<State> generatePossibleStates(List<IMethodDefinition> actions, ActionAnalysisResults actionsResult, HashSet<State> knownStates)
        {
            Contract.Requires(actions != null);
            Contract.Requires(actionsResult != null);
            Contract.Requires(knownStates != null);

            var unknownActions = new HashSet<IMethodDefinition>(actions);

            unknownActions.ExceptWith(actionsResult.EnabledActions);
            unknownActions.ExceptWith(actionsResult.DisabledActions);

            var states = new List<State>();

            var v = new State();
            //TODO: sacar los linq cuando se cambie el modelo completamente
            v.EnabledActions.UnionWith(from a in actionsResult.EnabledActions select new CciAction(a));
            v.DisabledActions.UnionWith(from a in actionsResult.DisabledActions select new CciAction(a));
            v.DisabledActions.UnionWith(from a in unknownActions select new CciAction(a));
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

                    w.EnabledActions.Add(new CciAction(m));
                    w.EnabledActions.UnionWith(states[i].EnabledActions);
                    w.DisabledActions.UnionWith(states[i].DisabledActions);
                    w.DisabledActions.Remove(new CciAction(m));

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

                if (!instrumentedEpas.Contains(typeUniqueName))
                {
                    var type = (from t in inputAssembly.DecompiledModule.AllTypes
                                where typeUniqueName == t.GetUniqueName()
                                select t as NamespaceTypeDefinition)
                                .First();

                    instrumenter.InstrumentType(type, typeAnalysis.EPA);
                    instrumentedEpas.Add(typeUniqueName);
                }
            }

            inputAssembly.InjectContracts(contractProvider);
            inputAssembly.Save(outputFileName);
        }
    }
}