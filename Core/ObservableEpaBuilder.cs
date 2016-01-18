using System;
using System.Collections.Generic;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public class ObservableEpaBuilder : IEpaBuilder
    {
        protected IEpaBuilder epaBuilder;

        public ObservableEpaBuilder(IEpaBuilder epaBuilder)
        {
            this.epaBuilder = epaBuilder;
            StateAdded += (sender, args) => { };
            TransitionAdded += (sender, args) => { };
        }

        public void Add(State s)
        {
            epaBuilder.Add(s);
            StateAdded(this, new StateAddedEventArgs(epaBuilder.Type, epaBuilder, s));
        }

        public void Add(Transition t)
        {
            epaBuilder.Add(t);
            TransitionAdded(this, new TransitionAddedEventArgs(epaBuilder.Type, epaBuilder, t));
        }

        public IReadOnlyCollection<State> States
        {
            get { return epaBuilder.States; }
        }

        public IReadOnlyCollection<Transition> Transitions
        {
            get { return epaBuilder.Transitions; }
        }

        public Epa Build()
        {
            return epaBuilder.Build();
        }


        public TypeDefinition Type
        {
            get { return epaBuilder.Type; }
        }

        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;
    }
}