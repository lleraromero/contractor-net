using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracer
{
    public interface ITraceReceiver
    {
        void ExitScopeTraceReceived(TraceInfo info);

        void EnterScopeTraceReceived(TraceInfo info);

        void ConditionTraceReceived(TraceInfo info);

        void SimpleStatementTraceReceived(TraceInfo info);
    }
}
