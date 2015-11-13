using System.Collections.Generic;
using System.Threading;
using Contractor.Core.Model;

namespace Contractor.Gui
{
    internal class MethodFilterPresenter
    {
        protected IMethodFilterScreen screen;

        public MethodFilterPresenter(IMethodFilterScreen screen)
        {
            this.screen = screen;
        }

        public void LoadMethods(TypeDefinition typeDefinition)
        {
            screen.LoadMethods(typeDefinition);
        }

        public IEnumerable<Action> SelectedMethods()
        {
            return screen.SelectedMethods;
        }
    }
}