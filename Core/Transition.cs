using Microsoft.Cci;
using System;
using Contractor.Utils;

namespace Contractor.Core
{
    // Immutable representation of an EPA transition
    public interface ITransition
    {
        string Action { get; }
        State SourceState { get; }
        State TargetState { get; }
        bool IsUnproven { get; }
    }

    public class Transition : ITransition, IEquatable<Transition>
    {
        public Action Action { get; private set; }
        public State SourceState { get; private set; }
        public State TargetState { get; private set; }
        public bool IsUnproven { get; private set; }

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

        #region ITransition members
        string ITransition.Action
        {
            get { return this.Action.ToString(); }
        }

        State ITransition.SourceState
        {
            get { return this.SourceState as State; }
        }

        State ITransition.TargetState
        {
            get { return this.TargetState as State; }
        }

        bool ITransition.IsUnproven
        {
            get { return this.IsUnproven; }
        }
        #endregion

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
