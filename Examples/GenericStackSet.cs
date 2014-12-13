using System.Diagnostics.Contracts;

namespace Examples
{
    public class GenericStackSet<T>
    {
        private const int capacity = 5;
        private int size;
        private T[] data;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(size >= 0);
            Contract.Invariant(size <= capacity);
            Contract.Invariant(data.Length == capacity);
        }

        public GenericStackSet()
        {
            size = 0;
            data = new T[capacity];
        }

        [Pure]
        public int Capacity { get { return capacity; } }

        [Pure]
        public int Size { get { return size; } }

        public void Push(T item)
        {
            Contract.Requires(Size < Capacity);

            if (!Belongs(item))
            {
                data[size] = item;
                size++;
            }
        }

        public T Pop()
        {
            Contract.Requires(Size > 0);

            return data[--size];
        }

        [Pure]
        public bool Belongs(T item)
        {
            bool result = false;
            for (int i = 0; !result && i < size; i++)
            {
                result = data[i].Equals(item);
            }
            return result;
        }
    }
}
