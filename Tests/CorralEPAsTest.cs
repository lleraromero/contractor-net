using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Contractor.Core;
using System.IO;

namespace Tests
{
    [TestClass]
    public class CorralEPAsTest
    {
        private static Dictionary<string, TypeAnalysisResult> epas;

        [ClassInitialize()]
        public static void GenerateEPAs(TestContext tc)
        {
            Configuration.Initialize();
            Configuration.TempPath = Directory.GetParent(tc.TestDir).ToString();
            Configuration.InlineMethodsBody = true;

            epas = new Dictionary<string, TypeAnalysisResult>();
            using (var generator = new EpaGenerator(EpaGenerator.Backend.Corral))
            {
                var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));
                generator.LoadAssembly(ExamplesPath);
                epas["Examples.FiniteStack"] = generator.GenerateEpa("Examples.FiniteStack");
            }
        }

        [TestMethod]
        public void TestStack()
        {
            var epa = epas["Examples.FiniteStack"].EPA;

            Assert.AreEqual(new List<IState>(epa.States).Count, 4);
        }
    }
}
