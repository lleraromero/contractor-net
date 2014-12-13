using System.Diagnostics.Contracts;

namespace Examples
{
    public class GenericStackFields<T>
    {
        public const int capacity = 5;
        public int size;
        public T[] data;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(size >= 0);
            Contract.Invariant(size <= capacity);
            Contract.Invariant(data.Length == capacity);
        }

        public GenericStackFields()
        {
            size = 0;
            data = new T[capacity];
        }

        public void Push(T item)
        {
            Contract.Requires(size < capacity);

            data[size++] = item;
        }

        public T Pop()
        {
            Contract.Requires(size > 0);

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
