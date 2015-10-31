using Microsoft.Cci;
using Microsoft.Cci.Contracts;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using Action = Contractor.Core.Model.Action;

namespace Contractor.Core
{
    [ContractClass(typeof(IAssemblyXXXContracts))]
    public interface IAssemblyXXX
    {
        ISet<string> Types();
        ISet<Action> Constructors(string type);
        ISet<Action> Actions(string type);
        IMethodContract GetContractFor(IMethodDefinition method);
    }

    #region IAssemblyXXX Contracts
    [ContractClassFor(typeof(IAssemblyXXX))]
    abstract class IAssemblyXXXContracts : IAssemblyXXX
    {
        [Pure]
        public ISet<string> Types()
        {
            throw new NotImplementedException();
        }

        [Pure]
        public ISet<Action> Constructors(string typeName)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeName));
            Contract.Requires(Types().Contains(typeName));
            throw new NotImplementedException();
        }

        [Pure]
        public ISet<Action> Actions(string typeName)
        {
            Contract.Requires(!string.IsNullOrEmpty(typeName));
            Contract.Requires(Types().Contains(typeName));
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
