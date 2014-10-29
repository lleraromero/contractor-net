using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp
{
    public class StackC<T>
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
            //Contract.Invariant(size==Size);
            //Contract.Invariant(capacity == Capacity);
        }

        public StackC()
        {
            size = 0;
            data = new T[capacity];
        }
       
        public int Size
        {
            get { return size; }
        }
        [Pure]
        public int Capacity
        {
            get
            {
                //Contract.Ensures(Contract.Result<int>() == capacity);
                return capacity;
            }
        }
        public void Push(T item)
        {
            Contract.Requires(size < capacity);
           // Contract.Ensures(((Stack<T>)this).Size == Contract.OldValue(((Stack<T>)this).Size + 1));
            //Contract.Requires(true && (Size < Capacity) && (Size > 0));
            //Contract.Ensures(true && (Size < Capacity) && (Size > 0));

            // MUTANT 1: data[size--] = item;
            // MUTANT 2: data[size] = item;
            // MUTANT 3: data[++size] = item;
            // MUTANT 19: data[size++] = null;
            data[size++] = item;
        }

        public T Pop()
        {
            Contract.Requires(size > 0);
            //Contract.Requires(true && !(Size < Capacity) && (Size > 0));
            //Contract.Ensures(true && Size < Capacity && (Size > 0));

            // MUTANT 4: return data[size--];
            // MUTANT 5: return data[size];
            // MUTANT 6: return data[--size];
            // MUTANT 7: return data[size - 1];
            // MUTANT 21: --size; return data[0]; 
            return data[--size];
        }

        public bool belongs(T item)
        {
            //Contract.Requires(true && (Size < Capacity) && (Size > 0));
            //Contract.Ensures(true && Size < Capacity && (Size > 0));

            //MUTANT bool result = true;

            //MUTANT 11: for (int i = 0; result && i<size ;i++ )
            //MUTANT 12: for (int i = 0; !result && i<size ;i-- )
            //MUTANT 13: for (int i = 0; !result && i<size ;i-- )
            //MUTANT 14: for (int i = 0; !result || i<size ;i++ )
            //MUTANT 20: bool result = true;

            bool result = false;
            for (int i = 0; !result && i<size ;i++ )
            {
                result = data[i].Equals(item);
            }
            return result;
        }
        /*
        public bool badBelongs(T item)
        {
            bool result = false;
            for (int i = 0; !result ; i++)
            {
                result = data[i].Equals(item);
            }
            return result;
        }
         */
    }
}
