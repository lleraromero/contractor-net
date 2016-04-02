using System.Collections.Generic;
using System.Diagnostics.Contracts;

namespace Examples.Zoppi
{
    public class Stack<T>
    {
        public List<T> list;

        [ContractInvariantMethod]
        private void Invariant()
        {
			Contract.Invariant(list != null);
			Contract.Invariant(list.Count >= 0);
			Contract.Invariant(list.Count <= 5);
        }

        public Stack()
        {
            Contract.Requires(true);

            list = new List<T>();
        }

        public void Push(T obj)
        {
            Contract.Requires(list.Count < 5);

            list.Add(obj);
        }

        public T Pop()
        {
			Contract.Requires(list.Count > 0);
			//Contract.Ensures(list.Count == (Contract.OldValue<int>(list.Count) - 1));

            T obj = list[list.Count - 1];
            list.RemoveAt(list.Count - 1);
            return obj;
        }
    }
}
