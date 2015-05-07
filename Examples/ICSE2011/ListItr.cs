using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NoSuchElementException = System.Exception;
using ConcurrentModificationException = System.Exception;
using IllegalStateException = System.Exception;
using System.Diagnostics.Contracts;

namespace Examples.ICSE2011
{
    public class ArrayListJava
    {
        // AbstractList
        public int modCount = 0;
        // AbtrasctList

        public Object[] elementData;
        public int size;

        //public ArrayListJava(int initialCapacity)
        //{
        //    if (initialCapacity < 0)
        //        throw new ArgumentException("Illegal Capacity: " + initialCapacity);
        //    this.elementData = new Object[initialCapacity];
        //}

        public ArrayListJava()
        {
            this.elementData = new Object[10];
            this.size = 0;
        }

        Object get(int index)
        {
            rangeCheck(index);

            return (Object)elementData[index];
        }

        public Object remove(int index)
        {
            rangeCheck(index);

            modCount++;
            Object oldValue = elementData[index];

            int numMoved = size - index - 1;
            if (numMoved > 0)
                Array.Copy(elementData, index + 1, elementData, index, numMoved);
            elementData[--size] = null; // Let gc do its work

            return oldValue;
        }

        public Object set(int index, Object element)
        {
            rangeCheck(index);

            Object oldValue = elementData[index];
            elementData[index] = element;
            return oldValue;
        }

        void ensureCapacity(int minCapacity)
        {
            modCount++;
            int oldCapacity = elementData.Length;
            if (minCapacity > oldCapacity)
            {
                Object[] oldData = elementData;
                int newCapacity = (oldCapacity * 3) / 2 + 1;
                if (newCapacity < minCapacity)
                    newCapacity = minCapacity;
                // minCapacity is usually close to size, so this is a win:
                Array.Resize(ref elementData, newCapacity);
                Contract.Assume(elementData.Length == newCapacity);
            }
        }

        public void add(int index, Object element)
        {
            rangeCheckForAdd(index);

            ensureCapacity(size + 1); // Increments modCount!!
            Array.Copy(elementData, index, elementData, index + 1, size - index);
            elementData[index] = element;
            size++;
        }

        private void rangeCheck(int index)
        {
            if (index >= size)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + size);
        }

