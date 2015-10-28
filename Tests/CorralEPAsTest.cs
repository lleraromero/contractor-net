using Contractor.Core;
using Contractor.Core.Model;
using Microsoft.Cci.MutableContracts;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;

namespace Tests
{
    [TestClass]
    public class CorralEPAsTest
    {
        private const string InputFilePath = @"..\..\..\Examples\obj\Debug\Decl\Examples.dll";
        protected static AssemblyXXX inputAssembly;
        protected static CodeContractAwareHostEnvironment host;

        [ClassInitialize()]
        public static void GenerateEPAs(TestContext tc)
        {
            Configuration.Initialize();
            Configuration.TempPath = Directory.GetParent(tc.TestDir).ToString();
            Configuration.InlineMethodsBody = true;

            var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, InputFilePath));

            host = new CodeContractAwareHostEnvironment();
            inputAssembly = new CciAssembly(ExamplesPath, null, host);
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            host.Dispose();
        }

        [TestMethod]
        public void TestVendingMachine()
        {
            var epaGenerator = new EpaGenerator(inputAssembly);
            epaGenerator.GenerateEpa("Examples.VendingMachine", new AnalyzerMock());
        }

        [TestMethod]
        public void TestLinear()
        {
            var epaGenerator = new EpaGenerator(inputAssembly);
            epaGenerator.GenerateEpa("Examples.Linear", new AnalyzerMock());
        }

        [TestMethod]
        public void TestDoor()
        {
            var epaGenerator = new EpaGenerator(inputAssembly);
            epaGenerator.GenerateEpa("Examples.Door", new AnalyzerMock());
        }

        [TestMethod]
        public void TestFiniteStack()
        {
            var epaGenerator = new EpaGenerator(inputAssembly);
            epaGenerator.GenerateEpa("Examples.FiniteStack", new AnalyzerMock());
        }
    }
}
