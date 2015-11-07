using System;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;

namespace Contractor.Gui
{
    internal interface IEpaViewerScreen
    {
        Graph Graph { set; }
        event EventHandler Reset;
    }

    public partial class EpaViewerScreen : UserControl, IEpaViewerScreen
    {
        public EpaViewerScreen()
        {
            InitializeComponent();

            graphViewer.OutsideAreaBrush = Brushes.White;
        }

        public Graph Graph
        {
            set { graphViewer.Graph = value; }
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

        public event EventHandler Reset;
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