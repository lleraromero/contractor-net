using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Gui
{
    internal class AnalysisEventArgs : EventArgs
    {
        protected IEnumerable<Action> selectedMethods;
        protected TypeDefinition typeToAnalyze;
        protected string engine;

        public AnalysisEventArgs(TypeDefinition typeDefinition, IEnumerable<Action> selectedMethods, string engine)
        {
            Contract.Requires(typeDefinition != null);
            Contract.Requires(selectedMethods != null);
            Contract.Requires(!string.IsNullOrEmpty(engine));

            this.typeToAnalyze = typeDefinition;
            this.selectedMethods = selectedMethods;
            this.engine = engine;
        }

        public TypeDefinition TypeToAnalyze
        {
            get { return typeToAnalyze; }
        }

        public IEnumerable<Action> SelectedMethods
        {
            get { return selectedMethods; }
        }

        public string Engine
        {
            get { return engine; }
        }
    }
}