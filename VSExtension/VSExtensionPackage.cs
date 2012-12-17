using System;
using System.Diagnostics;
using System.Globalization;
using System.Runtime.InteropServices;
using System.ComponentModel.Design;
using Microsoft.Win32;
using Microsoft.VisualStudio;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell;
using System.Windows;
using System.IO;
using Contractor.VSExtension.Properties;
using Contractor.Core;
using System.Windows.Forms;

namespace Contractor.VSExtension
{
    /// <summary>
    /// This is the class that implements the package exposed by this assembly.
    ///
    /// The minimum requirement for a class to be considered a valid package for Visual Studio
    /// is to implement the IVsPackage interface and register itself with the shell.
    /// This package uses the helper classes defined inside the Managed Package Framework (MPF)
    /// to do it: it derives from the Package class that provides the implementation of the 
    /// IVsPackage interface and uses the registration attributes defined in the framework to 
    /// register itself and its components with the shell.
    /// </summary>
    // This attribute tells the PkgDef creation utility (CreatePkgDef.exe) that this class is
    // a package.
    [PackageRegistration(UseManagedResourcesOnly = true)]
    // This attribute is used to register the informations needed to show the this package
    // in the Help/About dialog of Visual Studio.
    [InstalledProductRegistration("#110", "#112", "1.0", IconResourceID = 400)]
    // This attribute is needed to let the shell know that this package exposes some menus.
    [ProvideMenuResource("Menus.ctmenu", 1)]
    // This attribute registers a tool window exposed by this package.
    [ProvideToolWindow(typeof(ContractorExplorer), Orientation=ToolWindowOrientation.Bottom, Style=VsDockStyle.Linked)]
	[ProvideOptionPage(typeof(Options), "Contractor.NET", "General", 110, 113, false, SupportsProfiles=true)]
    [Guid(GuidList.guidVSExtensionPkgString)]
    public sealed class VSExtensionPackage : Package
    {
		public Options Options
		{
			get
			{
				var options = base.GetDialogPage(typeof(Options)) as Options;
				return options;
			}
		}

        /// <summary>
        /// This function is called when the user clicks the menu item that shows the 
        /// tool window. See the Initialize method to see how the menu item is associated to 
        /// this function using the OleMenuCommandService service and the MenuCommand class.
        /// </summary>
        private void ShowToolWindow(object sender, EventArgs e)
        {
            // Get the instance number 0 of this tool window. This window is single instance so this instance
            // is actually the only one.
            // The last flag is set to true so that if the tool window does not exists it will be created.
			ToolWindowPane window = this.FindToolWindow(typeof(ContractorExplorer), 0, true);
           
			if ((null == window) || (null == window.Frame))
                throw new NotSupportedException(Resources.CanNotCreateWindow);

            IVsWindowFrame windowFrame = (IVsWindowFrame)window.Frame;
            Microsoft.VisualStudio.ErrorHandler.ThrowOnFailure(windowFrame.Show());
        }

		private void ShowOptionPage(object sender, EventArgs e)
		{
			base.ShowOptionPage(typeof(Options));
		}

        /// <summary>
        /// Initialization of the package; this method is called right after the package is sited, so this is the place
        /// where you can put all the initilaization code that rely on services provided by VisualStudio.
        /// </summary>
        protected override void Initialize()
        {
            base.Initialize();

            // Add our command handlers for menu (commands must exist in the .vsct file)
            OleMenuCommandService mcs = base.GetService(typeof(IMenuCommandService)) as OleMenuCommandService;

			if (mcs != null)
            {
				CommandID commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ContractorExplorerButton);
				MenuCommand menuCmd = new MenuCommand(ShowToolWindow, commandId);
				mcs.AddCommand(menuCmd);

				commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.OptionPageButton);
				menuCmd = new MenuCommand(ShowOptionPage, commandId);
				mcs.AddCommand(menuCmd);
            }

			try
			{
				Configuration.Initialize();
			}
			catch (Exception ex)
			{
				ContractorExplorerControl.HandleException(ex);
			}
        }
    }
}
