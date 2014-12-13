using System.Diagnostics.Contracts;

namespace Examples
{
    public class GenericStack<T>
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

        public GenericStack()
        {
            size = 0;
            data = new T[capacity];
        }

        public int Size { get { return size; } }

        [Pure]
        public int Capacity { get { return capacity; } }

        public void Push(T item)
        {
            Contract.Requires(Size < Capacity);

            data[size++] = item;
        }

        public T Pop()
        {
            Contract.Requires(Size > 0);

            return data[--size];
        }

        public bool belongs(T item)
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
