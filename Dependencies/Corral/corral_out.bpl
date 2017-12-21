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
  modifies $Alloc;



implementation {:ForceInline} $BoxFromInt(i: int) returns (r: Ref)
{

  anon0:
    call {:si_unique_call 0} r := Alloc();
    assume $TypeConstructor($DynamicType(r)) == $IntValueType;
    assume Union2Int(r) == i;
    return;
}



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

axiom !$Subtype(T$System.AssertFailedException(), T$System.UnauthorizedAccessException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.NotImplementedException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.RankException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.TimeoutException());

axiom !$Subtype(T$System.AssertFailedException(), T$System.Exception());

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

function T$Interop() : Ref;

const unique T$Interop: int;

function T$Interop.Kernel32() : Ref;

const unique T$Interop.Kernel32: int;

var F$Interop.Kernel32.SEM_FAILCRITICALERRORS: int;

var F$Interop.Kernel32.FORMAT_MESSAGE_IGNORE_INSERTS: int;

var F$Interop.Kernel32.FORMAT_MESSAGE_FROM_HMODULE: int;

var F$Interop.Kernel32.FORMAT_MESSAGE_FROM_SYSTEM: int;

var F$Interop.Kernel32.FORMAT_MESSAGE_ARGUMENT_ARRAY: int;

var F$Interop.Kernel32.ERROR_INSUFFICIENT_BUFFER: int;

var F$Interop.Kernel32.InitialBufferSize: int;

var F$Interop.Kernel32.BufferSizeIncreaseFactor: int;

var F$Interop.Kernel32.MaxAllowedBufferSize: int;

var F$Interop.Kernel32.MAX_PATH: int;

var F$Interop.Kernel32.MAX_DIRECTORY_PATH: int;

var F$Interop.Kernel32.CREDUI_MAX_USERNAME_LENGTH: int;

procedure Interop.Kernel32.CancelIoEx$System.Runtime.InteropServices.SafeHandle$System.Threading.NativeOverlapped$(handle$in: Ref, lpOverlapped$in: Ref) returns ($result: bool);



procedure Interop.Kernel32.ConnectNamedPipe$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Threading.NativeOverlapped$(handle$in: Ref, overlapped$in: Ref) returns ($result: bool);



procedure Interop.Kernel32.ConnectNamedPipe$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr(handle$in: Ref, overlapped$in: int) returns ($result: bool);



procedure Interop.Kernel32.CreateNamedPipe$System.String$System.Int32$System.Int32$System.Int32$System.Int32$System.Int32$System.Int32$Interop.Kernel32.SECURITY_ATTRIBUTES$(pipeName$in: Ref, openMode$in: int, pipeMode$in: int, maxInstances$in: int, outBufferSize$in: int, inBufferSize$in: int, defaultTimeout$in: int, securityAttributes$in: Ref) returns (securityAttributes$out: Ref, $result: Ref);



procedure Interop.Kernel32.CreateNamedPipeClient$System.String$System.Int32$System.IO.FileShare$Interop.Kernel32.SECURITY_ATTRIBUTES$$System.IO.FileMode$System.Int32$System.IntPtr(lpFileName$in: Ref, dwDesiredAccess$in: int, dwShareMode$in: int, secAttrs$in: Ref, dwCreationDisposition$in: int, dwFlagsAndAttributes$in: int, hTemplateFile$in: int) returns (secAttrs$out: Ref, $result: Ref);



procedure Interop.Kernel32.DisconnectNamedPipe$Microsoft.Win32.SafeHandles.SafePipeHandle(hNamedPipe$in: Ref) returns ($result: bool);



procedure Interop.Kernel32.FlushFileBuffers$System.Runtime.InteropServices.SafeHandle(hHandle$in: Ref) returns ($result: bool);



procedure Interop.Kernel32.FormatMessage$System.Int32$System.IntPtr$System.UInt32$System.Int32$System.Text.StringBuilder$System.Int32$System.IntPtrarray(dwFlags$in: int, lpSource$in: int, dwMessageId$in: int, dwLanguageId$in: int, lpBuffer$in: Ref, nSize$in: int, arguments$in: Ref) returns ($result: int);



procedure Interop.Kernel32.GetMessage$System.Int32(errorCode$in: int) returns ($result: Ref);
  modifies $Alloc;



var {:extern} F$System.IntPtr.Zero: int;

procedure Interop.Kernel32.GetMessage$System.IntPtr$System.Int32(moduleHandle$in: int, errorCode$in: int) returns ($result: Ref);
  modifies $Alloc;



implementation Interop.Kernel32.GetMessage$System.Int32(errorCode$in: int) returns ($result: Ref)
{
  var errorCode: int;
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    errorCode := errorCode$in;
    assume {:breadcrumb 0} true;
    call {:si_unique_call 1} $tmp0 := Interop.Kernel32.GetMessage$System.IntPtr$System.Int32(F$System.IntPtr.Zero, errorCode);
    goto anon3_Then, anon3_Else;

  anon3_Then:
    assume {:partition} $Exception != null;
    return;

  anon3_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    $result := $tmp0;
    return;
}



procedure {:extern} System.Text.StringBuilder.#ctor$System.Int32($this: Ref, capacity$in: int);



function {:extern} T$System.Text.StringBuilder() : Ref;

const {:extern} unique T$System.Text.StringBuilder: int;

procedure Interop.Kernel32.TryGetErrorMessage$System.IntPtr$System.Int32$System.Text.StringBuilder$System.String$(moduleHandle$in: int, errorCode$in: int, sb$in: Ref, errorMsg$in: Ref) returns (errorMsg$out: Ref, $result: bool);
  modifies $Alloc;



procedure {:extern} System.Text.StringBuilder.get_Capacity($this: Ref) returns ($result: int);



procedure {:extern} System.Text.StringBuilder.set_Capacity$System.Int32($this: Ref, value$in: int);



const {:value "Unknown error (0x{0:x})"} unique $string_literal_Unknown$error$$0x$0$x$$_0: Ref;

procedure {:extern} System.String.Format$System.String$System.Object(format$in: Ref, arg0$in: Ref) returns ($result: Ref);



implementation Interop.Kernel32.GetMessage$System.IntPtr$System.Int32(moduleHandle$in: int, errorCode$in: int) returns ($result: Ref)
{
  var moduleHandle: int;
  var errorCode: int;
  var local_0_Ref: Ref;
  var $tmp0: Ref;
  var local_1_Ref: Ref;
  var $tmp1: bool;
  var $tmp2: int;
  var $tmp3: int;
  var $tmp4: Ref;
  var $tmp5: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    moduleHandle := moduleHandle$in;
    errorCode := errorCode$in;
    assume {:breadcrumb 1} true;
    call {:si_unique_call 2} $tmp0 := Alloc();
    call {:si_unique_call 3} System.Text.StringBuilder.#ctor$System.Int32($tmp0, 256);
    assume $DynamicType($tmp0) == T$System.Text.StringBuilder();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.Text.StringBuilder;
    local_0_Ref := $tmp0;
    goto IL_000b;

  IL_000b:
    call {:si_unique_call 4} local_1_Ref, $tmp1 := Interop.Kernel32.TryGetErrorMessage$System.IntPtr$System.Int32$System.Text.StringBuilder$System.String$(moduleHandle, errorCode, local_0_Ref, local_1_Ref);
    goto anon13_Then, anon13_Else;

  anon13_Then:
    assume {:partition} $Exception != null;
    return;

  anon13_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    goto anon14_Then, anon14_Else;

  anon14_Then:
    assume {:partition} $tmp1;
    $result := local_1_Ref;
    return;

  anon14_Else:
    assume {:partition} !$tmp1;
    goto anon5;

  anon5:
    call {:si_unique_call 5} $tmp2 := System.Text.StringBuilder.get_Capacity(local_0_Ref);
    call {:si_unique_call 6} System.Text.StringBuilder.set_Capacity$System.Int32(local_0_Ref, $tmp2 * 4);
    call {:si_unique_call 7} $tmp3 := System.Text.StringBuilder.get_Capacity(local_0_Ref);
    goto anon15_Then, anon15_Else;

  anon15_Then:
    assume {:partition} $Exception != null;
    return;

  anon15_Else:
    assume {:partition} $Exception == null;
    goto anon7;

  anon7:
    goto anon16_Then, anon16_Else;

  anon16_Then:
    assume {:partition} $tmp3 < 66560;
    goto IL_000b;

  anon16_Else:
    assume {:partition} 66560 <= $tmp3;
    goto anon10;

  anon10:
    call {:si_unique_call 8} $tmp4 := $BoxFromInt(errorCode);
    call {:si_unique_call 9} $tmp5 := System.String.Format$System.String$System.Object($string_literal_Unknown$error$$0x$0$x$$_0, $tmp4);
    goto anon17_Then, anon17_Else;

  anon17_Then:
    assume {:partition} $Exception != null;
    return;

  anon17_Else:
    assume {:partition} $Exception == null;
    goto anon12;

  anon12:
    $result := $tmp5;
    return;
}



const {:value ""} unique $string_literal__1: Ref;

procedure {:extern} System.IntPtr.op_Inequality$System.IntPtr$System.IntPtr(value1$in: int, value2$in: int) returns ($result: bool);



procedure {:extern} System.Text.StringBuilder.get_Length($this: Ref) returns ($result: int);



procedure {:extern} System.Text.StringBuilder.get_Chars$System.Int32($this: Ref, index$in: int) returns ($result: int);



procedure {:extern} System.Text.StringBuilder.ToString$System.Int32$System.Int32($this: Ref, startIndex$in: int, length$in: int) returns ($result: Ref);



procedure {:extern} System.Runtime.InteropServices.Marshal.GetLastWin32Error() returns ($result: int);



implementation Interop.Kernel32.TryGetErrorMessage$System.IntPtr$System.Int32$System.Text.StringBuilder$System.String$(moduleHandle$in: int, errorCode$in: int, sb$in: Ref, errorMsg$in: Ref) returns (errorMsg$out: Ref, $result: bool)
{
  var moduleHandle: int;
  var errorCode: int;
  var sb: Ref;
  var local_1_int: int;
  var $tmp0: bool;
  var $tmp1: int;
  var $tmp2: int;
  var $tmp3: int;
  var local_2_int: int;
  var $tmp4: int;
  var local_3_int: int;
  var $tmp5: Ref;
  var $tmp6: int;
  var $tmp7: Ref;
  var $tmp8: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    moduleHandle := moduleHandle$in;
    errorCode := errorCode$in;
    sb := sb$in;
    errorMsg$out := errorMsg$in;
    assume {:breadcrumb 2} true;
    errorMsg$out := $string_literal__1;
    local_1_int := 12800;
    call {:si_unique_call 10} $tmp0 := System.IntPtr.op_Inequality$System.IntPtr$System.IntPtr(moduleHandle, F$System.IntPtr.Zero);
    goto anon34_Then, anon34_Else;

  anon34_Then:
    assume {:partition} $Exception != null;
    return;

  anon34_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    goto anon35_Then, anon35_Else;

  anon35_Then:
    assume {:partition} $tmp0;
    local_1_int := BitwiseOr(local_1_int, 2048);
    goto anon5;

  anon35_Else:
    assume {:partition} !$tmp0;
    goto anon5;

  anon5:
    call {:si_unique_call 11} $tmp1 := System.Text.StringBuilder.get_Capacity(sb);
    goto anon36_Then, anon36_Else;

  anon36_Then:
    assume {:partition} $Exception != null;
    return;

  anon36_Else:
    assume {:partition} $Exception == null;
    goto anon7;

  anon7:
    call {:si_unique_call 12} $tmp2 := Interop.Kernel32.FormatMessage$System.Int32$System.IntPtr$System.UInt32$System.Int32$System.Text.StringBuilder$System.Int32$System.IntPtrarray(local_1_int, moduleHandle, errorCode, 0, sb, $tmp1, null);
    goto anon37_Then, anon37_Else;

  anon37_Then:
    assume {:partition} $Exception != null;
    return;

  anon37_Else:
    assume {:partition} $Exception == null;
    goto anon9;

  anon9:
    goto anon38_Then, anon38_Else;

  anon38_Then:
    assume {:partition} $tmp2 != 0;
    call {:si_unique_call 13} $tmp3 := System.Text.StringBuilder.get_Length(sb);
    goto anon39_Then, anon39_Else;

  anon39_Then:
    assume {:partition} $Exception != null;
    return;

  anon39_Else:
    assume {:partition} $Exception == null;
    goto anon12;

  anon12:
    local_2_int := $tmp3;
    goto IL_005b;

  IL_0041:
    call {:si_unique_call 14} $tmp4 := System.Text.StringBuilder.get_Chars$System.Int32(sb, local_2_int - 1);
    goto anon40_Then, anon40_Else;

  anon40_Then:
    assume {:partition} $Exception != null;
    return;

  anon40_Else:
    assume {:partition} $Exception == null;
    goto anon14;

  anon14:
    local_3_int := $tmp4;
    goto anon41_Then, anon41_Else;

  anon41_Then:
    assume {:partition} local_3_int > 32;
    goto anon17;

  anon41_Else:
    assume {:partition} 32 >= local_3_int;
    goto anon17;

  anon17:
    goto anon42_Then, anon42_Else;

  anon42_Then:
    assume {:partition} (if local_3_int > 32 then local_3_int != 46 else false);
    goto anon22;

  anon42_Else:
    assume {:partition} !(if local_3_int > 32 then local_3_int != 46 else false);
    local_2_int := local_2_int - 1;
    goto IL_005b;

  IL_005b:
    goto anon43_Then, anon43_Else;

  anon43_Then:
    assume {:partition} local_2_int > 0;
    goto IL_0041;

  anon43_Else:
    assume {:partition} 0 >= local_2_int;
    goto anon22;

  anon22:
    call {:si_unique_call 15} $tmp5 := System.Text.StringBuilder.ToString$System.Int32$System.Int32(sb, 0, local_2_int);
    goto anon44_Then, anon44_Else;

  anon44_Then:
    assume {:partition} $Exception != null;
    return;

  anon44_Else:
    assume {:partition} $Exception == null;
    goto anon24;

  anon24:
    errorMsg$out := $tmp5;
    goto anon33;

  anon38_Else:
    assume {:partition} $tmp2 == 0;
    call {:si_unique_call 16} $tmp6 := System.Runtime.InteropServices.Marshal.GetLastWin32Error();
    goto anon45_Then, anon45_Else;

  anon45_Then:
    assume {:partition} $Exception != null;
    return;

  anon45_Else:
    assume {:partition} $Exception == null;
    goto anon27;

  anon27:
    goto anon46_Then, anon46_Else;

  anon46_Then:
    assume {:partition} $tmp6 == 122;
    $result := false;
    return;

  anon46_Else:
    assume {:partition} $tmp6 != 122;
    goto anon30;

  anon30:
    call {:si_unique_call 17} $tmp7 := $BoxFromInt(errorCode);
    call {:si_unique_call 18} $tmp8 := System.String.Format$System.String$System.Object($string_literal_Unknown$error$$0x$0$x$$_0, $tmp7);
    goto anon47_Then, anon47_Else;

  anon47_Then:
    assume {:partition} $Exception != null;
    return;

  anon47_Else:
    assume {:partition} $Exception == null;
    goto anon32;

  anon32:
    errorMsg$out := $tmp8;
    goto anon33;

  anon33:
    $result := true;
    return;
}



procedure Interop.Kernel32.GetFileType$System.Runtime.InteropServices.SafeHandle(hFile$in: Ref) returns ($result: int);



procedure Interop.Kernel32.GetNamedPipeHandleState$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Int32$$System.IntPtr$System.IntPtr$System.IntPtr$System.IntPtr$System.Int32(hNamedPipe$in: Ref, lpState$in: int, lpCurInstances$in: int, lpMaxCollectionCount$in: int, lpCollectDataTimeout$in: int, lpUserName$in: int, nMaxUserNameSize$in: int) returns (lpState$out: int, $result: bool);



procedure Interop.Kernel32.GetNamedPipeHandleState$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr$System.IntPtr$System.IntPtr$System.IntPtr$System.Text.StringBuilder$System.Int32(hNamedPipe$in: Ref, lpState$in: int, lpCurInstances$in: int, lpMaxCollectionCount$in: int, lpCollectDataTimeout$in: int, lpUserName$in: Ref, nMaxUserNameSize$in: int) returns ($result: bool);



procedure Interop.Kernel32.GetNamedPipeHandleState$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr$System.Int32$$System.IntPtr$System.IntPtr$System.IntPtr$System.Int32(hNamedPipe$in: Ref, lpState$in: int, lpCurInstances$in: int, lpMaxCollectionCount$in: int, lpCollectDataTimeout$in: int, lpUserName$in: int, nMaxUserNameSize$in: int) returns (lpCurInstances$out: int, $result: bool);



procedure Interop.Kernel32.GetNamedPipeInfo$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Int32$$System.IntPtr$System.IntPtr$System.IntPtr(hNamedPipe$in: Ref, lpFlags$in: int, lpOutBufferSize$in: int, lpInBufferSize$in: int, lpMaxInstances$in: int) returns (lpFlags$out: int, $result: bool);



