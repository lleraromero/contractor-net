using Contractor.Core.Model;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core.Model
{
    interface IEpaBuilder
    {
        void Add(State s);
        void Remove(State s);
        void Add(Transition t);
        void Remove(Transition t);
        Epa Build();
    }

    //class EpaBuilderNotifier : IEpaBuilder
    //{
    //    protected EpaBuilder epaBuilder;

    //    public EpaBuilderNotifier(EpaBuilder epaBuilder)
    //    {
    //        this.epaBuilder = epaBuilder;
    //    }

    //    public event EventHandler<StateAddedEventArgs> stateAdded;
    //    public event EventHandler<TransitionAddedEventArgs> transitionAdded;

    //    public void Add(State s)
    //    {
    //        this.epaBuilder.Add(s);
    //    }

    //    public void Remove(State s)
    //    {
    //        this.epaBuilder.Remove(s);
    //    }

    //    public void Add(Transition t)
    //    {
    //        this.epaBuilder.Add(t);
    //    }

    //    public void Remove(Transition t)
    //    {
    //        this.epaBuilder.Remove(t);
    //    }
    //}
}
