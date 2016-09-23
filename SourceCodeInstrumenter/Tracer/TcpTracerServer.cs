using ProtoBuf;
using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Tracer.Poco;

namespace Tracer
{
    internal class TcpTracerServer : ITracerServer
    {
        public TraceReceiver Receiver { get; set; }
        private TcpListener tcpListener;

        public TcpTracerServer(TraceReceiver receiver)
        {
            this.Receiver = receiver;
        }

        public void Initialize()
        {
            try
            {
                //Use local IP address, and use the same in the client.
                var ipAd = IPAddress.Parse("127.0.0.1");

                //Initializes the Listener.
                tcpListener = new TcpListener(ipAd, 8001);

                //Start Listeneting at the specified port.
                tcpListener.Start();
            }
            catch (Exception e)
            {
                throw e;
            }
        }

        public void StartReceivingTrace()
        {
            //At this point the program waits for a connection, so the object that
            //calls this function does not continue execution until one is received.
            //Maybe this must be executed in another trhead, to be able to receive
            //more than a single connection.
            TcpClient client;
            try
            {
                client = tcpListener.AcceptTcpClient();
            }
            catch (Exception e)
            {
                throw e;
            }

            var receiving = true;
            bool receiversListening = true;
            while (receiving)
            {
                try
                {
                    Stream stream = (Stream)client.GetStream();
                    TraceInfo info = Serializer.DeserializeWithLengthPrefix<TraceInfo>(stream, PrefixStyle.Fixed32);
                    bool receiverIsListening = Receiver.TraceReceived(info);
                    Serializer.SerializeWithLengthPrefix(stream, receiverIsListening, PrefixStyle.Fixed32);
                }
                catch
                {
                    //TODO: Maybe we want to catch the specific exception, now it is
                    //used only to avoid exit the function without close the listener.
                    receiving = false;
                    continue;
                }

                if (!receiversListening)
                {
                    receiving = false;
                }
            }

            client.Close();
            tcpListener.Stop();
        }
    }
}