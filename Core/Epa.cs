using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    public class Epa : IEquatable<Epa>
    {
        protected string type;
        protected Dictionary<IState, HashSet<ITransition>> graph;
        protected IState initial;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Transitions.All(t => States.Contains(t.TargetState)));
            Contract.Invariant(Transitions.All(t => States.Contains(t.SourceState)));
        }

        public Epa(string type, Dictionary<IState, HashSet<ITransition>> graph, IState initial)
        {
            this.type = type;
            this.graph = graph;
            this.initial = initial;
        }

        public string Type { get { return this.type; } }
        public IState Initial { get { return this.initial; } }

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

        public HashSet<ITransition> this[IState s]
        {
            get { return new HashSet<ITransition>(graph[s]); }
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
