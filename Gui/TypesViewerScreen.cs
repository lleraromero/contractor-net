﻿using System;
using System.Windows.Forms;
using Contractor.Core.Model;

namespace Contractor.Gui
{
    internal interface ITypesViewerScreen
    {
        TreeNode Root { set; }
        void Reset();
        event EventHandler<ITypeDefinition> TypeSelected;
        event EventHandler<ITypeDefinition> StartAnalysis;
    }

    public partial class TypesViewerScreen : UserControl, ITypesViewerScreen
    {
        public TypesViewerScreen()
        {
            InitializeComponent();
            trvTypes.ShowPlusMinus = true;
            trvTypes.Sorted = true;
        }

        public event EventHandler<ITypeDefinition> TypeSelected;
        public event EventHandler<ITypeDefinition> StartAnalysis;

        public TreeNode Root
        {
            set
            {
                trvTypes.BeginUpdate();
                trvTypes.Nodes.Add(value);
                trvTypes.EndUpdate();
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
            var typeDefinition = e.Node.Tag as ITypeDefinition;
            if (typeDefinition != null && TypeSelected != null)
            {
                TypeSelected(sender, typeDefinition);
            }
        }

        protected void trvTypes_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            var typeDefinition = e.Node.Tag as ITypeDefinition;
            if (typeDefinition != null && StartAnalysis != null)
            {
                StartAnalysis(sender, typeDefinition);
            }
        }
    }
}