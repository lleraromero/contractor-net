using Contractor.Utils;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    // Immutable representation of an EPA state
    public interface IState
    {
        uint Id { get; }
        bool IsInitial { get; }
        string Name { get; }
        List<string> EnabledActions { get; }
        List<string> DisabledActions { get; }
    }

    public class State : IState, IEquatable<State>
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
                                                       : "empty";
            }
        }
        public SortedSet<IMethodDefinition> EnabledActions { get; private set; }
        public SortedSet<IMethodDefinition> DisabledActions { get; private set; }

        public State()
        {
            this.Id = uint.MaxValue;
            this.IsInitial = false;
            this.EnabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
            this.DisabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
        }

        public override string ToString()
        {
            return this.UniqueName;
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

        List<string> IState.EnabledActions
        {
            get { return (from a in this.EnabledActions select Utils.Extensions.GetDisplayName(a)).ToList(); }
        }

        List<string> IState.DisabledActions
        {
            get { return (from a in this.DisabledActions select Utils.Extensions.GetDisplayName(a)).ToList(); }
        }
        #endregion

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