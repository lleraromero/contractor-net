using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Contractor.Core;

namespace Contractor.Gui
{
	class Options
	{
		public bool CollapseTransitions { get; set; }
		public bool UnprovenTransitions { get; set; }
		public bool StateDescription { get; set; }

		public Options()
		{
			this.CollapseTransitions = true;
			this.UnprovenTransitions = true;
			this.StateDescription = true;
		}

		public string CheckerArguments
		{
			get { return Configuration.CheckerArguments; }
			set { Configuration.CheckerArguments = value; }
		}

		public bool InlineMethodsBody
		{
			get { return Configuration.InlineMethodsBody; }
			set { Configuration.InlineMethodsBody = value; }
		}
	}
}
