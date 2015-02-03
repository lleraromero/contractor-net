#define PROPAGATION

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
        private Dictionary<string, TypeAnalysisResult> epas;
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
            epas = new Dictionary<string, TypeAnalysisResult>();
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
            foreach (var typeAnalysis in epas.Values)
                typeAnalysis.EPA.Instrumented = false;
        }

        public Dictionary<string, TypeAnalysisResult> GenerateEpas(CancellationToken token)
        {
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
            var analysisResult = new TypeAnalysisResult();
            analysisResult.EPA = epa;
            analysisResult.TotalDuration = analysisTimer.Elapsed;
            analysisResult.Statistics["TotalAnalyzerDuration"] = checker.TotalAnalysisDuration;
            analysisResult.Statistics["ExecutionsCount"] = checker.ExecutionsCount;
            analysisResult.Statistics["TotalGeneratedQueriesCount"] = checker.TotalGeneratedQueriesCount;
            analysisResult.Statistics["UnprovenQueriesCount"] = checker.UnprovenQueriesCount;
            analysisResult.Backend = this.backend;

            #region PropagationExperiment

#if PROPAGATION
            var pngSerializer = new EpaBinarySerializer();
            using (var epa1 = File.Create(string.Format("{0}\\{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), TypeHelper.GetTypeName(type, NameFormattingOptions.OmitContainingNamespace))))
            {
                pngSerializer.Serialize(epa1, epa);
            }

            var transitionsCount = epa.Transitions.Count;
            var propagationAnalysis = Stopwatch.StartNew();
            PropagateCode(epa, checker, inputAssembly);
            propagationAnalysis.Stop();

            analysisResult.Statistics["PropagationPhaseDuration"] = propagationAnalysis.Elapsed;
            analysisResult.Statistics["PropagationPhaseRemovedTransitions"] = transitionsCount - epa.Transitions.Count;

            using (var epa2 = File.Create(string.Format("{0}\\{1}.png", Environment.GetFolderPath(Environment.SpecialFolder.Desktop), TypeHelper.GetTypeName(type, NameFormattingOptions.OmitContainingNamespace) + "_post")))
            {
                pngSerializer.Serialize(epa2, epa);
            }
#endif
            #endregion

            if (this.TypeAnalysisDone != null)
            {
                var eventArgs = new TypeAnalysisDoneEventArgs(typeDisplayName, analysisResult);
                this.TypeAnalysisDone(this, eventArgs);
            }
        }

        #region Propagation experiment
#if PROPAGATION
        private void PropagateCode(Epa epa, IAnalyzer checker, AssemblyInfo assembly)
        {
            var CodeOf = new Dictionary<IState, List<MethodDefinition>>();
            var PreviousState = new Dictionary<IState, List<State>>();
            var cp = assembly.ExtractContracts();

            // Perform a topological traversal of the EPA to find states that already have its method dependencies computed.
            // Which means, all the paths from the constructor to the current state are represented in the current state code.

            // TODO(lleraromero): Replace the Dictionary with an implementation of a Priority Queue
            var toVisit = new Dictionary<IState, int>(epa.States.Count);
            foreach (var s in epa.States)
            {
                toVisit.Add(s, epa.Transitions.Count(t => t.TargetState == s));
            }

            while (toVisit.Any(kvp => kvp.Value == 0))
            {
                var currentState = toVisit.First(kvp => kvp.Value == 0).Key;
                toVisit[currentState] = int.MaxValue;

                // currentState's code has changed?
                if (CodeOf.ContainsKey(currentState) && CodeOf[currentState] != null && CodeOf[currentState].Count > 0)
                {
                    // Analyse existing transitions in currentState
                    var possibleTargets = epa[currentState].Select(t => t.TargetState as State).ToList();
                    var possibleActions = epa[currentState].Select(t => (t as Transition).Action).ToList();

                    foreach (var action in possibleActions)
                    {
                        var persistentTransitions = new HashSet<Transition>();
                        for (int i = 0; i < CodeOf[currentState].Count; i++)
                        {
                            // Merge the code this state depend on with 'action' in a new MethodDefinition
                            var previousActions = CodeOf[currentState][i];
                            var mergedAction = Concat(previousActions, action as MethodDefinition, cp);
                            var previousState = PreviousState[currentState][i];
                            var transitionsResults = checker.AnalyzeTransitions(previousState, mergedAction, possibleTargets);

                            if (i == 0)
                                persistentTransitions.UnionWith(transitionsResults.Transitions);
                            else
                                persistentTransitions.IntersectWith(transitionsResults.Transitions);
                        }

                        Contract.Assert(persistentTransitions.Count <= epa[currentState].Count(t => (t as Transition).Action == action));
                        if (persistentTransitions.Count < epa[currentState].Count(t => (t as Transition).Action == action))
                        {
                            var transitions = epa[currentState].Where(t => (t as Transition).Action == action);
                            foreach (var t in transitions)
                            {
                                if (!persistentTransitions.Any(trans => trans.TargetState.Equals(t.TargetState)))
                                {
                                    epa.RemoveTransition(t as Transition);
                                }
                            }
                        }
                    }
                }

                var transToNeighbours = epa[currentState];
                foreach (var t in transToNeighbours)
                {
                    var neighbour = t.TargetState;

                    // Propagate CodeOf[currentState] to CodeOf[neighbour], ie. CodeOf[neighbour] = CodeOf[currentState]++CodeOf[t]
                    if (!CodeOf.ContainsKey(neighbour) || CodeOf[neighbour] == null)
                    {
                        CodeOf[neighbour] = new List<MethodDefinition>();
                        PreviousState[neighbour] = new List<State>();
                    }

                    if (CodeOf.ContainsKey(currentState) && CodeOf[currentState] != null)
                    {
                        for (int j = 0; j < CodeOf[currentState].Count; j++)
                        {
                            CodeOf[neighbour].Add(Concat(CodeOf[currentState][j], (t as Transition).Action as MethodDefinition, cp));
                            PreviousState[neighbour].Add(PreviousState[currentState][j]);
                        }
                    }
                    else
                    {
                        CodeOf[neighbour].Add((t as Transition).Action as MethodDefinition);
                        PreviousState[neighbour].Add(currentState as State);
                    }
                    toVisit[neighbour]--;
                }
            }

        }

        private MethodDefinition Concat(MethodDefinition first, MethodDefinition second, ContractProvider cp)
        {
            var mergedMethods = new MethodDefinition()
            {
                CallingConvention = CallingConvention.HasThis,
                ContainingTypeDefinition = first.ContainingTypeDefinition,
                InternFactory = this.host.InternFactory,
                IsStatic = false,
                Name = this.host.NameTable.GetNameFor(MemberHelper.GetMethodSignature(first, NameFormattingOptions.None) + MemberHelper.GetMethodSignature(second, NameFormattingOptions.None)),
                Type = this.host.PlatformType.SystemVoid,
                Visibility = TypeMemberVisibility.Public
            };

            // The new contract will contain first's precondition and second's postconditions
            cp.AssociateMethodWithContract(mergedMethods, ConcatedContracts(first, second, cp));

            var mergedBlock = new BlockStatement();
            mergedBlock.Statements.AddRange(GetFirstStmtBlock(first, cp));
            mergedBlock.Statements.AddRange(GetSecondStmtBlock(second, cp));

            var newSourceMethodBody = new SourceMethodBody(this.host)
            {
                Block = mergedBlock,
                IsNormalized = false,
                LocalsAreZeroed = first.Body.LocalsAreZeroed,
                MethodDefinition = mergedMethods,
            };

            mergedMethods.Body = newSourceMethodBody;
            return mergedMethods;
        }

        private MethodContract ConcatedContracts(MethodDefinition first, MethodDefinition second, IContractProvider cp)
        {
            var contract = new MethodContract();
            if (cp.GetMethodContractFor(first) != null && cp.GetMethodContractFor(first).Preconditions != null)
            {
                contract.Preconditions.AddRange(cp.GetMethodContractFor(first).Preconditions);
            }
            if (cp.GetMethodContractFor(second) != null && cp.GetMethodContractFor(second).Postconditions != null)
            {
                contract.Postconditions.AddRange(cp.GetMethodContractFor(second).Postconditions);
            }
            return contract;
        }

        private IEnumerable<IStatement> GetFirstStmtBlock(MethodDefinition first, IContractProvider cp)
        {
            var block = new BlockStatement();

            IBlockStatement firstBodyBlock = null;
            if (first.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
            {
                var firstBody = first.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
                firstBodyBlock = firstBody.Block;
            }
            else if (first.Body is SourceMethodBody)
            {
                var firstBody = first.Body as SourceMethodBody;
                firstBodyBlock = firstBody.Block;
            }

            block.Statements.AddRange(firstBodyBlock.Statements);
            // We need to remove return statements, otherwise, the second block won't make any sense
            if (block.Statements.Last() is IReturnStatement)
            {
                block.Statements.RemoveAt(block.Statements.Count - 1);
            }

            // After the last statement of the block we are going to assume the postcondition of the first block
            var mc = cp.GetMethodContractFor(first);
            if (mc != null && mc.Postconditions.Count() > 0)
            {
                var assumes = from post in mc.Postconditions
                              select new AssumeStatement()
                              {
                                  Condition = post.Condition,
                                  OriginalSource = post.OriginalSource
                              };
                block.Statements.AddRange(assumes);
            }
            return block.Statements;
        }

        private IEnumerable<IStatement> GetSecondStmtBlock(MethodDefinition second, IContractProvider cp)
        {
            var block = new BlockStatement();

            // Before the first statement of the block we are going to assert the precondition of the second block
            var mc = cp.GetMethodContractFor(second);

            if (mc != null && mc.Preconditions.Count() > 0)
            {
                var asserts = from pre in mc.Preconditions
                              select new AssertStatement()
                              {
                                  Condition = pre.Condition,
                                  OriginalSource = pre.OriginalSource
                              };

                block.Statements.AddRange(asserts);
            }

            IBlockStatement secondBodyBlock = null;
            if (second.Body is Microsoft.Cci.ILToCodeModel.SourceMethodBody)
            {
                var secondBody = second.Body as Microsoft.Cci.ILToCodeModel.SourceMethodBody;
                secondBodyBlock = secondBody.Block;
            }
            else if (second.Body is SourceMethodBody)
            {
                var secondBody = second.Body as SourceMethodBody;
                secondBodyBlock = secondBody.Block;
            }
            block.Statements.AddRange(secondBodyBlock.Statements);
            return block.Statements;
        }
#endif
        #endregion

        private List<State> generatePossibleStates(List<IMethodDefinition> actions, ActionAnalysisResults actionsResult, HashSet<IState> knownStates)
        {
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