using System;
using System.Collections.Generic;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Action = Contractor.Core.Model.Action;

namespace Core.Tests
{
    internal class StringTypeDefinition : TypeDefinition
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

        public override IMethodContract GetContractFor(IMethodDefinition method)
        {
            throw new NotImplementedException();
        }
    }
}