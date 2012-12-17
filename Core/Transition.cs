using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Microsoft.Cci;

namespace Contractor.Core
{
	class Transition : ITransition
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
			get { return this.Action.Name.Value; }
		}

		bool ITransition.IsUnproven
		{
			get { return this.IsUnproven; }
		}

		#endregion

		public override string ToString()
		{
			return string.Format("{0} - {1} -> {2}", this.SourceState, this.Action.Name.Value, this.TargetState);
		}
	}
}
