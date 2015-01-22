using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Examples.ICSE2011
{
    public class LinkedList
    {
        public Queue<int> buffer { get; private set; }
        public int size { get; private set; }

        public LinkedList()
        {
            Contract.Ensures(buffer != null);

            buffer = new Queue<int>();
            size = 0;
        }

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(buffer == null || size >= 0);
        }

        public void Add(int e)
        {
            Contract.Requires(buffer != null);

            buffer.Enqueue(e);
            size++;
        }

        public void Remove()
        {
            Contract.Requires(buffer != null && size > 0);

            Contract.Assume(buffer.Count == size);
            buffer.Dequeue();
            size--;
        }

        public void Destroy()
        {
            Contract.Requires(buffer != null);

            buffer.Clear();
            buffer = null;
            size = -1;
        }
    }
}