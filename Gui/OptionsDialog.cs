using System.Windows.Forms;

namespace Contractor.Gui
{
	partial class OptionsDialog : Form
	{
		public OptionsDialog()
		{
			InitializeComponent();
		}

		public OptionsDialog(Options options) : this()
		{
			this.Options = options;
		}

		public Options Options
		{
			get
			{
				var options = new Options();

				options.InlineMethodsBody = checkboxInlineMethodsBody.Checked;
				options.CollapseTransitions = checkboxCollapseTransitions.Checked;
				options.UnprovenTransitions = checkboxUnprovenTransitions.Checked;
				options.StateDescription = checkboxStatesDescriptions.Checked;
				options.CheckerArguments = textboxCheckerArguments.Text;

				return options;
			}
			set
			{
				checkboxInlineMethodsBody.Checked = value.InlineMethodsBody;
				checkboxCollapseTransitions.Checked = value.CollapseTransitions;
				checkboxUnprovenTransitions.Checked = value.UnprovenTransitions;
				checkboxStatesDescriptions.Checked = value.StateDescription;
				textboxCheckerArguments.Text = value.CheckerArguments;
			}
		}
	}
}
