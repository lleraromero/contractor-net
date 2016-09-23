using System.Diagnostics;

namespace Tracer.Poco
{
    [DebuggerDisplay("FileName={FileName}, Line={Line}, Node={CSharpSyntaxNode}")]
    public class Stmt
    {
        public int FileId { get; set; }
        public string FileName { get; set; }
        public int SpanStart { get; set; }
        public int SpanEnd { get; set; }
        public int Line { get; set; }
        public object CSharpSyntaxNode { get; set; }
        public TraceType TraceType { get; set; }
        // override object.Equals
        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }
            if (obj == this) return true;

            var other = (Stmt) obj;

            return SpanStart == other.SpanStart
                   && SpanEnd == other.SpanEnd
                   && FileId == other.FileId;
        }

        // override object.GetHashCode
        public override int GetHashCode()
        {
            return FileId * 17 + SpanStart * 13 + SpanEnd;
        }

        public override string ToString()
        {
            return "Line=" + Line + ", Node=" + CSharpSyntaxNode;
        }
    }
}