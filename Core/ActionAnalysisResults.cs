using System.Collections.Generic;
using System.Collections.Immutable;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public class ActionAnalysisResults
    {
        protected HashSet<Action> enabledActions;
        protected HashSet<Action> disabledActions;

        public ActionAnalysisResults(ISet<Action> enabledActions, ISet<Action> disabledActions)
        {
            Contract.Requires(enabledActions != null && !enabledActions.Contains(null));
            Contract.Requires(disabledActions != null && !disabledActions.Contains(null));

            this.enabledActions = new HashSet<Action>(enabledActions);
            this.disabledActions = new HashSet<Action>(disabledActions);
        }

        public IImmutableSet<Action> EnabledActions
        {
            get { return enabledActions.ToImmutableHashSet(); }
        }

        public IImmutableSet<Action> DisabledActions
        {
            get { return disabledActions.ToImmutableHashSet(); }
        }
    }
}