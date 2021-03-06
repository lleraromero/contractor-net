﻿using System.Diagnostics.Contracts;

namespace Examples.Zoppi
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
            Contract.Invariant(data.Length == capacity);
        }

        public StackInforme()
        {
            count = 0;
            data = new T[capacity];
        }
        public int Count
        {
            get { return count; }
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
