using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell;
using Contractor.Core;
using System.ComponentModel;

//TODO: agregar la opcion de borrar o no los contratos al momento de instrumentar

namespace Contractor.VSExtension
{
	[Guid(GuidList.guidOptionsString)]
	public class Options : DialogPage
	{
		[Category("Advanced")]
		[DisplayName("Checker Arguments")]
		[Description("Command line arguments passed to Code Contracts static checker")]
		[Editor(typeof(System.ComponentModel.Design.MultilineStringEditor), typeof(System.Drawing.Design.UITypeEditor))]
		public string CheckerArguments { get; set; }

		[Category("Advanced")]
		[DisplayName("Inline Methods Body")]
		[Description("Inline methods body instead of method calls")]
		public bool InlineMethodsBody { get; set; }

		[Category("Appearance")]
		[DisplayName("Collapse Transitions")]
		[Description("Collapse transitions between states")]
		public bool CollapseTransitions { get; set; }

		[Category("Appearance")]
		[DisplayName("Unproven Transitions")]
		[Description("Distinguish unproven transitions with '?'")]
		public bool UnprovenTransitions { get; set; }

		[Category("Appearance")]
		[DisplayName("State Description")]
		[Description("Show states descriptions")]
		public bool StateDescription { get; set; }

		public Options()
		{
			this.InlineMethodsBody = Configuration.InlineMethodsBody;
			this.CheckerArguments = Configuration.CheckerArguments;

			this.CollapseTransitions = true;
			this.UnprovenTransitions = true;
			this.StateDescription = true;
		}

		protected override void OnApply(PageApplyEventArgs e)
		{
			base.OnApply(e);

			if (e.ApplyBehavior == ApplyKind.Apply)
			{
				Configuration.InlineMethodsBody = this.InlineMethodsBody;
				Configuration.CheckerArguments = this.CheckerArguments;
			}
		}
	}
}