procedure Interop.Kernel32.GetNamedPipeInfo$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr$System.Int32$$System.IntPtr$System.IntPtr(hNamedPipe$in: Ref, lpFlags$in: int, lpOutBufferSize$in: int, lpInBufferSize$in: int, lpMaxInstances$in: int) returns (lpOutBufferSize$out: int, $result: bool);



procedure Interop.Kernel32.GetNamedPipeInfo$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr$System.IntPtr$System.Int32$$System.IntPtr(hNamedPipe$in: Ref, lpFlags$in: int, lpOutBufferSize$in: int, lpInBufferSize$in: int, lpMaxInstances$in: int) returns (lpInBufferSize$out: int, $result: bool);



procedure Interop.Kernel32.ReadFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.Int32$$System.IntPtr(handle$in: Ref, bytes$in: Ref, numBytesToRead$in: int, numBytesRead$in: int, mustBeZero$in: int) returns (numBytesRead$out: int, $result: int);



procedure Interop.Kernel32.ReadFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.IntPtr$System.Threading.NativeOverlapped$(handle$in: Ref, bytes$in: Ref, numBytesToRead$in: int, numBytesRead_mustBeZero$in: int, overlapped$in: Ref) returns ($result: int);



procedure Interop.Kernel32.SetNamedPipeHandleState$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Int32$$System.IntPtr$System.IntPtr(hNamedPipe$in: Ref, lpMode$in: Ref, lpMaxCollectionCount$in: int, lpCollectDataTimeout$in: int) returns ($result: bool);



procedure Interop.Kernel32.WaitNamedPipe$System.String$System.Int32(name$in: Ref, timeout$in: int) returns ($result: bool);



procedure Interop.Kernel32.WriteFile$System.IntPtr$System.Byte$$System.Int32$System.Int32$$System.IntPtr(handle$in: int, bytes$in: Ref, numBytesToWrite$in: int, numBytesWritten$in: int, mustBeZero$in: int) returns (numBytesWritten$out: int, $result: int);



procedure Interop.Kernel32.WriteFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.Int32$$System.IntPtr(handle$in: Ref, bytes$in: Ref, numBytesToWrite$in: int, numBytesWritten$in: int, mustBeZero$in: int) returns (numBytesWritten$out: int, $result: int);



procedure Interop.Kernel32.WriteFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.IntPtr$System.Threading.NativeOverlapped$(handle$in: Ref, bytes$in: Ref, numBytesToWrite$in: int, numBytesWritten_mustBeZero$in: int, lpOverlapped$in: Ref) returns ($result: int);



procedure Interop.Kernel32.#ctor($this: Ref);



procedure {:extern} System.Object.#ctor($this: Ref);



function T$Interop.Kernel32.IOReparseOptions() : Ref;

const unique T$Interop.Kernel32.IOReparseOptions: int;

var F$Interop.Kernel32.IOReparseOptions.IO_REPARSE_TAG_FILE_PLACEHOLDER: int;

var F$Interop.Kernel32.IOReparseOptions.IO_REPARSE_TAG_MOUNT_POINT: int;

procedure Interop.Kernel32.IOReparseOptions.#ctor($this: Ref);



procedure T$Interop.Kernel32.IOReparseOptions.#cctor();



function T$Interop.Kernel32.FileOperations() : Ref;

const unique T$Interop.Kernel32.FileOperations: int;

var F$Interop.Kernel32.FileOperations.OPEN_EXISTING: int;

var F$Interop.Kernel32.FileOperations.COPY_FILE_FAIL_IF_EXISTS: int;

var F$Interop.Kernel32.FileOperations.FILE_ACTION_ADDED: int;

var F$Interop.Kernel32.FileOperations.FILE_ACTION_REMOVED: int;

var F$Interop.Kernel32.FileOperations.FILE_ACTION_MODIFIED: int;

var F$Interop.Kernel32.FileOperations.FILE_ACTION_RENAMED_OLD_NAME: int;

var F$Interop.Kernel32.FileOperations.FILE_ACTION_RENAMED_NEW_NAME: int;

var F$Interop.Kernel32.FileOperations.FILE_FLAG_BACKUP_SEMANTICS: int;

var F$Interop.Kernel32.FileOperations.FILE_FLAG_FIRST_PIPE_INSTANCE: int;

var F$Interop.Kernel32.FileOperations.FILE_FLAG_OVERLAPPED: int;

var F$Interop.Kernel32.FileOperations.FILE_LIST_DIRECTORY: int;

procedure Interop.Kernel32.FileOperations.#ctor($this: Ref);



procedure T$Interop.Kernel32.FileOperations.#cctor();



function T$Interop.Kernel32.FileTypes() : Ref;

const unique T$Interop.Kernel32.FileTypes: int;

var F$Interop.Kernel32.FileTypes.FILE_TYPE_UNKNOWN: int;

var F$Interop.Kernel32.FileTypes.FILE_TYPE_DISK: int;

var F$Interop.Kernel32.FileTypes.FILE_TYPE_CHAR: int;

var F$Interop.Kernel32.FileTypes.FILE_TYPE_PIPE: int;

procedure Interop.Kernel32.FileTypes.#ctor($this: Ref);



procedure T$Interop.Kernel32.FileTypes.#cctor();



function T$Interop.Kernel32.GenericOperations() : Ref;

const unique T$Interop.Kernel32.GenericOperations: int;

var F$Interop.Kernel32.GenericOperations.GENERIC_READ: int;

var F$Interop.Kernel32.GenericOperations.GENERIC_WRITE: int;

procedure Interop.Kernel32.GenericOperations.#ctor($this: Ref);



procedure T$Interop.Kernel32.GenericOperations.#cctor();



function T$Interop.Kernel32.PipeOptions() : Ref;

const unique T$Interop.Kernel32.PipeOptions: int;

var F$Interop.Kernel32.PipeOptions.PIPE_ACCESS_INBOUND: int;

var F$Interop.Kernel32.PipeOptions.PIPE_ACCESS_OUTBOUND: int;

var F$Interop.Kernel32.PipeOptions.PIPE_ACCESS_DUPLEX: int;

var F$Interop.Kernel32.PipeOptions.PIPE_TYPE_BYTE: int;

var F$Interop.Kernel32.PipeOptions.PIPE_TYPE_MESSAGE: int;

var F$Interop.Kernel32.PipeOptions.PIPE_READMODE_BYTE: int;

var F$Interop.Kernel32.PipeOptions.PIPE_READMODE_MESSAGE: int;

var F$Interop.Kernel32.PipeOptions.PIPE_UNLIMITED_INSTANCES: int;

procedure Interop.Kernel32.PipeOptions.#ctor($this: Ref);



procedure T$Interop.Kernel32.PipeOptions.#cctor();



function T$Interop.Kernel32.SecurityOptions() : Ref;

const unique T$Interop.Kernel32.SecurityOptions: int;

var F$Interop.Kernel32.SecurityOptions.SECURITY_SQOS_PRESENT: int;

var F$Interop.Kernel32.SecurityOptions.SECURITY_ANONYMOUS: int;

var F$Interop.Kernel32.SecurityOptions.SECURITY_IDENTIFICATION: int;

var F$Interop.Kernel32.SecurityOptions.SECURITY_IMPERSONATION: int;

var F$Interop.Kernel32.SecurityOptions.SECURITY_DELEGATION: int;

procedure Interop.Kernel32.SecurityOptions.#ctor($this: Ref);



procedure T$Interop.Kernel32.SecurityOptions.#cctor();



function T$Interop.Kernel32.SECURITY_ATTRIBUTES() : Ref;

const unique T$Interop.Kernel32.SECURITY_ATTRIBUTES: int;

procedure Interop.Kernel32.SECURITY_ATTRIBUTES.#default_ctor($this: Ref);



const unique F$Interop.Kernel32.SECURITY_ATTRIBUTES.nLength: Field;

const unique F$Interop.Kernel32.SECURITY_ATTRIBUTES.lpSecurityDescriptor: Field;

const unique F$Interop.Kernel32.SECURITY_ATTRIBUTES.bInheritHandle: Field;

procedure Interop.Kernel32.SECURITY_ATTRIBUTES.#copy_ctor(this: Ref) returns (other: Ref);
  free ensures this != other;



procedure T$Interop.Kernel32.#cctor();



function T$Interop.Errors() : Ref;

const unique T$Interop.Errors: int;

var F$Interop.Errors.ERROR_SUCCESS: int;

var F$Interop.Errors.ERROR_INVALID_FUNCTION: int;

var F$Interop.Errors.ERROR_FILE_NOT_FOUND: int;

var F$Interop.Errors.ERROR_PATH_NOT_FOUND: int;

var F$Interop.Errors.ERROR_ACCESS_DENIED: int;

var F$Interop.Errors.ERROR_INVALID_HANDLE: int;

var F$Interop.Errors.ERROR_NOT_ENOUGH_MEMORY: int;

var F$Interop.Errors.ERROR_INVALID_DATA: int;

var F$Interop.Errors.ERROR_INVALID_DRIVE: int;

var F$Interop.Errors.ERROR_NO_MORE_FILES: int;

var F$Interop.Errors.ERROR_NOT_READY: int;

var F$Interop.Errors.ERROR_BAD_COMMAND: int;

var F$Interop.Errors.ERROR_BAD_LENGTH: int;

var F$Interop.Errors.ERROR_SHARING_VIOLATION: int;

var F$Interop.Errors.ERROR_LOCK_VIOLATION: int;

var F$Interop.Errors.ERROR_HANDLE_EOF: int;

var F$Interop.Errors.ERROR_FILE_EXISTS: int;

var F$Interop.Errors.ERROR_INVALID_PARAMETER: int;

var F$Interop.Errors.ERROR_BROKEN_PIPE: int;

var F$Interop.Errors.ERROR_CALL_NOT_IMPLEMENTED: int;

var F$Interop.Errors.ERROR_INSUFFICIENT_BUFFER: int;

var F$Interop.Errors.ERROR_INVALID_NAME: int;

var F$Interop.Errors.ERROR_NEGATIVE_SEEK: int;

var F$Interop.Errors.ERROR_DIR_NOT_EMPTY: int;

var F$Interop.Errors.ERROR_BAD_PATHNAME: int;

var F$Interop.Errors.ERROR_LOCK_FAILED: int;

var F$Interop.Errors.ERROR_BUSY: int;

var F$Interop.Errors.ERROR_ALREADY_EXISTS: int;

var F$Interop.Errors.ERROR_BAD_EXE_FORMAT: int;

var F$Interop.Errors.ERROR_ENVVAR_NOT_FOUND: int;

var F$Interop.Errors.ERROR_FILENAME_EXCED_RANGE: int;

var F$Interop.Errors.ERROR_EXE_MACHINE_TYPE_MISMATCH: int;

var F$Interop.Errors.ERROR_PIPE_BUSY: int;

var F$Interop.Errors.ERROR_NO_DATA: int;

var F$Interop.Errors.ERROR_PIPE_NOT_CONNECTED: int;

var F$Interop.Errors.ERROR_MORE_DATA: int;

var F$Interop.Errors.ERROR_NO_MORE_ITEMS: int;

var F$Interop.Errors.ERROR_PARTIAL_COPY: int;

var F$Interop.Errors.ERROR_ARITHMETIC_OVERFLOW: int;

var F$Interop.Errors.ERROR_PIPE_CONNECTED: int;

var F$Interop.Errors.ERROR_PIPE_LISTENING: int;

var F$Interop.Errors.ERROR_OPERATION_ABORTED: int;

var F$Interop.Errors.ERROR_IO_INCOMPLETE: int;

var F$Interop.Errors.ERROR_IO_PENDING: int;

var F$Interop.Errors.ERROR_NO_TOKEN: int;

var F$Interop.Errors.ERROR_DLL_INIT_FAILED: int;

var F$Interop.Errors.ERROR_COUNTER_TIMEOUT: int;

var F$Interop.Errors.ERROR_NO_ASSOCIATION: int;

var F$Interop.Errors.ERROR_DDE_FAIL: int;

var F$Interop.Errors.ERROR_DLL_NOT_FOUND: int;

var F$Interop.Errors.ERROR_NOT_FOUND: int;

var F$Interop.Errors.ERROR_NON_ACCOUNT_SID: int;

var F$Interop.Errors.ERROR_NOT_ALL_ASSIGNED: int;

var F$Interop.Errors.ERROR_UNKNOWN_REVISION: int;

var F$Interop.Errors.ERROR_INVALID_OWNER: int;

var F$Interop.Errors.ERROR_INVALID_PRIMARY_GROUP: int;

var F$Interop.Errors.ERROR_NO_SUCH_PRIVILEGE: int;

var F$Interop.Errors.ERROR_PRIVILEGE_NOT_HELD: int;

var F$Interop.Errors.ERROR_INVALID_ACL: int;

var F$Interop.Errors.ERROR_INVALID_SECURITY_DESCR: int;

var F$Interop.Errors.ERROR_INVALID_SID: int;

var F$Interop.Errors.ERROR_BAD_IMPERSONATION_LEVEL: int;

var F$Interop.Errors.ERROR_CANT_OPEN_ANONYMOUS: int;

var F$Interop.Errors.ERROR_NO_SECURITY_ON_OBJECT: int;

var F$Interop.Errors.ERROR_TRUSTED_RELATIONSHIP_FAILURE: int;

var F$Interop.Errors.ERROR_RESOURCE_LANG_NOT_FOUND: int;

var F$Interop.Errors.EFail: int;

var F$Interop.Errors.E_FILENOTFOUND: int;

procedure Interop.Errors.#ctor($this: Ref);



procedure T$Interop.Errors.#cctor();



function T$Interop.Advapi32() : Ref;

const unique T$Interop.Advapi32: int;

procedure Interop.Advapi32.ImpersonateNamedPipeClient$Microsoft.Win32.SafeHandles.SafePipeHandle(hNamedPipe$in: Ref) returns ($result: bool);



procedure Interop.Advapi32.RevertToSelf() returns ($result: bool);



procedure T$Interop.Advapi32.#cctor();



procedure T$Interop.#cctor();



function T$MockedKernel32() : Ref;

const unique T$MockedKernel32: int;

procedure MockedKernel32.GetNamedPipeInfo$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Int32$$System.IntPtr$System.IntPtr$System.IntPtr(hNamedPipe$in: Ref, lpFlags$in: int, lpOutBufferSize$in: int, lpInBufferSize$in: int, lpMaxInstances$in: int) returns (lpFlags$out: int, $result: bool);



procedure MockedKernel32.GetNamedPipeInfo$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr$System.Int32$$System.IntPtr$System.IntPtr(hNamedPipe$in: Ref, lpFlags$in: int, lpOutBufferSize$in: int, lpInBufferSize$in: int, lpMaxInstances$in: int) returns (lpOutBufferSize$out: int, $result: bool);



procedure MockedKernel32.GetNamedPipeInfo$Microsoft.Win32.SafeHandles.SafePipeHandle$System.IntPtr$System.IntPtr$System.Int32$$System.IntPtr(hNamedPipe$in: Ref, lpFlags$in: int, lpOutBufferSize$in: int, lpInBufferSize$in: int, lpMaxInstances$in: int) returns (lpInBufferSize$out: int, $result: bool);



procedure MockedKernel32.SetNamedPipeHandleState$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Int32$$System.IntPtr$System.IntPtr(_handle$in: Ref, p$in: Ref, intPtr1$in: int, intPtr2$in: int) returns ($result: bool);



procedure MockedKernel32.WriteFile$System.IntPtr$System.Byte$$System.Int32$System.Int32$$System.IntPtr(handle$in: int, bytes$in: Ref, numBytesToWrite$in: int, numBytesWritten$in: int, mustBeZero$in: int) returns (numBytesWritten$out: int, $result: int);



procedure MockedKernel32.WriteFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.Int32$$System.IntPtr(handle$in: Ref, bytes$in: Ref, numBytesToWrite$in: int, numBytesWritten$in: int, mustBeZero$in: int) returns (numBytesWritten$out: int, $result: int);



procedure MockedKernel32.WriteFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.IntPtr$System.Threading.NativeOverlapped$(handle$in: Ref, bytes$in: Ref, numBytesToWrite$in: int, numBytesWritten_mustBeZero$in: int, lpOverlapped$in: Ref) returns ($result: int);



procedure MockedKernel32.FlushFileBuffers$Microsoft.Win32.SafeHandles.SafePipeHandle(_handle$in: Ref) returns ($result: bool);



procedure {:extern} System.NotImplementedException.#ctor($this: Ref);



procedure MockedKernel32.ReadFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.Int32$$System.IntPtr(handle$in: Ref, bytes$in: Ref, numBytesToRead$in: int, numBytesRead$in: int, mustBeZero$in: int) returns (numBytesRead$out: int, $result: int);



procedure MockedKernel32.ReadFile$System.Runtime.InteropServices.SafeHandle$System.Byte$$System.Int32$System.IntPtr$System.Threading.NativeOverlapped$(handle$in: Ref, bytes$in: Ref, numBytesToRead$in: int, numBytesRead_mustBeZero$in: int, overlapped$in: Ref) returns ($result: int);



procedure MockedKernel32.GetNamedPipeHandleState$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Int32$$System.IntPtr$System.IntPtr$System.IntPtr$System.IntPtr$System.Int32(SafePipeHandle$in: Ref, flags$in: int, intPtr1$in: int, intPtr2$in: int, intPtr3$in: int, intPtr4$in: int, p$in: int) returns (flags$out: int, $result: bool);



