using System.Collections.Generic;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;

namespace Contractor.Core.Model
{
    public abstract class TypeDefinition
    {
        public abstract string Name { get; }
        public abstract ISet<Action> Constructors();
        public abstract ISet<Action> Actions();
        public abstract IMethodContract GetContractFor(IMethodDefinition method);
    }
}
