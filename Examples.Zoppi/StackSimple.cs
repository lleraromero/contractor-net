using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    // Tambien funciona con un array generico de tipo T
    // y usando elems.Length en lugar de capacity
    public class StackSimple
    {
		public int capacity;
        public object[] elems;
		public int count;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(elems != null);
            Contract.Invariant(capacity > 1);
			Contract.Invariant(count >= 0);
			Contract.Invariant(count <= capacity);
        }

		public StackSimple(int size)
        {
            Contract.Requires(size > 1);
			//Contract.Ensures(count == 0);

            capacity = size;
            elems = new object[capacity];
			count = 0;
        }

        public void Push(object elem)
        {
			Contract.Requires(count < capacity);
			//Contract.Ensures(count == Contract.OldValue<int>(count) + 1);

            elems[count] = elem;
			count++;
        }

        public object Pop()
        {
			Contract.Requires(count > 0);
			//Contract.Ensures(count == Contract.OldValue<int>(count) - 1);

			count--;
            return elems[count];
        }
    }
}
