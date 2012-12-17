using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Contractor.Core
{
	public interface IState
	{
		string Name { get; }
		bool IsInitial { get; }
		IEnumerable<string> EnabledActions { get; }
		IEnumerable<string> DisabledActions { get; }
	}
}
