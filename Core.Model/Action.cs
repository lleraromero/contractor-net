using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using System;

namespace Contractor.Core.Model
{
    public abstract class Action : IEquatable<Action>
    {
        public abstract string Name { get; }
        public abstract IMethodDefinition Method { get; set; }
        public abstract IMethodContract Contract { get; set; }
        public abstract bool IsPure { get; }

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
}
