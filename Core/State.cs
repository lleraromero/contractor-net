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

        public StringAction(string name)
        {
            this.name = name;
        }

        public override string ToString()
        {
            return this.name;
        }
    }

    public abstract class Action
    {
        public abstract string Name { get; }
    }

    // Immutable representation of an EPA state
    public interface IState
    {
        uint Id { get; }
        bool IsInitial { get; }
        string Name { get; }
        List<Action> EnabledActions { get; }
        List<Action> DisabledActions { get; }
    }

    public class CciState : IState, IEquatable<CciState>
    {
        private class NamedEntityComparer : Comparer<IMethodDefinition>
        {
            public override int Compare(IMethodDefinition x, IMethodDefinition y)
            {
                return x.GetUniqueName().CompareTo(y.GetUniqueName());
            }
        }

        private const string methodNameDelimiter = "$";

        public uint Id { get; set; }
        public bool IsInitial { get; set; }
        public string UniqueName
        {
            get
            {
                return (this.EnabledActions.Count > 0) ? string.Join(methodNameDelimiter, from a in this.EnabledActions select a.GetUniqueName())
                                                       : "deadlock";
            }
        }
        public SortedSet<IMethodDefinition> EnabledActions { get; private set; }
        public SortedSet<IMethodDefinition> DisabledActions { get; private set; }

        public CciState()
        {
            this.Id = uint.MaxValue;
            this.IsInitial = false;
            this.EnabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
            this.DisabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
        }

        public override string ToString()
        {
            return (this.EnabledActions.Count > 0) ? string.Join(Environment.NewLine, from a in this.EnabledActions select a.GetDisplayName())
                                                       : "{no methods}";
        }

        #region IState members
        uint IState.Id
        {
            get { return this.Id; }
        }

        bool IState.IsInitial
        {
            get { return this.IsInitial; }
        }

        string IState.Name
        {
            get { return this.UniqueName; }
        }

        List<Action> IState.EnabledActions
        {
            get { return (from a in this.EnabledActions select (Action)new StringAction(Utils.Extensions.GetUniqueName(a))).ToList(); }
        }

        List<Action> IState.DisabledActions
        {
            get { return (from a in this.DisabledActions select (Action)new StringAction(Utils.Extensions.GetUniqueName(a))).ToList(); }
        }
        #endregion

        #region IEquatable
        public bool Equals(CciState other)
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
            return Equals((CciState)obj);
        }

        public override int GetHashCode()
        {
            return this.UniqueName.GetHashCode();
        }
        #endregion
    }
}