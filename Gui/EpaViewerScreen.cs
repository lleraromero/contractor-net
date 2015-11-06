using System.Drawing;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;

namespace Contractor.Gui
{
    internal interface IEpaViewerScreen
    {
        Graph Graph { set; }
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
    }
}