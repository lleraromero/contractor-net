using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;
using System.Threading;

namespace Tracer.Poco
{
    internal class FileTracerClient : ITracerClient
    {
        StreamWriter sw;

        public void Initialize()
        {
            sw = File.CreateText(@"c:\temp\trace.txt");
        }

        public void Trace(int fileId, int traceType, int spanStart, int spanEnd)
        {
            string separator = ",";
            string trace = fileId + separator + traceType + separator + spanStart + separator + spanEnd;
            sw.WriteLine(trace);
            sw.Flush();
        }
    }
}
