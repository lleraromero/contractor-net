using System;
using System.Threading;
using Contractor.Core.Model;
using Contractor.Gui.Models;
using Contractor.Gui.Views;

namespace Contractor.Gui.Presenters
{
    internal class EpaViewerPresenter
    {
        protected readonly IEpaViewerScreen screen;
        protected IEpaViewer epaViewer;
        protected SynchronizationContext syncContext;

        public EpaViewerPresenter(IEpaViewerScreen screen, IEpaViewer epaViewer, SynchronizationContext syncContext)
        {
            this.screen = screen;
            this.epaViewer = epaViewer;
            this.syncContext = syncContext;

            this.screen.Graph = this.epaViewer.Graph;
            this.screen.Reset += (sender, args) => { RefreshScreen(); };
            StateSelected2 += (sender, args) => { };
            this.screen.StateSelected += (sender, args) => { StateSelected2(sender, args); };
        }

        public event EventHandler<State> StateSelected2;

        public void Reset()
        {
            epaViewer = new EpaViewer();
            RefreshScreen();
        }

        public void AddState(State state)
        {
            epaViewer.AddState(state);
            RefreshScreen();
        }

        public void AddTransition(Transition transition)
        {
            epaViewer.AddTransition(transition);
            RefreshScreen();
        }

        protected void RefreshScreen()
        {
            syncContext.Post(_ => screen.Graph = epaViewer.Graph, null);
        }
    }
}