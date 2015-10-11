using Contractor.Core;
using Contractor.Core.Model;
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
        private static EpaGenerator epaGenerator;
        private const string InputFilePath = @"..\..\..\Examples\obj\Debug\Decl\Examples.dll";

        [ClassInitialize()]
        public static void GenerateEPAs(TestContext tc)
        {
            Configuration.Initialize();
            Configuration.TempPath = Directory.GetParent(tc.TestDir).ToString();
            Configuration.InlineMethodsBody = true;

            epaGenerator = new EpaGenerator(EpaGenerator.Backend.Corral);
            var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, InputFilePath));
            epaGenerator.LoadAssembly(ExamplesPath);
        }

        [ClassCleanup()]
        public static void Cleanup()
        {
            epaGenerator.Dispose();
        }

        //[TestMethod]
        //public void TestFiniteStack()
        //{
        //    var cancellationSource = new CancellationTokenSource();
        //    var epa = epaGenerator.GenerateEpa("Examples.FiniteStack", cancellationSource.Token).EPA;

        //    var filePath = Path.Combine(Configuration.TempPath, @"..\Tests\EPAs\FiniteStack.xml");
        //    using (Stream oStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        var deserializedEPA = new EpaXmlSerializer().Deserialize(oStream, InputFilePath);
        //        IsAnOverapproximation(epa, deserializedEPA);
        //    }
        //}

        //[TestMethod]
        //public void TestLinear()
        //{
        //    var cancellationSource = new CancellationTokenSource();
        //    var epa = epaGenerator.GenerateEpa("Examples.Linear", cancellationSource.Token).EPA;

        //    var filePath = Path.Combine(Configuration.TempPath, @"..\Tests\EPAs\Linear.xml");
        //    using (Stream oStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        var deserializedEPA = new EpaXmlSerializer().Deserialize(oStream, InputFilePath);
        //        IsAnOverapproximation(epa, deserializedEPA);
        //    }
        //}

        //[TestMethod]
        //public void TestDoor()
        //{
        //    var cancellationSource = new CancellationTokenSource();
        //    var epa = epaGenerator.GenerateEpa("Examples.Door", cancellationSource.Token).EPA;

        //    var filePath = Path.Combine(Configuration.TempPath, @"..\Tests\EPAs\Door.xml");
        //    using (Stream oStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        var deserializedEPA = new EpaXmlSerializer().Deserialize(oStream, InputFilePath);
        //        IsAnOverapproximation(epa, deserializedEPA);
        //    }
        //}

        //[TestMethod]
        //public void TestListItr()
        //{
        //    var cancellationSource = new CancellationTokenSource();
        //    var epa = epaGenerator.GenerateEpa("Examples.ICSE2011.ListItr",
        //        new List<string>(){"ListItr", "add", "next", "previous", "remove", "set"},
        //        cancellationSource.Token).EPA;

        //    var filePath = Path.Combine(Configuration.TempPath, @"..\Tests\EPAs\ListItr.xml");
        //    using (Stream oStream = new FileStream(filePath, FileMode.Open))
        //    {
        //        var deserializedEPA = new EpaXmlSerializer().Deserialize(oStream, InputFilePath);
        //        IsAnOverapproximation(epa, deserializedEPA);
        //    }
        //}

        private void IsAnOverapproximation(Epa epaUnderTest, Epa epaOracle)
        {
            Assert.IsTrue(Contract.ForAll(epaOracle.States, s => epaUnderTest.States.Any(s2 => s.Equals(s2))), 
                            "Does not contain all the states");
            Assert.IsTrue(Contract.ForAll(epaOracle.Transitions, t => epaUnderTest.Transitions.Any(t2 => t.Equals(t2) )), 
                            "Does not contain all the transitions");
        }
    }
}
