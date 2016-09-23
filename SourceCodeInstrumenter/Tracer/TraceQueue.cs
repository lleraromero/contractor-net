using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using Tracer.Poco;

namespace Tracer
{
    class TraceQueue
    {
        bool IsEmpty = true;
        Queue<TraceInfo> queue = new Queue<TraceInfo>();
        Semaphore elems = new Semaphore(0, 1);

        internal TraceInfo Take()
        {
            TraceInfo info = null;
            lock (this)
            {
                while (!IsAddingComplete && IsEmpty)
                {
                    Monitor.Wait(this, 1000);
                }
                if (IsEmpty && IsAddingComplete) return null;
                info = queue.Dequeue();
                if (queue.Count == 0)
                {
                    IsEmpty = true;
                }
            }
            if (info == null)
            {
                throw new Exception("Error! info es null!");
            }
            return info;
        }

        internal void Add(TraceInfo trace)
        {
            lock (this)
            {
                queue.Enqueue(trace);
                if (queue.Count == 1)
                {
                    IsEmpty = false;
                    Monitor.Pulse(this);
                }
            }
        }

        internal TraceInfo Peek()
        {
            TraceInfo info = null;
            lock (this)
            {
                while (!IsAddingComplete && IsEmpty)
                {
                    Monitor.Wait(this, 1000);
                }
                if (IsEmpty && IsAddingComplete) return null;
                info = queue.Peek();
            }
            return info;
        }

        public bool IsAddingComplete { get; set; }
    }
}
