using System.Collections.Generic;
using System.Linq;
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
            var methods = typeDefinition.Constructors()
                .Union(typeDefinition.Actions());
            screen.LoadMethods(methods);
        }

        public void Reset()
        {
            screen.LoadMethods(new List<Action>());
        }

        public IEnumerable<Action> SelectedMethods()
        {
            return screen.SelectedMethods;
        }
    }
}