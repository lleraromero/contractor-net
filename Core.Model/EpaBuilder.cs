using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core;

namespace Contractor.Core.Model
{
    public class EpaBuilder : IEpaBuilder
    {
        protected Dictionary<State, HashSet<Transition>> graph;
        protected TypeDefinition type;
        protected State initial;

        public EpaBuilder(TypeDefinition Type, State initial)
        {
            this.type = Type;
            this.graph = new Dictionary<State, HashSet<Transition>>();

            Add(initial);
            this.initial = initial;
        }

        public TypeDefinition Type { get { return this.type; } }

        public State Initial { get { return initial; } }

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

        public void Add(State s)
        {
            Contract.Requires(!States.Contains(s));
            Contract.Ensures(States.Contains(s));

            graph[s] = new HashSet<Transition>();
        }

        public void Remove(State s)
        {
            Contract.Requires(States.Contains(s));

            var existingTransitions = Transitions.Where(t => t.SourceState == s || t.TargetState == s);
            if (!existingTransitions.Any())
            {
                graph.Remove(s);
            }
            else
            {
                foreach (var t in existingTransitions)
                {
                    Remove(t as Transition);
                }
            }
        }

        public void Add(Transition t)
        {
            Contract.Requires(States.Contains(t.SourceState));
            Contract.Requires(States.Contains(t.TargetState));
            Contract.Requires(!Transitions.Contains(t));

            graph[t.SourceState].Add(t);
        }

        public void Remove(Transition t)
        {
            Contract.Requires(Transitions.Contains(t));

            graph[t.SourceState].Remove(t);
            // If sourceState has no transitions from or to it, it has to be deleted
            var sourceState = t.SourceState;
            if (Transitions.All(x => x.SourceState != sourceState && x.TargetState != sourceState))
            {
                Remove(sourceState);
            }
            // If targetState has no transitions from or to it, it has to be deleted
            var targetState = t.TargetState;
            if (Transitions.All(x => x.SourceState != targetState && x.TargetState != targetState))
            {
                Remove(targetState);
            }
        }

        public Epa Build()
        {
            return new Epa(type, graph, initial);
        }
    }
}
