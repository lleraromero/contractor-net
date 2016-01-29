using System;
using System.Collections.Generic;
using Contractor.Core.Model;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using Action = Contractor.Core.Model.Action;
using ITypeDefinition = Contractor.Core.Model.ITypeDefinition;

namespace Core.Tests
{
    internal class StringTypeDefinition : ITypeDefinition
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

        public IMethodContract GetContractFor(IMethodDefinition method)
        {
            throw new NotImplementedException();
        }
    }
}