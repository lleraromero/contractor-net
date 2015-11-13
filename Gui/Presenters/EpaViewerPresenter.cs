using Contractor.Core.Model;

namespace Contractor.Gui
{
    internal class EpaViewerPresenter
    {
        protected readonly IEpaViewerScreen screen;
        protected IEpaViewer epaViewer;

        public EpaViewerPresenter(IEpaViewerScreen screen, IEpaViewer epaViewer)
        {
            this.screen = screen;
            this.epaViewer = epaViewer;

            this.screen.Graph = this.epaViewer.Graph;
            this.screen.Reset += (sender, args) => { RefreshScreen(); };
        }

        public void Reset()
        {
            epaViewer = new EpaViewer();
            screen.Graph = epaViewer.Graph;
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
            screen.Graph = epaViewer.Graph;
        }
    }
}