procedure MockedKernel32.GetFileType$Microsoft.Win32.SafeHandles.SafePipeHandle(safePipeHandle$in: Ref) returns ($result: int);



procedure MockedKernel32.#ctor($this: Ref);



procedure T$MockedKernel32.#cctor();



function T$System.IO.Win32Marshal() : Ref;

const unique T$System.IO.Win32Marshal: int;

procedure System.IO.Win32Marshal.GetExceptionForLastWin32Error() returns ($result: Ref);



var {:extern} F$System.String.Empty: Ref;

procedure System.IO.Win32Marshal.GetExceptionForWin32Error$System.Int32$System.String(errorCode$in: int, path$in: Ref) returns ($result: Ref);
  modifies $Alloc;



procedure System.IO.Win32Marshal.GetExceptionForLastWin32Error$System.String(path$in: Ref) returns ($result: Ref);



procedure System.IO.Win32Marshal.GetExceptionForWin32Error$System.Int32(errorCode$in: int) returns ($result: Ref);
  modifies $Alloc;



implementation System.IO.Win32Marshal.GetExceptionForWin32Error$System.Int32(errorCode$in: int) returns ($result: Ref)
{
  var errorCode: int;
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    errorCode := errorCode$in;
    assume {:breadcrumb 26} true;
    call {:si_unique_call 19} $tmp0 := System.IO.Win32Marshal.GetExceptionForWin32Error$System.Int32$System.String(errorCode, F$System.String.Empty);
    goto anon3_Then, anon3_Else;

  anon3_Then:
    assume {:partition} $Exception != null;
    return;

  anon3_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    $result := $tmp0;
    return;
}



procedure {:extern} System.String.get_Length($this: Ref) returns ($result: int);



procedure {:extern} System.IO.FileNotFoundException.#ctor($this: Ref);



procedure {:extern} System.IO.DirectoryNotFoundException.#ctor($this: Ref);



procedure {:extern} System.UnauthorizedAccessException.#ctor($this: Ref);



procedure {:extern} System.IO.IOException.#ctor($this: Ref);



procedure {:extern} System.IO.PathTooLongException.#ctor($this: Ref);



procedure System.IO.Win32Marshal.GetMessage$System.Int32(errorCode$in: int) returns ($result: Ref);
  modifies $Alloc;



procedure System.IO.Win32Marshal.MakeHRFromErrorCode$System.Int32(errorCode$in: int) returns ($result: int);



procedure {:extern} System.IO.IOException.#ctor$System.String$System.Int32($this: Ref, message$in: Ref, hresult$in: int);



procedure {:extern} System.OperationCanceledException.#ctor($this: Ref);



function {:extern} T$System.OperationCanceledException() : Ref;

const {:extern} unique T$System.OperationCanceledException: int;

implementation System.IO.Win32Marshal.GetExceptionForWin32Error$System.Int32$System.String(errorCode$in: int, path$in: Ref) returns ($result: Ref)
{
  var errorCode: int;
  var path: Ref;
  var local_0_int: int;
  var $tmp0: int;
  var $tmp1: Ref;
  var $tmp2: Ref;
  var $tmp3: int;
  var $tmp4: Ref;
  var $tmp5: Ref;
  var $tmp6: int;
  var $tmp7: Ref;
  var $tmp8: Ref;
  var $tmp9: int;
  var $tmp10: Ref;
  var $tmp11: Ref;
  var $tmp12: Ref;
  var $tmp13: Ref;
  var $tmp14: int;
  var $tmp15: int;
  var $tmp16: Ref;
  var $tmp17: Ref;
  var $tmp18: int;
  var $tmp19: Ref;
  var $tmp20: Ref;
  var $tmp21: Ref;
  var $tmp22: Ref;
  var $tmp23: int;
  var $localExc: Ref;
  var $label: int;

  anon0:
    errorCode := errorCode$in;
    path := path$in;
    assume {:breadcrumb 27} true;
    local_0_int := errorCode;
    goto anon63_Then, anon63_Else;

  anon63_Then:
    assume {:partition} local_0_int <= 80;
    goto anon64_Then, anon64_Else;

  anon64_Then:
    assume {:partition} local_0_int - 2 == 0;
    goto IL_0021;

  anon64_Else:
    assume {:partition} local_0_int - 2 != 0;
    goto anon65_Then, anon65_Else;

  anon65_Then:
    assume {:partition} local_0_int - 2 == 1;
    goto IL_0023;

  anon65_Else:
    assume {:partition} local_0_int - 2 != 1;
    goto anon66_Then, anon66_Else;

  anon66_Then:
    assume {:partition} local_0_int - 2 == 2;
    goto IL_0025;

  anon66_Else:
    assume {:partition} local_0_int - 2 != 2;
    goto anon67_Then, anon67_Else;

  anon67_Then:
    assume {:partition} local_0_int - 2 == 3;
    goto IL_002a;

  anon67_Else:
    assume {:partition} local_0_int - 2 != 3;
    goto anon6;

  anon6:
    goto IL_002c;

  IL_0021:
    goto IL_0080;

  IL_0023:
    goto IL_0094;

  IL_0025:
    goto IL_010e;

  IL_002a:
    goto IL_00a8;

  IL_002c:
    goto anon68_Then, anon68_Else;

  anon68_Then:
    assume {:partition} local_0_int == 32;
    goto IL_00e6;

  anon68_Else:
    assume {:partition} local_0_int != 32;
    goto anon9;

  anon9:
    goto anon69_Then, anon69_Else;

  anon69_Then:
    assume {:partition} local_0_int == 80;
    goto IL_00fa;

  anon69_Else:
    assume {:partition} local_0_int != 80;
    goto IL_010e;

  anon63_Else:
    assume {:partition} 80 < local_0_int;
    goto anon70_Then, anon70_Else;

  anon70_Then:
    assume {:partition} local_0_int <= 183;
    goto anon71_Then, anon71_Else;

  anon71_Then:
    assume {:partition} local_0_int == 87;
    goto IL_00d4;

  anon71_Else:
    assume {:partition} local_0_int != 87;
    goto anon16;

  anon16:
    goto anon72_Then, anon72_Else;

  anon72_Then:
    assume {:partition} local_0_int == 183;
    goto IL_00bc;

  anon72_Else:
    assume {:partition} local_0_int != 183;
    goto IL_010e;

  anon70_Else:
    assume {:partition} 183 < local_0_int;
    goto anon73_Then, anon73_Else;

  anon73_Then:
    assume {:partition} local_0_int != 206;
    goto anon74_Then, anon74_Else;

  anon74_Then:
    assume {:partition} local_0_int == 995;
    goto IL_0108;

  anon74_Else:
    assume {:partition} local_0_int != 995;
    goto anon23;

  anon23:
    goto IL_010e;

  IL_0080:
    call {:si_unique_call 20} $tmp0 := System.String.get_Length(path);
    goto anon75_Then, anon75_Else;

  anon75_Then:
    assume {:partition} $Exception != null;
    return;

  anon75_Else:
    assume {:partition} $Exception == null;
    goto anon25;

  anon25:
    goto anon76_Then, anon76_Else;

  anon76_Then:
    assume {:partition} $tmp0 == 0;
    call {:si_unique_call 21} $tmp1 := Alloc();
    call {:si_unique_call 22} System.IO.FileNotFoundException.#ctor($tmp1);
    assume $DynamicType($tmp1) == T$System.IO.FileNotFoundException();
    assume $TypeConstructor($DynamicType($tmp1)) == T$System.IO.FileNotFoundException;
    $result := $tmp1;
    return;

  anon76_Else:
    assume {:partition} $tmp0 != 0;
    goto anon28;

  anon28:
    call {:si_unique_call 23} $tmp2 := Alloc();
    call {:si_unique_call 24} System.IO.FileNotFoundException.#ctor($tmp2);
    assume $DynamicType($tmp2) == T$System.IO.FileNotFoundException();
    assume $TypeConstructor($DynamicType($tmp2)) == T$System.IO.FileNotFoundException;
    $result := $tmp2;
    return;

  IL_0094:
    call {:si_unique_call 25} $tmp3 := System.String.get_Length(path);
    goto anon77_Then, anon77_Else;

  anon77_Then:
    assume {:partition} $Exception != null;
    return;

  anon77_Else:
    assume {:partition} $Exception == null;
    goto anon30;

  anon30:
    goto anon78_Then, anon78_Else;

  anon78_Then:
    assume {:partition} $tmp3 == 0;
    call {:si_unique_call 26} $tmp4 := Alloc();
    call {:si_unique_call 27} System.IO.DirectoryNotFoundException.#ctor($tmp4);
    assume $DynamicType($tmp4) == T$System.IO.DirectoryNotFoundException();
    assume $TypeConstructor($DynamicType($tmp4)) == T$System.IO.DirectoryNotFoundException;
    $result := $tmp4;
    return;

  anon78_Else:
    assume {:partition} $tmp3 != 0;
    goto anon33;

  anon33:
    call {:si_unique_call 28} $tmp5 := Alloc();
    call {:si_unique_call 29} System.IO.DirectoryNotFoundException.#ctor($tmp5);
    assume $DynamicType($tmp5) == T$System.IO.DirectoryNotFoundException();
    assume $TypeConstructor($DynamicType($tmp5)) == T$System.IO.DirectoryNotFoundException;
    $result := $tmp5;
    return;

  IL_00a8:
    call {:si_unique_call 30} $tmp6 := System.String.get_Length(path);
    goto anon79_Then, anon79_Else;

  anon79_Then:
    assume {:partition} $Exception != null;
    return;

  anon79_Else:
    assume {:partition} $Exception == null;
    goto anon35;

  anon35:
    goto anon80_Then, anon80_Else;

  anon80_Then:
    assume {:partition} $tmp6 == 0;
    call {:si_unique_call 31} $tmp7 := Alloc();
    call {:si_unique_call 32} System.UnauthorizedAccessException.#ctor($tmp7);
    assume $DynamicType($tmp7) == T$System.UnauthorizedAccessException();
    assume $TypeConstructor($DynamicType($tmp7)) == T$System.UnauthorizedAccessException;
    $result := $tmp7;
    return;

  anon80_Else:
    assume {:partition} $tmp6 != 0;
    goto anon38;

  anon38:
    call {:si_unique_call 33} $tmp8 := Alloc();
    call {:si_unique_call 34} System.UnauthorizedAccessException.#ctor($tmp8);
    assume $DynamicType($tmp8) == T$System.UnauthorizedAccessException();
    assume $TypeConstructor($DynamicType($tmp8)) == T$System.UnauthorizedAccessException;
    $result := $tmp8;
    return;

  IL_00bc:
    call {:si_unique_call 35} $tmp9 := System.String.get_Length(path);
    goto anon81_Then, anon81_Else;

  anon81_Then:
    assume {:partition} $Exception != null;
    return;

  anon81_Else:
    assume {:partition} $Exception == null;
    goto anon40;

  anon40:
    goto anon82_Then, anon82_Else;

  anon82_Then:
    assume {:partition} $tmp9 != 0;
    goto anon43;

  anon82_Else:
    assume {:partition} $tmp9 == 0;
    goto IL_010e;

  anon43:
    call {:si_unique_call 36} $tmp10 := Alloc();
    call {:si_unique_call 37} System.IO.IOException.#ctor($tmp10);
    assume $DynamicType($tmp10) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp10)) == T$System.IO.IOException;
    $result := $tmp10;
    return;

  anon73_Else:
    assume {:partition} local_0_int == 206;
    goto anon45;

  anon45:
    call {:si_unique_call 38} $tmp11 := Alloc();
    call {:si_unique_call 39} System.IO.PathTooLongException.#ctor($tmp11);
    assume $DynamicType($tmp11) == T$System.IO.PathTooLongException();
    assume $TypeConstructor($DynamicType($tmp11)) == T$System.IO.PathTooLongException;
    $result := $tmp11;
    return;

  IL_00d4:
    call {:si_unique_call 40} $tmp12 := Alloc();
    call {:si_unique_call 41} $tmp13 := System.IO.Win32Marshal.GetMessage$System.Int32(errorCode);
    goto anon83_Then, anon83_Else;

  anon83_Then:
    assume {:partition} $Exception != null;
    return;

  anon83_Else:
    assume {:partition} $Exception == null;
    goto anon47;

  anon47:
    call {:si_unique_call 42} $tmp14 := System.IO.Win32Marshal.MakeHRFromErrorCode$System.Int32(errorCode);
    goto anon84_Then, anon84_Else;

  anon84_Then:
    assume {:partition} $Exception != null;
    return;

  anon84_Else:
    assume {:partition} $Exception == null;
    goto anon49;

  anon49:
    call {:si_unique_call 43} System.IO.IOException.#ctor$System.String$System.Int32($tmp12, $tmp13, $tmp14);
    assume $DynamicType($tmp12) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp12)) == T$System.IO.IOException;
    $result := $tmp12;
    return;

  IL_00e6:
    call {:si_unique_call 44} $tmp15 := System.String.get_Length(path);
    goto anon85_Then, anon85_Else;

  anon85_Then:
    assume {:partition} $Exception != null;
    return;

  anon85_Else:
    assume {:partition} $Exception == null;
    goto anon51;

  anon51:
    goto anon86_Then, anon86_Else;

  anon86_Then:
    assume {:partition} $tmp15 == 0;
    call {:si_unique_call 45} $tmp16 := Alloc();
    call {:si_unique_call 46} System.IO.IOException.#ctor($tmp16);
    assume $DynamicType($tmp16) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp16)) == T$System.IO.IOException;
    $result := $tmp16;
    return;

  anon86_Else:
    assume {:partition} $tmp15 != 0;
    goto anon54;

  anon54:
    call {:si_unique_call 47} $tmp17 := Alloc();
    call {:si_unique_call 48} System.IO.IOException.#ctor($tmp17);
    assume $DynamicType($tmp17) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp17)) == T$System.IO.IOException;
    $result := $tmp17;
    return;

  IL_00fa:
    call {:si_unique_call 49} $tmp18 := System.String.get_Length(path);
    goto anon87_Then, anon87_Else;

  anon87_Then:
    assume {:partition} $Exception != null;
    return;

  anon87_Else:
    assume {:partition} $Exception == null;
    goto anon56;

  anon56:
    goto anon88_Then, anon88_Else;

  anon88_Then:
    assume {:partition} $tmp18 != 0;
    call {:si_unique_call 50} $tmp19 := Alloc();
    call {:si_unique_call 51} System.IO.IOException.#ctor($tmp19);
    assume $DynamicType($tmp19) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp19)) == T$System.IO.IOException;
    $result := $tmp19;
    return;

  IL_0108:
    call {:si_unique_call 52} $tmp20 := Alloc();
    call {:si_unique_call 53} System.OperationCanceledException.#ctor($tmp20);
    assume $DynamicType($tmp20) == T$System.OperationCanceledException();
    assume $TypeConstructor($DynamicType($tmp20)) == T$System.OperationCanceledException;
    $result := $tmp20;
    return;

  anon88_Else:
    assume {:partition} $tmp18 == 0;
    goto IL_010e;

  IL_010e:
    call {:si_unique_call 54} $tmp21 := Alloc();
    call {:si_unique_call 55} $tmp22 := System.IO.Win32Marshal.GetMessage$System.Int32(errorCode);
    goto anon89_Then, anon89_Else;

  anon89_Then:
    assume {:partition} $Exception != null;
    return;

  anon89_Else:
    assume {:partition} $Exception == null;
    goto anon60;

  anon60:
    call {:si_unique_call 56} $tmp23 := System.IO.Win32Marshal.MakeHRFromErrorCode$System.Int32(errorCode);
    goto anon90_Then, anon90_Else;

  anon90_Then:
    assume {:partition} $Exception != null;
    return;

  anon90_Else:
    assume {:partition} $Exception == null;
    goto anon62;

  anon62:
    call {:si_unique_call 57} System.IO.IOException.#ctor$System.String$System.Int32($tmp21, $tmp22, $tmp23);
    assume $DynamicType($tmp21) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp21)) == T$System.IO.IOException;
    $result := $tmp21;
    return;
}



procedure System.IO.Win32Marshal.TryMakeWin32ErrorCodeFromHR$System.Int32(hr$in: int) returns ($result: int);



implementation System.IO.Win32Marshal.GetMessage$System.Int32(errorCode$in: int) returns ($result: Ref)
{
  var errorCode: int;
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    errorCode := errorCode$in;
    assume {:breadcrumb 30} true;
    call {:si_unique_call 58} $tmp0 := Interop.Kernel32.GetMessage$System.Int32(errorCode);
    goto anon3_Then, anon3_Else;

  anon3_Then:
    assume {:partition} $Exception != null;
    return;

  anon3_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    $result := $tmp0;
    return;
}



procedure T$System.IO.Win32Marshal.#cctor();



function T$System.IO.Error() : Ref;

const unique T$System.IO.Error: int;

procedure System.IO.Error.GetEndOfFile() returns ($result: Ref);
  modifies $Alloc;



procedure {:extern} System.IO.EndOfStreamException.#ctor($this: Ref);



implementation System.IO.Error.GetEndOfFile() returns ($result: Ref)
{
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 31} true;
    call {:si_unique_call 59} $tmp0 := Alloc();
    call {:si_unique_call 60} System.IO.EndOfStreamException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.IO.EndOfStreamException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.IO.EndOfStreamException;
    $result := $tmp0;
    return;
}



