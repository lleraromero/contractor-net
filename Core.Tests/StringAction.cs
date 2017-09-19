using System;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Action = Contractor.Core.Model.Action;

namespace Core.Tests
{
    internal class StringAction : Action
    {
        protected string name;

        public StringAction(string name)
        {
            this.name = name;
        }

        public override string Name
        {
            get { return name; }
        }

        public override IMethodDefinition Method
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override IMethodContract Contract
        {
            get { throw new NotImplementedException(); }
            set { throw new NotImplementedException(); }
        }

        public override bool IsPure
        {
            get { throw new NotImplementedException(); }
        }

        public override string ToString()
        {
            return name;
        }

        #region IEquatable

        public override bool Equals(Action other)
        {
            return base.Equals((object) other) && Equals((StringAction) other);
        }

        public bool Equals(StringAction other)
        {
            return name.Equals(other.name);
        }

        public override int GetHashCode()
        {
            return name.GetHashCode();
        }

        #endregion
    }
}