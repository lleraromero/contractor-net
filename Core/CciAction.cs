using Microsoft.Cci;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Contractor.Utils;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
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
}
