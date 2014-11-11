using Microsoft.Cci;

namespace Contractor.Core
{
    class Transition : ITransition
    {
        public IMethodDefinition Action { get; private set; }
        public IState SourceState { get; private set; }
        public IState TargetState { get; private set; }
        public bool IsUnproven { get; private set; }

        public Transition(State source, IMethodDefinition action, State target, bool isUnproven)
        {
            this.SourceState = source.EPAState;
            this.Action = action;
            this.TargetState = target.EPAState;
            this.IsUnproven = isUnproven;
        }

        #region ITransition

        IState ITransition.SourceState
        {
            get { return this.SourceState; }
        }

        IState ITransition.TargetState
        {
            get { return this.TargetState; }
        }

        string ITransition.Name
        {
            get { return Utils.Extensions.GetDisplayName(this.Action); }
        }

        bool ITransition.IsUnproven
        {
            get { return this.IsUnproven; }
        }

        #endregion

        public override string ToString()
        {
            return string.Format("{0} - {1} -> {2}", this.SourceState, Utils.Extensions.GetDisplayName(this.Action), this.TargetState);
        }
    }
}
