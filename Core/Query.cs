using System.Diagnostics.Contracts;
using Contractor.Core.Model;

namespace Contractor.Core
{
    public abstract class Query
    {
        public abstract Action Method { get; }
    }

    public enum QueryType
    {
        Positive,
        Negative
    }

    public enum QueryResult
    {
        Reachable,
        MaybeReachable,
        Unreachable
    }

    public class TransitionQuery : Query
    {
        protected readonly Action queryMethod;
        protected readonly State sourceState;
        protected readonly Action action;
        protected readonly State targetState;

        public TransitionQuery(Action queryMethod, State sourceState, Action action, State targetState)
        {
            Contract.Requires(queryMethod != null);
            Contract.Requires(sourceState != null);
            Contract.Requires(action != null);
            Contract.Requires(targetState != null);
            
            this.queryMethod = queryMethod;
            this.sourceState = sourceState;
            this.action = action;
            this.targetState = targetState;
        }

        public override Action Method
        {
            get { return queryMethod; }
        }

        public State SourceState
        {
            get { return sourceState; }
        }

        public Action Action
        {
            get { return action; }
        }

        public State TargetState
        {
            get { return targetState; }
        }
    }

    public class ActionQuery : Query
    {
        protected readonly Action queryMethod;
        protected readonly QueryType queryType;
        protected readonly Action actionUnderTest;

        public ActionQuery(Action queryMethod, QueryType queryType, Action actionUnderTest)
        {
            Contract.Requires(queryMethod != null);
            Contract.Requires(actionUnderTest != null);

            this.queryMethod = queryMethod;
            this.queryType = queryType;
            this.actionUnderTest = actionUnderTest;
        }

        public override Action Method
        {
            get { return queryMethod; }
        }

        public QueryType Type
        {
            get { return queryType; }
        }

        public Action ActionUnderTest
        {
            get { return actionUnderTest; }
        }
    }
}