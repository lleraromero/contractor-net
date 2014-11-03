using Contractor.Utils;
using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Contractor.Core
{
    class State : IState, IEquatable<State>
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
        public string UniqueName;
        public bool IsInitial;
        public SortedSet<IMethodDefinition> EnabledActions { get; private set; }
        public SortedSet<IMethodDefinition> DisabledActions { get; private set; }

        public State()
        {
            this.Id = 0;
            this.UniqueName = string.Empty;
            this.IsInitial = false;
            this.EnabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
            this.DisabledActions = new SortedSet<IMethodDefinition>(new NamedEntityComparer());
        }

        public void Sort()
        {
            if (this.EnabledActions.Count > 0)
            {
                this.UniqueName = string.Join(methodNameDelimiter, from a in this.EnabledActions select a.GetUniqueName());
            }
            else
            {
                this.UniqueName = "empty";
            }
        }

        public EpaState EpaState
        {
            get
            {
                var s = new EpaState(this.Id, this.UniqueName);
                s.EnabledActions.AddRange(from a in this.EnabledActions select a.GetUniqueName());
                s.DisabledActions.AddRange(from a in this.DisabledActions select a.GetUniqueName());
                return s;
            }
        }

        #region IState

        string IState.Name
        {
            get { return this.UniqueName; }
        }

        bool IState.IsInitial
        {
            get { return this.IsInitial; }
        }

        IEnumerable<string> IState.EnabledActions
        {
            get { return from a in this.EnabledActions select Utils.Extensions.GetDisplayName(a); }
        }

        IEnumerable<string> IState.DisabledActions
        {
            get { return from a in this.DisabledActions select Utils.Extensions.GetDisplayName(a); }
        }

        #endregion IState

        public bool Equals(State other)
        {
            return this.UniqueName.Equals(other.UniqueName);
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