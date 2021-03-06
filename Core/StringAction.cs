﻿using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using System;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class StringAction : Action
    {
        protected string name;
        public StringAction(string name)
        {
            System.Diagnostics.Contracts.Contract.Requires(!string.IsNullOrEmpty(name));

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

        public override IMethodContract Contract
        {
            get { throw new NotImplementedException(); }
        }

        public override bool IsPure
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
