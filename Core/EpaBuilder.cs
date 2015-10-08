using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    public class EpaBuilder
    {
        protected Dictionary<IState, HashSet<ITransition>> graph;
        protected string type;
        protected IState initial;

        public EpaBuilder(string Type)
        {
            this.type = Type;
            this.graph = new Dictionary<IState, HashSet<ITransition>>();
        }

        public string Type { get { return this.type; } }

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

        public void Add(IState s)
        {
            Contract.Requires(!States.Contains(s));
            Contract.Ensures(States.Contains(s));

            graph[s] = new HashSet<ITransition>();
        }

        public void Remove(IState s)
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
                    Remove(t as Transition);
                }
            }
        }

        public void Add(ITransition t)
        {
            Contract.Requires(States.Contains(t.SourceState));
            Contract.Requires(States.Contains(t.TargetState));
            Contract.Requires(!Transitions.Contains(t));

            graph[t.SourceState].Add(t);
        }

        public void Remove(ITransition t)
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

        public IState Initial
        {
            get
            {
                Contract.Requires(Initial != null);
                return initial;
            }
            set
            {
                Contract.Requires(States.Contains(value));
                initial = value;
            }
        }

        public Epa Build()
        {
            return new Epa(type, graph, initial);
        }
    }
}
