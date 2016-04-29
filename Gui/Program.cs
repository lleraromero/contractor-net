using System;
using System.Threading;
using System.Windows.Forms;
using Contractor.Gui.Models;
using Contractor.Gui.Presenters;
using Contractor.Gui.Views;

namespace Contractor.Gui
{
    internal static class Program
    {
        /// <summary>
        ///     The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);
            Application.Run(new Main());
        }
    }
}