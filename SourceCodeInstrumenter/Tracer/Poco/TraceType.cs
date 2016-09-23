namespace Tracer.Poco
{
    public enum TraceType
    {
        SimpleStatement,
        EnterCondition,
        EnterExpression,
        ExitCondition,
        EnterMethod,
        ExitMethod,
        EnterStaticMethod,
        ExitStaticMethod,
        EnterConstructor,
        ExitConstructor,
        EnterStaticConstructor,
        ExitStaticConstructor,
        EndInvocation,
        EndMemberAccess,
        Break,
        ExitUsing,
        BeforeConstructor
    }
}