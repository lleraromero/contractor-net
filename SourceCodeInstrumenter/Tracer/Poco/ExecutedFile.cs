using System.Collections.Generic;

namespace Tracer.Poco
{
    public class ExecutedFile
    {
        public string Path { get; set; }
        public string Name { get; set; }
        public IList<Stmt> SlicedStmts { get; set; }
    }
}