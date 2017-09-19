using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Diagnostics.Contracts;
using System.IO;

namespace Contractor.Console.Tests
{
    [TestClass]
    public class TestConsoleFlags
    {
        [TestMethod]
        [ExpectedException(typeof(System.ArgumentNullException))]
        public void TestNullArgs()
        {
            int r = 0;
            //r = Contractor.Console.Program.Main(null);
        }

        [TestMethod]
        public void TestEmptyArgs()
        {
            int r=0;
            string[] args = {};
            //r = Contractor.Console.Program.Main(args);
            //Contract.Assert(r==-1);
        }
    }
}
