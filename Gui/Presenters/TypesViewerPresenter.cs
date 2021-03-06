﻿using System;
using System.Threading;
using System.Windows.Forms;
using Contractor.Core.Model;
using Contractor.Gui.Models;

namespace Contractor.Gui.Presenters
{
    internal class TypesViewerPresenter
    {
        protected ITypesViewerScreen screen;
        protected SynchronizationContext syncContext;
        protected ITypesViewer typesViewer;

        public TypesViewerPresenter(ITypesViewerScreen screen, ITypesViewer typesViewer, SynchronizationContext syncContext)
        {
            this.screen = screen;
            this.typesViewer = typesViewer;
            this.syncContext = syncContext;

            this.screen.StartAnalysis += (sender, typeDefinition) =>
            {
                if (StartAnalysis != null)
                {
                    StartAnalysis(sender, typeDefinition);
                }
            };

            this.screen.TypeSelected += (sender, typeDefinition) =>
            {
                if (TypeSelected != null)
                {
                    TypeSelected(sender, typeDefinition);
                }
            };
        }

        public event EventHandler<ITypeDefinition> StartAnalysis;
        public event EventHandler<ITypeDefinition> TypeSelected;

        public void ShowTypes(IAssembly assembly)
        {
            var rootNode = typesViewer.GetTypesTreeRoot(assembly);
            syncContext.Post(param =>
            {
                screen.Reset();
                screen.Root = param as TreeNode;
            }, rootNode);
        }
    }
}