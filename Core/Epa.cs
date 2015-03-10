using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    public class Epa : IEquatable<Epa>
    {
        private Dictionary<IState, HashSet<ITransition>> graph;
        internal bool GenerationCompleted { get; set; }
        internal bool Instrumented { get; set; }
        public string Type { get; set; }

        public Epa()
        {
            Contract.Ensures(!this.GenerationCompleted && !this.Instrumented);

            this.graph = new Dictionary<IState, HashSet<ITransition>>();
            
            this.GenerationCompleted = false;
            this.Instrumented = false;
        }

        public HashSet<IState> States
        {
            get { return new HashSet<IState>(graph.Keys); }
        }

        public HashSet<ITransition> Transitions
        {
            get
            {
                if (graph.Values.Count == 0)
                {
                    return new HashSet<ITransition>();
                }
                else
                {
                    return new HashSet<ITransition>(((IEnumerable<IEnumerable<ITransition>>)graph.Values).Aggregate((acum, l) => acum.Union(l)));
                }
            }
        }

        public IState Initial
        {
            get
            {
                Contract.Requires(States.Where(s => s.IsInitial).Count() == 1);

                return States.First<IState>(s => s.IsInitial);
            }
        }

        public void AddState(IState s)
        {
            Contract.Requires(!States.Contains(s));
            Contract.Ensures(States.Contains(s));
         
            graph[s] = new HashSet<ITransition>(); 
        }

        public void RemoveState(IState s)
        {
            Contract.Requires(States.Contains(s));

            var existingTransitions = Transitions.Where(t => t.SourceState == s || t.TargetState == s);
            if (existingTransitions.Count() == 0)
            {
                graph.Remove(s);
            }
            else
            {
                foreach (var t in existingTransitions)
                {
                    RemoveTransition(t as Transition);
                }
            }
        }

        public void AddTransition(ITransition t)
        {
            Contract.Requires(States.Contains(t.SourceState));
            Contract.Requires(States.Contains(t.TargetState));
            Contract.Requires(!Transitions.Contains(t));

            graph[t.SourceState].Add(t);
        }

        public void RemoveTransition(ITransition t)
        {
            Contract.Requires(Transitions.Contains(t));

            graph[t.SourceState].Remove(t);
            // If sourceState has no transitions from or to it, it has to be deleted
            var sourceState = t.SourceState;
            if (Transitions.All(x => x.SourceState != sourceState && x.TargetState != sourceState))
            {
                RemoveState(sourceState);
            }
            // If targetState has no transitions from or to it, it has to be deleted
            var targetState = t.TargetState;
            if (Transitions.All(x => x.SourceState != targetState && x.TargetState != targetState))
            {
                RemoveState(targetState);
            }
        }

        public HashSet<ITransition> this[IState s]
        {
            get { return new HashSet<ITransition>(graph[s]); }
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Transitions.All(t => States.Contains(t.TargetState)));
            Contract.Invariant(Transitions.All(t => States.Contains(t.SourceState)));
        }

        #region IEquatable
        public bool Equals(Epa other)
        {
            return string.Equals(this.Type, other.Type) && AreSetsEqual(this.States, other.States) && AreSetsEqual(this.Transitions, other.Transitions);
        }

        private bool AreSetsEqual<T>(HashSet<T> first, HashSet<T> second)
        {
            return first.Count == second.Count &&
                // Both sets have the same amount of each element
                first.All(e => first.Count(e2 => e.Equals(e2)) == second.Count(e2 => e.Equals(e2)));
        }
        #endregion
    }
}
