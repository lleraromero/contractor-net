using System;
using System.Diagnostics.Contracts;
using System.Windows.Forms;
using Contractor.Core.Model;

namespace Contractor.Gui
{
    internal interface ITypesViewerScreen
    {
        TreeNode Root { set; }
        TypeDefinition SelectedType { get; }
        void Reset();
        event EventHandler TypeSelected;
        event EventHandler<TypeDefinition> StartAnalysis;
    }

    public partial class TypesViewerScreen : UserControl, ITypesViewerScreen
    {
        public TypesViewerScreen()
        {
            InitializeComponent();
            trvTypes.ShowPlusMinus = true;
            trvTypes.Sorted = true;
        }

        public event EventHandler TypeSelected;
        public event EventHandler<TypeDefinition> StartAnalysis;

        public TreeNode Root
        {
            set
            {
                trvTypes.BeginUpdate();
                trvTypes.Nodes.Add(value);
                trvTypes.EndUpdate();
            }
        }

        public TypeDefinition SelectedType
        {
            get
            {
                Contract.Ensures(Contract.Result<TypeDefinition>() != null);
                return trvTypes.SelectedNode.Tag as TypeDefinition;
            }
        }

        public void Reset()
        {
            trvTypes.BeginUpdate();
            trvTypes.Nodes.Clear();
            trvTypes.EndUpdate();
        }

        protected void trvTypes_AfterSelect(object sender, TreeViewEventArgs e)
        {
            var typeDefinition = e.Node.Tag as TypeDefinition;
            if (typeDefinition != null && TypeSelected != null)
            {
                TypeSelected(sender, e);
            }
        }

        protected void trvTypes_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var typeDefinition = e.Node.Tag as TypeDefinition;
            if (typeDefinition != null && StartAnalysis != null)
            {
                StartAnalysis(sender, typeDefinition);
            }
        }
    }
}