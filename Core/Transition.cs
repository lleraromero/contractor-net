using Microsoft.Cci;
using System;

namespace Contractor.Core
{
    // Immutable representation of an EPA transition
    public class ITransition : Transition
    {
        public ITransition(IMethodDefinition action, State target, bool isUnproven)
            : base(action, target, isUnproven)
        { 
        }

        public new IState TargetState
        {
            get { return base.TargetState.EPAState; }
        }

        public string Name
        {
            get { return Utils.Extensions.GetDisplayName(this.Action); }
        }
    }

    public class Transition : IEquatable<Transition>
    {
        public IMethodDefinition Action { get; private set; }
        public State TargetState { get; private set; }
        public bool IsUnproven { get; private set; }

        public Transition(IMethodDefinition action, State target, bool isUnproven)
        {
            this.Action = action;
            this.TargetState = target;
            this.IsUnproven = isUnproven;
        }

        public ITransition EPATransition
        {
            get
            {
                return new ITransition(this.Action, this.TargetState, this.IsUnproven);
            }
        }

        public override string ToString()
        {
            return string.Format("-- {0} --> {1}", Utils.Extensions.GetDisplayName(this.Action), this.TargetState);
        }

        public bool Equals(Transition other)
        {
            return this.Action.Equals(other.Action) && this.TargetState.Equals(other.TargetState);
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
            return this.Action.GetHashCode() ^ this.TargetState.GetHashCode();
        }
    }
}
