using System;
using System.Threading;
using System.Windows.Forms;
using Contractor.Core;
using Contractor.Core.Model;

namespace Contractor.Gui
{
    internal class TypesViewerPresenter
    {
        public event EventHandler<TypeDefinition> StartAnalysis;
        public event EventHandler TypeSelected;

        protected ITypesViewerScreen screen;
        protected SynchronizationContext syncContext;
        protected ITypesViewer typesViewer;

        public TypesViewerPresenter(ITypesViewerScreen screen, ITypesViewer typesViewer, SynchronizationContext syncContext)
        {
            this.screen = screen;
            this.typesViewer = typesViewer;
            this.syncContext = syncContext;

            this.screen.StartAnalysis += (sender, definition) =>
            {
                if (StartAnalysis != null)
                {
                    StartAnalysis(sender, definition);
                }
            };

            this.screen.TypeSelected += (sender, args) =>
            {
                if (TypeSelected != null)
                {
                    TypeSelected(sender, args);
                }
            };
        }

        public void ShowTypes(IAssemblyXXX assembly)
        {
            var rootNode = typesViewer.GetTypesTreeRoot(assembly);
            syncContext.Post(param => { screen.Root = param as TreeNode; }, rootNode);
        }

        public TypeDefinition GetSelectedType()
        {
            return screen.SelectedType;
        }

        public void Reset()
        {
            syncContext.Post(_ => { screen.Reset(); }, null);
        }
    }
}