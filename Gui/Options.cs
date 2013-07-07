using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contractor.Core;

namespace Contractor.Gui
{
	class Options
	{
		public string CheckerArguments { get; set; }
		public bool InlineMethodsBody { get; set; }
		public bool CollapseTransitions { get; set; }
		public bool UnprovenTransitions { get; set; }
		public bool StateDescription { get; set; }

		public Options()
		{
			this.InlineMethodsBody = Configuration.InlineMethodsBody;
			this.CheckerArguments = Configuration.CheckerArguments;

			this.CollapseTransitions = true;
			this.UnprovenTransitions = true;
			this.StateDescription = true;
		}
	}
}
