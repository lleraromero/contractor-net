using Contractor.Utils;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    public class State : IEquatable<State>
    {
        private class NamedEntityComparer : Comparer<Action>
        {
            public override int Compare(Action x, Action y)
            {
                return x.Method.GetUniqueName().CompareTo(y.Method.GetUniqueName());
            }
        }

        private const string methodNameDelimiter = "$";

        protected SortedSet<Action> enabledActions;
        protected SortedSet<Action> disabledActions;

        public SortedSet<Action> EnabledActions { get { return enabledActions; } set { enabledActions = value; } }
        public SortedSet<Action> DisabledActions { get { return disabledActions; } set { disabledActions = value; } }

        public State()
        {
            this.EnabledActions = new SortedSet<Action>(new NamedEntityComparer());
            this.DisabledActions = new SortedSet<Action>(new NamedEntityComparer());
        }

        public string Name
        {
            get
            {
                return (this.EnabledActions.Count > 0) ? string.Join(methodNameDelimiter, from a in this.EnabledActions select a.Method.GetUniqueName())
                                                     : "deadlock";
            }
        }

        public override string ToString()
        {
            return (this.EnabledActions.Count > 0) ? string.Join(Environment.NewLine, from a in this.EnabledActions select a.Method.GetDisplayName())
                                                       : "{no methods}";
        }

        #region IEquatable
        public bool Equals(State other)
        {
            return this.Name.Equals(other.Name);
        }

        public override int GetHashCode()
        {
            return this.Name.GetHashCode();
        }
        #endregion

        //public static bool operator !=(State s1, State s2)
        //{
        //    return !s1.Equals(s2);
        //}

        //public static bool operator ==(State s1, State s2)
        //{
        //    return s1.Equals(s2);
        //}
    }
}