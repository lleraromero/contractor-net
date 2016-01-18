using System;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public abstract class TypeEventArgs : EventArgs
    {
        public abstract TypeDefinition Type { get; }
        public abstract IEpaBuilder EpaBuilder { get; }
    }

    public class StateAddedEventArgs : TypeEventArgs
    {
        protected IEpaBuilder epaBuider;
        protected State state;
        protected TypeDefinition type;

        public StateAddedEventArgs(TypeDefinition type, IEpaBuilder epaBuilder, State state)
        {
            Contract.Requires(type != null);
            Contract.Requires(epaBuilder != null);
            Contract.Requires(state != null);

            this.type = type;
            epaBuider = epaBuilder;
            this.state = state;
        }

        public override TypeDefinition Type
        {
            get { return type; }
        }

        public override IEpaBuilder EpaBuilder
        {
            get { return epaBuider; }
        }

        public State State
        {
            get { return state; }
        }
    }

    public class TransitionAddedEventArgs : TypeEventArgs
    {
        protected IEpaBuilder epaBuilder;
        protected Transition transition;
        protected TypeDefinition type;

        public TransitionAddedEventArgs(TypeDefinition type, IEpaBuilder epaBuilder, Transition transition)
        {
            Contract.Requires(type != null);
            Contract.Requires(epaBuilder != null);
            Contract.Requires(transition != null);

            this.type = type;
            this.epaBuilder = epaBuilder;
            this.transition = transition;
        }

        public override TypeDefinition Type
        {
            get { return type; }
        }

        public override IEpaBuilder EpaBuilder
        {
            get { return epaBuilder; }
        }

        public Transition Transition
        {
            get { return transition; }
        }
    }
}