        private void rangeCheckForAdd(int index)
        {
            if (index > size || index < 0)
                throw new IndexOutOfRangeException("Index: " + index + ", Size: " + size);
        }
    }

    /*
    The preconditions and invariants were extracted from ListItr.c
    */
    // TODO: add support for nested classes
    public class ListItr
    {
        /* Itr */
        public int cursor; // index of next element to return
        public int lastRet; // index of last element returned; -1 if no such
        public int expectedModCount;
        /* Itr */

        public ArrayListJava array;

        [ContractInvariantMethod]
        private void ObjectInvariant()
        {
            //Guido's Invariants
            Contract.Invariant(0 <= cursor && cursor <= array.size);
            Contract.Invariant(0 <= array.size && array.size <= array.elementData.Length);
            Contract.Invariant(-1 == lastRet                                    //haven't showed anything or is invalid
                                || (cursor < array.size && lastRet == cursor)   // going backwards
                                || (cursor > 0 && lastRet == cursor - 1));      // going forward
            Contract.Invariant(0 <= expectedModCount);
            Contract.Invariant(0 <= array.modCount);
            Contract.Invariant(10 <= array.elementData.Length);

            //New candidate
            //Contract.Invariant(lastRet == -1 || cursor - 1 <= lastRet);
            //Contract.Invariant(lastRet <= cursor);

            //Daikon's Invariants
            //Contract.Invariant(cursor >= 0);
            //Contract.Invariant(lastRet >= -1);
            ////Contract.Invariant(expectedModCount >= 0);
            //Contract.Invariant(cursor >= lastRet);
            ////Contract.Invariant(cursor <= expectedModCount);
            ////Contract.Invariant(lastRet < expectedModCount);


            //Contract.Invariant(this.array.size == this.array.elementData.Length); //Fix add elementData: this.outer.size == size(this.outer[])
            //Contract.Invariant(this.array != null);
            //Contract.Invariant(Contract.ForAll(this.array.elementData, x => x != null)); //Fix add elementData: this.outer[] elements != null
            //Contract.Invariant(this.array.elementData != null);
            //Contract.Invariant(this.array.elementData.Length == 10);
            //Contract.Invariant(this.array.elementData[this.array.size] == null);
            //Contract.Invariant(this.array.size < this.array.elementData.Length - 1);
        }


        public ListItr(ArrayListJava a, int index)
        {
            //Contract.Requires(a != null);
            //Contract.Requires(a.elementData != null);

            Contract.Requires(0 <= index && index <= a.size);
            Contract.Requires(0 <= a.size && a.size <= a.elementData.Length);
            Contract.Requires(0 <= a.modCount);
            Contract.Requires(10 <= a.elementData.Length);

            array = a;
            array.size = a.size;

            cursor = index;
            lastRet = -1;
            expectedModCount = a.modCount;
        }

        /* Itr */
        private void checkForComodification()
        {
            if (array.modCount != expectedModCount)
                throw new ConcurrentModificationException();
        }

        public bool hasNext()
        {
            return cursor != array.size;
        }

        //DIFF
        public Object next()
        {
            Contract.Requires(array.elementData != null);
            Contract.Requires(array.modCount == expectedModCount);
            Contract.Requires(0 <= cursor && cursor < array.size);

            checkForComodification();
            int i = cursor;
            if (i >= array.size)
            {
                throw new NoSuchElementException();
            }
            Object[] elementData = array.elementData;
            if (i >= elementData.Length)
            {
                throw new ConcurrentModificationException();
            }
            cursor = i + 1;
            return (Object)elementData[lastRet = i];
        }

        // DIFF
        public void remove()
        {
            Contract.Requires(array.elementData != null);
            Contract.Requires(lastRet != -1);
            Contract.Requires(array.modCount == expectedModCount);
            Contract.Requires(0 <= lastRet && lastRet < array.size);

            if (lastRet < 0)
            {
                throw new IllegalStateException();
            }
            checkForComodification();

            try
            {
                array.remove(lastRet);
                cursor = lastRet;
                lastRet = -1;
                expectedModCount = array.modCount;
            }
            catch (IndexOutOfRangeException)
            {
                throw new ConcurrentModificationException();
            }
        }
        /* Itr */

        public bool hasPrevious()
        {
            return cursor != 0;
        }

        // DIFF
        public object previous()
        {
            Contract.Requires(array.elementData != null);
            Contract.Requires(array.modCount == expectedModCount);
            Contract.Requires(0 <= cursor - 1 && cursor - 1 < array.size);

            checkForComodification();
            int i = cursor - 1;
            if (i < 0)
            {
                throw new NoSuchElementException();
            }
            Object[] elementData = array.elementData;
            if (i >= elementData.Length)
            {
                throw new ConcurrentModificationException();
            }
            cursor = i;
            return (Object)elementData[lastRet = i];
        }

        public int nextIndex()
        {
            return cursor;
        }

        public int previousIndex()
        {
            return cursor - 1;
        }

        // DIFF
        public void set(Object e)
        {
            Contract.Requires(array.elementData != null);
            Contract.Requires(lastRet != -1);
            Contract.Requires(array.modCount == expectedModCount);
            Contract.Requires(0 <= lastRet && lastRet < array.size);

            if (lastRet < 0)
            {
                throw new IllegalStateException();
            }
            checkForComodification();

            try
            {
                array.set(lastRet, e);
            }
            catch (IndexOutOfRangeException)
            {
                throw new ConcurrentModificationException();
            }
        }

        public void add(Object e)
        {
            Contract.Requires(array.elementData != null);
            Contract.Requires(array.modCount == expectedModCount);
            Contract.Requires(0 <= cursor && cursor <= array.size);

            checkForComodification();

            try
            {
                int i = cursor;
                array.add(i, e);
                cursor = i + 1;
                lastRet = -1;
                expectedModCount = array.modCount;
            }
            catch (IndexOutOfRangeException)
            {
                throw new ConcurrentModificationException();
            }
        }
    }
}