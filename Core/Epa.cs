using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    public class Epa : IEquatable<Epa>
    {
        protected string type;
        protected Dictionary<State, HashSet<Transition>> graph;
        protected State initial;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Transitions.All(t => States.Contains(t.TargetState)));
            Contract.Invariant(Transitions.All(t => States.Contains(t.SourceState)));
        }

        public Epa(string type, Dictionary<State, HashSet<Transition>> graph, State initial)
        {
            this.type = type;
            this.graph = graph;
            this.initial = initial;
        }

        public string Type { get { return this.type; } }
        public State Initial { get { return this.initial; } }

        public HashSet<State> States
        {
            get { return new HashSet<State>(graph.Keys); }
        }

        public HashSet<Transition> Transitions
        {
            get
            {
                if (graph.Values.Count == 0)
                {
                    return new HashSet<Transition>();
                }
                else
                {
                    return new HashSet<Transition>(((IEnumerable<IEnumerable<Transition>>)graph.Values).Aggregate((acum, l) => acum.Union(l)));
                }
            }
        }

        public HashSet<Transition> this[State s]
        {
            get { return new HashSet<Transition>(graph[s]); }
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
