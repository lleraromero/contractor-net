using System.Collections.Generic;

namespace Contractor.Core.Model
{
    public interface IEpaBuilder
    {
        IReadOnlyCollection<State> States { get; }
        IReadOnlyCollection<Transition> Transitions { get; }
        TypeDefinition Type { get; }
        void Add(Transition t);
        void Add(State s);
        Epa Build();
    }
}