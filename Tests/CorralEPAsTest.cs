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
        protected static IAssemblyXXX inputAssembly;
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
            var epaGenerator = new EpaGenerator();
            var typeToAnalyze = "Examples.VendingMachine";
            var constructors = inputAssembly.Constructors(typeToAnalyze);
            var actions = inputAssembly.Actions(typeToAnalyze);
            epaGenerator.GenerateEpa(typeToAnalyze, new AnalyzerMock(), constructors, actions);
        }

        [TestMethod]
        public void TestLinear()
        {
            var epaGenerator = new EpaGenerator();
            var typeToAnalyze = "Examples.Linear";
            var constructors = inputAssembly.Constructors(typeToAnalyze);
            var actions = inputAssembly.Actions(typeToAnalyze);
            epaGenerator.GenerateEpa(typeToAnalyze, new AnalyzerMock(), constructors, actions);
        }

        [TestMethod]
        public void TestDoor()
        {
            var epaGenerator = new EpaGenerator();
            var typeToAnalyze = "Examples.Door";
            var constructors = inputAssembly.Constructors(typeToAnalyze);
            var actions = inputAssembly.Actions(typeToAnalyze);
            epaGenerator.GenerateEpa(typeToAnalyze, new AnalyzerMock(), constructors, actions);
        }

        [TestMethod]
        public void TestFiniteStack()
        {
            var epaGenerator = new EpaGenerator();
            var typeToAnalyze = "Examples.FiniteStack";
            var constructors = inputAssembly.Constructors(typeToAnalyze);
            var actions = inputAssembly.Actions(typeToAnalyze);
            epaGenerator.GenerateEpa(typeToAnalyze, new AnalyzerMock(), constructors, actions);
        }
    }
}
