using ProtoBuf;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Threading;
using Tracer.Poco;

namespace Tracer
{
    internal class TcpTracerClient : ITracerClient
    {
        private TcpClient tcpClient;

        public TcpTracerClient()
        {
            AlreadyClosed = false;
        }

        private bool AlreadyClosed { get; set; }

        public void Initialize()
        {
            if (AlreadyClosed) return;

            var connected = false;
            while (!connected)
            {
                try
                {
                    tcpClient = new TcpClient("127.0.0.1", 8001);
                    connected = true;
                }
                catch
                {
                    Thread.Sleep(500);
                }
            }
        }

        public void Trace(int fileId, int traceType, int spanStart, int spanEnd)
        {
            if (AlreadyClosed) return;
            try
            {
                //Encoding.
                Stream stm = tcpClient.GetStream();
                TraceInfo info = new TraceInfo() { FileId = fileId, SpanStart = spanStart, SpanEnd = spanEnd, TraceType = (TraceType)traceType };
                Serializer.SerializeWithLengthPrefix(stm, info, PrefixStyle.Fixed32);
                stm.Flush();

                bool continueSending = Serializer.DeserializeWithLengthPrefix<bool>(stm, PrefixStyle.Fixed32);

                if (!continueSending)
                {
                    AlreadyClosed = true;
                    tcpClient.GetStream().Close();
                    tcpClient.Close();
                }
            }
            catch
            {
                AlreadyClosed = true;
                tcpClient.GetStream().Close();
                tcpClient.Close();
            }
        }
    }
}