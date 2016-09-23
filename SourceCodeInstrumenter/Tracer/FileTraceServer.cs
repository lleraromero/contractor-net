﻿using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Text;

namespace Tracer.Poco
{
    internal class FileTracerServer : ITracerServer
    {
        public TraceReceiver Receiver { get; set; }
        public string TraceInput { get; set; }

        public FileTracerServer(TraceReceiver receiver, string traceInput)
        {
            this.Receiver = receiver;
            TraceInput = traceInput;
        }

        public void Initialize()
        {
        }

        public void StartReceivingTrace()
        {
            char separator = ',';
            var lines = File.ReadAllLines(TraceInput);
            foreach (var line in lines)
            {
                var data = line.Split(separator);
                var fileId = Convert.ToInt32(data[0]);
                var traceType = (TraceType)Convert.ToInt32(data[1]);
                var spanStart = Convert.ToInt32(data[2]);
                var spanEnd = Convert.ToInt32(data[3]);
                TraceInfo info = new TraceInfo() { FileId = fileId, TraceType = traceType, SpanStart = spanStart, SpanEnd = spanEnd };
                Receiver.TraceReceived(info);
            }
            Receiver.StopReceiving();
        }
    }
}
