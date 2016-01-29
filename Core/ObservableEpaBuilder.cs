using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public class ObservableEpaBuilder : IEpaBuilder
    {
        protected IEpaBuilder epaBuilder;

        public ObservableEpaBuilder(IEpaBuilder epaBuilder)
        {
            Contract.Requires(epaBuilder != null);

            this.epaBuilder = epaBuilder;
            StateAdded += (sender, args) => { };
            TransitionAdded += (sender, args) => { };
        }

        public void Add(Transition t)
        {
            epaBuilder.Add(t);

            Contract.Assert(TransitionAdded != null);
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

        public void Add(State s)
        {
            Contract.Requires(s != null);
            
            Contract.Assert(StateAdded != null);
            StateAdded(this, new StateAddedEventArgs(epaBuilder.Type, epaBuilder, s));
        }
    }
}