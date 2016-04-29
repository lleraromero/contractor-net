using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Contractor.Core.Model;
using Microsoft.Msagl.Drawing;

namespace Contractor.Gui.Views
{
    internal interface IEpaViewerScreen
    {
        Graph Graph { set; }
        event EventHandler Reset;
        event EventHandler<State> StateSelected;
    }

    public partial class EpaViewerScreen : UserControl, IEpaViewerScreen
    {
        public EpaViewerScreen()
        {
            InitializeComponent();

            graphViewer.OutsideAreaBrush = Brushes.White;

            Reset += (sender, args) => { };
            StateSelected += (sender, args) => { };
        }

        public Graph Graph
        {
            set
            {
                graphViewer.Graph = value;
                foreach (var entity in graphViewer.Entities)
                {
                    if (entity is IViewerNode)
                    {
                        // TODO(lleraromero): No lo estamos agregando muchas veces?
                        entity.MarkedForDraggingEvent += OnNodeMarkedForDragging;
                    }
                }
            }
        }

        public event EventHandler<State> StateSelected;

        public event EventHandler Reset;

        protected void OnNodeMarkedForDragging(object sender, EventArgs e)
        {
            var state = (sender as IViewerNode).Node.UserData as State;
            StateSelected(sender, state);
        }

        private void btnZoomIn_Click(object sender, EventArgs e)
        {
            Contract.Requires(graphViewer.Graph != null);

            graphViewer.ZoomInPressed();
        }

        private void btnZoomOut_Click(object sender, EventArgs e)
        {
            Contract.Requires(graphViewer.Graph != null);

            graphViewer.ZoomOutPressed();
        }

        private void btnBestFit_Click(object sender, EventArgs e)
        {
            Contract.Requires(graphViewer.Graph != null);

            graphViewer.FitGraphBoundingBox();
            graphViewer.ZoomF = 1.0;
        }

        private void btnReset_Click(object sender, EventArgs e)
        {
            Contract.Requires(Reset != null);

            Reset(sender, e);
        }

        private void btnPan_Click(object sender, EventArgs e)
        {
            Contract.Requires(graphViewer.Graph != null);

            graphViewer.PanButtonPressed = !graphViewer.PanButtonPressed;
            btnPan.Checked = graphViewer.PanButtonPressed;
        }
    }
}