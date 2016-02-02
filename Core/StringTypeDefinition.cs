using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class StringTypeDefinition : ITypeDefinition
    {
        protected string name;

        public StringTypeDefinition(string name)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));

            this.name = name;
        }

        public string Name
        {
            get { return name; }
        }

        public ISet<Action> Constructors()
        {
            throw new NotSupportedException();
        }

        public ISet<Action> Actions()
        {
            throw new NotSupportedException();
        }

        public Microsoft.Cci.Contracts.IMethodContract GetContractFor(Microsoft.Cci.IMethodDefinition method)
        {
            throw new NotSupportedException();
        }
    }
}
