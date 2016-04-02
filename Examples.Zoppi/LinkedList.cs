using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Runtime;
using System.Runtime.InteropServices;
using System.Runtime.Serialization;
using System.Security.Permissions;
using System.Threading;

namespace Examples.Zoppi
{
	[Serializable, DebuggerDisplay("Count = {Count}"), ComVisible(false)]
	public class LinkedList<T> : ICollection<T>, IEnumerable<T>, ICollection, IEnumerable, ISerializable, IDeserializationCallback
	{
		// Fields
		private object _syncRoot;
		internal int count;
		private const string CountName = "Count";
		internal LinkedListNode<T> head;
		private SerializationInfo siInfo;
		private const string ValuesName = "Data";
		internal int version;
		private const string VersionName = "Version";

		// Methods
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public LinkedList()
		{
			Contract.Ensures(this.Count == 0);
		}

		public LinkedList(IEnumerable<T> collection)
		{
			Contract.Requires(collection != null);
			if (collection == null)
			{
				throw new ArgumentNullException("collection");
			}
			using (IEnumerator<T> enumerator = collection.GetEnumerator())
			{
				while (enumerator.MoveNext())
				{
					this.AddLast(enumerator.Current);
				}
			}
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		protected LinkedList(SerializationInfo info, StreamingContext context)
		{
			this.siInfo = info;
		}

		public LinkedListNode<T> AddAfter(LinkedListNode<T> node, T value)
		{
			Contract.Requires(node != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			this.ValidateNode(node);
			LinkedListNode<T> newNode = new LinkedListNode<T>(node.list, value);
			this.InternalInsertNodeBefore(node.next, newNode);
			return newNode;
		}

		public void AddAfter(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			Contract.Requires(node != null);
			Contract.Requires(newNode != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			this.ValidateNode(node);
			this.ValidateNewNode(newNode);
			this.InternalInsertNodeBefore(node.next, newNode);
			newNode.list = (LinkedList<T>)this;
		}

		public void AddBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			Contract.Requires(node != null);
			Contract.Requires(newNode != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			this.ValidateNode(node);
			this.ValidateNewNode(newNode);
			this.InternalInsertNodeBefore(node, newNode);
			newNode.list = (LinkedList<T>)this;
			if (node == this.head)
			{
				this.head = newNode;
			}
		}

		public LinkedListNode<T> AddBefore(LinkedListNode<T> node, T value)
		{
			Contract.Requires(node != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			this.ValidateNode(node);
			LinkedListNode<T> newNode = new LinkedListNode<T>(node.list, value);
			this.InternalInsertNodeBefore(node, newNode);
			if (node == this.head)
			{
				this.head = newNode;
			}
			return newNode;
		}

		public void AddFirst(LinkedListNode<T> node)
		{
			Contract.Requires(node != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			this.ValidateNewNode(node);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(node);
			}
			else
			{
				this.InternalInsertNodeBefore(this.head, node);
				this.head = node;
			}
			node.list = (LinkedList<T>)this;
		}

		public LinkedListNode<T> AddFirst(T value)
		{
			Contract.Ensures(Contract.Result<LinkedListNode<T>>() != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			LinkedListNode<T> newNode = new LinkedListNode<T>((LinkedList<T>)this, value);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(newNode);
				return newNode;
			}
			this.InternalInsertNodeBefore(this.head, newNode);
			this.head = newNode;
			return newNode;
		}

		public LinkedListNode<T> AddLast(T value)
		{
			Contract.Ensures(Contract.Result<LinkedListNode<T>>() != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			LinkedListNode<T> newNode = new LinkedListNode<T>((LinkedList<T>)this, value);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(newNode);
				return newNode;
			}
			this.InternalInsertNodeBefore(this.head, newNode);
			return newNode;
		}

		public void AddLast(LinkedListNode<T> node)
		{
			Contract.Requires(node != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			this.ValidateNewNode(node);
			if (this.head == null)
			{
				this.InternalInsertNodeToEmptyList(node);
			}
			else
			{
				this.InternalInsertNodeBefore(this.head, node);
			}
			node.list = (LinkedList<T>)this;
		}

		public void Clear()
		{
			LinkedListNode<T> head = this.head;
			while (head != null)
			{
				LinkedListNode<T> node = head;
				head = head.Next;
				node.Invalidate();
			}
			this.head = null;
			this.count = 0;
			this.version++;
		}

		public bool Contains(T value)
		{
			return (this.Find(value) != null);
		}

		public void CopyTo(T[] array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if ((index < 0) || (index > array.Length))
			{
				object[] args = new object[] { index };
				throw new ArgumentOutOfRangeException("index", "IndexOutOfRange");
			}
			if ((array.Length - index) < this.Count)
			{
				throw new ArgumentException("Arg_InsufficientSpace");
			}
			LinkedListNode<T> head = this.head;
			if (head != null)
			{
				do
				{
					array[index++] = head.item;
					head = head.next;
				}
				while (head != this.head);
			}
		}

		public LinkedListNode<T> Find(T value)
		{
			LinkedListNode<T> head = this.head;
			EqualityComparer<T> comparer = EqualityComparer<T>.Default;
			if (head != null)
			{
				if (value != null)
				{
					do
					{
						if (comparer.Equals(head.item, value))
						{
							return head;
						}
						head = head.next;
					}
					while (head != this.head);
				}
				else
				{
					do
					{
						if (head.item == null)
						{
							return head;
						}
						head = head.next;
					}
					while (head != this.head);
				}
			}
			return null;
		}

		public LinkedListNode<T> FindLast(T value)
		{
			if (this.head != null)
			{
				LinkedListNode<T> prev = this.head.prev;
				LinkedListNode<T> node2 = prev;
				EqualityComparer<T> comparer = EqualityComparer<T>.Default;
				if (node2 != null)
				{
					if (value != null)
					{
						do
						{
							if (comparer.Equals(node2.item, value))
							{
								return node2;
							}
							node2 = node2.prev;
						}
						while (node2 != prev);
					}
					else
					{
						do
						{
							if (node2.item == null)
							{
								return node2;
							}
							node2 = node2.prev;
						}
						while (node2 != prev);
					}
				}
			}
			return null;
		}

		[SecurityPermission(SecurityAction.LinkDemand, Flags = SecurityPermissionFlag.SerializationFormatter)]
		public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
		{
			if (info == null)
			{
				throw new ArgumentNullException("info");
			}
			info.AddValue("Version", this.version);
			info.AddValue("Count", this.count);
			if (this.count != 0)
			{
				T[] array = new T[this.Count];
				this.CopyTo(array, 0);
				info.AddValue("Data", array, typeof(T[]));
			}
		}

		private void InternalInsertNodeBefore(LinkedListNode<T> node, LinkedListNode<T> newNode)
		{
			newNode.next = node;
			newNode.prev = node.prev;
			node.prev.next = newNode;
			node.prev = newNode;
			this.version++;
			this.count++;
		}

		private void InternalInsertNodeToEmptyList(LinkedListNode<T> newNode)
		{
			newNode.next = newNode;
			newNode.prev = newNode;
			this.head = newNode;
			this.version++;
			this.count++;
		}

		internal void InternalRemoveNode(LinkedListNode<T> node)
		{
			if (node.next == node)
			{
				this.head = null;
			}
			else
			{
				node.next.prev = node.prev;
				node.prev.next = node.next;
				if (this.head == node)
				{
					this.head = node.next;
				}
			}
			node.Invalidate();
			this.count--;
			this.version++;
		}

		public virtual void OnDeserialization(object sender)
		{
			if (this.siInfo != null)
			{
				int num = this.siInfo.GetInt32("Version");
				if (this.siInfo.GetInt32("Count") != 0)
				{
					T[] localArray = (T[])this.siInfo.GetValue("Data", typeof(T[]));
					if (localArray == null)
					{
						throw new SerializationException("Serialization_MissingValues");
					}
					for (int i = 0; i < localArray.Length; i++)
					{
						this.AddLast(localArray[i]);
					}
				}
				else
				{
					this.head = null;
				}
				this.version = num;
				this.siInfo = null;
			}
		}

		public bool Remove(T value)
		{
			Contract.Ensures(Contract.Result<bool>() ? (this.Count == (Contract.OldValue<int>(this.Count) - 1)) : true);
			LinkedListNode<T> node = this.Find(value);
			if (node != null)
			{
				this.InternalRemoveNode(node);
				return true;
			}
			return false;
		}

		public void Remove(LinkedListNode<T> node)
		{
			Contract.Requires(node != null);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) - 1));
			this.ValidateNode(node);
			this.InternalRemoveNode(node);
		}

		public void RemoveFirst()
		{
			Contract.Requires(this.Count > 0);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) - 1));
			if (this.head == null)
			{
				throw new InvalidOperationException("LinkedListEmpty");
			}
			this.InternalRemoveNode(this.head);
		}

