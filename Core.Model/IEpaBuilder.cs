using System.Collections.Generic;

namespace Contractor.Core.Model
{
    public interface IEpaBuilder
    {
        IReadOnlyCollection<State> States { get; }
        IReadOnlyCollection<Transition> Transitions { get; }
        TypeDefinition Type { get; }
        void Add(State s);
        void Remove(State s);
        void Add(Transition t);
        void Remove(Transition t);
        void SetStateAsInitial(State initial);
        Epa Build();
    }
}