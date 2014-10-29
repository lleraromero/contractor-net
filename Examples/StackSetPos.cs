using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace wp
{
    public class StackSetPos<T>
    {
        private const int capacity = 5;
        private int size;
        private T[] data;
        //public T item;

        [ContractInvariantMethod]
        private void Invariant()
        {
            Contract.Invariant(size >= 0);
            Contract.Invariant(size <= capacity);
            Contract.Invariant(data.Length == capacity);
            //Contract.Invariant(NotFull == (Size!=Capacity));
            //Contract.Invariant(Empty == (Size==0) );
            //Contract.Invariant(size == Size);
            //Contract.Invariant(capacity == Capacity);
            //INV DEBERIA TENER QUE LOS ELEM NO SE REPITEN
        }

        public StackSetPos()
        {

            //MUTANT 8: data = new T[capacity - 1];
            //MUTANT 9: size = -1;

            size = 0;
            data = new T[capacity];
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

        //[Pure]
        //public int Size2()
        //{
        //        Contract.Ensures(Contract.Result<int>() == size);
        //        return size;
        //}



        [Pure]
        public int Size
        {
            get
            {
                //Contract.Ensures(Contract.Result<int>() == size);
                return size;
            }
        }
        
        public void Push(T item)
        {
            Contract.Requires(Size < Capacity);
            //Contract.Requires(true && (Size < Capacity) && (Size > 0));
            //Contract.Ensures(true && !(Size < Capacity) && (Size > 0));
            //Contract.Ensures(size > 0 && size <= capacity);
            //Contract.Ensures((size == Contract.OldValue(((StackSetPos<T>)this).size + 1)));
            //((IArray)this).Count == Contract.OldValue(((IArray)this).Count) + 1
            //Contract.Requires(true && Size < Capacity && (Size > 0));
            //Contract.Ensures(true && !(Size < Capacity) && (Size > 0));

            //Contract.Ensures(Belongs(item));

            //Contract.Ensures(!Contract.OldValue<StackSetPos<T>>(this).belongs(item) && Size == Contract.OldValue<StackSetPos<T>>(this).Size + 1 && belongs(item));
            //Contract.Ensures((!Contract.OldValue<StackSetPos<T>>(this).belongs(item) && Size == Contract.OldValue<StackSetPos<T>>(this).Size + 1 && belongs(item)) || (Contract.OldValue<StackSetPos<T>>(this).belongs(item) && Size == (Contract.OldValue<StackSetPos<T>>(this).Size )));
            //Contract.Assume(!Contract.Exists<T>(data, e => e.Equals(item)));
            //bool result = !belongs(item);
            //Contract.Assert(result);
            //for (int i = 0; !result && i < size; i++)
            //{
            //    result = data[i].Equals(item);
            //}

            // Contract.Assume(result == true);
            
            //if(result) 

            //MUTANT 17: if (Belongs(item))
            //MUTANT 18: if (!Belongs(null))

            if (!Belongs(item))
            //if (Size==1)
            {
                data[size] = item;
                size++;
            }
            //else
            //{
            //    data[size++] = item;
            //}
            //Contract.Assume(Belongs(item));
        }

        public T Pop()
        {
            Contract.Requires(Size > 0);
            //Contract.Requires(true && (Size < Capacity) && (Size > 0));
            //Contract.Ensures(true && (Size < Capacity) && !(Size > 0));


            //Contract.Ensures(Contract.OldValue<StackSetPos<T>>(this).Size==Size+1);
            //Contract.Requires(true && (Size < Capacity) && (Size > 0));
            // Contract.Ensures(true && Size < Capacity && !(Size > 0));

            //MUTANT 22: --size; return 0;  
            return data[--size];
        }

        [Pure]
        public bool Belongs(T item)
        {
            //Contract.Requires(true && !(size < data.Length) && (size > 0));
            //Contract.Ensures(true && !(size < data.Length) && (size > 0));

            //Contract.Ensures(Contract.Result<bool>() == data.Contains<T>(item));
            //Contract.Ensures(Contract.Result<bool>() == Contract.Exists<T>(data, e => e.Equals(item)));

            //MUTANT 15:for (int i = 0; !!!result && i<size ;i++ )
            //MUTANT 16:for (int i = 0; !result;i++ )

            bool result = false;
            for (int i = 0; !result && i < size; i++)
            {
                result = data[i].Equals(item);
            }
            //Contract.Assume(result == Contract.Exists<T>(data, e => e.Equals(item)));
            return result;
            //return item!=null;
        }



        //public T pop3to2()
        //{
        //    //Contract.Requires(stack != null);

        //    Contract.Requires(true && Size2() < Capacity && (Size2() > 0));
        //    Contract.Ensures(true && (Size2() < Capacity) && !(Size2() > 0));
        //    return data[--size];
        //}

        //public float m()
        //{
        //    return 1/size;
        //}

        //public float m2()
        //{
        //    return 1/Size;
        //}

        

        //[Pure]
        //public bool NotFull
        //{
        //    get
        //    {
        //        if (Size == Capacity)
        //            return false;
        //        else
        //            return true;
        //    }
        //}

        //[Pure]
        //public bool NotFull2
        //{
        //    get
        //    {
        //        if (size * (2+10) < capacity *(3*4))
        //            return true;
        //        else
        //            return false;
        //    }
        //}

        //[Pure]
        //public bool Empty
        //{
        //    get
        //    {
        //        return Size == 0;
        //    }
        //}
        //public float m3()
        //{
        //    if (NotFull)
        //    {
        //        return 1 / Size;
        //    }
        //    else
        //    {
        //        return 0;
        //    }

        //}

        //public void add_inl(T item)
        //{
        //    Contract.Ensures(Belongs(item));
        //    bool result = false;
        //    for (int i = 0; !result && i < size; i++)
        //    {
        //        result = data[i].Equals(item);
        //    }
        //    if (!result)
        //    {
        //        float r = 1 / Size;
        //        data[size] = item;
        //        size++;
        //    }
        //}


        //public float m4(T item)
        //{
        //    //this.item = item;
        //    //Contract.Requires(Belongs(item));
        //    //Contract.Ensures((Contract.Result<float>() == 0 && !Belongs(item)) || !Belongs(item) && Contract.Result<float>() >0);
        //    if (Belongs2)
        //    {
        //        return 1 / Size;
        //    }
        //    else
        //    {
        //        return 0;
        //    }
        //}

        //[Pure]
        //public bool Belongs2
        //{
        //    get
        //    {
        //        bool result = false;
        //        for (int i = 0; !result && i < size; i++)
        //        {
        //            result = data[i].Equals(item);
        //        }
        //        return result;
        //    }
        //}



    }
}
