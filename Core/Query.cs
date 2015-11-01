using Contractor.Core.Model;

namespace Contractor.Core
{
    public class Query
    {
        protected Action action;
        public Action Action { get { return action; } }

        public Query(Action action)
        {
            this.action = action;
        }
    }

    public class ReachableQuery : Query
    {
        public ReachableQuery(Action action) : base(action) { }
    }

    public class UnreachableQuery : Query
    {
        public UnreachableQuery(Action action) : base(action) { }
    }

    public class MayBeReachableQuery : Query
    {
        public MayBeReachableQuery(Action action) : base(action) { }
    }
}
