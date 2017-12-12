type HeapType = [Ref][Field]Union;

procedure Alloc() returns (x: Ref);
  modifies $Alloc;



implementation {:ForceInline} Alloc() returns (x: Ref)
{

  anon0:
    assume ($Alloc[x] <==> false) && x != null;
    $Alloc[x] := true;
    return;
}



procedure System.Object.GetType(this: Ref) returns ($result: Ref);



axiom Union2Int(null) == 0;

axiom Union2Bool(null) <==> false;

function $ThreadDelegate(Ref) : Ref;

procedure System.Threading.Thread.#ctor$System.Threading.ParameterizedThreadStart(this: Ref, start$in: Ref);



procedure System.Threading.Thread.Start$System.Object(this: Ref, parameter$in: Ref);



procedure Wrapper_System.Threading.ParameterizedThreadStart.Invoke$System.Object(this: Ref, obj$in: Ref);



procedure {:extern} System.Threading.ParameterizedThreadStart.Invoke$System.Object(this: Ref, obj$in: Ref);



procedure System.Threading.Thread.#ctor$System.Threading.ThreadStart(this: Ref, start$in: Ref);



procedure System.Threading.Thread.Start(this: Ref);



procedure Wrapper_System.Threading.ThreadStart.Invoke(this: Ref);



procedure {:extern} System.Threading.ThreadStart.Invoke(this: Ref);



procedure System.String.op_Equality$System.String$System.String(a$in: Ref, b$in: Ref) returns ($result: bool);



procedure System.String.op_Inequality$System.String$System.String(a$in: Ref, b$in: Ref) returns ($result: bool);



var $Heap: HeapType;

function {:inline} Read(H: HeapType, o: Ref, f: Field) : Union
{
  H[o][f]
}

function {:inline} Write(H: HeapType, o: Ref, f: Field, v: Union) : HeapType
{
  H[o := H[o][f := v]]
}

var $ArrayContents: [Ref][int]Union;

function $ArrayLength(Ref) : int;

type Field;

type Union = Ref;

type Ref;

const unique null: Ref;

type Type = Ref;

function $TypeConstructor(Ref) : int;

type Real;

const unique $DefaultReal: Real;

procedure $BoxFromBool(b: bool) returns (r: Ref);



procedure $BoxFromInt(i: int) returns (r: Ref);



procedure $BoxFromReal(r: Real) returns (rf: Ref);



procedure $BoxFromUnion(u: Union) returns (r: Ref);



const unique $BoolValueType: int;

const unique $IntValueType: int;

const unique $RealValueType: int;

function {:inline} $Unbox2Bool(r: Ref) : bool
{
  Union2Bool(r)
}

function {:inline} $Unbox2Int(r: Ref) : int
{
  Union2Int(r)
}

function {:inline} $Unbox2Real(r: Ref) : Real
{
  Union2Real(r)
}

function {:inline} $Unbox2Union(r: Ref) : Union
{
  r
}

function Union2Bool(u: Union) : bool;

function Union2Int(u: Union) : int;

function Union2Real(u: Union) : Real;

function Bool2Union(boolValue: bool) : Union;

function Int2Union(intValue: int) : Union;

function Real2Union(realValue: Real) : Union;

function Int2Real(int) : Real;

function Real2Int(Real) : int;

function RealPlus(Real, Real) : Real;

function RealMinus(Real, Real) : Real;

function RealTimes(Real, Real) : Real;

function RealDivide(Real, Real) : Real;

function RealModulus(Real, Real) : Real;

function RealLessThan(Real, Real) : bool;

function RealLessThanOrEqual(Real, Real) : bool;

function RealGreaterThan(Real, Real) : bool;

function RealGreaterThanOrEqual(Real, Real) : bool;

function BitwiseAnd(int, int) : int;

function BitwiseOr(int, int) : int;

function BitwiseExclusiveOr(int, int) : int;

function BitwiseNegation(int) : int;

function RightShift(int, int) : int;

function LeftShift(int, int) : int;

function $DynamicType(Ref) : Type;

function $As(a: Ref, b: Type) : Ref;

axiom (forall a: Ref, b: Type :: { $As(a, b): Ref } $As(a, b): Ref == (if $Subtype($DynamicType(a), b) then a else null));

