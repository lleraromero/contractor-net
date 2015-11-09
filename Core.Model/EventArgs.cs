using System;
using System.Diagnostics.Contracts;

namespace Contractor.Core.Model
{
    public abstract class TypeEventArgs : EventArgs
    {
        public abstract TypeDefinition Type { get; }
    }

    public class StateAddedEventArgs : TypeEventArgs
    {
        protected TypeDefinition type;

        public StateAddedEventArgs(TypeDefinition type, Tuple<EpaBuilder, State> epaAndState)
        {
            Contract.Requires(type != null);
            Contract.Requires(epaAndState != null);

            this.type = type;
            EpaAndState = epaAndState;
        }

        public Tuple<EpaBuilder, State> EpaAndState { get; private set; }

        public override TypeDefinition Type
        {
            get { return type; }
        }
    }

    public class TransitionAddedEventArgs : TypeEventArgs
    {
        protected TypeDefinition type;

        public TransitionAddedEventArgs(TypeDefinition type, Transition transition, State sourceState, EpaBuilder epaBuilder)
        {
            Transition = transition;
            SourceState = sourceState;
            this.epaBuilder = epaBuilder;
            this.type = type;
        }

        public Transition Transition { get; private set; }

        public State SourceState { get; private set; }

        public EpaBuilder epaBuilder { get; private set; }

        public override TypeDefinition Type
        {
            get { return type; }
        }
    }
}