procedure System.IO.Error.GetPipeNotOpen() returns ($result: Ref);
  modifies $Alloc;



procedure {:extern} System.ObjectDisposedException.#ctor$System.String($this: Ref, objectName$in: Ref);



implementation System.IO.Error.GetPipeNotOpen() returns ($result: Ref)
{
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 32} true;
    call {:si_unique_call 61} $tmp0 := Alloc();
    call {:si_unique_call 62} System.ObjectDisposedException.#ctor$System.String($tmp0, null);
    assume $DynamicType($tmp0) == T$System.ObjectDisposedException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.ObjectDisposedException;
    $result := $tmp0;
    return;
}



procedure System.IO.Error.GetReadNotSupported() returns ($result: Ref);



procedure {:extern} System.NotSupportedException.#ctor($this: Ref);



procedure System.IO.Error.GetSeekNotSupported() returns ($result: Ref);



procedure System.IO.Error.GetWriteNotSupported() returns ($result: Ref);
  modifies $Alloc;



implementation System.IO.Error.GetWriteNotSupported() returns ($result: Ref)
{
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 35} true;
    call {:si_unique_call 63} $tmp0 := Alloc();
    call {:si_unique_call 64} System.NotSupportedException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.NotSupportedException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.NotSupportedException;
    $result := $tmp0;
    return;
}



procedure System.IO.Error.GetOperationAborted() returns ($result: Ref);



procedure T$System.IO.Error.#cctor();



function T$System.IO.Pipes2.NamedPipeServerStream() : Ref;

const unique T$System.IO.Pipes2.NamedPipeServerStream: int;

var F$System.IO.Pipes2.NamedPipeServerStream.MaxAllowedServerInstances: int;

procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String($this: Ref, pipeName$in: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String$System.IO.Pipes2.PipeDirection$System.Int32$System.IO.Pipes2.PipeTransmissionMode$System.IO.Pipes2.PipeOptions$System.Int32$System.Int32$System.IO.HandleInheritability($this: Ref, pipeName$in: Ref, direction$in: int, maxNumberOfServerInstances$in: int, transmissionMode$in: int, options$in: int, inBufferSize$in: int, outBufferSize$in: int, inheritability$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String$System.IO.Pipes2.PipeDirection($this: Ref, pipeName$in: Ref, direction$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String$System.IO.Pipes2.PipeDirection$System.Int32($this: Ref, pipeName$in: Ref, direction$in: int, maxNumberOfServerInstances$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String$System.IO.Pipes2.PipeDirection$System.Int32$System.IO.Pipes2.PipeTransmissionMode($this: Ref, pipeName$in: Ref, direction$in: int, maxNumberOfServerInstances$in: int, transmissionMode$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String$System.IO.Pipes2.PipeDirection$System.Int32$System.IO.Pipes2.PipeTransmissionMode$System.IO.Pipes2.PipeOptions($this: Ref, pipeName$in: Ref, direction$in: int, maxNumberOfServerInstances$in: int, transmissionMode$in: int, options$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.String$System.IO.Pipes2.PipeDirection$System.Int32$System.IO.Pipes2.PipeTransmissionMode$System.IO.Pipes2.PipeOptions$System.Int32$System.Int32($this: Ref, pipeName$in: Ref, direction$in: int, maxNumberOfServerInstances$in: int, transmissionMode$in: int, options$in: int, inBufferSize$in: int, outBufferSize$in: int);



procedure System.IO.Pipes2.PipeStream.#ctor$System.IO.Pipes2.PipeDirection$System.IO.Pipes2.PipeTransmissionMode$System.Int32($this: Ref, direction$in: int, transmissionMode$in: int, outBufferSize$in: int);



procedure {:extern} System.ArgumentNullException.#ctor($this: Ref);



procedure {:extern} System.ArgumentException.#ctor($this: Ref);



procedure {:extern} System.ArgumentOutOfRangeException.#ctor($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeServerStream.Create$System.String$System.IO.Pipes2.PipeDirection$System.Int32$System.IO.Pipes2.PipeTransmissionMode$System.IO.Pipes2.PipeOptions$System.Int32$System.Int32$System.IO.HandleInheritability($this: Ref, pipeName$in: Ref, direction$in: int, maxNumberOfServerInstances$in: int, transmissionMode$in: int, options$in: int, inBufferSize$in: int, outBufferSize$in: int, inheritability$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeServerStream.#ctor$System.IO.Pipes2.PipeDirection$System.Boolean$System.Boolean$Microsoft.Win32.SafeHandles.SafePipeHandle($this: Ref, direction$in: int, isAsync$in: bool, isConnected$in: bool, safePipeHandle$in: Ref);



procedure {:extern} System.Runtime.InteropServices.SafeHandle.get_IsInvalid($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.ValidateHandleIsPipe$Microsoft.Win32.SafeHandles.SafePipeHandle($this: Ref, safePipeHandle$in: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.PipeStream.InitializeHandle$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Boolean$System.Boolean($this: Ref, handle$in: Ref, isExposed$in: bool, isAsync$in: bool);



procedure System.IO.Pipes2.PipeStream.set_State$System.IO.Pipes2.PipeState($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeServerStream.Finalize($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.Dispose$System.Boolean($this: Ref, disposing$in: bool);



procedure {:extern} System.Object.Finalize($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.WaitForConnectionAsync($this: Ref) returns ($result: Ref);



procedure {:extern} System.Threading.CancellationToken.get_None() returns ($result: Ref);



procedure System.Threading.CancellationToken.#copy_ctor(this: Ref) returns (other: Ref);
  free ensures this != other;



procedure System.IO.Pipes2.NamedPipeServerStream.WaitForConnectionAsync$System.Threading.CancellationToken($this: Ref, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.BeginWaitForConnection$System.AsyncCallback$System.Object($this: Ref, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.Threading.Tasks.TaskToApm.Begin$System.Threading.Tasks.Task$System.AsyncCallback$System.Object(task$in: Ref, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.EndWaitForConnection$System.IAsyncResult($this: Ref, asyncResult$in: Ref);



procedure System.Threading.Tasks.TaskToApm.End$System.IAsyncResult(asyncResult$in: Ref);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.NamedPipeServerStream.CheckConnectOperationsServer($this: Ref);



procedure System.IO.Pipes2.PipeStream.get_State($this: Ref) returns ($result: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.get_InternalHandle($this: Ref) returns ($result: Ref);



procedure {:extern} System.Runtime.InteropServices.SafeHandle.get_IsClosed($this: Ref) returns ($result: bool);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeServerStream.CheckDisconnectOperations($this: Ref);



procedure {:extern} System.InvalidOperationException.#ctor($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.Stream_Close($this: Ref);



procedure System.IO.Pipes2.PipeStream.STREAMClose($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.Stream_Dispose($this: Ref);



procedure System.IO.Pipes2.PipeStream.STREAMDispose($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_Flush($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.Flush($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_ReadByte($this: Ref) returns ($result: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.ReadByte($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_WriteByte$System.Byte($this: Ref, value$in: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.WriteByte$System.Byte($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_Read$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int) returns ($result: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.Read$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_ReadAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.PipeStream.ReadAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_BeginRead$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.BeginRead$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_EndRead$System.IAsyncResult($this: Ref, asyncResult$in: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.EndRead$System.IAsyncResult($this: Ref, asyncResult$in: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_Write$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.Write$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);
  modifies $Alloc, $Exception, $Heap;



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_WriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.PipeStream.WriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_BeginWrite$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.BeginWrite$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_EndWrite$System.IAsyncResult($this: Ref, asyncResult$in: Ref);



procedure System.IO.Pipes2.PipeStream.EndWrite$System.IAsyncResult($this: Ref, asyncResult$in: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_IsConnected($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.get_IsConnected($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_IsAsync($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.get_IsAsync($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_IsMessageComplete($this: Ref) returns ($result: bool);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.PipeStream.get_IsMessageComplete($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_SafePipeHandle($this: Ref) returns ($result: Ref);



procedure {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.get_SafePipeHandle($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_CanRead($this: Ref) returns ($result: bool);



procedure {:System.Diagnostics.Contracts.Pure} System.IO.Pipes2.PipeStream.get_CanRead($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_CanWrite($this: Ref) returns ($result: bool);



procedure {:System.Diagnostics.Contracts.Pure} System.IO.Pipes2.PipeStream.get_CanWrite($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_CanSeek($this: Ref) returns ($result: bool);



procedure {:System.Diagnostics.Contracts.Pure} System.IO.Pipes2.PipeStream.get_CanSeek($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_Length($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.get_Length($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeServerStream.get_PipeStream_Position($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.get_Position($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeServerStream.set_PipeStream_Position$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.PipeStream.set_Position$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_SetLength$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.PipeStream.SetLength$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeServerStream.PipeStream_Seek$System.Int64$System.IO.SeekOrigin($this: Ref, offset$in: int, origin$in: int) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.Seek$System.Int64$System.IO.SeekOrigin($this: Ref, offset$in: int, origin$in: int) returns ($result: int);



const {:value "fullPipeName is null or empty"} unique $string_literal_fullPipeName$is$null$or$empty_2: Ref;

procedure {:extern} System.NotImplementedException.#ctor$System.String($this: Ref, message$in: Ref);



const {:value "invalid pipe direction"} unique $string_literal_invalid$pipe$direction_3: Ref;

const {:value "inBufferSize is negative"} unique $string_literal_inBufferSize$is$negative_4: Ref;

const {:value "outBufferSize is negative"} unique $string_literal_outBufferSize$is$negative_5: Ref;

const {:value "maxNumberOfServerInstances is invalid"} unique $string_literal_maxNumberOfServerInstances$is$invalid_6: Ref;

const {:value "transmissionMode is out of range"} unique $string_literal_transmissionMode$is$out$of$range_7: Ref;

const {:value "\\.\pipe\"} unique $string_literal_$$.$pipe$_8: Ref;

procedure {:extern} System.String.Concat$System.String$System.String(str0$in: Ref, str1$in: Ref) returns ($result: Ref);



procedure {:extern} System.IO.Path.GetFullPath$System.String(path$in: Ref) returns ($result: Ref);



const {:value "\\.\pipe\anonymous"} unique $string_literal_$$.$pipe$anonymous_9: Ref;

procedure {:extern} System.String.Equals$System.String$System.String$System.StringComparison(a$in: Ref, b$in: Ref, comparisonType$in: int) returns ($result: bool);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.GetSecAttrs$System.IO.HandleInheritability(inheritability$in: int) returns ($result: Ref);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.NamedPipeServerStream.WaitForConnection($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.CheckConnectOperationsServerWithHandle($this: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.WaitForConnectionCoreAsync$System.Threading.CancellationToken($this: Ref, cancellationToken$in: Ref) returns ($result: Ref);



procedure {:extern} System.Threading.Tasks.Task.GetAwaiter($this: Ref) returns ($result: Ref);



procedure {:extern} System.Runtime.CompilerServices.TaskAwaiter.GetResult($this: Ref);



procedure {:extern} System.Threading.CancellationToken.get_IsCancellationRequested($this: Ref) returns ($result: bool);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeServerStream.Disconnect($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeServerStream.GetImpersonationUserName($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.CheckWriteOperations($this: Ref);
  modifies $Alloc, $Exception;



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.WinIOError$System.Int32($this: Ref, errorCode$in: int) returns ($result: Ref);
  modifies $Heap, $Alloc;



procedure {:extern} System.Object.ToString($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.ConnectionCompletionSource.#ctor$System.IO.Pipes2.NamedPipeServerStream$System.Threading.CancellationToken($this: Ref, server$in: Ref, cancellationToken$in: Ref);



function T$System.IO.Pipes2.ConnectionCompletionSource() : Ref;

const unique T$System.IO.Pipes2.ConnectionCompletionSource: int;

procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeCompletionSource`1.get_Overlapped($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeCompletionSource`1.ReleaseResources($this: Ref);



procedure System.IO.Pipes2.PipeCompletionSource`1.SetCompletedSynchronously($this: Ref);



procedure System.IO.Pipes2.PipeCompletionSource`1.RegisterForCancellation($this: Ref);



procedure {:extern} System.Threading.Tasks.TaskCompletionSource`1.get_Task($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeServerStream.STATE$System_IO_Pipes2_NamedPipeServerStream_PipeStream_ReadSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_PipeStream_WriteSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_Stream_Close~System_IO_Pipes2_NamedPipeServerStream_PipeStream_WriteSystem_BytearraySystem_Int32System_Int32~STATE$System_IO_Pipes2_NamedPipeServerStream_PipeStream_ReadSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_PipeStream_WriteSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_Stream_Close$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);
  modifies $Exception, $Alloc, $Heap;



implementation System.IO.Pipes2.NamedPipeServerStream.STATE$System_IO_Pipes2_NamedPipeServerStream_PipeStream_ReadSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_PipeStream_WriteSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_Stream_Close~System_IO_Pipes2_NamedPipeServerStream_PipeStream_WriteSystem_BytearraySystem_Int32System_Int32~STATE$System_IO_Pipes2_NamedPipeServerStream_PipeStream_ReadSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_PipeStream_WriteSystem_BytearraySystem_Int32System_Int32$System_IO_Pipes2_NamedPipeServerStream_Stream_Close$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var local_0_int: int;
  var local_1_int: int;
  var local_2_Ref: Ref;
  var local_3_Ref: Ref;
  var local_4_Ref: Ref;
  var local_5_Ref: Ref;
  var local_6_Ref: Ref;
  var local_7_Ref: Ref;
  var local_8_Ref: Ref;
  var local_9_Ref: Ref;
  var local_10_Ref: Ref;
  var local_11_Ref: Ref;
  var local_12_Ref: Ref;
  var local_13_Ref: Ref;
  var local_14_Ref: Ref;
  var local_15_Ref: Ref;
  var local_16_Ref: Ref;
  var local_17_Ref: Ref;
  var local_18_Ref: Ref;
  var local_19_Ref: Ref;
  var local_20_Ref: Ref;
  var local_21_Ref: Ref;
  var local_22_Ref: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    $Exception := null;
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    assume {:breadcrumb 83} true;
    local_0_int := 0;
    local_1_int := 25;
    call {:si_unique_call 65} System.IO.Pipes2.PipeStream.Write$System.Bytearray$System.Int32$System.Int32($this, buffer, offset, count);
    goto anon32_Then, anon32_Else;

  anon32_Then:
    assume {:partition} $Exception != null;
    assert false;
    goto catch0;

  anon32_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    $label := -1;
    goto finally0;

  catch0:
    $localExc := $Exception;
    $Exception := null;
    goto anon33_Then, anon33_Else;

  anon33_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.OverflowException());
    local_2_Ref := $localExc;
    local_0_int := 1;
    $label := -1;
    goto finally0;

  anon33_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.OverflowException());
    goto anon34_Then, anon34_Else;

  anon34_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.IndexOutOfRangeException());
    local_3_Ref := $localExc;
    local_0_int := 2;
    $label := -1;
    goto finally0;

  anon34_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.IndexOutOfRangeException());
    goto anon35_Then, anon35_Else;

  anon35_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.NullReferenceException());
    local_4_Ref := $localExc;
    local_0_int := 3;
    $label := -1;
    goto finally0;

  anon35_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.NullReferenceException());
    goto anon36_Then, anon36_Else;

  anon36_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.DivideByZeroException());
    local_5_Ref := $localExc;
    local_0_int := 4;
    $label := -1;
    goto finally0;

  anon36_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.DivideByZeroException());
    goto anon37_Then, anon37_Else;

  anon37_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.InvalidCastException());
    local_6_Ref := $localExc;
    local_0_int := 5;
    $label := -1;
    goto finally0;

  anon37_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.InvalidCastException());
    goto anon38_Then, anon38_Else;

  anon38_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.ArgumentNullException());
    local_7_Ref := $localExc;
    local_0_int := 6;
    $label := -1;
    goto finally0;

  anon38_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.ArgumentNullException());
    goto anon39_Then, anon39_Else;

  anon39_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.ArgumentOutOfRangeException());
    local_8_Ref := $localExc;
    local_0_int := 7;
    $label := -1;
    goto finally0;

  anon39_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.ArgumentOutOfRangeException());
    goto anon40_Then, anon40_Else;

  anon40_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.ArgumentException());
    local_9_Ref := $localExc;
    local_0_int := 8;
    $label := -1;
    goto finally0;

  anon40_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.ArgumentException());
    goto anon41_Then, anon41_Else;

  anon41_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.IO.FileNotFoundException());
    local_10_Ref := $localExc;
    local_0_int := 9;
    $label := -1;
    goto finally0;

  anon41_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.IO.FileNotFoundException());
    goto anon42_Then, anon42_Else;

  anon42_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.IO.DirectoryNotFoundException());
    local_11_Ref := $localExc;
    local_0_int := 10;
    $label := -1;
    goto finally0;

  anon42_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.IO.DirectoryNotFoundException());
    goto anon43_Then, anon43_Else;

  anon43_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.IO.PathTooLongException());
    local_12_Ref := $localExc;
    local_0_int := 11;
    $label := -1;
    goto finally0;

  anon43_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.IO.PathTooLongException());
    goto anon44_Then, anon44_Else;

  anon44_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.IO.EndOfStreamException());
    local_13_Ref := $localExc;
    local_0_int := 12;
    $label := -1;
    goto finally0;

  anon44_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.IO.EndOfStreamException());
    goto anon45_Then, anon45_Else;

  anon45_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.IO.IOException());
    local_14_Ref := $localExc;
    local_0_int := 13;
    $label := -1;
    goto finally0;

  anon45_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.IO.IOException());
    goto anon46_Then, anon46_Else;

  anon46_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.ObjectDisposedException());
    local_15_Ref := $localExc;
    local_0_int := 14;
    $label := -1;
    goto finally0;

  anon46_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.ObjectDisposedException());
    goto anon47_Then, anon47_Else;

  anon47_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.InvalidOperationException());
    local_16_Ref := $localExc;
    local_0_int := 15;
    $label := -1;
    goto finally0;

  anon47_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.InvalidOperationException());
    goto anon48_Then, anon48_Else;

  anon48_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.NotSupportedException());
    local_17_Ref := $localExc;
    local_0_int := 16;
    $label := -1;
    goto finally0;

  anon48_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.NotSupportedException());
    goto anon49_Then, anon49_Else;

  anon49_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.UnauthorizedAccessException());
    local_18_Ref := $localExc;
    local_0_int := 21;
    $label := -1;
    goto finally0;

  anon49_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.UnauthorizedAccessException());
    goto anon50_Then, anon50_Else;

  anon50_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.NotImplementedException());
    local_19_Ref := $localExc;
    local_0_int := 22;
    $label := -1;
    goto finally0;

  anon50_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.NotImplementedException());
    goto anon51_Then, anon51_Else;

  anon51_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.RankException());
    local_20_Ref := $localExc;
    local_0_int := 23;
    $label := -1;
    goto finally0;

  anon51_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.RankException());
    goto anon52_Then, anon52_Else;

  anon52_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.TimeoutException());
    local_21_Ref := $localExc;
    local_0_int := 24;
    $label := -1;
    goto finally0;

  anon52_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.TimeoutException());
    goto anon53_Then, anon53_Else;

  anon53_Then:
    assume {:partition} $Subtype($DynamicType($localExc), T$System.Exception());
    local_22_Ref := $localExc;
    local_0_int := 25;
    $label := -1;
    goto finally0;

  anon53_Else:
    assume {:partition} !$Subtype($DynamicType($localExc), T$System.Exception());
    goto anon54_Then, anon54_Else;

  anon54_Then:
    assume {:partition} false;
    return;

  anon54_Else:
    assume {:partition} !false;
    goto anon25;

  anon25:
    $Exception := $localExc;
    $label := -1;
    goto finally0;

  finally0:
    goto anon55_Then, anon55_Else;

  anon55_Then:
    assume {:partition} true;
    goto continuation0;

  anon55_Else:
    assume {:partition} !true;
    goto continuation0;

  continuation0:
    goto anon56_Then, anon56_Else;

  anon56_Then:
    assume {:partition} $Exception != null;
    return;

  anon56_Else:
    assume {:partition} $Exception == null;
    goto anon28;

  anon28:
    goto anon57_Then, anon57_Else;

  anon57_Then:
    assume {:partition} local_0_int == local_1_int;
    goto anon31;

  anon57_Else:
    assume {:partition} local_0_int != local_1_int;
    goto anon31;

  anon31:
    assert (if local_0_int == local_1_int then 0 else 1) != 0;
    return;
}



procedure T$System.IO.Pipes2.NamedPipeServerStream.#cctor();



function TResult$T$System.IO.Pipes2.PipeCompletionSource`1(parent: Ref) : Ref;

function T$System.IO.Pipes2.PipeCompletionSource`1(TResult: Ref) : Ref;

const unique T$System.IO.Pipes2.PipeCompletionSource`1: int;

var F$System.IO.Pipes2.PipeCompletionSource`1.NoResult: int;

var F$System.IO.Pipes2.PipeCompletionSource`1.ResultSuccess: int;

var F$System.IO.Pipes2.PipeCompletionSource`1.ResultError: int;

var F$System.IO.Pipes2.PipeCompletionSource`1.RegisteringCancellation: int;

var F$System.IO.Pipes2.PipeCompletionSource`1.CompletedCallback: int;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._cancellationToken: Field;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._threadPoolBinding: Field;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._cancellationRegistration: Field;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._errorCode: Field;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._overlapped: Field;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._state: Field;

const unique F$System.IO.Pipes2.PipeCompletionSource`1._cancellationHasBeenRegistered: Field;

procedure System.IO.Pipes2.PipeCompletionSource`1.#ctor$System.Threading.ThreadPoolBoundHandle$System.Threading.CancellationToken$System.Object($this: Ref, handle$in: Ref, cancellationToken$in: Ref, pinData$in: Ref);



procedure System.Threading.CancellationToken.#default_ctor($this: Ref);



function {:extern} T$System.Threading.CancellationToken() : Ref;

const {:extern} unique T$System.Threading.CancellationToken: int;

procedure System.Threading.CancellationTokenRegistration.#default_ctor($this: Ref);



function {:extern} T$System.Threading.CancellationTokenRegistration() : Ref;

const {:extern} unique T$System.Threading.CancellationTokenRegistration: int;

procedure {:extern} System.Threading.Tasks.TaskCompletionSource`1.#ctor($this: Ref);



const {:value "handle is null"} unique $string_literal_handle$is$null_10: Ref;

procedure System.Threading.ThreadPoolBoundHandle.AllocateNativeOverlapped``1$System.IO.Pipes2.PipeCompletionSource$``0$$System.Object($this: Ref, pipeCompletionSource$in: Ref, pinData$in: Ref, TResult: Ref) returns ($result: Ref);



const {:value "Cannot register for cancellation twice"} unique $string_literal_Cannot$register$for$cancellation$twice_11: Ref;

procedure {:extern} System.Threading.CancellationTokenRegistration.Dispose($this: Ref);



procedure System.IO.Pipes2.PipeCompletionSource`1.AsyncCallback$System.UInt32$System.UInt32($this: Ref, errorCode$in: int, numBytes$in: int);



procedure {:extern} System.Threading.Interlocked.Exchange$System.Int32$$System.Int32(location1$in: int, value$in: int) returns (location1$out: int, $result: int);



procedure System.IO.Pipes2.PipeCompletionSource`1.CompleteCallback$System.Int32($this: Ref, resultState$in: int);



procedure System.IO.Pipes2.PipeCompletionSource`1.HandleError$System.Int32($this: Ref, errorCode$in: int);



procedure System.IO.Pipes2.PipeCompletionSource`1.Cancel($this: Ref);



procedure System.Threading.ThreadPoolBoundHandle.get_Handle($this: Ref) returns ($result: Ref);



const {:value "CancelIoEx finished with error code {0}."} unique $string_literal_CancelIoEx$finished$with$error$code$$0$._12: Ref;

procedure {:extern} System.Diagnostics.Debug.WriteLine$System.String$System.Objectarray(format$in: Ref, args$in: Ref);



procedure System.IO.Pipes2.PipeCompletionSource`1.HandleUnexpectedCancellation($this: Ref);



procedure {:extern} System.Threading.Tasks.TaskCompletionSource`1.TrySetCanceled($this: Ref) returns ($result: bool);



const {:value "Unexpected result state "} unique $string_literal_Unexpected$result$state$_13: Ref;

procedure {:extern} System.String.Concat$System.Object$System.Object(arg0$in: Ref, arg1$in: Ref) returns ($result: Ref);



procedure {:extern} System.Threading.CancellationToken.get_CanBeCanceled($this: Ref) returns ($result: bool);



procedure T$System.IO.Pipes2.PipeCompletionSource`1.#cctor();



function T$System.IO.Pipes2.PipeStream() : Ref;

const unique T$System.IO.Pipes2.PipeStream: int;

var F$System.IO.Pipes2.PipeStream.AnonymousPipeName: Ref;

var F$System.IO.Pipes2.PipeStream._DefaultCopyBufferSize: int;

var F$System.IO.Pipes2.PipeStream.CheckOperationsRequiresSetHandle: bool;

var F$System.IO.Pipes2.PipeStream.s_zeroTask: Ref;

const unique F$System.IO.Pipes2.PipeStream._handle: Field;

const unique F$System.IO.Pipes2.PipeStream._canRead: Field;

const unique F$System.IO.Pipes2.PipeStream._canWrite: Field;

const unique F$System.IO.Pipes2.PipeStream._isAsync: Field;

const unique F$System.IO.Pipes2.PipeStream._isMessageComplete: Field;

const unique F$System.IO.Pipes2.PipeStream._isFromExistingHandle: Field;

const unique F$System.IO.Pipes2.PipeStream._isHandleExposed: Field;

const unique F$System.IO.Pipes2.PipeStream._readMode: Field;

const unique F$System.IO.Pipes2.PipeStream._transmissionMode: Field;

const unique F$System.IO.Pipes2.PipeStream._pipeDirection: Field;

const unique F$System.IO.Pipes2.PipeStream._outBufferSize: Field;

const unique F$System.IO.Pipes2.PipeStream._state: Field;

var F$System.IO.Pipes2.PipeStream.t_singleByteArray: Ref;

var F$System.IO.Pipes2.PipeStream.Null: Ref;

const unique F$System.IO.Pipes2.PipeStream._activeReadWriteTask: Field;

const unique F$System.IO.Pipes2.PipeStream._asyncActiveSemaphore: Field;

const unique F$System.IO.Pipes2.PipeStream._threadPoolBinding: Field;

procedure System.IO.Pipes2.PipeStream.#ctor$System.IO.Pipes2.PipeDirection$System.Int32($this: Ref, direction$in: int, bufferSize$in: int);



procedure System.IO.Pipes2.PipeStream.Init$System.IO.Pipes2.PipeDirection$System.IO.Pipes2.PipeTransmissionMode$System.Int32($this: Ref, direction$in: int, transmissionMode$in: int, outBufferSize$in: int);



procedure System.IO.Pipes2.PipeStream.InitializeAsyncHandle$Microsoft.Win32.SafeHandles.SafePipeHandle($this: Ref, handle$in: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.PipeStream.ReadAsync2$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.CheckReadWriteArgs$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);
  modifies $Alloc, $Exception;



procedure System.IO.Pipes2.PipeStream.CheckReadOperations($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.ReadCore$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.Read2$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.STREAMReadAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.UpdateMessageCompletion$System.Boolean($this: Ref, completion$in: bool);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.PipeStream.ReadAsyncCore$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.STREAMBeginRead$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



function {:extern} T$System.Int32() : Ref;

const {:extern} unique T$System.Int32: int;

procedure System.Threading.Tasks.TaskToApm.End``1$System.IAsyncResult(asyncResult$in: Ref, TResult: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.STREAMEndRead$System.IAsyncResult($this: Ref, asyncResult$in: Ref) returns ($result: int);



const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._isRead: Field;

procedure {:extern} System.Threading.Tasks.Task`1.GetAwaiter($this: Ref) returns ($result: Ref);



procedure {:extern} System.Runtime.CompilerServices.TaskAwaiter`1.GetResult($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.FinishTrackingAsyncOperation($this: Ref);



procedure System.IO.Pipes2.PipeStream.WriteAsync2$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);
  modifies $Alloc, $Exception, $Heap;



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.WriteCore$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);
  modifies $Alloc, $Exception, $Heap;



implementation System.IO.Pipes2.PipeStream.Write$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var $tmp0: Ref;
  var $tmp1: Ref;
  var $tmp2: Ref;
  var $tmp3: Ref;
  var $tmp4: Ref;
  var $tmp5: bool;
  var $tmp6: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    assume {:breadcrumb 104} true;
    assume $this != null;
    goto anon24_Then, anon24_Else;

  anon24_Then:
    assume {:partition} Union2Bool(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._isAsync));
    call {:si_unique_call 66} $tmp0 := System.Threading.CancellationToken.get_None();
    goto anon25_Then, anon25_Else;

  anon25_Then:
    assume {:partition} $Exception != null;
    return;

  anon25_Else:
    assume {:partition} $Exception == null;
    goto anon3;

  anon3:
    call {:si_unique_call 67} $tmp1 := System.Threading.CancellationToken.#copy_ctor($tmp0);
    call {:si_unique_call 68} $tmp2 := System.IO.Pipes2.PipeStream.WriteAsync2$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this, buffer, offset, count, $tmp1);
    goto anon26_Then, anon26_Else;

  anon26_Then:
    assume {:partition} $Exception != null;
    return;

  anon26_Else:
    assume {:partition} $Exception == null;
    goto anon5;

  anon5:
    call {:si_unique_call 69} $tmp3 := System.Threading.Tasks.Task.GetAwaiter($tmp2);
    goto anon27_Then, anon27_Else;

  anon27_Then:
    assume {:partition} $Exception != null;
    return;

  anon27_Else:
    assume {:partition} $Exception == null;
    goto anon7;

  anon7:
    $tmp4 := $tmp3;
    call {:si_unique_call 70} System.Runtime.CompilerServices.TaskAwaiter.GetResult($tmp4);
    goto anon28_Then, anon28_Else;

  anon28_Then:
    assume {:partition} $Exception != null;
    return;

  anon28_Else:
    assume {:partition} $Exception == null;
    goto anon9;

  anon9:
    return;

  anon24_Else:
    assume {:partition} !Union2Bool(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._isAsync));
    goto anon11;

  anon11:
    call {:si_unique_call 71} System.IO.Pipes2.PipeStream.CheckReadWriteArgs$System.Bytearray$System.Int32$System.Int32($this, buffer, offset, count);
    goto anon29_Then, anon29_Else;

  anon29_Then:
    assume {:partition} $Exception != null;
    return;

  anon29_Else:
    assume {:partition} $Exception == null;
    goto anon13;

  anon13:
    call {:si_unique_call 72} $tmp5 := System.IO.Pipes2.PipeStream.get_CanWrite($this);
    goto anon30_Then, anon30_Else;

  anon30_Then:
    assume {:partition} $Exception != null;
    return;

  anon30_Else:
    assume {:partition} $Exception == null;
    goto anon15;

  anon15:
    goto anon31_Then, anon31_Else;

  anon31_Then:
    assume {:partition} !$tmp5;
    call {:si_unique_call 73} $tmp6 := System.IO.Error.GetWriteNotSupported();
    goto anon32_Then, anon32_Else;

  anon32_Then:
    assume {:partition} $Exception != null;
    return;

  anon32_Else:
    assume {:partition} $Exception == null;
    goto anon18;

  anon18:
    $Exception := $tmp6;
    return;

  anon31_Else:
    assume {:partition} $tmp5;
    goto anon20;

  anon20:
    call {:si_unique_call 74} System.IO.Pipes2.PipeStream.CheckWriteOperations($this);
    goto anon33_Then, anon33_Else;

  anon33_Then:
    assume {:partition} $Exception != null;
    return;

  anon33_Else:
    assume {:partition} $Exception == null;
    goto anon22;

  anon22:
    call {:si_unique_call 75} System.IO.Pipes2.PipeStream.WriteCore$System.Bytearray$System.Int32$System.Int32($this, buffer, offset, count);
    goto anon34_Then, anon34_Else;

  anon34_Then:
    assume {:partition} $Exception != null;
    return;

  anon34_Else:
    assume {:partition} $Exception == null;
    return;
}



procedure System.IO.Pipes2.PipeStream.Write2$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);



procedure System.IO.Pipes2.PipeStream.STREAMWriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.PipeStream.WriteAsyncCore$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);
  modifies $Alloc, $Exception, $Heap;



implementation System.IO.Pipes2.PipeStream.WriteAsync2$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var cancellationToken: Ref;
  var $tmp0: bool;
  var $tmp1: Ref;
  var $tmp2: bool;
  var $tmp3: Ref;
  var $tmp4: Ref;
  var $tmp5: Ref;
  var $tmp6: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    cancellationToken := cancellationToken$in;
    assume {:breadcrumb 106} true;
    call {:si_unique_call 76} System.IO.Pipes2.PipeStream.CheckReadWriteArgs$System.Bytearray$System.Int32$System.Int32($this, buffer, offset, count);
    goto anon27_Then, anon27_Else;

  anon27_Then:
    assume {:partition} $Exception != null;
    return;

  anon27_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    call {:si_unique_call 77} $tmp0 := System.IO.Pipes2.PipeStream.get_CanWrite($this);
    goto anon28_Then, anon28_Else;

  anon28_Then:
    assume {:partition} $Exception != null;
    return;

  anon28_Else:
    assume {:partition} $Exception == null;
    goto anon4;

  anon4:
    goto anon29_Then, anon29_Else;

  anon29_Then:
    assume {:partition} !$tmp0;
    call {:si_unique_call 78} $tmp1 := System.IO.Error.GetWriteNotSupported();
    goto anon30_Then, anon30_Else;

  anon30_Then:
    assume {:partition} $Exception != null;
    return;

  anon30_Else:
    assume {:partition} $Exception == null;
    goto anon7;

  anon7:
    $Exception := $tmp1;
    return;

  anon29_Else:
    assume {:partition} $tmp0;
    goto anon9;

  anon9:
    call {:si_unique_call 79} $tmp2 := System.Threading.CancellationToken.get_IsCancellationRequested(cancellationToken);
    goto anon31_Then, anon31_Else;

  anon31_Then:
    assume {:partition} $Exception != null;
    return;

  anon31_Else:
    assume {:partition} $Exception == null;
    goto anon11;

  anon11:
    goto anon32_Then, anon32_Else;

  anon32_Then:
    assume {:partition} $tmp2;
    $result := null;
    return;

  anon32_Else:
    assume {:partition} !$tmp2;
    goto anon14;

  anon14:
    call {:si_unique_call 80} System.IO.Pipes2.PipeStream.CheckWriteOperations($this);
    goto anon33_Then, anon33_Else;

  anon33_Then:
    assume {:partition} $Exception != null;
    return;

  anon33_Else:
    assume {:partition} $Exception == null;
    goto anon16;

  anon16:
    assume $this != null;
    goto anon34_Then, anon34_Else;

  anon34_Then:
    assume {:partition} !Union2Bool(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._isAsync));
    call {:si_unique_call 81} $tmp3 := System.Threading.CancellationToken.#copy_ctor(cancellationToken);
    call {:si_unique_call 82} $tmp4 := System.IO.Pipes2.PipeStream.STREAMWriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this, buffer, offset, count, $tmp3);
    goto anon35_Then, anon35_Else;

  anon35_Then:
    assume {:partition} $Exception != null;
    return;

  anon35_Else:
    assume {:partition} $Exception == null;
    goto anon19;

  anon19:
    $result := $tmp4;
    return;

  anon34_Else:
    assume {:partition} Union2Bool(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._isAsync));
    goto anon21;

  anon21:
    goto anon36_Then, anon36_Else;

  anon36_Then:
    assume {:partition} count == 0;
    $result := null;
    return;

  anon36_Else:
    assume {:partition} count != 0;
    goto anon24;

  anon24:
    call {:si_unique_call 83} $tmp5 := System.Threading.CancellationToken.#copy_ctor(cancellationToken);
    call {:si_unique_call 84} $tmp6 := System.IO.Pipes2.PipeStream.WriteAsyncCore$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this, buffer, offset, count, $tmp5);
    goto anon37_Then, anon37_Else;

  anon37_Then:
    assume {:partition} $Exception != null;
    return;

  anon37_Else:
    assume {:partition} $Exception == null;
    goto anon26;

  anon26:
    $result := $tmp6;
    return;
}



implementation System.IO.Pipes2.PipeStream.STREAMWriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var cancellationToken: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    cancellationToken := cancellationToken$in;
    assume {:breadcrumb 108} true;
    $result := null;
    return;
}



procedure System.IO.Pipes2.PipeStream.STREAMBeginWrite$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.STREAMEndWrite$System.IAsyncResult($this: Ref, asyncResult$in: Ref);



implementation System.IO.Pipes2.PipeStream.CheckReadWriteArgs$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var $tmp0: Ref;
  var $tmp1: Ref;
  var $tmp2: Ref;
  var $tmp3: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    assume {:breadcrumb 111} true;
    goto anon12_Then, anon12_Else;

  anon12_Then:
    assume {:partition} buffer == null;
    call {:si_unique_call 85} $tmp0 := Alloc();
    call {:si_unique_call 86} System.ArgumentNullException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.ArgumentNullException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.ArgumentNullException;
    $Exception := $tmp0;
    return;

  anon12_Else:
    assume {:partition} buffer != null;
    goto anon3;

  anon3:
    goto anon13_Then, anon13_Else;

  anon13_Then:
    assume {:partition} offset < 0;
    call {:si_unique_call 87} $tmp1 := Alloc();
    call {:si_unique_call 88} System.ArgumentOutOfRangeException.#ctor($tmp1);
    assume $DynamicType($tmp1) == T$System.ArgumentOutOfRangeException();
    assume $TypeConstructor($DynamicType($tmp1)) == T$System.ArgumentOutOfRangeException;
    $Exception := $tmp1;
    return;

  anon13_Else:
    assume {:partition} 0 <= offset;
    goto anon6;

  anon6:
    goto anon14_Then, anon14_Else;

  anon14_Then:
    assume {:partition} count < 0;
    call {:si_unique_call 89} $tmp2 := Alloc();
    call {:si_unique_call 90} System.ArgumentOutOfRangeException.#ctor($tmp2);
    assume $DynamicType($tmp2) == T$System.ArgumentOutOfRangeException();
    assume $TypeConstructor($DynamicType($tmp2)) == T$System.ArgumentOutOfRangeException;
    $Exception := $tmp2;
    return;

  anon14_Else:
    assume {:partition} 0 <= count;
    goto anon9;

  anon9:
    goto anon15_Then, anon15_Else;

  anon15_Then:
    assume {:partition} $ArrayLength(buffer) - offset < count;
    call {:si_unique_call 91} $tmp3 := Alloc();
    call {:si_unique_call 92} System.ArgumentException.#ctor($tmp3);
    assume $DynamicType($tmp3) == T$System.ArgumentException();
    assume $TypeConstructor($DynamicType($tmp3)) == T$System.ArgumentException;
    $Exception := $tmp3;
    return;

  anon15_Else:
    assume {:partition} count <= $ArrayLength(buffer) - offset;
    return;
}



procedure {:System.Diagnostics.Conditional "DEBUG"} System.IO.Pipes2.PipeStream.DebugAssertReadWriteArgs$System.Bytearray$System.Int32$System.Int32$Microsoft.Win32.SafeHandles.SafePipeHandle(buffer$in: Ref, offset$in: int, count$in: int, handle$in: Ref);



const {:value "buffer is null"} unique $string_literal_buffer$is$null_14: Ref;

const {:value "offset is negative"} unique $string_literal_offset$is$negative_15: Ref;

const {:value "count is negative"} unique $string_literal_count$is$negative_16: Ref;

const {:value "offset + count is too big"} unique $string_literal_offset$$$count$is$too$big_17: Ref;

const {:value "handle is closed"} unique $string_literal_handle$is$closed_18: Ref;

procedure System.IO.Pipes2.PipeStream.get_SingleByteArray() returns ($result: Ref);



procedure {:extern} System.Runtime.InteropServices.SafeHandle.Dispose($this: Ref);



procedure System.IO.Pipes2.PipeStream.UninitializeAsyncHandle($this: Ref);



procedure System.IO.Pipes2.PipeStream.STREAMDispose$System.Boolean($this: Ref, disposing$in: bool);



procedure System.IO.Pipes2.PipeStream.set_IsConnected$System.Boolean($this: Ref, value$in: bool);



procedure System.IO.Pipes2.PipeStream.get_IsHandleExposed($this: Ref) returns ($result: bool);



implementation System.IO.Pipes2.PipeStream.get_CanWrite($this: Ref) returns ($result: bool)
{
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 127} true;
    assume $this != null;
    $result := Union2Bool(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._canWrite));
    return;
}



procedure System.IO.Pipes2.PipeStream.CheckPipePropertyOperations($this: Ref);



implementation System.IO.Pipes2.PipeStream.CheckWriteOperations($this: Ref)
{
  var $tmp0: Ref;
  var $tmp1: Ref;
  var $tmp2: Ref;
  var $tmp3: Ref;
  var $tmp4: Ref;
  var $tmp5: bool;
  var $tmp6: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 136} true;
    assume $this != null;
    goto anon25_Then, anon25_Else;

  anon25_Then:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) == 0;
    call {:si_unique_call 93} $tmp0 := Alloc();
    call {:si_unique_call 94} System.InvalidOperationException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.InvalidOperationException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.InvalidOperationException;
    $Exception := $tmp0;
    return;

  anon25_Else:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) != 0;
    goto anon3;

  anon3:
    assume $this != null;
    goto anon26_Then, anon26_Else;

  anon26_Then:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) == 3;
    call {:si_unique_call 95} $tmp1 := Alloc();
    call {:si_unique_call 96} System.InvalidOperationException.#ctor($tmp1);
    assume $DynamicType($tmp1) == T$System.InvalidOperationException();
    assume $TypeConstructor($DynamicType($tmp1)) == T$System.InvalidOperationException;
    $Exception := $tmp1;
    return;

  anon26_Else:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) != 3;
    goto anon6;

  anon6:
    assume $this != null;
    goto anon27_Then, anon27_Else;

  anon27_Then:
    assume {:partition} Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle) == null;
    call {:si_unique_call 97} $tmp2 := Alloc();
    call {:si_unique_call 98} System.InvalidOperationException.#ctor($tmp2);
    assume $DynamicType($tmp2) == T$System.InvalidOperationException();
    assume $TypeConstructor($DynamicType($tmp2)) == T$System.InvalidOperationException;
    $Exception := $tmp2;
    return;

  anon27_Else:
    assume {:partition} Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle) != null;
    goto anon9;

  anon9:
    assume $this != null;
    goto anon28_Then, anon28_Else;

  anon28_Then:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) == 2;
    call {:si_unique_call 99} $tmp3 := Alloc();
    call {:si_unique_call 100} System.IO.IOException.#ctor($tmp3);
    assume $DynamicType($tmp3) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp3)) == T$System.IO.IOException;
    $Exception := $tmp3;
    return;

  anon28_Else:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) != 2;
    goto anon12;

  anon12:
    assume $this != null;
    goto anon29_Then, anon29_Else;

  anon29_Then:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) != 4;
    assume $this != null;
    goto anon30_Then, anon30_Else;

  anon30_Then:
    assume {:partition} Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle) != null;
    goto anon16;

  anon30_Else:
    assume {:partition} Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle) == null;
    return;

  anon16:
    assume $this != null;
    $tmp4 := Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle);
    call {:si_unique_call 101} $tmp5 := System.Runtime.InteropServices.SafeHandle.get_IsClosed($tmp4);
    goto anon31_Then, anon31_Else;

  anon31_Then:
    assume {:partition} $Exception != null;
    return;

  anon31_Else:
    assume {:partition} $Exception == null;
    goto anon18;

  anon18:
    goto anon32_Then, anon32_Else;

  anon32_Then:
    assume {:partition} $tmp5;
    goto anon22;

  anon32_Else:
    assume {:partition} !$tmp5;
    return;

  anon29_Else:
    assume {:partition} Union2Int(Read($Heap, $this, F$System.IO.Pipes2.PipeStream._state)) == 4;
    goto anon22;

  anon22:
    call {:si_unique_call 102} $tmp6 := System.IO.Error.GetPipeNotOpen();
    goto anon33_Then, anon33_Else;

  anon33_Then:
    assume {:partition} $Exception != null;
    return;

  anon33_Else:
    assume {:partition} $Exception == null;
    goto anon24;

  anon24:
    $Exception := $tmp6;
    return;
}



procedure {:extern} System.GC.SuppressFinalize$System.Object(obj$in: Ref);



procedure System.IO.Pipes2.PipeStream.EnsureAsyncActiveSemaphoreInitialized($this: Ref) returns ($result: Ref);



procedure {:extern} System.Threading.SemaphoreSlim.#ctor$System.Int32$System.Int32($this: Ref, initialCount$in: int, maxCount$in: int);



function {:extern} T$System.Threading.SemaphoreSlim() : Ref;

const {:extern} unique T$System.Threading.SemaphoreSlim: int;

procedure System.IO.Pipes2.PipeStream.BeginReadInternal$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object$System.Boolean$System.Boolean($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref, serializeAsynchronously$in: bool, apm$in: bool) returns ($result: Ref);



procedure {:extern} System.Threading.SemaphoreSlim.WaitAsync($this: Ref) returns ($result: Ref);



procedure {:extern} System.Threading.SemaphoreSlim.Wait($this: Ref);



procedure {:extern} System.Threading.Tasks.Task.get_Status($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.BeginWriteInternal$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object$System.Boolean$System.Boolean($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref, serializeAsynchronously$in: bool, apm$in: bool) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.RunReadWriteTaskWhenReady$System.Threading.Tasks.Task$System.IO.Pipes2.PipeStream.ReadWriteTask($this: Ref, asyncWaiter$in: Ref, readWriteTask$in: Ref);



procedure {:extern} System.Threading.Tasks.Task.get_IsCompleted($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.RunReadWriteTask$System.IO.Pipes2.PipeStream.ReadWriteTask($this: Ref, readWriteTask$in: Ref);



const {:value "Expected no other readers or writers"} unique $string_literal_Expected$no$other$readers$or$writers_19: Ref;

const {:value "Must have been initialized in order to get here."} unique $string_literal_Must$have$been$initialized$in$order$to$get$here._20: Ref;

procedure {:extern} System.Threading.SemaphoreSlim.Release($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.GetPipePath$System.String$System.String(serverName$in: Ref, pipeName$in: Ref) returns ($result: Ref);



const {:value "\\"} unique $string_literal_$$_21: Ref;

const {:value "\pipe\"} unique $string_literal_$pipe$_22: Ref;

procedure {:extern} System.String.Concat$System.String$System.String$System.String$System.String(str0$in: Ref, str1$in: Ref, str2$in: Ref, str3$in: Ref) returns ($result: Ref);



procedure System.Threading.ThreadPoolBoundHandle.BindHandle$System.Runtime.InteropServices.SafeHandle(handle$in: Ref) returns ($result: Ref);



procedure System.Threading.ThreadPoolBoundHandle.Dispose($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.ReadFileNative$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Bytearray$System.Int32$System.Int32$System.Threading.NativeOverlapped$$System.Int32$($this: Ref, handle$in: Ref, buffer$in: Ref, offset$in: int, count$in: int, overlapped$in: Ref, errorCode$in: int) returns (errorCode$out: int, $result: int);



const {:value "PipeStream's ReadCore is likely broken."} unique $string_literal_PipeStream's$ReadCore$is$likely$broken._23: Ref;

procedure PipeStream.ReadWriteCompletionSource.#ctor$System.IO.Pipes2.PipeStream$System.Bytearray$System.Threading.CancellationToken$System.Boolean($this: Ref, pipeStream$in: Ref, buffer$in: Ref, cancellationToken$in: Ref, isWrite$in: bool);
  modifies $Heap, $Alloc;



function T$PipeStream.ReadWriteCompletionSource() : Ref;

const unique T$PipeStream.ReadWriteCompletionSource: int;

procedure {:System.Runtime.CompilerServices.CompilerGenerated} PipeStream.ReadWriteCompletionSource.get_Overlapped($this: Ref) returns ($result: Ref);



const {:extern} unique F$System.Threading.NativeOverlapped.InternalLow: Field;

procedure PipeStream.ReadWriteCompletionSource.ReleaseResources($this: Ref);
  modifies $Alloc, $Exception;



procedure PipeStream.ReadWriteCompletionSource.RegisterForCancellation($this: Ref);
  modifies $Alloc, $Exception;



procedure {:System.Runtime.CompilerServices.CompilerGenerated} PipeStream.ReadWriteCompletionSource.get_Task($this: Ref) returns ($result: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.WriteFileNative$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Bytearray$System.Int32$System.Int32$System.Threading.NativeOverlapped$$System.Int32$($this: Ref, handle$in: Ref, buffer$in: Ref, offset$in: int, count$in: int, overlapped$in: Ref, errorCode$in: int) returns (errorCode$out: int, $result: int);



const {:value "PipeStream's WriteCore is likely broken."} unique $string_literal_PipeStream's$WriteCore$is$likely$broken._24: Ref;

implementation System.IO.Pipes2.PipeStream.WriteCore$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var local_0_int: int;
  var local_1_int: int;
  var $tmp0: Ref;
  var $tmp1: int;
  var $tmp2: Ref;
  var $tmp3: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    assume {:breadcrumb 157} true;
    local_0_int := 0;
    assume $this != null;
    call {:si_unique_call 103} $tmp0 := $BoxFromInt(0);
    call {:si_unique_call 104} local_0_int, $tmp1 := System.IO.Pipes2.PipeStream.WriteFileNative$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Bytearray$System.Int32$System.Int32$System.Threading.NativeOverlapped$$System.Int32$($this, Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle), buffer, offset, count, $tmp0, local_0_int);
    goto anon10_Then, anon10_Else;

  anon10_Then:
    assume {:partition} $Exception != null;
    return;

  anon10_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    local_1_int := $tmp1;
    goto anon11_Then, anon11_Else;

  anon11_Then:
    assume {:partition} local_1_int == -1;
    call {:si_unique_call 105} $tmp2 := System.IO.Pipes2.PipeStream.WinIOError$System.Int32($this, local_0_int);
    goto anon12_Then, anon12_Else;

  anon12_Then:
    assume {:partition} $Exception != null;
    return;

  anon12_Else:
    assume {:partition} $Exception == null;
    goto anon5;

  anon5:
    $Exception := $tmp2;
    return;

  anon11_Else:
    assume {:partition} local_1_int != -1;
    goto anon7;

  anon7:
    goto anon13_Then, anon13_Else;

  anon13_Then:
    assume {:partition} local_1_int < 0;
    call {:si_unique_call 106} $tmp3 := Alloc();
    call {:si_unique_call 107} System.NotImplementedException.#ctor$System.String($tmp3, $string_literal_PipeStream's$WriteCore$is$likely$broken._24);
    assume $DynamicType($tmp3) == T$System.NotImplementedException();
    assume $TypeConstructor($DynamicType($tmp3)) == T$System.NotImplementedException;
    $Exception := $tmp3;
    return;

  anon13_Else:
    assume {:partition} 0 <= local_1_int;
    return;
}



implementation System.IO.Pipes2.PipeStream.WriteAsyncCore$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref)
{
  var buffer: Ref;
  var offset: int;
  var count: int;
  var cancellationToken: Ref;
  var local_0_Ref: Ref;
  var $tmp0: Ref;
  var $tmp1: Ref;
  var local_1_int: int;
  var $tmp2: Ref;
  var $tmp3: int;
  var $tmp4: Ref;
  var $tmp5: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    buffer := buffer$in;
    offset := offset$in;
    count := count$in;
    cancellationToken := cancellationToken$in;
    assume {:breadcrumb 158} true;
    call {:si_unique_call 108} $tmp0 := Alloc();
    call {:si_unique_call 109} $tmp1 := System.Threading.CancellationToken.#copy_ctor(cancellationToken);
    call {:si_unique_call 110} PipeStream.ReadWriteCompletionSource.#ctor$System.IO.Pipes2.PipeStream$System.Bytearray$System.Threading.CancellationToken$System.Boolean($tmp0, $this, buffer, $tmp1, true);
    assume $DynamicType($tmp0) == T$PipeStream.ReadWriteCompletionSource();
    assume $TypeConstructor($DynamicType($tmp0)) == T$PipeStream.ReadWriteCompletionSource;
    local_0_Ref := $tmp0;
    local_1_int := 0;
    assume $this != null;
    call {:si_unique_call 111} $tmp2 := PipeStream.ReadWriteCompletionSource.get_Overlapped(local_0_Ref);
    goto anon19_Then, anon19_Else;

  anon19_Then:
    assume {:partition} $Exception != null;
    return;

  anon19_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    call {:si_unique_call 112} local_1_int, $tmp3 := System.IO.Pipes2.PipeStream.WriteFileNative$Microsoft.Win32.SafeHandles.SafePipeHandle$System.Bytearray$System.Int32$System.Int32$System.Threading.NativeOverlapped$$System.Int32$($this, Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle), buffer, offset, count, $tmp2, local_1_int);
    goto anon20_Then, anon20_Else;

  anon20_Then:
    assume {:partition} $Exception != null;
    return;

  anon20_Else:
    assume {:partition} $Exception == null;
    goto anon4;

  anon4:
    goto anon21_Then, anon21_Else;

  anon21_Then:
    assume {:partition} $tmp3 == -1;
    goto anon7;

  anon21_Else:
    assume {:partition} $tmp3 != -1;
    goto anon7;

  anon7:
    goto anon22_Then, anon22_Else;

  anon22_Then:
    assume {:partition} (if $tmp3 == -1 then local_1_int != 997 else false);
    call {:si_unique_call 113} PipeStream.ReadWriteCompletionSource.ReleaseResources(local_0_Ref);
    goto anon23_Then, anon23_Else;

  anon23_Then:
    assume {:partition} $Exception != null;
    return;

  anon23_Else:
    assume {:partition} $Exception == null;
    goto anon10;

  anon10:
    call {:si_unique_call 114} $tmp4 := System.IO.Pipes2.PipeStream.WinIOError$System.Int32($this, local_1_int);
    goto anon24_Then, anon24_Else;

  anon24_Then:
    assume {:partition} $Exception != null;
    return;

  anon24_Else:
    assume {:partition} $Exception == null;
    goto anon12;

  anon12:
    $Exception := $tmp4;
    return;

  anon22_Else:
    assume {:partition} !(if $tmp3 == -1 then local_1_int != 997 else false);
    goto anon14;

  anon14:
    call {:si_unique_call 115} PipeStream.ReadWriteCompletionSource.RegisterForCancellation(local_0_Ref);
    goto anon25_Then, anon25_Else;

  anon25_Then:
    assume {:partition} $Exception != null;
    return;

  anon25_Else:
    assume {:partition} $Exception == null;
    goto anon16;

  anon16:
    call {:si_unique_call 116} $tmp5 := PipeStream.ReadWriteCompletionSource.get_Task(local_0_Ref);
    goto anon26_Then, anon26_Else;

  anon26_Then:
    assume {:partition} $Exception != null;
    return;

  anon26_Else:
    assume {:partition} $Exception == null;
    goto anon18;

  anon18:
    $result := $tmp5;
    return;
}



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.WaitForPipeDrain($this: Ref);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.PipeStream.get_TransmissionMode($this: Ref) returns ($result: int);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.PipeStream.get_InBufferSize($this: Ref) returns ($result: int);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.PipeStream.get_OutBufferSize($this: Ref) returns ($result: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.get_ReadMode($this: Ref) returns ($result: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.UpdateReadMode($this: Ref);



procedure {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} {:System.Security.SecurityCritical} System.IO.Pipes2.PipeStream.set_ReadMode$System.IO.Pipes2.PipeTransmissionMode($this: Ref, value$in: int);



procedure {:extern} System.Runtime.InteropServices.SafeHandle.SetHandleAsInvalid($this: Ref);



implementation System.IO.Pipes2.PipeStream.WinIOError$System.Int32($this: Ref, errorCode$in: int) returns ($result: Ref)
{
  var errorCode: int;
  var local_0_int: int;
  var $tmp0: Ref;
  var $tmp1: Ref;
  var $tmp2: Ref;
  var $tmp3: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    errorCode := errorCode$in;
    assume {:breadcrumb 169} true;
    local_0_int := errorCode;
    goto anon19_Then, anon19_Else;

  anon19_Then:
    assume {:partition} local_0_int <= 38;
    goto anon20_Then, anon20_Else;

  anon20_Then:
    assume {:partition} local_0_int == 6;
    goto IL_0048;

  anon20_Else:
    assume {:partition} local_0_int != 6;
    goto anon4;

  anon4:
    goto anon21_Then, anon21_Else;

  anon21_Then:
    assume {:partition} local_0_int == 38;
    goto IL_0042;

  anon21_Else:
    assume {:partition} local_0_int != 38;
    goto anon16;

  anon19_Else:
    assume {:partition} 38 < local_0_int;
    goto anon22_Then, anon22_Else;

  anon22_Then:
    assume {:partition} local_0_int != 109;
    goto anon23_Then, anon23_Else;

  anon23_Then:
    assume {:partition} local_0_int - 232 == 0;
    goto anon11;

  anon23_Else:
    assume {:partition} local_0_int - 232 != 0;
    goto anon24_Then, anon24_Else;

  anon24_Then:
    assume {:partition} local_0_int - 232 == 1;
    goto IL_0031;

  anon24_Else:
    assume {:partition} local_0_int - 232 != 1;
    goto anon11;

  anon11:
    goto IL_0033;

  IL_0031:
    goto IL_0035;

  IL_0033:
    goto anon16;

  anon22_Else:
    assume {:partition} local_0_int == 109;
    goto IL_0035;

  IL_0035:
    assume Union2Int(Int2Union(2)) == 2;
    $Heap := Write($Heap, $this, F$System.IO.Pipes2.PipeStream._state, Int2Union(2));
    call {:si_unique_call 117} $tmp0 := Alloc();
    call {:si_unique_call 118} System.IO.IOException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.IO.IOException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.IO.IOException;
    $result := $tmp0;
    return;

  IL_0042:
    call {:si_unique_call 119} $tmp1 := System.IO.Error.GetEndOfFile();
    goto anon25_Then, anon25_Else;

  anon25_Then:
    assume {:partition} $Exception != null;
    return;

  anon25_Else:
    assume {:partition} $Exception == null;
    goto anon13;

  anon13:
    $result := $tmp1;
    return;

  IL_0048:
    assume $this != null;
    $tmp2 := Read($Heap, $this, F$System.IO.Pipes2.PipeStream._handle);
    call {:si_unique_call 120} System.Runtime.InteropServices.SafeHandle.SetHandleAsInvalid($tmp2);
    goto anon26_Then, anon26_Else;

  anon26_Then:
    assume {:partition} $Exception != null;
    return;

  anon26_Else:
    assume {:partition} $Exception == null;
    goto anon15;

  anon15:
    assume Union2Int(Int2Union(2)) == 2;
    $Heap := Write($Heap, $this, F$System.IO.Pipes2.PipeStream._state, Int2Union(2));
    goto anon16;

  anon16:
    call {:si_unique_call 121} $tmp3 := System.IO.Win32Marshal.GetExceptionForWin32Error$System.Int32(errorCode);
    goto anon27_Then, anon27_Else;

  anon27_Then:
    assume {:partition} $Exception != null;
    return;

  anon27_Else:
    assume {:partition} $Exception == null;
    goto anon18;

  anon18:
    $result := $tmp3;
    return;
}



procedure System.IO.Pipes2.PipeStream.#cctor();



procedure {:extern} System.Threading.Tasks.Task.FromResult``1$``0(result$in: Ref, TResult: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.#ctor($this: Ref);



function T$System.IO.Pipes2.PipeStream.NullStream() : Ref;

const unique T$System.IO.Pipes2.PipeStream.NullStream: int;

var F$System.IO.Pipes2.PipeStream.NullStream.s_nullReadTask: Ref;

procedure {:extern} System.IO.Stream.#ctor($this: Ref);



procedure {:System.Diagnostics.Contracts.Pure} System.IO.Pipes2.PipeStream.NullStream.get_CanRead($this: Ref) returns ($result: bool);



procedure {:System.Diagnostics.Contracts.Pure} System.IO.Pipes2.PipeStream.NullStream.get_CanWrite($this: Ref) returns ($result: bool);



procedure {:System.Diagnostics.Contracts.Pure} System.IO.Pipes2.PipeStream.NullStream.get_CanSeek($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.NullStream.get_Length($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.NullStream.get_Position($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.NullStream.set_Position$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.PipeStream.NullStream.CopyToAsync$System.IO.Stream$System.Int32$System.Threading.CancellationToken($this: Ref, destination$in: Ref, bufferSize$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.Dispose$System.Boolean($this: Ref, disposing$in: bool);



procedure System.IO.Pipes2.PipeStream.NullStream.Flush($this: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.FlushAsync$System.Threading.CancellationToken($this: Ref, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.BeginRead$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure {:extern} System.IO.Stream.get_CanRead($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.NullStream.EndRead$System.IAsyncResult($this: Ref, asyncResult$in: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.NullStream.BeginWrite$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure {:extern} System.IO.Stream.get_CanWrite($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.PipeStream.NullStream.EndWrite$System.IAsyncResult($this: Ref, asyncResult$in: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.Read$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.NullStream.ReadAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.ReadByte($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.NullStream.Write$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);



procedure System.IO.Pipes2.PipeStream.NullStream.WriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.NullStream.WriteByte$System.Byte($this: Ref, value$in: int);



procedure System.IO.Pipes2.PipeStream.NullStream.Seek$System.Int64$System.IO.SeekOrigin($this: Ref, offset$in: int, origin$in: int) returns ($result: int);



procedure System.IO.Pipes2.PipeStream.NullStream.SetLength$System.Int64($this: Ref, length$in: int);



procedure T$System.IO.Pipes2.PipeStream.NullStream.#cctor();



function T$System.IO.Pipes2.PipeStream.ReadWriteTask() : Ref;

const unique T$System.IO.Pipes2.PipeStream.ReadWriteTask: int;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._apm: Field;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._stream: Field;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._buffer: Field;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._offset: Field;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._count: Field;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._callback: Field;

const unique F$System.IO.Pipes2.PipeStream.ReadWriteTask._context: Field;

var F$System.IO.Pipes2.PipeStream.ReadWriteTask.s_invokeAsyncCallback: Ref;

procedure System.IO.Pipes2.PipeStream.ReadWriteTask.ClearBeginState($this: Ref);



procedure System.IO.Pipes2.PipeStream.ReadWriteTask.#ctor$System.Boolean$System.Boolean$System.Func$System.Object$System.Int32$$System.Object$System.IO.Stream$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback($this: Ref, isRead$in: bool, apm$in: bool, function$in: Ref, state$in: Ref, stream$in: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref);



procedure {:extern} System.Threading.Tasks.Task`1.#ctor$System.Func$System.Object$`0$$System.Object$System.Threading.CancellationToken$System.Threading.Tasks.TaskCreationOptions($this: Ref, function$in: Ref, state$in: Ref, cancellationToken$in: Ref, creationOptions$in: int);



procedure {:extern} System.Threading.ExecutionContext.Capture() returns ($result: Ref);



procedure System.IO.Pipes2.PipeStream.ReadWriteTask.InvokeAsyncCallback$System.Object(completedTask$in: Ref);



procedure {:extern} System.AsyncCallback.Invoke$System.IAsyncResult($this: Ref, ar$in: Ref);



procedure T$System.IO.Pipes2.PipeStream.ReadWriteTask.#cctor();



function T$System.IO.Pipes2.NamedPipeClientStream() : Ref;

const unique T$System.IO.Pipes2.NamedPipeClientStream: int;

var F$System.IO.Pipes2.NamedPipeClientStream.CancellationCheckInterval: int;

const unique F$System.IO.Pipes2.NamedPipeClientStream._normalizedPipePath: Field;

const unique F$System.IO.Pipes2.NamedPipeClientStream._impersonationLevel: Field;

const unique F$System.IO.Pipes2.NamedPipeClientStream._pipeOptions: Field;

const unique F$System.IO.Pipes2.NamedPipeClientStream._inheritability: Field;

const unique F$System.IO.Pipes2.NamedPipeClientStream._direction: Field;

procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.String($this: Ref, pipeName$in: Ref);



const {:value "."} unique $string_literal_._25: Ref;

procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.String$System.String$System.IO.Pipes2.PipeDirection$System.IO.Pipes2.PipeOptions$System.Security.Principal.TokenImpersonationLevel$System.IO.HandleInheritability($this: Ref, serverName$in: Ref, pipeName$in: Ref, direction$in: int, options$in: int, impersonationLevel$in: int, inheritability$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.String$System.String($this: Ref, serverName$in: Ref, pipeName$in: Ref);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.String$System.String$System.IO.Pipes2.PipeDirection($this: Ref, serverName$in: Ref, pipeName$in: Ref, direction$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.String$System.String$System.IO.Pipes2.PipeDirection$System.IO.Pipes2.PipeOptions($this: Ref, serverName$in: Ref, pipeName$in: Ref, direction$in: int, options$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.String$System.String$System.IO.Pipes2.PipeDirection$System.IO.Pipes2.PipeOptions$System.Security.Principal.TokenImpersonationLevel($this: Ref, serverName$in: Ref, pipeName$in: Ref, direction$in: int, options$in: int, impersonationLevel$in: int);



procedure {:System.Security.SecuritySafeCritical} System.IO.Pipes2.NamedPipeClientStream.#ctor$System.IO.Pipes2.PipeDirection$System.Boolean$System.Boolean$Microsoft.Win32.SafeHandles.SafePipeHandle($this: Ref, direction$in: int, isAsync$in: bool, isConnected$in: bool, safePipeHandle$in: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.Finalize($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.Connect($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.Connect$System.Int32($this: Ref, timeout$in: int);



procedure System.IO.Pipes2.NamedPipeClientStream.CheckConnectOperationsClient($this: Ref);



procedure {:extern} System.Environment.get_TickCount() returns ($result: int);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeClientStream.ConnectInternal$System.Int32$System.Threading.CancellationToken$System.Int32($this: Ref, timeout$in: int, cancellationToken$in: Ref, startTime$in: int);



procedure System.Threading.SpinWait.#default_ctor($this: Ref);



function {:extern} T$System.Threading.SpinWait() : Ref;

const {:extern} unique T$System.Threading.SpinWait: int;

procedure {:extern} System.Threading.CancellationToken.ThrowIfCancellationRequested($this: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeClientStream.TryConnect$System.Int32$System.Threading.CancellationToken($this: Ref, timeout$in: int, cancellationToken$in: Ref) returns ($result: bool);



procedure {:extern} System.Threading.SpinWait.SpinOnce($this: Ref);



procedure {:extern} System.TimeoutException.#ctor($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.ConnectAsync($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.ConnectAsync$System.Int32$System.Threading.CancellationToken($this: Ref, timeout$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.ConnectAsync$System.Int32($this: Ref, timeout$in: int) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.ConnectAsync$System.Threading.CancellationToken($this: Ref, cancellationToken$in: Ref) returns ($result: Ref);



procedure {:System.Security.SecurityCritical} System.IO.Pipes2.NamedPipeClientStream.CheckPipePropertyOperations($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.Stream_Close($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.Stream_Dispose($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_Flush($this: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_ReadByte($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_WriteByte$System.Byte($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_Read$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_ReadAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_BeginRead$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_EndRead$System.IAsyncResult($this: Ref, asyncResult$in: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_Write$System.Bytearray$System.Int32$System.Int32($this: Ref, buffer$in: Ref, offset$in: int, count$in: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_WriteAsync$System.Bytearray$System.Int32$System.Int32$System.Threading.CancellationToken($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, cancellationToken$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_BeginWrite$System.Bytearray$System.Int32$System.Int32$System.AsyncCallback$System.Object($this: Ref, buffer$in: Ref, offset$in: int, count$in: int, callback$in: Ref, state$in: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_EndWrite$System.IAsyncResult($this: Ref, asyncResult$in: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_IsConnected($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_IsAsync($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_IsMessageComplete($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_SafePipeHandle($this: Ref) returns ($result: Ref);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_CanRead($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_CanWrite($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_CanSeek($this: Ref) returns ($result: bool);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_Length($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeClientStream.get_PipeStream_Position($this: Ref) returns ($result: int);



procedure System.IO.Pipes2.NamedPipeClientStream.set_PipeStream_Position$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_SetLength$System.Int64($this: Ref, value$in: int);



procedure System.IO.Pipes2.NamedPipeClientStream.PipeStream_Seek$System.Int64$System.IO.SeekOrigin($this: Ref, offset$in: int, origin$in: int) returns ($result: int);



procedure {:System.Security.SecurityCritical} {:System.Diagnostics.CodeAnalysis.SuppressMessage "Microsoft.Security", "CA2122:DoNotIndirectlyExposeMethodsWithLinkDemands"} System.IO.Pipes2.NamedPipeClientStream.get_NumberOfServerInstances($this: Ref) returns ($result: int);



procedure T$System.IO.Pipes2.NamedPipeClientStream.#cctor();



const unique F$System.IO.Pipes2.ConnectionCompletionSource._serverStream: Field;

procedure System.IO.Pipes2.ConnectionCompletionSource.SetCompletedSynchronously($this: Ref);



procedure System.IO.Pipes2.VoidResult.#default_ctor($this: Ref);



function T$System.IO.Pipes2.VoidResult() : Ref;

const unique T$System.IO.Pipes2.VoidResult: int;

procedure System.IO.Pipes2.VoidResult.#copy_ctor(this: Ref) returns (other: Ref);
  free ensures this != other;



procedure {:extern} System.Threading.Tasks.TaskCompletionSource`1.TrySetResult$`0($this: Ref, result$in: Ref) returns ($result: bool);



procedure System.IO.Pipes2.ConnectionCompletionSource.AsyncCallback$System.UInt32$System.UInt32($this: Ref, errorCode$in: int, numBytes$in: int);



procedure System.IO.Pipes2.ConnectionCompletionSource.HandleError$System.Int32($this: Ref, errorCode$in: int);



procedure {:extern} System.Threading.Tasks.TaskCompletionSource`1.TrySetException$System.Exception($this: Ref, exception$in: Ref) returns ($result: bool);



procedure System.IO.Pipes2.ConnectionCompletionSource.HandleUnexpectedCancellation($this: Ref);



procedure T$System.IO.Pipes2.ConnectionCompletionSource.#cctor();



function T$System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute() : Ref;

const unique T$System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute: int;

procedure System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute.#ctor($this: Ref);



procedure {:extern} System.Attribute.#ctor($this: Ref);



procedure T$System.Diagnostics.Contracts.ContractDeclarativeAssemblyAttribute.#cctor();



function T$System.Threading.Tasks.TaskToApm() : Ref;

const unique T$System.Threading.Tasks.TaskToApm: int;

procedure System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.#ctor$System.Threading.Tasks.Task$System.Object$System.Boolean($this: Ref, task$in: Ref, state$in: Ref, completedSynchronously$in: bool);



function T$System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult() : Ref;

const unique T$System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult: int;

procedure {:extern} System.Threading.Tasks.Task.get_AsyncState($this: Ref) returns ($result: Ref);



procedure System.Threading.Tasks.TaskToApm.InvokeCallbackWhenTaskCompletes$System.Threading.Tasks.Task$System.AsyncCallback$System.IAsyncResult(antecedent$in: Ref, callback$in: Ref, asyncResult$in: Ref);



const unique F$System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.Task: Field;

const {:value "TaskWrapperAsyncResult should never wrap a null Task."} unique $string_literal_TaskWrapperAsyncResult$should$never$wrap$a$null$Task._26: Ref;

function {:extern} T$System.Threading.Tasks.Task() : Ref;

const {:extern} unique T$System.Threading.Tasks.Task: int;

function {:extern} TResult$T$System.Threading.Tasks.Task`1(parent: Ref) : Ref;

function {:extern} T$System.Threading.Tasks.Task`1(TResult: Ref) : Ref;

const {:extern} unique T$System.Threading.Tasks.Task`1: int;

const unique F$System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult._state: Field;

const unique F$System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult._completedSynchronously: Field;

const {:value "If completedSynchronously is true, the task must be completed."} unique $string_literal_If$completedSynchronously$is$true$$the$task$must$be$completed._27: Ref;

procedure System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.System#IAsyncResult#get_AsyncState($this: Ref) returns ($result: Ref);



procedure System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.System#IAsyncResult#get_CompletedSynchronously($this: Ref) returns ($result: bool);



procedure System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.System#IAsyncResult#get_IsCompleted($this: Ref) returns ($result: bool);



procedure System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.System#IAsyncResult#get_AsyncWaitHandle($this: Ref) returns ($result: Ref);



procedure {:extern} System.IAsyncResult.get_AsyncWaitHandle($this: Ref) returns ($result: Ref);



procedure T$System.Threading.Tasks.TaskToApm.TaskWrapperAsyncResult.#cctor();



procedure T$System.Threading.Tasks.TaskToApm.#cctor();



function T$System.Threading.ThreadPoolBoundHandle() : Ref;

const unique T$System.Threading.ThreadPoolBoundHandle: int;

const unique F$System.Threading.ThreadPoolBoundHandle._handle: Field;

const unique F$System.Threading.ThreadPoolBoundHandle._isDisposed: Field;

procedure System.Threading.ThreadPoolBoundHandle.#ctor$System.Runtime.InteropServices.SafeHandle($this: Ref, handle$in: Ref);



procedure {:extern} System.Threading.ThreadPool.BindHandle$System.Runtime.InteropServices.SafeHandle(osHandle$in: Ref) returns ($result: bool);



procedure System.Threading.ThreadPoolBoundHandle.AllocateNativeOverlapped$System.Threading.IOCompletionCallback$System.Object$System.Object($this: Ref, callback$in: Ref, state$in: Ref, pinData$in: Ref) returns ($result: Ref);



procedure System.Threading.ThreadPoolBoundHandle.EnsureNotDisposed($this: Ref);



procedure System.Threading.ThreadPoolBoundHandleOverlapped.#ctor$System.Threading.IOCompletionCallback$System.Object$System.Object$System.Threading.PreAllocatedOverlapped($this: Ref, callback$in: Ref, state$in: Ref, pinData$in: Ref, preAllocated$in: Ref);



function T$System.Threading.ThreadPoolBoundHandleOverlapped() : Ref;

const unique T$System.Threading.ThreadPoolBoundHandleOverlapped: int;

const unique F$System.Threading.ThreadPoolBoundHandleOverlapped._boundHandle: Field;

const unique F$System.Threading.ThreadPoolBoundHandleOverlapped._nativeOverlapped: Field;

procedure System.Threading.ThreadPoolBoundHandle.AllocateNativeOverlapped$System.Threading.PreAllocatedOverlapped($this: Ref, preAllocated$in: Ref) returns ($result: Ref);



procedure System.Threading.PreAllocatedOverlapped.AddRef($this: Ref) returns ($result: bool);



const unique F$System.Threading.PreAllocatedOverlapped._overlapped: Field;

function {:extern} T$System.Object() : Ref;

const {:extern} unique T$System.Object: int;

procedure System.Threading.PreAllocatedOverlapped.Release($this: Ref);



procedure System.Threading.ThreadPoolBoundHandle.FreeNativeOverlapped$System.Threading.NativeOverlapped$($this: Ref, overlapped$in: Ref);



procedure System.Threading.ThreadPoolBoundHandle.GetNativeOverlappedState$System.Threading.NativeOverlapped$(overlapped$in: Ref) returns ($result: Ref);



procedure System.Threading.ThreadPoolBoundHandle.GetOverlappedWrapper$System.Threading.NativeOverlapped$$System.Threading.ThreadPoolBoundHandle(overlapped$in: Ref, expectedBoundHandle$in: Ref) returns ($result: Ref);



procedure {:extern} System.Threading.Overlapped.Unpack$System.Threading.NativeOverlapped$(nativeOverlappedPtr$in: Ref) returns ($result: Ref);



procedure T$System.Threading.ThreadPoolBoundHandle.#cctor();



var F$System.Threading.ThreadPoolBoundHandleOverlapped.s_completionCallback: Ref;

const unique F$System.Threading.ThreadPoolBoundHandleOverlapped._userCallback: Field;

const unique F$System.Threading.ThreadPoolBoundHandleOverlapped._userState: Field;

const unique F$System.Threading.ThreadPoolBoundHandleOverlapped._preAllocated: Field;

const unique F$System.Threading.ThreadPoolBoundHandleOverlapped._completed: Field;

procedure {:extern} System.Threading.Overlapped.#ctor($this: Ref);



procedure {:extern} System.Threading.Overlapped.Pack$System.Threading.IOCompletionCallback$System.Object($this: Ref, iocb$in: Ref, userData$in: Ref) returns ($result: Ref);



const {:extern} unique F$System.Threading.NativeOverlapped.OffsetLow: Field;

const {:extern} unique F$System.Threading.NativeOverlapped.OffsetHigh: Field;

procedure System.Threading.ThreadPoolBoundHandleOverlapped.CompletionCallback$System.UInt32$System.UInt32$System.Threading.NativeOverlapped$(errorCode$in: int, numBytes$in: int, nativeOverlapped$in: Ref);



procedure {:extern} System.Threading.IOCompletionCallback.Invoke$System.UInt32$System.UInt32$System.Threading.NativeOverlapped$($this: Ref, errorCode$in: int, numBytes$in: int, pOVERLAP$in: Ref);



procedure System.Threading.ThreadPoolBoundHandleOverlapped.#cctor();



const unique System.Threading.ThreadPoolBoundHandleOverlapped.CompletionCallback$System.UInt32$System.UInt32$System.Threading.NativeOverlapped$: int;

function Type0() : Ref;

function T$System.Threading.IDeferredDisposable() : Ref;

const unique T$System.Threading.IDeferredDisposable: int;

procedure System.Threading.IDeferredDisposable.OnFinalRelease$System.Boolean($this: Ref, disposed$in: bool);



function T$System.Threading.PreAllocatedOverlapped() : Ref;

const unique T$System.Threading.PreAllocatedOverlapped: int;

const unique F$System.Threading.PreAllocatedOverlapped._lifetime: Field;

procedure System.Threading.PreAllocatedOverlapped.#ctor$System.Threading.IOCompletionCallback$System.Object$System.Object($this: Ref, callback$in: Ref, state$in: Ref, pinData$in: Ref);



procedure System.Threading.DeferredDisposableLifetime$System.Threading.PreAllocatedOverlapped$.#default_ctor($this: Ref);



function T$T$System.Threading.DeferredDisposableLifetime`1(parent: Ref) : Ref;

function T$System.Threading.DeferredDisposableLifetime`1(T: Ref) : Ref;

const unique T$System.Threading.DeferredDisposableLifetime`1: int;

procedure System.Threading.DeferredDisposableLifetime`1.AddRef$`0($this: Ref, obj$in: Ref) returns ($result: bool);



procedure System.Threading.DeferredDisposableLifetime`1.Release$`0($this: Ref, obj$in: Ref);



procedure System.Threading.PreAllocatedOverlapped.Dispose($this: Ref);



procedure System.Threading.DeferredDisposableLifetime`1.Dispose$`0($this: Ref, obj$in: Ref);



procedure System.Threading.PreAllocatedOverlapped.Finalize($this: Ref);



procedure {:extern} System.Environment.get_HasShutdownStarted() returns ($result: bool);



procedure System.Threading.PreAllocatedOverlapped.System#Threading#IDeferredDisposable#OnFinalRelease$System.Boolean($this: Ref, disposed$in: bool);



procedure {:extern} System.Threading.Overlapped.Free$System.Threading.NativeOverlapped$(nativeOverlappedPtr$in: Ref);



procedure System.Threading.NativeOverlapped.#default_ctor($this: Ref);



function {:extern} T$System.Threading.NativeOverlapped() : Ref;

const {:extern} unique T$System.Threading.NativeOverlapped: int;

procedure T$System.Threading.PreAllocatedOverlapped.#cctor();



const unique System.Threading.DeferredDisposableLifetime: Ref;

procedure System.Threading.DeferredDisposableLifetime.#default_ctor($this: Ref);



const unique F$System.Threading.DeferredDisposableLifetime`1._count: Field;

procedure System.Threading.DeferredDisposableLifetime.#copy_ctor(this: Ref) returns (other: Ref);
  free ensures this != other;



procedure {:extern} System.Threading.Volatile.Read$System.Int32$(location$in: int) returns (location$out: int, $result: int);



procedure {:extern} System.Threading.Interlocked.CompareExchange$System.Int32$$System.Int32$System.Int32(location1$in: int, value$in: int, comparand$in: int) returns (location1$out: int, $result: int);



const unique F$PipeStream.ReadWriteCompletionSource.pipeStream: Field;

const unique F$PipeStream.ReadWriteCompletionSource.buffer: Field;

const unique F$PipeStream.ReadWriteCompletionSource.cancellationToken: Field;

const unique F$PipeStream.ReadWriteCompletionSource.isWrite: Field;

const unique F$PipeStream.ReadWriteCompletionSource.$Task$k__BackingField: Field;

const unique F$PipeStream.ReadWriteCompletionSource.$Overlapped$k__BackingField: Field;

implementation PipeStream.ReadWriteCompletionSource.#ctor$System.IO.Pipes2.PipeStream$System.Bytearray$System.Threading.CancellationToken$System.Boolean($this: Ref, pipeStream$in: Ref, buffer$in: Ref, cancellationToken$in: Ref, isWrite$in: bool)
{
  var pipeStream: Ref;
  var buffer: Ref;
  var cancellationToken: Ref;
  var isWrite: bool;
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    pipeStream := pipeStream$in;
    buffer := buffer$in;
    cancellationToken := cancellationToken$in;
    isWrite := isWrite$in;
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.pipeStream, null);
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.buffer, null);
    call {:si_unique_call 122} $tmp0 := Alloc();
    call {:si_unique_call 123} System.Threading.CancellationToken.#default_ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.Threading.CancellationToken();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.Threading.CancellationToken;
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.cancellationToken, $tmp0);
    assume Union2Bool(Bool2Union(false)) <==> false;
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.isWrite, Bool2Union(false));
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.$Task$k__BackingField, null);
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.$Overlapped$k__BackingField, null);
    assume {:breadcrumb 279} true;
    call {:si_unique_call 124} System.Object.#ctor($this);
    goto anon3_Then, anon3_Else;

  anon3_Then:
    assume {:partition} $Exception != null;
    return;

  anon3_Else:
    assume {:partition} $Exception == null;
    goto anon2;

  anon2:
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.pipeStream, pipeStream);
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.buffer, buffer);
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.cancellationToken, cancellationToken);
    assume Union2Bool(Bool2Union(isWrite)) <==> isWrite;
    $Heap := Write($Heap, $this, F$PipeStream.ReadWriteCompletionSource.isWrite, Bool2Union(isWrite));
    return;
}



implementation PipeStream.ReadWriteCompletionSource.ReleaseResources($this: Ref)
{
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 280} true;
    call {:si_unique_call 125} $tmp0 := Alloc();
    call {:si_unique_call 126} System.NotImplementedException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.NotImplementedException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.NotImplementedException;
    $Exception := $tmp0;
    return;
}



implementation PipeStream.ReadWriteCompletionSource.RegisterForCancellation($this: Ref)
{
  var $tmp0: Ref;
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 281} true;
    call {:si_unique_call 127} $tmp0 := Alloc();
    call {:si_unique_call 128} System.NotImplementedException.#ctor($tmp0);
    assume $DynamicType($tmp0) == T$System.NotImplementedException();
    assume $TypeConstructor($DynamicType($tmp0)) == T$System.NotImplementedException;
    $Exception := $tmp0;
    return;
}



implementation PipeStream.ReadWriteCompletionSource.get_Task($this: Ref) returns ($result: Ref)
{
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 282} true;
    assume $this != null;
    $result := Read($Heap, $this, F$PipeStream.ReadWriteCompletionSource.$Task$k__BackingField);
    return;
}



procedure {:System.Runtime.CompilerServices.CompilerGenerated} PipeStream.ReadWriteCompletionSource.set_Task$System.Threading.Tasks.Task$System.Int32$($this: Ref, value$in: Ref);



implementation PipeStream.ReadWriteCompletionSource.get_Overlapped($this: Ref) returns ($result: Ref)
{
  var $localExc: Ref;
  var $label: int;

  anon0:
    assume {:breadcrumb 284} true;
    assume $this != null;
    $result := Read($Heap, $this, F$PipeStream.ReadWriteCompletionSource.$Overlapped$k__BackingField);
    return;
}



procedure {:System.Runtime.CompilerServices.CompilerGenerated} PipeStream.ReadWriteCompletionSource.set_Overlapped$System.Threading.NativeOverlapped$($this: Ref, value$in: Ref);



procedure T$PipeStream.ReadWriteCompletionSource.#cctor();



procedure T$System.Threading.IOCompletionCallback$CreateDelegate(Method: int, Receiver: Ref, TypeParameters: Ref) returns (c: Ref);



procedure T$System.Threading.IOCompletionCallback$AddDelegate(a: Ref, b: Ref) returns (c: Ref);



procedure T$System.Threading.IOCompletionCallback$RemoveDelegate(a: Ref, b: Ref) returns (c: Ref);


