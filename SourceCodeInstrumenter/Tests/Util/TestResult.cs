using System.Collections.Generic;
using Tracer.Poco;

namespace Tests.Util
{
    public class TestResult
    {
        public Stmt Criteria { get; set; }
        public IList<Stmt> Executed { get; set; }
        public IList<Stmt> Sliced { get; set; }

        public TestResult()
        {
            Executed = new List<Stmt>();
            Sliced = new List<Stmt>();
        }
    }
}
