using System;
using System.Collections.Generic;
using Tracer.Poco;
using System.Linq;
using System.Threading;

namespace Tracer
{
    public class TraceReceiver
    {
        private ITracerServer tracerServer;
        private TraceQueue traceBuffer = new TraceQueue();
        public bool ReceivingStoped { get; set; }

        public TraceReceiver(string traceInput)
        {
            //Open connection.
            //tracerServer = new TcpTracerServer(this);
            if (traceInput == null)
            {
                tracerServer = new PipeTracerServer(this);
            }
            else
            {
                tracerServer = new FileTracerServer(this, traceInput);
            }
            tracerServer.Initialize();
            ReceivingStoped = false;
        }

        public void StartReceivingTrace()
        {
            tracerServer.StartReceivingTrace();
        }

        public bool TraceReceived(TraceInfo info)
        {
            if (!ReceivingStoped)
            {
                traceBuffer.Add(info);
                return true;
            }

            return false;
        }

        public TraceInfo Next()
        {
            return traceBuffer.Take();
        }

        public TraceInfo ObserveNext()
        {
            return traceBuffer.Peek();
        }

        public void StopReceiving()
        {
            traceBuffer.IsAddingComplete = true;
            ReceivingStoped = true;
        }
    }
}