using System;
using System.Windows.Forms;
using Contractor.Gui.Models;
using Contractor.Gui.Presenters;

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

            var presenter = new MainPresenter(new Main(), new MainModel());
            presenter.StartApplication();
        }
    }
}