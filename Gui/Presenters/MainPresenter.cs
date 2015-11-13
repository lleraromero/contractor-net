using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contractor.Gui.Models;

namespace Contractor.Gui.Presenters
{
    interface IMainPresenter
    {
        void StartApplication();
    }

    class MainPresenter : IMainPresenter
    {
        protected IMainScreen screen;
        protected IMainModel model;
        public MainPresenter(IMainScreen screen, IMainModel model)
        {
            this.screen = screen;
            this.model = model;
        }

        public void StartApplication()
        {
            screen.StartApplication();
        }
    }
}
