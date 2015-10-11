using Microsoft.Cci;
using System;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class StringAction : Action
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
        }

        #region IEquatable
        public override bool Equals(Action other)
        {
            return base.Equals((object)other) && Equals((StringAction)other);
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

        public override string ToString()
        {
            return name;
        }
    }
}
