using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Contractor.Core.Model
{
    [ContractClass(typeof(IEpaBuilderContracts))]
    public interface IEpaBuilder
    {
        IReadOnlyCollection<State> States { get; }
        IReadOnlyCollection<Transition> Transitions { get; }
        TypeDefinition Type { get; }
        void Add(Transition t);
        Epa Build();
    }

    [ContractClassFor(typeof (IEpaBuilder))]
    public abstract class IEpaBuilderContracts : IEpaBuilder
    {
        public IReadOnlyCollection<State> States
        {
            get { throw new System.NotImplementedException(); }
        }

        public IReadOnlyCollection<Transition> Transitions
        {
            get { throw new System.NotImplementedException(); }
        }

        public TypeDefinition Type
        {
            get { throw new System.NotImplementedException(); }
        }

        public void Add(Transition t)
        {
            Contract.Requires(t != null);
            throw new System.NotImplementedException();
        }

        public Epa Build()
        {
            throw new System.NotImplementedException();
        }
    }
}