function $Subtype(Type, Type) : bool;

axiom !$Subtype(T$System.OverflowException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.OverflowException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.OverflowException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.OverflowException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.OverflowException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.OverflowException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.OverflowException(), T$System.ArgumentException());

axiom !$Subtype(T$System.OverflowException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.OverflowException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.OverflowException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.OverflowException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.OverflowException(), T$System.IO.IOException());

axiom !$Subtype(T$System.OverflowException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.OverflowException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.OverflowException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.OverflowException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.OverflowException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.OverflowException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.OverflowException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.OverflowException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.OverflowException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.OverflowException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.OverflowException(), T$System.RankException());

axiom !$Subtype(T$System.OverflowException(), T$System.TimeoutException());

axiom !$Subtype(T$System.OverflowException(), T$System.Exception());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.OverflowException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.IO.IOException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.RankException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IndexOutOfRangeException(), T$System.Exception());

axiom !$Subtype(T$System.NullReferenceException(), T$System.OverflowException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.ArgumentException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IO.IOException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.RankException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.TimeoutException());

axiom !$Subtype(T$System.NullReferenceException(), T$System.Exception());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.OverflowException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.ArgumentException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IO.IOException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.RankException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.TimeoutException());

axiom !$Subtype(T$System.DivideByZeroException(), T$System.Exception());

axiom !$Subtype(T$System.InvalidCastException(), T$System.OverflowException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.ArgumentException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IO.IOException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.RankException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.TimeoutException());

axiom !$Subtype(T$System.InvalidCastException(), T$System.Exception());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.OverflowException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.ArgumentException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IO.IOException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.RankException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.TimeoutException());

axiom !$Subtype(T$System.ArgumentNullException(), T$System.Exception());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.OverflowException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.ArgumentException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IO.IOException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.RankException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.TimeoutException());

axiom !$Subtype(T$System.ArgumentOutOfRangeException(), T$System.Exception());

axiom !$Subtype(T$System.ArgumentException(), T$System.OverflowException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.ArgumentException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.ArgumentException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.ArgumentException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.ArgumentException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.ArgumentException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IO.IOException());

axiom !$Subtype(T$System.ArgumentException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.ArgumentException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.ArgumentException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.ArgumentException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.ArgumentException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.ArgumentException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.ArgumentException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.ArgumentException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.ArgumentException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.ArgumentException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.ArgumentException(), T$System.RankException());

axiom !$Subtype(T$System.ArgumentException(), T$System.TimeoutException());

axiom !$Subtype(T$System.ArgumentException(), T$System.Exception());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.OverflowException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.IO.IOException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.RankException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IO.FileNotFoundException(), T$System.Exception());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.OverflowException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IO.IOException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.RankException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IO.DirectoryNotFoundException(), T$System.Exception());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.OverflowException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.IO.IOException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.RankException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IO.PathTooLongException(), T$System.Exception());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.OverflowException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.IO.IOException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.RankException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IO.EndOfStreamException(), T$System.Exception());

axiom !$Subtype(T$System.IO.IOException(), T$System.OverflowException());

axiom !$Subtype(T$System.IO.IOException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.IO.IOException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IO.IOException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IO.IOException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IO.IOException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IO.IOException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IO.IOException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IO.IOException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.IO.IOException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.IO.IOException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.IO.IOException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.IO.IOException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IO.IOException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IO.IOException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IO.IOException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.IO.IOException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IO.IOException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IO.IOException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IO.IOException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IO.IOException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IO.IOException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IO.IOException(), T$System.RankException());

axiom !$Subtype(T$System.IO.IOException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IO.IOException(), T$System.Exception());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.OverflowException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.ArgumentException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IO.IOException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.RankException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.TimeoutException());

axiom !$Subtype(T$System.ObjectDisposedException(), T$System.Exception());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.OverflowException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.ArgumentException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IO.IOException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.RankException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.TimeoutException());

axiom !$Subtype(T$System.InvalidOperationException(), T$System.Exception());

axiom !$Subtype(T$System.NotSupportedException(), T$System.OverflowException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.ArgumentException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IO.IOException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.RankException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.TimeoutException());

