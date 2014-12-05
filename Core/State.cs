using Contractor.Utils;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    // Immutable representation of an EPA state
    public class IState : State
    {
        public string Name
        {
            get { return base.UniqueName; }
        }

        public new List<string> EnabledActions
        {
            get { return (from a in base.EnabledActions select Utils.Extensions.GetDisplayName(a)).ToList(); }
        }

        public new List<string> DisabledActions
        {
            get { return (from a in base.DisabledActions select Utils.Extensions.GetDisplayName(a)).ToList(); }
        }
    }

    public class State : IEquatable<State>
    {
        private class NamedEntityComparer : Comparer<IMethodDefinition>
        {
            public override int Compare(IMethodDefinition x, IMethodDefinition y)
            {
                return x.GetUniqueName().CompareTo(y.GetUniqueName());
            }
        }

        private const string methodNameDelimiter = "$";

        public uint Id;
        public bool IsInitial;
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
            this.Id = 0;
            this.IsInitial = false;
            this.EnabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
            this.DisabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
        }

        public IState EPAState
        {
            get
            {
                var s = new IState();
                s.Id = this.Id;
                s.IsInitial = this.IsInitial;
                ((State)s).EnabledActions = new SortedSet<IMethodDefinition>(this.EnabledActions, new NamedEntityComparer());
                ((State)s).DisabledActions = new SortedSet<IMethodDefinition>(this.DisabledActions, new NamedEntityComparer());
                return s;
            }
        }

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

        public override string ToString()
        {
            return this.UniqueName;
        }
    }
}