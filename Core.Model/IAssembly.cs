using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Contractor.Core.Model
{
    [ContractClass(typeof (IAssemblyContracts))]
    public interface IAssembly
    {
        IReadOnlyCollection<NamespaceDefinition> Namespaces();
        IReadOnlyCollection<ITypeDefinition> Types();
    }

    #region IAssembly Contracts

    [ContractClassFor(typeof (IAssembly))]
    internal abstract class IAssemblyContracts : IAssembly
    {
        [Pure]
        public IReadOnlyCollection<NamespaceDefinition> Namespaces()
        {
            throw new NotImplementedException();
        }

        [Pure]
        public IReadOnlyCollection<ITypeDefinition> Types()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}