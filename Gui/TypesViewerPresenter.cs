using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Contractor.Core;

namespace Contractor.Gui
{
    class TypesViewerPresenter
    {
        protected ITypesViewerScreen screen;
        protected ITypesViewer typesViewer;
        protected SynchronizationContext syncContext;

        public TypesViewerPresenter(ITypesViewerScreen screen, ITypesViewer typesViewer, SynchronizationContext syncContext)
        {
            this.screen = screen;
            this.typesViewer = typesViewer;
            this.syncContext = syncContext;
        }

        public void ShowTypes(IAssemblyXXX assembly)
        {
            var rootNode = typesViewer.GetTypesTreeRoot(assembly);
            syncContext.Post(new SendOrPostCallback(param =>
            {
                screen.Root = param as TreeNode;
            }), rootNode);
        }
    }
}
