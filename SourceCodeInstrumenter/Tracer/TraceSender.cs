using System;
using Tracer.Poco;

namespace Tracer
{
    public class TraceSender
    {
        static ITracerClient TracerClient { get; set; }

        static TraceSender()
        {
            //TracerClient = new TcpTracerClient();
            //TracerClient = new PipeTracerClient();
            TracerClient = new FileTracerClient();
            TracerClient.Initialize();
        }

        public static bool TraceSimpleStatement(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.SimpleStatement, spanStart, spanEnd);
            // Return true is neccesary for instrument conditions in IF, WHILE, etc.
            return true;
        }

        public static void TraceBreak(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.Break, spanStart, spanEnd);
        }

        public static void TraceExitUsing(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.ExitUsing, spanStart, spanEnd);
        }

        public static void TraceEnterCondition(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EnterCondition, spanStart, spanEnd);
        }

        public static void TraceExitCondition(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.ExitCondition, spanStart, spanEnd);
        }

        public static void TraceEnterMethod(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EnterMethod, spanStart, spanEnd);
        }

        public static void TraceExitMethod(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.ExitMethod, spanStart, spanEnd);
        }

        public static int TraceBeforeConstructor(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.BeforeConstructor, spanStart, spanEnd);
            return 42;
        }

        public static void TraceEnterConstructor(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EnterConstructor, spanStart, spanEnd);
        }

        public static void TraceExitConstructor(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.ExitConstructor, spanStart, spanEnd);
        }

        public static void TraceEnterStaticMethod(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EnterStaticMethod, spanStart, spanEnd);
        }

        public static void TraceExitStaticMethod(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.ExitStaticMethod, spanStart, spanEnd);
        }

        public static void TraceEnterStaticConstructor(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EnterStaticConstructor, spanStart, spanEnd);
        }

        public static void TraceExitStaticConstructor(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.ExitStaticConstructor, spanStart, spanEnd);
        }

        public static void TraceEndInvocation(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EndInvocation, spanStart, spanEnd);
        }

        public static void TraceEndMemberAccess(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EndMemberAccess, spanStart, spanEnd);
        }

        public static void TraceEnterExpression(int fileId, int spanStart, int spanEnd)
        {
            TracerClient.Trace(fileId, (int)TraceType.EnterExpression, spanStart, spanEnd);
        }

        public static T TraceConditionalOperator<T>(int fileId, Func<bool> condition, int spanStart, int spanEnd, Func<T> t1, int spanStartT1, int spanEndT1, Func<T> t2, int spanStartT2, int spanEndT2)
        {
            TraceSimpleStatement(fileId, spanStart, spanEnd);
            if (condition.Invoke())
            {
                TraceEnterCondition(fileId, spanStart, spanEnd);
                TraceSimpleStatement(fileId, spanStartT1, spanEndT1);
                var tmp = t1.Invoke();
                TraceExitCondition(fileId, spanStart, spanEnd);
                return tmp;
            }
            else
            {
                TraceEnterCondition(fileId, spanStart, spanEnd);
                TraceSimpleStatement(fileId, spanStartT2, spanEndT2);
                var tmp = t2.Invoke();
                TraceExitCondition(fileId, spanStart, spanEnd);
                return tmp;
            }
        }

        public static T TraceEnterExpression<T>(Func<T> expression, int fileId, int spanStart, int spanEnd)
        {
            TraceEnterExpression(fileId, spanStart, spanEnd);
            return expression.Invoke();
        }

        public static T TraceInvocationWrapper<T>(Func<T> function, int fileId, int spanStart, int spanEnd)
        {
            var sended = false;
            try
            {
                T ret = function.Invoke();
                TraceEndInvocation(fileId, spanStart, spanEnd);
                sended = true;
                return ret;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!sended)
                    TraceEndInvocation(fileId, spanStart, spanEnd);
            }
        }

        public static void TraceInvocationWrapper(Action function, int fileId, int spanStart, int spanEnd)
        {
            var sended = false;
            try
            { 
                function.Invoke();
                TraceEndInvocation(fileId, spanStart, spanEnd);
                sended = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!sended)
                    TraceEndInvocation(fileId, spanStart, spanEnd);
            }
        }

        public static T TraceMemberAccessWrapper<T>(Func<T> function, int fileId, int spanStart, int spanEnd)
        {
            var sended = false;
            try
            {
                T ret = function.Invoke();
                TraceEndMemberAccess(fileId, spanStart, spanEnd);
                sended = true;
                return ret;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!sended)
                    TraceEndMemberAccess(fileId, spanStart, spanEnd);
            }
        }

        public static void TraceMemberAccessWrapper(Action function, int fileId, int spanStart, int spanEnd)
        {
            var sended = false;
            try
            {
                function.Invoke();
                TraceEndMemberAccess(fileId, spanStart, spanEnd);
                sended = true;
            }
            catch
            {
                throw;
            }
            finally
            {
                if (!sended)
                    TraceEndMemberAccess(fileId, spanStart, spanEnd);
            }
        }
    }
}