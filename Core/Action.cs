using Contractor.Utils;
using Microsoft.Cci;
using System;

namespace Contractor.Core
{
    public abstract class Action : IEquatable<Action>
    {
        public abstract string Name { get; }
        public abstract IMethodDefinition Method { get; }
        #region IEquatable
        public override bool Equals(object obj)
        {
            // Again just optimization
            if (ReferenceEquals(null, obj)) return false;
            if (ReferenceEquals(this, obj)) return true;

            // Actually check the type, should not throw exception from Equals override
            if (obj.GetType() != this.GetType()) return false;

            return true;
        }
        public abstract bool Equals(Action other);
        public override int GetHashCode()
        {
            throw new NotSupportedException("This method should not be called");
        }
        #endregion
    }

    public class CciAction : Action
    {
        protected IMethodDefinition method;
        public override string Name
        {
            get { return method.GetUniqueName(); }
        }

        public override IMethodDefinition Method
        {
            get { return method; }
        }

        public CciAction(IMethodDefinition method)
        {
            this.method = method;
        }

        #region IEquatable
        public override bool Equals(Action other)
        {
            return base.Equals((object)other) && Equals((CciAction)other);
        }

        public bool Equals(CciAction other)
        {
            return method.Equals(other.method);
        }

        public override int GetHashCode()
        {
            return method.GetHashCode();
        }
        #endregion

        public override string ToString()
        {
            return method.GetDisplayName();
        }
    }

    public class StringAction : Action
    {
        protected string name;

        public override string Name
        {
            get { return name; }
        }

        public override IMethodDefinition Method
        {
            get { throw new NotImplementedException(); }
        }

        public StringAction(string name)
        {
            this.name = name;
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
            return this.name;
        }
    }
}
