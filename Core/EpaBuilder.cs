using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;

namespace Contractor.Core
{
    /// <summary>
    /// Thread-safe EPA builder. This class is used to guarantee that EPAs are created properly. See: Builder pattern.
    /// </summary>
    public class EpaBuilder : IEpaBuilder
    {
        protected readonly ITypeDefinition typeDefinition;
        protected ISet<Transition> transitions;

        public EpaBuilder(ITypeDefinition typeDefinition)
        {
            Contract.Requires(typeDefinition != null);

            this.typeDefinition = typeDefinition;
            transitions = new HashSet<Transition>();
        }

        public void Add(Transition t)
        {
            lock (transitions)
            {
                transitions.Add(t);
                
            }
        }

        public Epa Build()
        {
            lock (transitions)
            {
                // TODO: Y si hay varias compoenentes conexas? Resolver.
                return new Epa(typeDefinition, transitions.ToList());
            }
        }

        public IReadOnlyCollection<State> States
        {
            get
            {
                ISet<State> states;
                lock (transitions)
                {
                    states = new HashSet<State>();
                    states.Add(new State(typeDefinition.Constructors(), new HashSet<Action>()));
                    states.UnionWith(transitions.Select(t => t.SourceState));
                    states.UnionWith(transitions.Select(t => t.TargetState));
                }
                return states.ToList();
            }
        }

        public IReadOnlyCollection<Transition> Transitions
        {
            get
            {
                lock (transitions)
                {
                    return transitions.ToList();
                }
            }
        }

        public ITypeDefinition Type
        {
            get { return typeDefinition; }
        }
    }
}