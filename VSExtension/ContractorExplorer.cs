using System;
using System.Collections;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Windows;
using System.Runtime.InteropServices;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Shell;
using Microsoft.Msagl.GraphViewerGdi;
using Microsoft.Msagl.Drawing;
using System.Windows.Forms;
using System.IO;
using System.ComponentModel.Design;
using Contractor.VSExtension.Properties;

namespace Contractor.VSExtension
{
    /// <summary>
    /// This class implements the tool window exposed by this package and hosts a user control.
    ///
    /// In Visual Studio tool windows are composed of a frame (implemented by the shell) and a pane, 
    /// usually implemented by the package implementer.
    ///
    /// This class derives from the ToolWindowPane class provided from the MPF in order to use its 
    /// implementation of the IVsUIElementPane interface.
    /// </summary>
    [Guid(GuidList.guidContractorExplorerString)]
    public class ContractorExplorer : ToolWindowPane
    {
		private ContractorExplorerControl control;

        /// <summary>
        /// Standard constructor for the tool window.
        /// </summary>
        public ContractorExplorer() :
            base(null)
        {
            // Set the window title reading it from the resources.
            this.Caption = Resources.ToolWindowTitle;
            // Set the image that will appear on the tab of the window frame
            // when docked with an other window
            // The resource ID correspond to the one defined in the resx file
            // while the Index is the offset in the bitmap strip. Each image in
            // the strip being 16x16.
            this.BitmapResourceID = 301;
            this.BitmapIndex = 1;
			this.ToolBar = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ContractExplorerToolbar);
			this.control = new ContractorExplorerControl();
        }

		/// <summary>
		/// Initialization of the tool window; this method is called right after the tool window is sited, so this is the place
		/// where you can put all the initilaization code that rely on services provided by VisualStudio.
		/// </summary>
		protected override void Initialize()
		{
			base.Initialize();

			var pkg = base.Package as VSExtensionPackage;
			control.Options = pkg.Options;

			// Add our command handlers for menu (commands must exist in the .vsct file)
			OleMenuCommandService mcs = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;
			control.InitializeToolBar(mcs);
		}

		public override IWin32Window Window
		{
			get
			{
				return (IWin32Window) this.control;
			}
		}
	}
}
