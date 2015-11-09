using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Contractor.Gui
{
    interface ITypesViewerScreen
    {
        TreeNode Root { set; }
    }

    public partial class TypesViewerScreen : UserControl, ITypesViewerScreen
    {
        public TypesViewerScreen()
        {
            InitializeComponent();
            trvTypes.ShowPlusMinus = true;
            trvTypes.Sorted = true;
        }

        public TreeNode Root 
        {
            set 
            {
                trvTypes.BeginUpdate();
                trvTypes.Nodes.Add(value);
                trvTypes.EndUpdate();
            }
        }
    }
}
