using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;

namespace Contractor.Core.Model
{
    [ContractClass(typeof (ITypeDefinitionContracts))]
    public interface ITypeDefinition
    {
        string Name { get; }
        ISet<Action> Constructors();
        ISet<Action> Actions();
        IMethodContract GetContractFor(IMethodDefinition method);
    }

    #region ITypeDefinition Contracts

    [ContractClassFor(typeof (ITypeDefinition))]
    public abstract class ITypeDefinitionContracts : ITypeDefinition
    {
        public string Name
        {
            get { throw new NotImplementedException(); }
        }

        public ISet<Action> Constructors()
        {
            throw new NotImplementedException();
        }

        public ISet<Action> Actions()
        {
            throw new NotImplementedException();
        }

        public IMethodContract GetContractFor(IMethodDefinition method)
        {
            Contract.Requires(method != null);
            throw new NotImplementedException();
        }
    }

    #endregion
}