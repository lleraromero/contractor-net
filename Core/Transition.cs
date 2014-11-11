using Microsoft.Cci;

namespace Contractor.Core
{
    // Immutable representation of an EPA transition
    public class ITransition : Transition
    {
        public ITransition(State source, IMethodDefinition action, State target, bool isUnproven)
            : base(source, action, target, isUnproven)
        { 
        }

        public new IState SourceState
        {
            get { return base.SourceState.EPAState; }
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

    public class Transition
    {
        public IMethodDefinition Action { get; private set; }
        public State SourceState { get; private set; }
        public State TargetState { get; private set; }
        public bool IsUnproven { get; private set; }

        public Transition(State source, IMethodDefinition action, State target, bool isUnproven)
        {
            this.SourceState = source;
            this.Action = action;
            this.TargetState = target;
            this.IsUnproven = isUnproven;
        }

        public ITransition EPATransition
        {
            get
            {
                return new ITransition(this.SourceState, this.Action, this.TargetState, this.IsUnproven);
            }
        }

        public override string ToString()
        {
            return string.Format("{0} - {1} -> {2}", this.SourceState, Utils.Extensions.GetDisplayName(this.Action), this.TargetState);
        }
    }
}
