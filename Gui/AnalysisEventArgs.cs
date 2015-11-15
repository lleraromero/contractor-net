using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Gui
{
    internal class AnalysisEventArgs : EventArgs
    {
        protected IEnumerable<Action> selectedMethod;
        protected TypeDefinition typeToAnalyze;

        public AnalysisEventArgs(TypeDefinition typeDefinition, IEnumerable<Action> selectedMethods)
        {
            Contract.Requires(typeDefinition != null);
            Contract.Requires(selectedMethods != null);

            this.typeToAnalyze = typeDefinition;
            selectedMethod = selectedMethods;
        }

        public TypeDefinition TypeToAnalyze
        {
            get { return typeToAnalyze; }
        }

        public IEnumerable<Action> SelectedMethods
        {
            get { return selectedMethod; }
        }
    }
}