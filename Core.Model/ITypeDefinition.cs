using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Cci.Contracts;

namespace Contractor.Core.Model
{
    [ContractClass(typeof (ITypeDefinitionContracts))]
    public interface ITypeDefinition
    {
        string Name { get; }
        ISet<Action> Constructors();
        ISet<Action> Actions();
        ITypeContract TypeContract();
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

        public ITypeContract TypeContract()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}