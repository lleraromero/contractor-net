using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Collections;
using System.Runtime;
using System.Threading;
using System.Runtime.Serialization;
using System.Diagnostics.Contracts;

namespace API_Examples
{
    #region ThrowHelper

    internal static class ThrowHelper
    {
        internal static void ThrowWrongKeyTypeArgumentException(object key, Type targetType)
        {
            throw new ArgumentException("Arg_WrongType", "key");
        }

        internal static void ThrowWrongValueTypeArgumentException(object value, Type targetType)
        {
            throw new ArgumentException("Arg_WrongType", "value");
        }

        internal static void ThrowKeyNotFoundException()
        {
            throw new KeyNotFoundException();
        }

        internal static void ThrowArgumentException(ExceptionResource resource)
        {
            throw new ArgumentException("");
        }

        internal static void ThrowArgumentNullException(ExceptionArgument argument)
        {
            throw new ArgumentNullException(ThrowHelper.GetArgumentName(argument));
        }

        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument)
        {
            throw new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument));
        }

        internal static void ThrowArgumentOutOfRangeException(ExceptionArgument argument, ExceptionResource resource)
        {
            throw new ArgumentOutOfRangeException(ThrowHelper.GetArgumentName(argument), "");
        }

        internal static void ThrowInvalidOperationException(ExceptionResource resource)
        {
            throw new InvalidOperationException("");
        }

        internal static void ThrowSerializationException(ExceptionResource resource)
        {
            throw new SerializationException("");
        }

        internal static void ThrowNotSupportedException(ExceptionResource resource)
        {
            throw new NotSupportedException("");
        }

        internal static void IfNullAndNullsAreIllegalThenThrow<T>(object value, ExceptionArgument argName)
        {
            if (value != null || (object)default(T) == null)
                return;
            ThrowHelper.ThrowArgumentNullException(argName);
        }

        internal static string GetArgumentName(ExceptionArgument argument)
        {
            switch (argument)
            {
                case ExceptionArgument.obj:
                    return "obj";
                case ExceptionArgument.dictionary:
                    return "dictionary";
                case ExceptionArgument.array:
                    return "array";
                case ExceptionArgument.info:
                    return "info";
                case ExceptionArgument.key:
                    return "key";
                case ExceptionArgument.collection:
                    return "collection";
                case ExceptionArgument.match:
                    return "match";
                case ExceptionArgument.converter:
                    return "converter";
                case ExceptionArgument.queue:
                    return "queue";
                case ExceptionArgument.SystemStack:
                    return "SystemStack";
                case ExceptionArgument.capacity:
                    return "capacity";
                case ExceptionArgument.index:
                    return "index";
                case ExceptionArgument.startIndex:
                    return "startIndex";
                case ExceptionArgument.value:
                    return "value";
                case ExceptionArgument.count:
                    return "count";
                case ExceptionArgument.arrayIndex:
                    return "arrayIndex";
                case ExceptionArgument.item:
                    return "item";
                default:
                    return string.Empty;
            }
        }

        internal static string GetResourceName(ExceptionResource resource)
        {
            switch (resource)
            {
                case ExceptionResource.Argument_ImplementIComparable:
                    return "Argument_ImplementIComparable";
                case ExceptionResource.ArgumentOutOfRange_NeedNonNegNum:
                    return "ArgumentOutOfRange_NeedNonNegNum";
                case ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired:
                    return "ArgumentOutOfRange_NeedNonNegNumRequired";
                case ExceptionResource.Arg_ArrayPlusOffTooSmall:
                    return "Arg_ArrayPlusOffTooSmall";
                case ExceptionResource.Argument_AddingDuplicate:
                    return "Argument_AddingDuplicate";
                case ExceptionResource.Serialization_InvalidOnDeser:
                    return "Serialization_InvalidOnDeser";
                case ExceptionResource.Serialization_MismatchedCount:
                    return "Serialization_MismatchedCount";
                case ExceptionResource.Serialization_MissingValues:
                    return "Serialization_MissingValues";
                case ExceptionResource.Arg_RankMultiDimNotSupported:
                    return "Arg_MultiRank";
                case ExceptionResource.Arg_NonZeroLowerBound:
                    return "Arg_NonZeroLowerBound";
                case ExceptionResource.Argument_InvalidArrayType:
                    return "Invalid_Array_Type";
                case ExceptionResource.NotSupported_KeyCollectionSet:
                    return "NotSupported_KeyCollectionSet";
                case ExceptionResource.ArgumentOutOfRange_SmallCapacity:
                    return "ArgumentOutOfRange_SmallCapacity";
                case ExceptionResource.ArgumentOutOfRange_Index:
                    return "ArgumentOutOfRange_Index";
                case ExceptionResource.Argument_InvalidOffLen:
                    return "Argument_InvalidOffLen";
                case ExceptionResource.InvalidOperation_CannotRemoveFromSystemStackOrQueue:
                    return "InvalidOperation_CannotRemoveFromSystemStackOrQueue";
                case ExceptionResource.InvalidOperation_EmptyCollection:
                    return "InvalidOperation_EmptyCollection";
                case ExceptionResource.InvalidOperation_EmptyQueue:
                    return "InvalidOperation_EmptyQueue";
                case ExceptionResource.InvalidOperation_EnumOpCantHappen:
                    return "InvalidOperation_EnumOpCantHappen";
                case ExceptionResource.InvalidOperation_EnumFailedVersion:
                    return "InvalidOperation_EnumFailedVersion";
                case ExceptionResource.InvalidOperation_EmptySystemStack:
                    return "InvalidOperation_EmptySystemStack";
                case ExceptionResource.InvalidOperation_EnumNotStarted:
                    return "InvalidOperation_EnumNotStarted";
                case ExceptionResource.InvalidOperation_EnumEnded:
                    return "InvalidOperation_EnumEnded";
                case ExceptionResource.NotSupported_SortedListNestedWrite:
                    return "NotSupported_SortedListNestedWrite";
                case ExceptionResource.NotSupported_ValueCollectionSet:
                    return "NotSupported_ValueCollectionSet";
                default:
                    return string.Empty;
            }
        }
    }
    #endregion

    #region Enums

    internal enum ExceptionArgument
    {
        obj,
        dictionary,
        array,
        info,
        key,
        collection,
        match,
        converter,
        queue,
        SystemStack,
        capacity,
        index,
        startIndex,
        value,
        count,
        arrayIndex,
        item,
    }

    internal enum ExceptionResource
    {
        Argument_ImplementIComparable,
        ArgumentOutOfRange_NeedNonNegNum,
        ArgumentOutOfRange_NeedNonNegNumRequired,
        Arg_ArrayPlusOffTooSmall,
        Argument_AddingDuplicate,
        Serialization_InvalidOnDeser,
        Serialization_MismatchedCount,
        Serialization_MissingValues,
        Arg_RankMultiDimNotSupported,
        Arg_NonZeroLowerBound,
        Argument_InvalidArrayType,
        NotSupported_KeyCollectionSet,
        ArgumentOutOfRange_SmallCapacity,
        ArgumentOutOfRange_Index,
        Argument_InvalidOffLen,
        NotSupported_ReadOnlyCollection,
        InvalidOperation_CannotRemoveFromSystemStackOrQueue,
        InvalidOperation_EmptyCollection,
        InvalidOperation_EmptyQueue,
        InvalidOperation_EnumOpCantHappen,
        InvalidOperation_EnumFailedVersion,
        InvalidOperation_EmptySystemStack,
        InvalidOperation_EnumNotStarted,
        InvalidOperation_EnumEnded,
        NotSupported_SortedListNestedWrite,
        NotSupported_ValueCollectionSet,
    }

    #endregion ExceptionArgument

    [Serializable]
    public class SystemStack<T> //: IEnumerable<T>, ICollection, IEnumerable
    {
        private static T[] _emptyArray = new T[0];
        private T[] _array;
        private int _size;
        private int _version;
        //[NonSerialized]
        //private object _syncRoot;
        private const int _defaultCapacity = 4;

        [ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(_array != null);
			Contract.Invariant(this.Count <= _array.Length);
			Contract.Invariant(this.Count >= 0);
			Contract.Invariant(this.Count == _size);
		}

        [Pure]
        public int Count
        {
            [TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
            get
            {
                //Contract.Ensures(Contract.Result<int>() == _size);
                return this._size;
            }
        }

        //bool ICollection.IsSynchronized
        //{
        //    get
        //    {
        //        return false;
        //    }
        //}

        //object ICollection.SyncRoot
        //{
        //    get
        //    {
        //        if (this._syncRoot == null)
        //            Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), (object)null);
        //        return this._syncRoot;
        //    }
        //}

        public SystemStack()
        {
            Contract.Ensures(((SystemStack<T>)this).Count == 0);
            this._array = SystemStack<T>._emptyArray;
            this._size = 0;
            this._version = 0;
        }

        //public SystemStack(int capacity)
        //{
        //    Contract.Requires(capacity >= 0);
        //    Contract.Ensures(((SystemStack<T>)this).Count == 0);
        //    if (capacity < 0)
        //        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired);
        //    this._array = new T[capacity];
        //    this._size = 0;
        //    this._version = 0;
        //}

        //public SystemStack(IEnumerable<T> collection)
        //{
        //    if (collection == null)
        //        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
        //    ICollection<T> collection1 = collection as ICollection<T>;
        //    if (collection1 != null)
        //    {
        //        int count = collection1.Count;
        //        this._array = new T[count];
        //        collection1.CopyTo(this._array, 0);
        //        this._size = count;
        //    }
        //    else
        //    {
        //        this._size = 0;
        //        this._array = new T[4];
        //        foreach (T obj in collection)
        //            this.Push(obj);
        //    }
        //}

        public void Clear()
        {
            Contract.Ensures(((SystemStack<T>)this).Count == 0);
            Array.Clear((Array)this._array, 0, this._size);
            this._size = 0;
            ++this._version;
        }

        public bool Contains(T item)
        {
            Contract.Ensures(!Contract.Result<bool>() || ((SystemStack<T>)this).Count > 0);
            int index = this._size;
            EqualityComparer<T> @default = EqualityComparer<T>.Default;
            while (index-- > 0)
            {
                if ((object)item == null)
                {
                    if ((object)this._array[index] == null)
                        return true;
                }
                else if ((object)this._array[index] != null && @default.Equals(this._array[index], item))
                    return true;
            }
            return false;
        }

        //public void CopyTo(T[] array, int arrayIndex)
        //{
        //    Contract.Requires(0 <= arrayIndex && arrayIndex < array.Length);
        //    Contract.Requires(array.Length - arrayIndex >= ((SystemStack<T>)this).Count);
        //    if (array == null)
        //        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
        //    if (arrayIndex < 0 || arrayIndex > array.Length)
        //        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        //    if (array.Length - arrayIndex < this._size)
        //        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
        //    Array.Copy((Array)this._array, 0, (Array)array, arrayIndex, this._size);
        //    Array.Reverse((Array)array, arrayIndex, this._size);
        //}

        //void ICollection.CopyTo(Array array, int arrayIndex)
        //{
        //    if (array == null)
        //        ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
        //    if (array.Rank != 1)
        //        ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
        //    if (array.GetLowerBound(0) != 0)
        //        ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
        //    if (arrayIndex < 0 || arrayIndex > array.Length)
        //        ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_NeedNonNegNum);
        //    if (array.Length - arrayIndex < this._size)
        //        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
        //    try
        //    {
        //        Array.Copy((Array)this._array, 0, array, arrayIndex, this._size);
        //        Array.Reverse(array, arrayIndex, this._size);
        //    }
        //    catch (ArrayTypeMismatchException ex)
        //    {
        //        ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
        //    }
        //}

        //public SystemStack<T>.Enumerator GetEnumerator()
        //{
        //    return new SystemStack<T>.Enumerator(this);
        //}

        //IEnumerator<T> IEnumerable<T>.GetEnumerator()
        //{
        //    return (IEnumerator<T>)new SystemStack<T>.Enumerator(this);
        //}

        //IEnumerator IEnumerable.GetEnumerator()
        //{
        //    return (IEnumerator)new SystemStack<T>.Enumerator(this);
        //}

        //public void TrimExcess()
        //{
        //    if (this._size >= (int)((double)this._array.Length * 0.9))
        //        return;
        //    T[] objArray = new T[this._size];
        //    Array.Copy((Array)this._array, 0, (Array)objArray, 0, this._size);
        //    this._array = objArray;
        //    ++this._version;
        //}

        //public T Peek()
        //{
        //    Contract.Requires(((SystemStack<T>)this).Count > 0);
        //    if (this._size == 0)
        //        ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptySystemStack);
        //    return this._array[this._size - 1];
        //}

        public T Pop()
        {
            Contract.Requires(((SystemStack<T>)this).Count > 0);
            Contract.Ensures(((SystemStack<T>)this).Count == Contract.OldValue<int>(((SystemStack<T>)this).Count) - 1);
            if (this._size == 0)
                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptySystemStack);
            ++this._version;
            T obj = this._array[--this._size];
            this._array[this._size] = default(T);
            return obj;
        }

        public void Push(T item)
        {
            Contract.Ensures(((SystemStack<T>)this).Count == Contract.OldValue<int>(((SystemStack<T>)this).Count) + 1);
            if (this._size == this._array.Length)
            {
                T[] objArray = new T[this._array.Length == 0 ? 4 : 2 * this._array.Length];
                Array.Copy((Array)this._array, 0, (Array)objArray, 0, this._size);
                this._array = objArray;
            }
            this._array[this._size++] = item;
            ++this._version;
        }

        //Resultado esperado: correct ensures
        //Resultado obtenido: unproven ensures
		//Accion esperada: Pop pertenece a A-
		//Accion obtenida: Pop no pertenece a A-
        private void dummy_ctor_Not_Pop()
        {
            Contract.Requires(this._array != null);
            Contract.Requires(this._size <= this._array.Length);
            Contract.Requires(this._size >= 0);
            Contract.Ensures(this.Count <= 0);
            this._array = SystemStack<T>._emptyArray;
            this._size = 0;
            Contract.Assume(this.Count == 0);
            this._version = 0;
        }

		//Resultado esperado: correct ensures
		//Resultado obtenido: unproven ensures
		//Accion esperada: no considerar el estado destino como inicial
		//Accion obtenida: considerar el estado destino como inicial
		private void dummy_ctor_get_CountPushPopClearContains()
		{
			Contract.Requires(this._array != null);
			Contract.Requires(this._size <= this._array.Length);
			Contract.Requires(this._size >= 0);
			Contract.Ensures(this._array != null);
			Contract.Ensures(this._size <= this._array.Length);
			Contract.Ensures(this._size >= 0);
			Contract.Ensures(this.Count <= 0);
			this._array = SystemStack<T>._emptyArray;
			this._size = 0;
			Contract.Assume(this.Count == 0);
			this._version = 0;
		}

        //Resultado esperado: false ensures
		//Resultado obtenido: correct ensures
        //Accion esperada: Pop no pertenece a A-
		//Accion obtenida: Pop pertenece a A-
        private int get_CountPushPopClearContains_get_Count_Not_Pop()
        {
            Contract.Requires(this._array != null);
            Contract.Requires(this._size <= this._array.Length);
            Contract.Requires(this._size >= 0);
            Contract.Requires(this.Count > 0);
            Contract.Ensures(this.Count <= 0);
            return this._size;
        }

        //Resultado esperado: false ensures
		//Resultado obtenido: correct ensures
        //Accion esperada: agregar transicion del estado a si mismo
		//Accion obtenida: no agregar transicion del estado a si mismo
        private int get_CountPushPopClearContains_get_Count_get_CountPushPopClearContains()
        {
            Contract.Requires(this._array != null);
            Contract.Requires(this._size <= this._array.Length);
            Contract.Requires(this._size >= 0);
            Contract.Requires(this.Count > 0);
            Contract.Ensures(this._array != null);
            Contract.Ensures(this._size <= this._array.Length);
            Contract.Ensures(this._size >= 0);
            Contract.Ensures(this.Count <= 0);
            return this._size;
        }

        //public T[] ToArray()
        //{
        //    Contract.Ensures(Contract.Result<T[]>() != null);
        //    T[] objArray = new T[this._size];
        //    for (int index = 0; index < this._size; ++index)
        //        objArray[index] = this._array[this._size - index - 1];
        //    return objArray;
        //}

        //[Serializable]
        //public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
        //{
        //    private SystemStack<T> _SystemStack;
        //    private int _index;
        //    private int _version;
        //    private T currentElement;

        //    public T Current
        //    {
        //        get
        //        {
        //            if (this._index == -2)
        //                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumNotStarted);
        //            if (this._index == -1)
        //                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumEnded);
        //            return this.currentElement;
        //        }
        //    }

        //    object IEnumerator.Current
        //    {
        //        get
        //        {
        //            if (this._index == -2)
        //                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumNotStarted);
        //            if (this._index == -1)
        //                ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumEnded);
        //            return (object)this.currentElement;
        //        }
        //    }

        //    internal Enumerator(SystemStack<T> SystemStack)
        //    {
        //        this._SystemStack = SystemStack;
        //        this._version = this._SystemStack._version;
        //        this._index = -2;
        //        this.currentElement = default(T);
        //    }

        //    public void Dispose()
        //    {
        //        this._index = -1;
        //    }

        //    public bool MoveNext()
        //    {
        //        if (this._version != this._SystemStack._version)
        //            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
        //        if (this._index == -2)
        //        {
        //            this._index = this._SystemStack._size - 1;
        //            bool flag = this._index >= 0;
        //            if (flag)
        //                this.currentElement = this._SystemStack._array[this._index];
        //            return flag;
        //        }
        //        else
        //        {
        //            if (this._index == -1)
        //                return false;
        //            bool flag = --this._index >= 0;
        //            this.currentElement = !flag ? default(T) : this._SystemStack._array[this._index];
        //            return flag;
        //        }
        //    }

        //    void IEnumerator.Reset()
        //    {
        //        if (this._version != this._SystemStack._version)
        //            ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
        //        this._index = -2;
        //        this.currentElement = default(T);
        //    }
        //}
    }
}
