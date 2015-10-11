using System;

namespace Contractor.Core.Model
{
    public class Transition : IEquatable<Transition>
    {
        protected Action action;
        protected State sourceState;
        protected State targetState;
        protected bool isUnproven;

        public Action Action { get { return action; } }
        public State SourceState { get { return sourceState; } }
        public State TargetState { get { return targetState; } }
        public bool IsUnproven { get { return isUnproven; } }

        public Transition(Action action, State source, State target, bool isUnproven)
        {
            this.action = action;
            this.sourceState = source;
            this.targetState = target;
            this.isUnproven = isUnproven;
        }

        #region IEquatable
        public override bool Equals(object obj)
        {
            // Again just optimization
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Actually check the type, should not throw exception from Equals override
            if (obj.GetType() != this.GetType()) return false;

            return Equals((Transition)obj);
        }
        public bool Equals(Transition other)
        {
            return this.action.Equals(other.action) && this.sourceState.Equals(other.sourceState) && this.targetState.Equals(other.targetState);
        }

        public override int GetHashCode()
        {
            return this.action.GetHashCode() ^ this.sourceState.GetHashCode() ^ this.targetState.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return string.Format("{0} -- {1} --> {2}", this.sourceState, this.action, this.targetState);
        }
    }
}
