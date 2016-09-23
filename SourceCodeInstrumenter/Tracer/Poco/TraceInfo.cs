using ProtoBuf;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracer.Poco
{
    [ProtoContract]
    public class TraceInfo
    {
        [ProtoMember(1)]
        public int FileId { get; set; }
        [ProtoMember(2)]
        public int SpanStart { get; set; }
        [ProtoMember(3)]
        public int SpanEnd { get; set; }
        [ProtoMember(4)]
        public TraceType TraceType { get; set; }
    }
}