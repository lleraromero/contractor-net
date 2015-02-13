using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Microsoft.Cci.MutableCodeModel;
using Microsoft.Cci.MutableContracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    class FeasiblePathsPass
    {
        private IContractAwareHost host;

        public FeasiblePathsPass(IContractAwareHost host)
        {
            this.host = host;
        }

        private class PriorityState : IComparable<PriorityState>, IEquatable<PriorityState>
        {
            public int dependencies;
            public IState state;

            public PriorityState(int dependencies, IState state)
            {
                this.dependencies = dependencies;
                this.state = state;
            }

            public int CompareTo(PriorityState other)
            {
                return this.dependencies.CompareTo(other.dependencies);
            }

            public bool Equals(PriorityState other)
            {
                return this.state.Equals(other.state);
            }
        }

        public void Run(Epa epa, IAnalyzer checker, AssemblyInfo assembly)
        {
            var CodeOf = new Dictionary<IState, List<MethodDefinition>>();
            var PreviousState = new Dictionary<IState, List<State>>();
            var cp = assembly.ExtractContracts();

            // Perform a topological traversal of the EPA to find states that already have its method dependencies computed.
            // Which means, all the paths from the constructor to the current state are represented in the current state code.
            var toVisit = new PriorityQueue<PriorityState>();
            foreach (var s in epa.States)
            {
                var nonPureTransCount = epa.Transitions.Count(t => t.TargetState == s &&
                                                                    (cp.GetMethodContractFor((t as Transition).Action) == null ||
                                                                    !cp.GetMethodContractFor((t as Transition).Action).IsPure));
                toVisit.Enqueue(new PriorityState(nonPureTransCount, s));
            }

            while (toVisit.Count() > 0)
            {
                var currentState = toVisit.Dequeue().state;

                // currentState's code has changed?
                if (CodeOf.ContainsKey(currentState) && CodeOf[currentState] != null && CodeOf[currentState].Count > 0)
                {
                    // Analyse existing transitions in currentState
                    var possibleTargets = epa[currentState].Select(t => t.TargetState as State).ToList();
                    var possibleActions = new HashSet<IMethodDefinition>(epa[currentState].Select(t => (t as Transition).Action)).ToList();

                    foreach (var action in possibleActions)
                    {
                        var persistentTransitions = new HashSet<Transition>();
                        for (int i = 0; i < CodeOf[currentState].Count; i++)
                        {
                            // Merge the code this state depend on with 'action' in a new MethodDefinition
                            var previousActions = CodeOf[currentState][i];
                            var mergedAction = Concat(previousActions, action as MethodDefinition, cp);
                            var previousState = PreviousState[currentState][i];

                            var actionsResult = checker.AnalyzeActions(previousState as State, previousActions, possibleActions);
                            var inconsistentActions = actionsResult.EnabledActions.Intersect(actionsResult.DisabledActions).ToList();

                            foreach (var act in inconsistentActions)
                            {
                                actionsResult.EnabledActions.Remove(act);
                                actionsResult.DisabledActions.Remove(act);
                            }

                            //var possibleTargets = generatePossibleStates(possibleActions, actionsResult, epa.States);
                            // Which states are reachable from the current state (aka source) using 'action'?
                            var transitionsResults = checker.AnalyzeTransitions(previousState, mergedAction, possibleTargets);
                            var transToAdd = from t in transitionsResults.Transitions select new Transition(action, currentState as State, t.TargetState, t.IsUnproven);
                            persistentTransitions.UnionWith(transToAdd);
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
                    var actionContract = cp.GetMethodContractFor((t as Transition).Action);
                    if (actionContract != null && actionContract.IsPure)
                    {
                        continue;
                    }

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

                    if (toVisit.Find(new PriorityState(-1, neighbour)) != null)
                    {
                        toVisit.Find(new PriorityState(-1, neighbour))
                            .dependencies--;
                    }
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
                Name = this.host.NameTable.GetNameFor(MemberHelper.GetMethodSignature(first, NameFormattingOptions.OmitContainingNamespace | NameFormattingOptions.OmitContainingType)
                + "$" + MemberHelper.GetMethodSignature(second, NameFormattingOptions.OmitContainingNamespace | NameFormattingOptions.OmitContainingType)),
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
    }
}
