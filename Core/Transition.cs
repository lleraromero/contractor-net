using System;

namespace Contractor.Core
{
    public class Transition : IEquatable<Transition>
    {
        public Action Action { get; protected set; }
        public State SourceState { get; protected set; }
        public State TargetState { get; protected set; }
        public bool IsUnproven { get; protected set; }

        public Transition(Action action, State source, State target, bool isUnproven)
        {
            this.Action = action;
            this.SourceState = source;
            this.TargetState = target;
            this.IsUnproven = isUnproven;
        }

        public override string ToString()
        {
            return string.Format("{0} -- {1} --> {2}", this.SourceState, this.Action.ToString(), this.TargetState);
        }

        #region IEquatable members
        public bool Equals(Transition other)
        {
            return this.Action.Name.Equals(other.Action.Name) && this.SourceState.Equals(other.SourceState) && this.TargetState.Equals(other.TargetState);
        }

        public override bool Equals(object obj)
        {
            // Again just optimization
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Actually check the type, should not throw exception from Equals override
            if (obj.GetType() != this.GetType()) return false;

            // Call the implementation from IEquatable
            return Equals((Transition)obj);
        }

        public override int GetHashCode()
        {
            return this.Action.GetHashCode() ^ this.SourceState.GetHashCode() ^ this.TargetState.GetHashCode();
        }
        #endregion
    }
}
