using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Diagnostics.Contracts;

namespace API_Examples
{
    public class StackInforme<T>
    {
        public const int capacity = 5;
        public int count;
        private T[] data;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(count >= 0);
            Contract.Invariant(count <= capacity);
        }

        public StackInforme()
        {
            count = 0;
            data = new T[capacity];
        }
        public void Push(T item)
        {
            Contract.Requires(count < capacity);
            data[count++] = item;
        }
        public T Pop()
        {
            Contract.Requires(count > 0);
            return data[--count];
        }
    }
}
