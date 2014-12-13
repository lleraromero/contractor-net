using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;

namespace Contractor.Core
{
    public class Epa
    {
        private Dictionary<IState, HashSet<ITransition>> graph;
        internal bool GenerationCompleted { get; set; }
        internal bool Instrumented { get; set; }

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

        public void AddState(State s)
        {
            Contract.Requires(!States.Contains(s as IState));
            Contract.Ensures(States.Contains(s as IState));
         
            graph[s as IState] = new HashSet<ITransition>(); 
        }

        public void AddTransition(Transition t)
        {
            Contract.Requires(States.Contains(t.SourceState as IState));
            Contract.Requires(States.Contains(t.TargetState as IState));
            Contract.Requires(!Transitions.Contains(t as ITransition));

            graph[t.SourceState as IState].Add(t as ITransition);
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
    }
}
