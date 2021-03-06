﻿using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

namespace Contractor.Core.Model
{
    public class Epa : IEquatable<Epa>
    {
        protected readonly ITypeDefinition type;
        protected Dictionary<State, ISet<Transition>> graph;

        public Epa(ITypeDefinition typeDefinition, IReadOnlyCollection<Transition> transitions)
        {
            type = typeDefinition;
            graph = new Dictionary<State, ISet<Transition>>();

            foreach (var t in transitions)
            {
                if (!graph.ContainsKey(t.SourceState))
                {
                    graph.Add(t.SourceState, new HashSet<Transition>());
                }
                graph[t.SourceState].Add(t);
            }
        }

        public ITypeDefinition Type
        {
            get { return type; }
        }

        public State Initial
        {
            get { return new State(type.Constructors(), new HashSet<Action>()); }
        }

        public IImmutableSet<State> States
        {
            get
            {
                var states = new HashSet<State>();

                // The state with constructors is always available
                states.Add(new State(type.Constructors(), new HashSet<Action>()));

                foreach (var t in Transitions)
                {
                    states.Add(t.SourceState);
                    states.Add(t.TargetState);
                }
                return states.ToImmutableHashSet();
            }
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
            return type.Equals(other.type) &&
                   HashSet<State>.CreateSetComparer().Equals(new HashSet<State>(graph.Keys), new HashSet<State>(other.graph.Keys)) &&
                   graph.Keys.All(
                       s => HashSet<Transition>.CreateSetComparer().Equals(graph[s] as HashSet<Transition>, other.graph[s] as HashSet<Transition>));
        }

        public override int GetHashCode()
        {
            return type.GetHashCode();
        }

        #endregion
    }
}