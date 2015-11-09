namespace Contractor.Core.Model
{
    internal interface IEpaBuilder
    {
        void Add(State s);
        void Remove(State s);
        void Add(Transition t);
        void Remove(Transition t);
        Epa Build();
    }
}