axiom !$Subtype(T$System.NotSupportedException(), T$System.Exception());

axiom !$Subtype(T$System.IllegalStateException(), T$System.OverflowException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.ArgumentException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.IO.IOException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.RankException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.TimeoutException());

axiom !$Subtype(T$System.IllegalStateException(), T$System.Exception());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.OverflowException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.ArgumentException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IO.IOException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.RankException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.TimeoutException());

axiom !$Subtype(T$System.ConcurrentModificationException(), T$System.Exception());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.OverflowException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.ArgumentException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IO.IOException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.RankException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.TimeoutException());

axiom !$Subtype(T$System.NoSuchElementException(), T$System.Exception());

axiom !$Subtype(T$System.AssertFailedException(), T$System.OverflowException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.ArgumentException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IO.IOException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.RankException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.TimeoutException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.Exception());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.OverflowException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.ArgumentException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IO.IOException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.RankException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.TimeoutException());

axiom !$Subtype(T$System.OperationCanceledException(), T$System.Exception());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.OverflowException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.ArgumentException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IO.IOException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.RankException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.TimeoutException());

axiom !$Subtype(T$System.UnauthorizedAccessException(), T$System.Exception());

axiom !$Subtype(T$System.NotImplementedException(), T$System.OverflowException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.ArgumentException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IO.IOException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.RankException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.TimeoutException());

axiom !$Subtype(T$System.NotImplementedException(), T$System.Exception());

axiom !$Subtype(T$System.RankException(), T$System.OverflowException());

axiom !$Subtype(T$System.RankException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.RankException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.RankException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.RankException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.RankException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.RankException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.RankException(), T$System.ArgumentException());

axiom !$Subtype(T$System.RankException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.RankException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.RankException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.RankException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.RankException(), T$System.IO.IOException());

axiom !$Subtype(T$System.RankException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.RankException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.RankException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.RankException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.RankException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.RankException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.RankException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.RankException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.RankException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.RankException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.RankException(), T$System.TimeoutException());

axiom !$Subtype(T$System.RankException(), T$System.Exception());

axiom !$Subtype(T$System.TimeoutException(), T$System.OverflowException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.TimeoutException(), T$System.NullReferenceException());

