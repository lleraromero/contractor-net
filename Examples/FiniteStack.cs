using System.Diagnostics.Contracts;

namespace Examples
{
    public class FiniteStack
    {
        // TODO: CodeContracts demora mucho mas cuando se usan properties en vez de fields
        public int Max { get; private set; }
        public int Next { get; private set; }
        
        public FiniteStack()
        {
            Contract.Ensures(Max == 5 && Next == -1);

            Max = 5;
            Next = -1;
        }
        
        public FiniteStack(int size)
        {
            Contract.Requires(size > 2);
            Contract.Ensures(Max == size && Next == -1);

            Max = size;
            Next = -1;
        }
        
        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(Max > 2 && Next >= -1 && Max >= Next);
        }
        
        public void Pop()
        {
            Contract.Requires(Next > -1);
            Contract.Ensures(Next == Contract.OldValue<int>(Next) - 1);

            Next = Next - 1;
        }
        
        public void Push(int elem)
        {
            Contract.Requires(Next < Max);
            Contract.Ensures(Next == Contract.OldValue<int>(Next) + 1);

            Next = Next + 1;
        }
    }
}