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
        protected ISet<Action> constructors;
        protected ISet<Action> actions;

        public StringTypeDefinition(string name, ISet<Action> constructors, ISet<Action> actions)
        {
            Contract.Requires(!string.IsNullOrEmpty(name));
            Contract.Requires(constructors != null);
            Contract.Requires(actions != null);

            this.name = name;
            this.constructors = constructors;
            this.actions = actions;
        }

        public string Name
        {
            get { return name; }
        }

        public ISet<Action> Constructors()
        {
            return constructors;
        }

        public ISet<Action> Actions()
        {
            return actions;
        }

        public Microsoft.Cci.Contracts.IMethodContract GetContractFor(Microsoft.Cci.IMethodDefinition method)
        {
            throw new NotImplementedException();
        }
    }
}
