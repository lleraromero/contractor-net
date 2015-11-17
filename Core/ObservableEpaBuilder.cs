using System;
using System.Collections.Generic;
using Contractor.Core.Model;

namespace Contractor.Core
{
    internal class ObservableEpaBuilder : IEpaBuilder
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

        public void Remove(State s)
        {
            epaBuilder.Remove(s);
        }

        public void Add(Transition t)
        {
            epaBuilder.Add(t);
            TransitionAdded(this, new TransitionAddedEventArgs(epaBuilder.Type, epaBuilder, t));
        }

        public void Remove(Transition t)
        {
            epaBuilder.Remove(t);
        }

        public void SetStateAsInitial(State initial)
        {
            epaBuilder.SetStateAsInitial(initial);
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