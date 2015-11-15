using System;
using System.Threading;
using System.Windows.Forms;
using Contractor.Core;
using Contractor.Core.Model;

namespace Contractor.Gui
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

        public event EventHandler<TypeDefinition> StartAnalysis;
        public event EventHandler<TypeDefinition> TypeSelected;

        public void ShowTypes(IAssemblyXXX assembly)
        {
            var rootNode = typesViewer.GetTypesTreeRoot(assembly);
            syncContext.Post(param => { screen.Reset(); screen.Root = param as TreeNode; }, rootNode);
        }
    }
}