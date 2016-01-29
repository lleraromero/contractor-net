using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Contractor.Core.Model
{
    [ContractClass(typeof (IEpaBuilderContracts))]
    public interface IEpaBuilder
    {
        IReadOnlyCollection<State> States { get; }
        IReadOnlyCollection<Transition> Transitions { get; }
        ITypeDefinition Type { get; }
        void Add(Transition t);
        Epa Build();
    }

    #region IEpaBuilder Contracts

    [ContractClassFor(typeof (IEpaBuilder))]
    public abstract class IEpaBuilderContracts : IEpaBuilder
    {
        public IReadOnlyCollection<State> States
        {
            get { throw new NotImplementedException(); }
        }

        public IReadOnlyCollection<Transition> Transitions
        {
            get { throw new NotImplementedException(); }
        }

        public ITypeDefinition Type
        {
            get { throw new NotImplementedException(); }
        }

        public void Add(Transition t)
        {
            Contract.Requires(t != null);
            throw new NotImplementedException();
        }

        public Epa Build()
        {
            throw new NotImplementedException();
        }
    }

    #endregion
}