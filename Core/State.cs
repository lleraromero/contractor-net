using Contractor.Utils;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    public class StringAction : Action
    {
        protected string name;
        public override string Name
        {
            get { return name; }
        }

        protected IMethodDefinition method;
        public override IMethodDefinition Method
        {
            get { throw new NotImplementedException(); }
        }

        public StringAction(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.name;
        }
    }

    public class CciAction : Action
    {
        public override string Name
        {
            get { return method.Name.Value; }
        }

        protected IMethodDefinition method;
        public override IMethodDefinition Method
        {
            get { return method; }
        }

        public CciAction(IMethodDefinition method)
        {
            this.method = method;
        }

        public override string ToString()
        {
            return method.GetUniqueName();
        }
    }

    public abstract class Action
    {
        public abstract string Name { get; }
        public abstract IMethodDefinition Method { get; }
    }

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

        protected uint id;
        protected SortedSet<Action> enabledActions;
        protected SortedSet<Action> disabledActions;

        public uint Id { get { return id; } set { id = value; } }
        public SortedSet<Action> EnabledActions { get { return enabledActions; } set { enabledActions = value; } }
        public SortedSet<Action> DisabledActions { get { return disabledActions; } set { disabledActions = value; } }

        public State()
        {
            this.id = uint.MaxValue;
            this.EnabledActions = new SortedSet<Action>(new NamedEntityComparer());
            this.DisabledActions = new SortedSet<Action>(new NamedEntityComparer());
        }

        public string UniqueName
        {
            get
            {
                return (this.EnabledActions.Count > 0) ? string.Join(methodNameDelimiter, from a in this.EnabledActions select a.Method.GetUniqueName())
                                                       : "deadlock";
            }
        }

        public string Name
        {
            get { return this.UniqueName; }
        }

        public override string ToString()
        {
            return (this.EnabledActions.Count > 0) ? string.Join(Environment.NewLine, from a in this.EnabledActions select a.Method.GetDisplayName())
                                                       : "{no methods}";
        }

        #region IEquatable
        public bool Equals(State other)
        {
            return this.UniqueName.Equals(other.UniqueName);
        }

        public override bool Equals(object obj)
        {
            // Again just optimization
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Actually check the type, should not throw exception from Equals override
            if (obj.GetType() != this.GetType()) return false;

            // Call the implementation from IEquatable
            return Equals((State)obj);
        }

        public override int GetHashCode()
        {
            return this.UniqueName.GetHashCode();
        }
        #endregion
    }
}