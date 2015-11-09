using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core.Model
{
    public class Epa : IEquatable<Epa>
    {
        protected Dictionary<State, HashSet<Transition>> graph;
        protected State initial;
        protected TypeDefinition type;

        public Epa(TypeDefinition type, Dictionary<State, HashSet<Transition>> graph, State initial)
        {
            Contract.Requires(type != null);
            Contract.Requires(graph != null);
            Contract.Requires(graph.Values.All(trans => trans.All(t => graph.Keys.Contains(t.SourceState) && graph.Keys.Contains(t.TargetState))));
            Contract.Requires(initial != null && graph.Keys.Contains(initial));

            this.type = type;
            this.graph = graph;
            this.initial = initial;
        }

        public TypeDefinition Type
        {
            get { return type; }
        }

        public State Initial
        {
            get { return initial; }
        }

        public IImmutableSet<State> States
        {
            get { return graph.Keys.ToImmutableHashSet(); }
        }

        public IImmutableSet<Transition> Transitions
        {
            get
            {
                return graph.Values.Aggregate(new HashSet<Transition>(), (acum, l) => new HashSet<Transition>(acum.Union(l)))
                    .ToImmutableHashSet();
            }
        }

        #region IEquatable

        public override bool Equals(object obj)
        {
            // Again just optimization
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Actually check the type, should not throw exception from Equals override
            if (obj.GetType() != GetType()) return false;

            return Equals((Epa) obj);
        }

        public bool Equals(Epa other)
        {
            return type.Equals(other.type) && initial.Equals(other.initial) &&
                   HashSet<State>.CreateSetComparer().Equals(new HashSet<State>(graph.Keys), new HashSet<State>(other.graph.Keys)) &&
                   graph.Keys.All(s => HashSet<Transition>.CreateSetComparer().Equals(graph[s], other.graph[s]));
        }

        public override int GetHashCode()
        {
            //TODO: mejorar para que valga la propiedad equals => mismo hash
            return type.GetHashCode() ^ initial.GetHashCode();
        }

        #endregion
    }
}