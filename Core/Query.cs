using Contractor.Core.Model;

namespace Contractor.Core
{
    class Query
    {
        protected Action action;
        public Action Action { get { return action; } }

        public Query(Action action)
        {
            this.action = action;
        }
    }

    class ReachableQuery : Query
    {
        public ReachableQuery(Action action) : base(action) { }
    }

    class UnreachableQuery : Query
    {
        public UnreachableQuery(Action action) : base(action) { }
    }

    class MayBeReachableQuery : Query
    {
        public MayBeReachableQuery(Action action) : base(action) { }
    }
}
