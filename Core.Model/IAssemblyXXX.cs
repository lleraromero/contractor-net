﻿using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Microsoft.Cci;
using Microsoft.Cci.Contracts;

namespace Contractor.Core.Model
{
    [ContractClass(typeof (IAssemblyXXXContracts))]
    public interface IAssemblyXXX
    {
        IReadOnlyCollection<NamespaceDefinition> Namespaces();
        IReadOnlyCollection<TypeDefinition> Types();
        IMethodContract GetContractFor(IMethodDefinition method);
    }

    #region IAssemblyXXX Contracts

    [ContractClassFor(typeof (IAssemblyXXX))]
    internal abstract class IAssemblyXXXContracts : IAssemblyXXX
    {
        [Pure]
        public IReadOnlyCollection<NamespaceDefinition> Namespaces()
        {
            throw new NotImplementedException();
        }

        [Pure]
        public IReadOnlyCollection<TypeDefinition> Types()
        {
            throw new NotImplementedException();
        }

        [Pure]
        public IMethodContract GetContractFor(IMethodDefinition method)
        {
            Contract.Requires(method != null);
            Contract.Ensures(Contract.Result<IMethodContract>() != null);
            throw new NotImplementedException();
        }
    }

    #endregion
}