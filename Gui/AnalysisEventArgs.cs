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
        protected ITypeDefinition typeToAnalyze;
        protected string engine;
        protected string conditions;

        public AnalysisEventArgs(ITypeDefinition typeDefinition, IEnumerable<Action> selectedMethods, string engine, string conditions)
        {
            Contract.Requires(typeDefinition != null);
            Contract.Requires(selectedMethods != null);
            Contract.Requires(!string.IsNullOrEmpty(engine));
            Contract.Requires(!string.IsNullOrEmpty(conditions));

            this.typeToAnalyze = typeDefinition;
            this.selectedMethods = selectedMethods;
            this.engine = engine;
            this.conditions = conditions;
        }

        public ITypeDefinition TypeToAnalyze
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

        public string Conditions
        { 
            get { return conditions; } 
        }
    }
}