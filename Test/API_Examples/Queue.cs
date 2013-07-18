using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Diagnostics;
using System.Collections;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Threading;

namespace API_Examples
{
	[Serializable, ComVisible(false), DebuggerDisplay("Count = {Count}")]
	public class Queue<T> : IEnumerable<T>, ICollection, IEnumerable
	{
		// Fields
		private T[] _array;
		private const int _DefaultCapacity = 4;
		private static T[] _emptyArray;
		private const int _GrowFactor = 200;
		private int _head;
		private const int _MinimumGrow = 4;
		private const int _ShrinkThreshold = 0x20;
		private int _size;
		[NonSerialized]
		private object _syncRoot;
		private int _tail;
		private int _version;

		[ContractInvariantMethod]
		private void Invariant()
		{
			Contract.Invariant(_array != null);
			Contract.Invariant(this.Count <= _array.Length);
			Contract.Invariant(this.Count >= 0);
			Contract.Invariant(this.Count == _size);
		}

		// Methods
		static Queue()
		{
			Queue<T>._emptyArray = new T[0];
		}

		public Queue()
		{
			Contract.Ensures(this.Count == 0);
			this._array = Queue<T>._emptyArray;
		}

		public Queue(IEnumerable<T> collection)
		{
			if (collection == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.collection);
			}
			this._array = new T[4];
			this._size = 0;
			this._version = 0;
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					this.Enqueue(enumerator.Current);
				}
			}
		}

		public Queue(int capacity)
		{
			Contract.Ensures(this.Count == 0);
			if (capacity < 0)
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.capacity, ExceptionResource.ArgumentOutOfRange_NeedNonNegNumRequired);
			}
			this._array = new T[capacity];
			this._head = 0;
			this._tail = 0;
			this._size = 0;
		}

		public void Clear()
		{
			Contract.Ensures(this.Count == 0);
			if (this._head < this._tail)
			{
				Array.Clear(this._array, this._head, this._size);
			}
			else
			{
				Array.Clear(this._array, this._head, this._array.Length - this._head);
				Array.Clear(this._array, 0, this._tail);
			}
			this._head = 0;
			this._tail = 0;
			this._size = 0;
			this._version++;
		}

		public bool Contains(T item)
		{
			int index = this._head;
			int num2 = this._size;
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			while (num2-- > 0)
			{
				if (item == null)
				{
					if (this._array[index] == null)
					{
						return true;
					}
				}
				else if ((this._array[index] != null) && comparer.Equals(this._array[index], item))
				{
					return true;
				}
				index = (index + 1) % this._array.Length;
			}
			return false;
		}

		public void CopyTo(T[] array, int arrayIndex)
		{
			Contract.Requires(array != null);
			Contract.Requires(arrayIndex >= 0);
			Contract.Requires((arrayIndex + this.Count) <= array.Length);
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			if ((arrayIndex < 0) || (arrayIndex > array.Length))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.arrayIndex, ExceptionResource.ArgumentOutOfRange_Index);
			}
			int length = array.Length;
			if ((length - arrayIndex) < this._size)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			}
			int num2 = ((length - arrayIndex) >= this._size) ? this._size : (length - arrayIndex);
			if (num2 != 0)
			{
				int num3 = ((this._array.Length - this._head) >= num2) ? num2 : (this._array.Length - this._head);
				Array.Copy(this._array, this._head, array, arrayIndex, num3);
				num2 -= num3;
				if (num2 > 0)
				{
					Array.Copy(this._array, 0, array, (arrayIndex + this._array.Length) - this._head, num2);
				}
			}
		}

		public T Dequeue()
		{
			Contract.Requires(this.Count > 0);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) - 1));
			if (this._size == 0)
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyQueue);
			}
			T local = this._array[this._head];
			this._array[this._head] = default(T);
			this._head = (this._head + 1) % this._array.Length;
			this._size--;
			this._version++;
			return local;
		}

		public void Enqueue(T item)
		{
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			if (this._size == this._array.Length)
			{
				int capacity = (this._array.Length * 200) / 100;
				if (capacity < (this._array.Length + 4))
				{
					capacity = this._array.Length + 4;
				}
				this.SetCapacity(capacity);
			}
			this._array[this._tail] = item;
			this._tail = (this._tail + 1) % this._array.Length;
			this._size++;
			this._version++;
		}

		internal T GetElement(int i)
		{
			return this._array[(this._head + i) % this._array.Length];
		}

		public T Peek()
		{
			Contract.Requires(this.Count > 0);
			if (this._size == 0)
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptyQueue);
			}
			return this._array[this._head];
		}

		private void SetCapacity(int capacity)
		{
			T[] destinationArray = new T[capacity];
			if (this._size > 0)
			{
				if (this._head < this._tail)
				{
					Array.Copy(this._array, this._head, destinationArray, 0, this._size);
				}
				else
				{
					Array.Copy(this._array, this._head, destinationArray, 0, this._array.Length - this._head);
					Array.Copy(this._array, 0, destinationArray, this._array.Length - this._head, this._tail);
				}
			}
			this._array = destinationArray;
			this._head = 0;
			this._tail = (this._size != capacity) ? this._size : 0;
			this._version++;
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				ThrowHelper.ThrowArgumentNullException(ExceptionArgument.array);
			}
			if (array.Rank != 1)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_RankMultiDimNotSupported);
			}
			if (array.GetLowerBound(0) != 0)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Arg_NonZeroLowerBound);
			}
			int length = array.Length;
			if ((index < 0) || (index > length))
			{
				ThrowHelper.ThrowArgumentOutOfRangeException(ExceptionArgument.index, ExceptionResource.ArgumentOutOfRange_Index);
			}
			if ((length - index) < this._size)
			{
				ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidOffLen);
			}
			int num2 = ((length - index) >= this._size) ? this._size : (length - index);
			if (num2 != 0)
			{
				try
				{
					int num3 = ((this._array.Length - this._head) >= num2) ? num2 : (this._array.Length - this._head);
					Array.Copy(this._array, this._head, array, index, num3);
					num2 -= num3;
					if (num2 > 0)
					{
						Array.Copy(this._array, 0, array, (index + this._array.Length) - this._head, num2);
					}
				}
				catch (ArrayTypeMismatchException)
				{
					ThrowHelper.ThrowArgumentException(ExceptionResource.Argument_InvalidArrayType);
				}
			}
		}

		public T[] ToArray()
		{
			Contract.Ensures(Contract.Result<T[]>() != null);
			T[] destinationArray = new T[this._size];
			if (this._size != 0)
			{
				if (this._head < this._tail)
				{
					Array.Copy(this._array, this._head, destinationArray, 0, this._size);
					return destinationArray;
				}
				Array.Copy(this._array, this._head, destinationArray, 0, this._array.Length - this._head);
				Array.Copy(this._array, 0, destinationArray, this._array.Length - this._head, this._tail);
			}
			return destinationArray;
		}

		public void TrimExcess()
		{
			if (this._size < ((int)(this._array.Length * 0.9)))
			{
				this.SetCapacity(this._size);
			}
		}

		// Properties
		[Pure]
		public int Count
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this._size;
			}
		}

		bool ICollection.IsSynchronized
		{
			get
			{
				return false;
			}
		}

		object ICollection.SyncRoot
		{
			get
			{
				if (this._syncRoot == null)
				{
					Interlocked.CompareExchange<object>(ref this._syncRoot, new object(), null);
				}
				return this._syncRoot;
			}
		}

		// Nested Types
		[Serializable, StructLayout(LayoutKind.Sequential)]
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator
		{
			private Queue<T> _q;
			private int _index;
			private int _version;
			private T _currentElement;
			internal Enumerator(Queue<T> q)
			{
				this._q = q;
				this._version = this._q._version;
				this._index = -1;
				this._currentElement = default(T);
			}

			public void Dispose()
			{
				this._index = -2;
				this._currentElement = default(T);
			}

			public bool MoveNext()
			{
				if (this._version != this._q._version)
				{
					ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
				}
				if (this._index == -2)
				{
					return false;
				}
				this._index++;
				if (this._index == this._q._size)
				{
					this._index = -2;
					this._currentElement = default(T);
					return false;
				}
				this._currentElement = this._q.GetElement(this._index);
				return true;
			}

			public T Current
			{
				get
				{
					if (this._index < 0)
					{
						if (this._index == -1)
						{
							ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumNotStarted);
						}
						else
						{
							ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumEnded);
						}
					}
					return this._currentElement;
				}
			}
			object IEnumerator.Current
			{
				get
				{
					if (this._index < 0)
					{
						if (this._index == -1)
						{
							ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumNotStarted);
						}
						else
						{
							ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumEnded);
						}
					}
					return this._currentElement;
				}
			}
			void IEnumerator.Reset()
			{
				if (this._version != this._q._version)
				{
					ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumFailedVersion);
				}
				this._index = -1;
				this._currentElement = default(T);
			}
		}

		IEnumerator<T> IEnumerable<T>.GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}
}
