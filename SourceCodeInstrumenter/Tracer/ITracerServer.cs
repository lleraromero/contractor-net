using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracer
{
    interface ITracerServer
    {
        TraceReceiver Receiver {get;set;}
        void Initialize();
        void StartReceivingTrace();
    }
}
