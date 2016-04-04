using System;
using System.Diagnostics.Contracts;

namespace Contractor.Core.Model
{
    public class Transition : IEquatable<Transition>
    {
        protected Action action;
        protected State sourceState;
        protected State targetState;
        protected bool isUnproven;
        protected string condition;
        public Action Action { get { return action; } }
        public State SourceState { get { return sourceState; } }
        public State TargetState { get { return targetState; } }
        public bool IsUnproven { get { return isUnproven; } }

        public string Condition { get { return condition; } }
        public Transition(Action action, State source, State target, bool isUnproven, string condition)
        {
            Contract.Requires(action != null);
            Contract.Requires(source != null);
            Contract.Requires(target != null);

            this.action = action;
            this.sourceState = source;
            this.targetState = target;
            this.isUnproven = isUnproven;
            this.condition = condition;
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
            return string.Format("{0} -- {1}cs: {3} --> {2}", this.sourceState, this.action, this.targetState, this.condition);
        }
    }
}
