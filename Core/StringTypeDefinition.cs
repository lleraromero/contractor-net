using System;
using System.Collections.Generic;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    class StringTypeDefinition : TypeDefinition
    {
        protected string name;
        protected ISet<Action> constructors;
        protected ISet<Action> actions;

        public StringTypeDefinition(string name, ISet<Action> constructors, ISet<Action> actions)
        {
            this.name = name;
            this.constructors = constructors;
            this.actions = actions;
        }

        public override string Name
        {
            get { return name; }
        }

        public override ISet<Action> Constructors()
        {
            return constructors;
        }

        public override ISet<Action> Actions()
        {
            return actions;
        }

        public override Microsoft.Cci.Contracts.IMethodContract GetContractFor(Microsoft.Cci.IMethodDefinition method)
        {
            throw new NotImplementedException();
        }
    }
}
