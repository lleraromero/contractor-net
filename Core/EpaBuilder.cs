using System;
using System.Collections.Generic;
using System.Linq;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public class EpaBuilder : IEpaBuilder
    {
        protected readonly TypeDefinition typeDefinition;
        protected ISet<Transition> transitions;

        public EpaBuilder(TypeDefinition typeDefinition)
        {
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
                    states = new HashSet<State>(transitions.Select(t => t.SourceState));
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

        public TypeDefinition Type
        {
            get { return typeDefinition; }
        }
    }
}