using System;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public abstract class TypeEventArgs : EventArgs
    {
        public abstract ITypeDefinition Type { get; }
        public abstract IEpaBuilder EpaBuilder { get; }
    }

    public class StateAddedEventArgs : TypeEventArgs
    {
        protected IEpaBuilder epaBuider;
        protected State state;
        protected ITypeDefinition type;

        public StateAddedEventArgs(ITypeDefinition type, IEpaBuilder epaBuilder, State state)
        {
            Contract.Requires(type != null);
            Contract.Requires(epaBuilder != null);
            Contract.Requires(state != null);

            this.type = type;
            epaBuider = epaBuilder;
            this.state = state;
        }

        public override ITypeDefinition Type
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
        protected ITypeDefinition type;

        public TransitionAddedEventArgs(ITypeDefinition type, IEpaBuilder epaBuilder, Transition transition)
        {
            Contract.Requires(type != null);
            Contract.Requires(epaBuilder != null);
            Contract.Requires(transition != null);

            this.type = type;
            this.epaBuilder = epaBuilder;
            this.transition = transition;
        }

        public override ITypeDefinition Type
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