		public void RemoveLast()
		{
			Contract.Requires(this.Count > 0);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) - 1));
			if (this.head == null)
			{
				throw new InvalidOperationException("LinkedListEmpty");
			}
			this.InternalRemoveNode(this.head.prev);
		}

		void ICollection<T>.Add(T value)
		{
			this.AddLast(value);
		}

		void ICollection.CopyTo(Array array, int index)
		{
			if (array == null)
			{
				throw new ArgumentNullException("array");
			}
			if (array.Rank != 1)
			{
				throw new ArgumentException("Arg_MultiRank");
			}
			if (array.GetLowerBound(0) != 0)
			{
				throw new ArgumentException("Arg_NonZeroLowerBound");
			}
			if (index < 0)
			{
				object[] args = new object[] { index };
				throw new ArgumentOutOfRangeException("index", "IndexOutOfRange");
			}
			if ((array.Length - index) < this.Count)
			{
				throw new ArgumentException("Arg_InsufficientSpace");
			}
			T[] localArray = array as T[];
			if (localArray != null)
			{
				this.CopyTo(localArray, index);
			}
			else
			{
				Type elementType = array.GetType().GetElementType();
				Type c = typeof(T);
				if (!elementType.IsAssignableFrom(c) && !c.IsAssignableFrom(elementType))
				{
					throw new ArgumentException("Invalid_Array_Type");
				}
				object[] objArray = array as object[];
				if (objArray == null)
				{
					throw new ArgumentException("Invalid_Array_Type");
				}
				LinkedListNode<T> head = this.head;
				try
				{
					if (head != null)
					{
						while (true)
						{
							objArray[index++] = head.item;
							head = head.next;
							if (head == this.head)
							{
								return;
							}
						}
					}
				}
				catch (ArrayTypeMismatchException)
				{
					throw new ArgumentException("Invalid_Array_Type");
				}
			}
		}

		internal void ValidateNewNode(LinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.list != null)
			{
				throw new InvalidOperationException("LinkedListNodeIsAttached");
			}
		}

		internal void ValidateNode(LinkedListNode<T> node)
		{
			if (node == null)
			{
				throw new ArgumentNullException("node");
			}
			if (node.list != this)
			{
				throw new InvalidOperationException("ExternalLinkedListNode");
			}
		}

		// Properties
		public int Count
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.count;
			}
		}

		public LinkedListNode<T> First
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				Contract.Ensures((this.Count == 0) ? (Contract.Result<LinkedListNode<T>>() == null) : true);
				Contract.Ensures((this.Count != 0) ? (Contract.Result<LinkedListNode<T>>() != null) : true);
				return this.head;
			}
		}

		public LinkedListNode<T> Last
		{
			get
			{
				Contract.Ensures((this.Count == 0) ? (Contract.Result<LinkedListNode<T>>() == null) : true);
				Contract.Ensures((this.Count != 0) ? (Contract.Result<LinkedListNode<T>>() != null) : true);
				if (this.head != null)
				{
					return this.head.prev;
				}
				return null;
			}
		}

		bool ICollection<T>.IsReadOnly
		{
			get
			{
				return false;
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
		public struct Enumerator : IEnumerator<T>, IDisposable, IEnumerator, ISerializable, IDeserializationCallback
		{
			private LinkedList<T> list;
			private LinkedListNode<T> node;
			private int version;
			private T current;
			private int index;
			private SerializationInfo siInfo;
			private const string LinkedListName = "LinkedList";
			private const string CurrentValueName = "Current";
			private const string VersionName = "Version";
			private const string IndexName = "Index";
			internal Enumerator(LinkedList<T> list)
			{
				this.list = list;
				this.version = list.version;
				this.node = list.head;
				this.current = default(T);
				this.index = 0;
				this.siInfo = null;
			}

			internal Enumerator(SerializationInfo info, StreamingContext context)
			{
				this.siInfo = info;
				this.list = null;
				this.version = 0;
				this.node = null;
				this.current = default(T);
				this.index = 0;
			}

			public T Current
			{
				[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
				get
				{
					return this.current;
				}
			}
			object IEnumerator.Current
			{
				get
				{
					if ((this.index == 0) || (this.index == (this.list.Count + 1)))
					{
						ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EnumOpCantHappen);
					}
					return this.current;
				}
			}
			public bool MoveNext()
			{
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				if (this.node == null)
				{
					this.index = this.list.Count + 1;
					return false;
				}
				this.index++;
				this.current = this.node.item;
				this.node = this.node.next;
				if (this.node == this.list.head)
				{
					this.node = null;
				}
				return true;
			}

			void IEnumerator.Reset()
			{
				if (this.version != this.list.version)
				{
					throw new InvalidOperationException("InvalidOperation_EnumFailedVersion");
				}
				this.current = default(T);
				this.node = this.list.head;
				this.index = 0;
			}

			public void Dispose()
			{
			}

			void ISerializable.GetObjectData(SerializationInfo info, StreamingContext context)
			{
				if (info == null)
				{
					throw new ArgumentNullException("info");
				}
				info.AddValue("LinkedList", this.list);
				info.AddValue("Version", this.version);
				info.AddValue("Current", this.current);
				info.AddValue("Index", this.index);
			}

			void IDeserializationCallback.OnDeserialization(object sender)
			{
				if (this.list == null)
				{
					if (this.siInfo == null)
					{
						throw new SerializationException("Serialization_InvalidOnDeser");
					}
					this.list = (LinkedList<T>)this.siInfo.GetValue("LinkedList", typeof(LinkedList<T>));
					this.version = this.siInfo.GetInt32("Version");
					this.current = (T)this.siInfo.GetValue("Current", typeof(T));
					this.index = this.siInfo.GetInt32("Index");
					if (this.list.siInfo != null)
					{
						this.list.OnDeserialization(sender);
					}
					if (this.index == (this.list.Count + 1))
					{
						this.node = null;
					}
					else
					{
						this.node = this.list.First;
						if ((this.node != null) && (this.index != 0))
						{
							for (int i = 0; i < this.index; i++)
							{
								this.node = this.node.next;
							}
							if (this.node == this.list.First)
							{
								this.node = null;
							}
						}
					}
					this.siInfo = null;
				}
			}
		}

		public IEnumerator<T> GetEnumerator()
		{
			throw new NotImplementedException();
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			throw new NotImplementedException();
		}
	}

	[ComVisible(false)]
	public sealed class LinkedListNode<T>
	{
		// Fields
		internal T item;
		internal LinkedList<T> list;
		internal LinkedListNode<T> next;
		internal LinkedListNode<T> prev;

		// Methods
		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		public LinkedListNode(T value)
		{
			this.item = value;
		}

		[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
		internal LinkedListNode(LinkedList<T> list, T value)
		{
			this.list = list;
			this.item = value;
		}

		internal void Invalidate()
		{
			this.list = null;
			this.next = null;
			this.prev = null;
		}

		// Properties
		public LinkedList<T> List
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.list;
			}
		}

		public LinkedListNode<T> Next
		{
			get
			{
				if ((this.next != null) && (this.next != this.list.head))
				{
					return this.next;
				}
				return null;
			}
		}

		public LinkedListNode<T> Previous
		{
			get
			{
				if ((this.prev != null) && (this != this.list.head))
				{
					return this.prev;
				}
				return null;
			}
		}

		public T Value
		{
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			get
			{
				return this.item;
			}
			[TargetedPatchingOptOut("Performance critical to inline this type of method across NGen image boundaries")]
			set
			{
				this.item = value;
			}
		}
	}
}
