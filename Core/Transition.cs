using Microsoft.Cci;
using System;
using Contractor.Utils;

namespace Contractor.Core
{
    // Immutable representation of an EPA transition
    public interface ITransition
    {
        string Action { get; }
        IState SourceState { get; }
        IState TargetState { get; }
        bool IsUnproven { get; }

        string Condition { get; }
    }

    public class Transition : ITransition, IEquatable<Transition>
    {
        public IMethodDefinition Action { get; private set; }
        public State SourceState { get; private set; }
        public State TargetState { get; private set; }
        public bool IsUnproven { get; private set; }
        public string Condition { get; private set; }

        public Transition(IMethodDefinition action, State source, State target, bool isUnproven, string condition)
        {
            this.Action = action;
            this.SourceState = source;
            this.TargetState = target;
            this.IsUnproven = isUnproven;
            this.Condition = condition;
        }

        public override string ToString()
        {
            return string.Format("{0} -- {1}cs:{3} --> {2}", this.SourceState, Utils.Extensions.GetDisplayName(this.Action), this.TargetState,this.Condition);
        }

        #region ITransition members
        string ITransition.Action
        {
            get { return Utils.Extensions.GetDisplayName(this.Action); }
        }

        IState ITransition.SourceState
        {
            get { return this.SourceState as IState; }
        }

        IState ITransition.TargetState
        {
            get { return this.TargetState as IState; }
        }

        bool ITransition.IsUnproven
        {
            get { return this.IsUnproven; }
        }
        #endregion

        #region IEquatable members
        public bool Equals(Transition other)
        {
            return this.Action.GetUniqueName().Equals(other.Action.GetUniqueName()) && this.SourceState.Equals(other.SourceState) && this.TargetState.Equals(other.TargetState);
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
