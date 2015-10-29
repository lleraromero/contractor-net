using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractor.Core.Model
{
    public abstract class TypeEventArgs : EventArgs
    {
        public string TypeFullName { get; private set; }

        public TypeEventArgs(string typeFullName)
        {
            this.TypeFullName = typeFullName;
        }
    }

    public class StateAddedEventArgs : TypeEventArgs
    {
        public Tuple<EpaBuilder, State> EpaAndState { get; private set; }

        public StateAddedEventArgs(string typeFullName, Tuple<EpaBuilder, State> epaAndState)
            : base(typeFullName)
        {
            this.EpaAndState = epaAndState;
        }
    }

    public class TransitionAddedEventArgs : TypeEventArgs
    {
        public Transition Transition { get; private set; }

        public State SourceState { get; private set; }

        public EpaBuilder epaBuilder { get; private set; }

        public TransitionAddedEventArgs(string typeFullName, Transition transition, State sourceState, EpaBuilder epaBuilder)
            : base(typeFullName)
        {
            this.Transition = transition;
            this.SourceState = sourceState;
            this.epaBuilder = epaBuilder;
        }
    }
}
