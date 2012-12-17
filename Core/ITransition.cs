using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contractor.Core
{
	public interface ITransition
	{
		IState SourceState { get; }
		IState TargetState { get; }
		string Name { get; }
		bool IsUnproven { get; }
	}
}
