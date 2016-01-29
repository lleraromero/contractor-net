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

            // Null-Object pattern
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

        public ITypeDefinition Type
        {
            get { return epaBuilder.Type; }
        }

        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;
    }
}