axiom !$Subtype(T$System.TimeoutException(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.TimeoutException(), T$System.InvalidCastException());

axiom !$Subtype(T$System.TimeoutException(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.TimeoutException(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.TimeoutException(), T$System.ArgumentException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IO.IOException());

axiom !$Subtype(T$System.TimeoutException(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.TimeoutException(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.TimeoutException(), T$System.NotSupportedException());

axiom !$Subtype(T$System.TimeoutException(), T$System.IllegalStateException());

axiom !$Subtype(T$System.TimeoutException(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.TimeoutException(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.TimeoutException(), T$System.AssertFailedException());

axiom !$Subtype(T$System.TimeoutException(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.TimeoutException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.TimeoutException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.TimeoutException(), T$System.RankException());

axiom !$Subtype(T$System.TimeoutException(), T$System.Exception());

axiom !$Subtype(T$System.Exception(), T$System.OverflowException());

axiom !$Subtype(T$System.Exception(), T$System.IndexOutOfRangeException());

axiom !$Subtype(T$System.Exception(), T$System.NullReferenceException());

axiom !$Subtype(T$System.Exception(), T$System.DivideByZeroException());

axiom !$Subtype(T$System.Exception(), T$System.InvalidCastException());

axiom !$Subtype(T$System.Exception(), T$System.ArgumentNullException());

axiom !$Subtype(T$System.Exception(), T$System.ArgumentOutOfRangeException());

axiom !$Subtype(T$System.Exception(), T$System.ArgumentException());

axiom !$Subtype(T$System.Exception(), T$System.IO.FileNotFoundException());

axiom !$Subtype(T$System.Exception(), T$System.IO.DirectoryNotFoundException());

axiom !$Subtype(T$System.Exception(), T$System.IO.PathTooLongException());

axiom !$Subtype(T$System.Exception(), T$System.IO.EndOfStreamException());

axiom !$Subtype(T$System.Exception(), T$System.IO.IOException());

axiom !$Subtype(T$System.Exception(), T$System.ObjectDisposedException());

axiom !$Subtype(T$System.Exception(), T$System.InvalidOperationException());

axiom !$Subtype(T$System.Exception(), T$System.NotSupportedException());

axiom !$Subtype(T$System.Exception(), T$System.IllegalStateException());

axiom !$Subtype(T$System.Exception(), T$System.ConcurrentModificationException());

axiom !$Subtype(T$System.Exception(), T$System.NoSuchElementException());

axiom !$Subtype(T$System.Exception(), T$System.AssertFailedException());

axiom !$Subtype(T$System.Exception(), T$System.OperationCanceledException());

axiom !$Subtype(T$System.Exception(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.Exception(), T$System.NotImplementedException());

axiom !$Subtype(T$System.Exception(), T$System.RankException());

axiom !$Subtype(T$System.Exception(), T$System.TimeoutException());

function {:extern} T$System.OverflowException() : Ref;

const {:extern} unique T$System.OverflowException: int;

function {:extern} T$System.IndexOutOfRangeException() : Ref;

const {:extern} unique T$System.IndexOutOfRangeException: int;

function {:extern} T$System.NullReferenceException() : Ref;

const {:extern} unique T$System.NullReferenceException: int;

function {:extern} T$System.DivideByZeroException() : Ref;

const {:extern} unique T$System.DivideByZeroException: int;

function {:extern} T$System.InvalidCastException() : Ref;

const {:extern} unique T$System.InvalidCastException: int;

function {:extern} T$System.ArgumentNullException() : Ref;

const {:extern} unique T$System.ArgumentNullException: int;

function {:extern} T$System.ArgumentOutOfRangeException() : Ref;

const {:extern} unique T$System.ArgumentOutOfRangeException: int;

function {:extern} T$System.ArgumentException() : Ref;

const {:extern} unique T$System.ArgumentException: int;

function {:extern} T$System.IO.FileNotFoundException() : Ref;

const {:extern} unique T$System.IO.FileNotFoundException: int;

function {:extern} T$System.IO.DirectoryNotFoundException() : Ref;

const {:extern} unique T$System.IO.DirectoryNotFoundException: int;

function {:extern} T$System.IO.PathTooLongException() : Ref;

const {:extern} unique T$System.IO.PathTooLongException: int;

function {:extern} T$System.IO.EndOfStreamException() : Ref;

const {:extern} unique T$System.IO.EndOfStreamException: int;

function {:extern} T$System.IO.IOException() : Ref;

const {:extern} unique T$System.IO.IOException: int;

function {:extern} T$System.ObjectDisposedException() : Ref;

const {:extern} unique T$System.ObjectDisposedException: int;

function {:extern} T$System.InvalidOperationException() : Ref;

const {:extern} unique T$System.InvalidOperationException: int;

function {:extern} T$System.NotSupportedException() : Ref;

const {:extern} unique T$System.NotSupportedException: int;

function {:extern} T$System.IllegalStateException() : Ref;

const {:extern} unique T$System.IllegalStateException: int;

function {:extern} T$System.ConcurrentModificationException() : Ref;

const {:extern} unique T$System.ConcurrentModificationException: int;

function {:extern} T$System.NoSuchElementException() : Ref;

const {:extern} unique T$System.NoSuchElementException: int;

function {:extern} T$System.AssertFailedException() : Ref;

const {:extern} unique T$System.AssertFailedException: int;

function {:extern} T$System.OperationCanceledException() : Ref;

const {:extern} unique T$System.OperationCanceledException: int;

function {:extern} T$System.UnauthorizedAccessException() : Ref;

const {:extern} unique T$System.UnauthorizedAccessException: int;

function {:extern} T$System.NotImplementedException() : Ref;

const {:extern} unique T$System.NotImplementedException: int;

function {:extern} T$System.RankException() : Ref;

const {:extern} unique T$System.RankException: int;

function {:extern} T$System.TimeoutException() : Ref;

const {:extern} unique T$System.TimeoutException: int;

function {:extern} T$System.Exception() : Ref;

const {:extern} unique T$System.Exception: int;

axiom $Subtype(T$System.OverflowException(), T$System.OverflowException());

axiom $Subtype(T$System.IndexOutOfRangeException(), T$System.IndexOutOfRangeException());

axiom $Subtype(T$System.NullReferenceException(), T$System.NullReferenceException());

axiom $Subtype(T$System.DivideByZeroException(), T$System.DivideByZeroException());

axiom $Subtype(T$System.InvalidCastException(), T$System.InvalidCastException());

axiom $Subtype(T$System.ArgumentNullException(), T$System.ArgumentNullException());

axiom $Subtype(T$System.ArgumentOutOfRangeException(), T$System.ArgumentOutOfRangeException());

axiom $Subtype(T$System.ArgumentException(), T$System.ArgumentException());

axiom $Subtype(T$System.IO.FileNotFoundException(), T$System.IO.FileNotFoundException());

axiom $Subtype(T$System.IO.DirectoryNotFoundException(), T$System.IO.DirectoryNotFoundException());

axiom $Subtype(T$System.IO.PathTooLongException(), T$System.IO.PathTooLongException());

axiom $Subtype(T$System.IO.EndOfStreamException(), T$System.IO.EndOfStreamException());

axiom $Subtype(T$System.IO.IOException(), T$System.IO.IOException());

axiom $Subtype(T$System.ObjectDisposedException(), T$System.ObjectDisposedException());

axiom $Subtype(T$System.InvalidOperationException(), T$System.InvalidOperationException());

axiom $Subtype(T$System.NotSupportedException(), T$System.NotSupportedException());

axiom $Subtype(T$System.IllegalStateException(), T$System.IllegalStateException());

axiom $Subtype(T$System.ConcurrentModificationException(), T$System.ConcurrentModificationException());

axiom $Subtype(T$System.NoSuchElementException(), T$System.NoSuchElementException());

axiom $Subtype(T$System.AssertFailedException(), T$System.AssertFailedException());

axiom $Subtype(T$System.OperationCanceledException(), T$System.OperationCanceledException());

axiom $Subtype(T$System.UnauthorizedAccessException(), T$System.UnauthorizedAccessException());

axiom $Subtype(T$System.NotImplementedException(), T$System.NotImplementedException());

axiom $Subtype(T$System.RankException(), T$System.RankException());

axiom $Subtype(T$System.TimeoutException(), T$System.TimeoutException());

axiom $Subtype(T$System.Exception(), T$System.Exception());

function $DisjointSubtree(Type, Type) : bool;

var $Alloc: [Ref]bool;

function {:builtin "MapImp"} $allocImp([Ref]bool, [Ref]bool) : [Ref]bool;

function {:builtin "MapConst"} $allocConstBool(bool) : [Ref]bool;

function $RefToDelegateMethod(int, Ref) : bool;

function $RefToDelegateReceiver(int, Ref) : Ref;

function $RefToDelegateTypeParameters(int, Ref) : Type;

var {:thread_local} $Exception: Ref;

const unique System.Collections.Generic2.ICollection: Ref;

procedure System.Collections.Generic2.ICollection`1.get_Count($this: Ref) returns ($result: int);



procedure System.Collections.Generic2.ICollection`1.get_IsReadOnly($this: Ref) returns ($result: bool);



procedure System.Collections.Generic2.ICollection`1.Add$`0($this: Ref, item$in: Ref);



procedure System.Collections.Generic2.ICollection`1.Clear($this: Ref);



procedure System.Collections.Generic2.ICollection`1.Contains$`0($this: Ref, item$in: Ref) returns ($result: bool);



procedure System.Collections.Generic2.ICollection`1.CopyTo$`0array$System.Int32($this: Ref, array$in: Ref, arrayIndex$in: int);



procedure System.Collections.Generic2.ICollection`1.Remove$`0($this: Ref, item$in: Ref) returns ($result: bool);



function T$T$System.Collections.Generic2.ICollectionContract`1(parent: Ref) : Ref;

function T$System.Collections.Generic2.ICollectionContract`1(T: Ref) : Ref;

const unique T$System.Collections.Generic2.ICollectionContract`1: int;

procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#get_Count($this: Ref) returns ($result: int);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#get_IsReadOnly($this: Ref) returns ($result: bool);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#Add$`0($this: Ref, item$in: Ref);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#Clear($this: Ref);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#Contains$`0($this: Ref, item$in: Ref) returns ($result: bool);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#CopyTo$`0array$System.Int32($this: Ref, array$in: Ref, arrayIndex$in: int);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic2#ICollection$T$#Remove$`0($this: Ref, item$in: Ref) returns ($result: bool);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#Generic#IEnumerable$T$#GetEnumerator($this: Ref) returns ($result: Ref);



procedure System.Collections.Generic2.ICollectionContract`1.System#Collections#IEnumerable#GetEnumerator($this: Ref) returns ($result: Ref);



procedure System.Collections.Generic2.ICollectionContract`1.#ctor($this: Ref);



procedure {:extern} System.Object.#ctor($this: Ref);



procedure T$System.Collections.Generic2.ICollectionContract`1.#cctor();



function T$System.Collections.Generic2.MyArray() : Ref;

const unique T$System.Collections.Generic2.MyArray: int;

var F$System.Collections.Generic2.MyArray.MaxArrayLength: int;

var F$System.Collections.Generic2.MyArray.MaxByteArrayLength: int;

procedure System.Collections.Generic2.MyArray.Copy$System.Objectarray$System.Int32$System.Objectarray$System.Int32$System.Int32(sourceArray$in: Ref, index1$in: int, destinationArray$in: Ref, index2$in: int, numMoved$in: int);



const {:value "sourceArray"} unique $string_literal_sourceArray_0: Ref;

procedure {:extern} System.ArgumentNullException.#ctor$System.String($this: Ref, paramName$in: Ref);



const {:value "destinationArray"} unique $string_literal_destinationArray_1: Ref;

const {:value "Array.Copy-Exception"} unique $string_literal_Array.Copy$Exception_2: Ref;

procedure {:extern} System.Exception.#ctor$System.String($this: Ref, message$in: Ref);



procedure System.Collections.Generic2.MyArray.Resize$System.Stringarray$$System.Int32(elementData$in: Ref, newCapacity$in: int) returns (elementData$out: Ref);



procedure {:extern} System.ArgumentOutOfRangeException.#ctor($this: Ref);



procedure System.Collections.Generic2.MyArray.#ctor($this: Ref);



procedure T$System.Collections.Generic2.MyArray.#cctor();



function T$System.Collections2.Stack() : Ref;

const unique T$System.Collections2.Stack: int;

const unique F$System.Collections2.Stack._array: Field;

const unique F$System.Collections2.Stack._size: Field;

const unique F$System.Collections2.Stack._version: Field;

const unique F$System.Collections2.Stack._syncRoot: Field;

var F$System.Collections2.Stack._defaultCapacity: int;

procedure System.Collections2.Stack.#ctor($this: Ref);



procedure System.Collections2.Stack.#ctor$System.Int32($this: Ref, initialCapacity$in: int);



const {:value "initialCapacity"} unique $string_literal_initialCapacity_3: Ref;

procedure {:extern} System.ArgumentOutOfRangeException.#ctor$System.String($this: Ref, paramName$in: Ref);



procedure System.Collections2.Stack.#ctor$System.Collections.ICollection($this: Ref, col$in: Ref);



procedure {:extern} System.Collections.ICollection.get_Count($this: Ref) returns ($result: int);



const {:value "col"} unique $string_literal_col_4: Ref;

procedure {:extern} System.Collections.IEnumerable.GetEnumerator($this: Ref) returns ($result: Ref);



procedure {:extern} System.Collections.IEnumerator.get_Current($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.Push$System.Object($this: Ref, obj$in: Ref);



procedure {:extern} System.Collections.IEnumerator.MoveNext($this: Ref) returns ($result: bool);



procedure System.Collections2.Stack.get_Count($this: Ref) returns ($result: int);



procedure System.Collections2.Stack.get_IsSynchronized($this: Ref) returns ($result: bool);



procedure System.Collections2.Stack.get_SyncRoot($this: Ref) returns ($result: Ref);



function {:extern} T$System.Object() : Ref;

const {:extern} unique T$System.Object: int;

procedure {:extern} System.Threading.Interlocked.CompareExchange``1$``0$$``0$``0(location1$in: Ref, value$in: Ref, comparand$in: Ref, T: Ref) returns (location1$out: Ref, $result: Ref);



procedure System.Collections2.Stack.Clear($this: Ref);



procedure {:extern} System.Array.Clear$System.Array$System.Int32$System.Int32(array$in: Ref, index$in: int, length$in: int);



procedure System.Collections2.Stack.Clone($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.Contains$System.Object($this: Ref, obj$in: Ref) returns ($result: bool);



procedure {:extern} System.Object.Equals$System.Object($this: Ref, obj$in: Ref) returns ($result: bool);



procedure System.Collections2.Stack.CopyTo$System.Array$System.Int32($this: Ref, array$in: Ref, index$in: int);



const {:value "array"} unique $string_literal_array_5: Ref;

procedure {:extern} System.Array.get_Rank($this: Ref) returns ($result: int);



procedure {:extern} System.ArgumentException.#ctor($this: Ref);



const {:value "index"} unique $string_literal_index_6: Ref;

procedure {:extern} System.Array.get_Length($this: Ref) returns ($result: int);



function {:extern} T$System.Objectarray() : Ref;

const {:extern} unique T$System.Objectarray: int;

procedure {:extern} System.Array.SetValue$System.Object$System.Int32($this: Ref, value$in: Ref, index$in: int);



procedure System.Collections2.Stack.GetEnumerator($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.StackEnumerator.#ctor$System.Collections2.Stack($this: Ref, stack$in: Ref);



function T$System.Collections2.Stack.StackEnumerator() : Ref;

const unique T$System.Collections2.Stack.StackEnumerator: int;

procedure System.Collections2.Stack.Peek($this: Ref) returns ($result: Ref);



procedure {:extern} System.InvalidOperationException.#ctor($this: Ref);



procedure System.Collections2.Stack.Pop($this: Ref) returns ($result: Ref);



procedure {:extern} System.Array.Copy$System.Array$System.Int32$System.Array$System.Int32$System.Int32(sourceArray$in: Ref, sourceIndex$in: int, destinationArray$in: Ref, destinationIndex$in: int, length$in: int);



procedure System.Collections2.Stack.Synchronized$System.Collections2.Stack(stack$in: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.SyncStack.#ctor$System.Collections2.Stack($this: Ref, stack$in: Ref);



function T$System.Collections2.Stack.SyncStack() : Ref;

const unique T$System.Collections2.Stack.SyncStack: int;

procedure System.Collections2.Stack.ToArray($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_get_Count($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_get_IsSynchronized($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_get_SyncRoot($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_Clear($this: Ref) returns ($result: Ref);
  modifies $Alloc;



implementation System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_Clear($this: Ref) returns ($result: Ref)
{
  var local_0_Ref: Ref;
  var $tmp0: Ref;
  var $tmp1: Ref;
  var $tmp2: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 32} true;
    local_0_Ref := null;
    assume $this != null;
    assume !(Union2Int(Read($Heap, $this, F$System.Collections2.Stack._size)) < 0);
    assume $this != null;
    assume Read($Heap, $this, F$System.Collections2.Stack._array) > null;
    assume $this != null;
    goto anon5_Then, anon5_Else;

  anon5_Then:
    assume {:partition} Read($Heap, $this, F$System.Collections2.Stack._syncRoot) == null;
    assume $this != null;
    $tmp0 := Read($Heap, $this, F$System.Collections2.Stack._syncRoot);
    call {:si_unique_call 0} $tmp1 := Alloc();
    call {:si_unique_call 1} System.Object.#ctor($tmp1);
    assume $DynamicType($tmp1) == T$System.Object();
    assume $TypeConstructor($DynamicType($tmp1)) == T$System.Object;
    call {:si_unique_call 2} $tmp0, $tmp2 := System.Threading.Interlocked.CompareExchange``1$``0$$``0$``0($tmp0, $tmp1, null, T$System.Object());
    goto anon6_Then, anon6_Else;

  anon6_Then:
    assume {:partition} $Exception != null;
    return;

  anon6_Else:
    assume {:partition} $Exception == null;
    goto anon4;

  anon5_Else:
    assume {:partition} Read($Heap, $this, F$System.Collections2.Stack._syncRoot) != null;
    goto anon4;

  anon4:
    assume $this != null;
    $result := Read($Heap, $this, F$System.Collections2.Stack._syncRoot);
    return;
}



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_Clone($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_ContainsSystem_Object($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_CopyToSystem_ArraySystem_Int32($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_GetEnumerator($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_Peek($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_Pop($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_PushSystem_Object($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.System_Collections2_Stack_get_SyncRoot~System_Collections2_Stack_ToArray($this: Ref) returns ($result: Ref);



procedure {:System.Diagnostics.Contracts.ContractInvariantMethod} System.Collections2.Stack.$InvariantMethod$($this: Ref);



procedure {:extern} System.Diagnostics.Contracts.Contract.Invariant$System.Boolean(condition$in: bool);



const unique F$System.Collections2.Stack.SyncStack._s: Field;

const unique F$System.Collections2.Stack.SyncStack._root: Field;

procedure System.Collections2.Stack.SyncStack.get_IsSynchronized($this: Ref) returns ($result: bool);



procedure System.Collections2.Stack.SyncStack.get_SyncRoot($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.SyncStack.get_Count($this: Ref) returns ($result: int);



procedure {:extern} System.Threading.Monitor.Enter$System.Object$System.Boolean$(obj$in: Ref, lockTaken$in: bool) returns (lockTaken$out: bool);



procedure {:extern} System.Threading.Monitor.Exit$System.Object(obj$in: Ref);



procedure System.Collections2.Stack.SyncStack.Contains$System.Object($this: Ref, obj$in: Ref) returns ($result: bool);



procedure System.Collections2.Stack.SyncStack.Clone($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.SyncStack.Clear($this: Ref);



procedure System.Collections2.Stack.SyncStack.CopyTo$System.Array$System.Int32($this: Ref, array$in: Ref, arrayIndex$in: int);



procedure System.Collections2.Stack.SyncStack.Push$System.Object($this: Ref, value$in: Ref);



procedure {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Contracts", "CC1055"} System.Collections2.Stack.SyncStack.Pop($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.SyncStack.GetEnumerator($this: Ref) returns ($result: Ref);



procedure {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Contracts", "CC1055"} System.Collections2.Stack.SyncStack.Peek($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.SyncStack.ToArray($this: Ref) returns ($result: Ref);



procedure T$System.Collections2.Stack.SyncStack.#cctor();



const unique F$System.Collections2.Stack.StackEnumerator._stack: Field;

const unique F$System.Collections2.Stack.StackEnumerator._index: Field;

const unique F$System.Collections2.Stack.StackEnumerator._version: Field;

const unique F$System.Collections2.Stack.StackEnumerator.currentElement: Field;

procedure System.Collections2.Stack.StackEnumerator.Clone($this: Ref) returns ($result: Ref);



procedure {:extern} System.Object.MemberwiseClone($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.StackEnumerator.MoveNext($this: Ref) returns ($result: bool);



procedure System.Collections2.Stack.StackEnumerator.get_Current($this: Ref) returns ($result: Ref);



procedure System.Collections2.Stack.StackEnumerator.Reset($this: Ref);



procedure T$System.Collections2.Stack.StackEnumerator.#cctor();



function T$System.Collections2.Stack.StackDebugView() : Ref;

const unique T$System.Collections2.Stack.StackDebugView: int;

const unique F$System.Collections2.Stack.StackDebugView.stack: Field;

procedure System.Collections2.Stack.StackDebugView.#ctor$System.Collections2.Stack($this: Ref, stack$in: Ref);



const {:value "stack"} unique $string_literal_stack_7: Ref;

procedure System.Collections2.Stack.StackDebugView.get_Items($this: Ref) returns ($result: Ref);



procedure T$System.Collections2.Stack.StackDebugView.#cctor();



procedure T$System.Collections2.Stack.#cctor();



function T$System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute() : Ref;

const unique T$System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute: int;

procedure System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute.#ctor($this: Ref);



procedure {:extern} System.Attribute.#ctor($this: Ref);



procedure T$System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute.#cctor();


