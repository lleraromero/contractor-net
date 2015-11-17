using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core.Model
{
    public class EpaBuilder : IEpaBuilder
    {
        protected Dictionary<State, HashSet<Transition>> graph;
        protected State initial;
        protected TypeDefinition type;

        public EpaBuilder(TypeDefinition type)
        {
            this.type = type;
            graph = new Dictionary<State, HashSet<Transition>>();
        }

        public State Initial
        {
            get { return initial; }
        }

        public TypeDefinition Type
        {
            get { return type; }
        }

        public void SetStateAsInitial(State initial)
        {
            if (!States.Contains(initial))
            {
                Add(initial);
            }
            this.initial = initial;
        }

        public IReadOnlyCollection<State> States
        {
            get { return new ReadOnlyCollection<State>(graph.Keys.ToList()); }
        }

        public IReadOnlyCollection<Transition> Transitions
        {
            get
            {
                return
                    new ReadOnlyCollection<Transition>(
                        graph.Values.Aggregate(new HashSet<Transition>(), (acum, l) => new HashSet<Transition>(acum.Union(l))).ToList());
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

            var existingTransitions = Transitions.Where(t => t.SourceState.Equals(s) || t.TargetState.Equals(s));
            if (!existingTransitions.Any())
            {
                graph.Remove(s);
            }
            else
            {
                foreach (var t in existingTransitions)
                {
                    Remove(t);
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
            if (Transitions.All(x => !x.SourceState.Equals(sourceState) && !x.TargetState.Equals(sourceState)))
            {
                Remove(sourceState);
            }
            // If targetState has no transitions from or to it, it has to be deleted
            var targetState = t.TargetState;
            if (Transitions.All(x => !x.SourceState.Equals(targetState) && !x.TargetState.Equals(targetState)))
            {
                Remove(targetState);
            }
        }

        public Epa Build()
        {
            Contract.Requires(Initial != null);
            return new Epa(type, graph, initial);
        }
    }
}