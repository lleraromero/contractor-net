﻿using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using Contractor.Core.Model;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    [ContractClass(typeof(IAssemblyXXXContracts))]
    public interface IAssemblyXXX
    {
        IReadOnlyCollection<NamespaceDefinition> Namespaces();
        IReadOnlyCollection<TypeDefinition> Types();
        ISet<Action> Constructors(TypeDefinition type);
        ISet<Action> Actions(TypeDefinition type);
        IMethodContract GetContractFor(IMethodDefinition method);
    }

    #region IAssemblyXXX Contracts
    [ContractClassFor(typeof(IAssemblyXXX))]
    abstract class IAssemblyXXXContracts : IAssemblyXXX
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
        public ISet<Action> Constructors(TypeDefinition type)
        {
            Contract.Requires(type != null);
            Contract.Requires(Types().Contains(type));
            throw new NotImplementedException();
        }

        [Pure]
        public ISet<Action> Actions(TypeDefinition type)
        {
            Contract.Requires(type != null);
            Contract.Requires(Types().Contains(type));
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
