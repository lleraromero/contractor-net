using System;
using System.Diagnostics.Contracts;
using ConcurrentModificationException = System.Exception;
using IllegalStateException = System.Exception;
using NoSuchElementException = System.Exception;

namespace Examples.ICSE2011
{
    /// <summary>
    /// Java 1.6 implementation of ArrayList and ListItr
    /// </summary>
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
    The preconditions were extracted from ListItr.c
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
            Contract.Invariant(lastRet == -1 || cursor - 1 <= lastRet);
            Contract.Invariant(lastRet <= cursor);
        }


        public ListItr(ArrayListJava a, int index)
        {
            Contract.Requires(a != null);
            Contract.Requires(a.elementData != null);

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

        [Pure]
        public bool hasNext()
        {
            return cursor != array.size;
        }

        // DIFF with Java 1.4
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

        // DIFF with Java 1.4
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

        [Pure]
        public bool hasPrevious()
        {
            return cursor != 0;
        }

        // DIFF with Java 1.4
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

        [Pure]
        public int nextIndex()
        {
            return cursor;
        }

        [Pure]
        public int previousIndex()
        {
            return cursor - 1;
        }

        // DIFF with Java 1.4
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