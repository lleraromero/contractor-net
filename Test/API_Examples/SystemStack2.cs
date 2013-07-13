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
	public class SystemStack<T>
	{
		// Fields
		private T[] _array;
		private const int _defaultCapacity = 4;
		private static T[] _emptyArray;
		private int _size;
		private int _version;

		// Methods
		[ContractInvariantMethod]
		private void InvariantMethod()
		{
			Contract.Invariant(this._array != null);
			Contract.Invariant(this._size <= this._array.Length);
			Contract.Invariant(this._size >= 0);
		}

		static SystemStack()
		{
			SystemStack<T>._emptyArray = new T[0];
		}

		public SystemStack()
		{
			Contract.Ensures(this.Count == 0);
			this._array = SystemStack<T>._emptyArray;
			this._size = 0;
			this._version = 0;
		}

		public void Clear()
		{
			Contract.Ensures(this.Count == 0);
			Array.Clear(this._array, 0, this._size);
			this._size = 0;
			this._version++;
		}

		public bool Contains(T item)
		{
			Contract.Ensures(Contract.Result<bool>() ? (this.Count > 0) : true);
			int index = this._size;
			EqualityComparer<T> @default = EqualityComparer<T>.Default;
			while (index-- > 0)
			{
				if (item == null)
				{
					if (this._array[index] == null)
					{
						return true;
					}
				}
				else if ((this._array[index] != null) && @default.Equals(this._array[index], item))
				{
					return true;
				}
			}
			return false;
		}

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

		public T Pop()
		{
			Contract.Requires(this.Count > 0);
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) - 1));
			if (this._size == 0)
			{
				ThrowHelper.ThrowInvalidOperationException(ExceptionResource.InvalidOperation_EmptySystemStack);
			}
			this._version++;
			T obj = this._array[--this._size];
			this._array[this._size] = default(T);
			return obj;
		}

		public void Push(T item)
		{
			Contract.Ensures(this.Count == (Contract.OldValue<int>(this.Count) + 1));
			if (this._size == this._array.Length)
			{
				T[] objArray = new T[(this._array.Length == 0) ? 4 : (2 * this._array.Length)];
				Array.Copy(this._array, 0, objArray, 0, this._size);
				this._array = objArray;
			}
			this._array[this._size++] = item;
			this._version++;
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
	}
}