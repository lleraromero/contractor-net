using Contractor.Utils;
using Microsoft.Cci;
using System;

namespace Contractor.Core
{
    public abstract class Action
    {
        public abstract string Name { get; }
        public abstract IMethodDefinition Method { get; }
    }

    public class CciAction : Action
    {
        protected IMethodDefinition method;
        public override string Name
        {
            get { return method.Name.Value; }
        }

        public override IMethodDefinition Method
        {
            get { return method; }
        }

        public CciAction(IMethodDefinition method)
        {
            this.method = method;
        }

        public override string ToString()
        {
            return method.GetUniqueName();
        }
    }

    public class StringAction : Action
    {
        protected string name;
        protected IMethodDefinition method;
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

        public override string ToString()
        {
            return this.name;
        }
    }
}
