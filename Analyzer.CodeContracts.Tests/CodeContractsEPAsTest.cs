using System;
using System.IO;
using System.Linq;
using Analysis.Cci;
using Contractor.Core;
using Contractor.Core.Model;
using FakeItEasy;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Analyzer.CodeContracts.Tests
{
    [TestClass]
    public class CodeContractsEPAsTest
    {
        //private const string InputFilePath = @"..\..\..\Examples\obj\Debug\Decl\Examples.dll";
        //protected static IAssembly inputAssembly;

        //[ClassInitialize]
        //public static void GenerateEPAs(TestContext tc)
        //{
        //    var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, InputFilePath));

        //    inputAssembly = new CciDecompiler().Decompile(ExamplesPath, null);
        //}

        //[TestMethod]
        //public void TestVendingMachine()
        //{
        //    //var Type = FindTypeDefinitionInAssemblyWithName("VendingMachine");
        //    //var epa = GenerateEpa(type)
        //    //assert
        //    //assert
        //    GenerateEpa("Examples.VendingMachine");
        //}

        //[TestMethod]
        //public void TestLinear()
        //{
        //    GenerateEpa("Examples.Linear");
        //}

        //[TestMethod]
        //public void TestDoor()
        //{
        //    GenerateEpa("Examples.Door");
        //}

        //[TestMethod]
        //public void TestFiniteStack()
        //{
        //    GenerateEpa("Examples.FiniteStack");
        //}

        //protected void GenerateEpa(string typeToAnalyze)
        //{
        //    var epaGenerator = new EpaGenerator(new AnalyzerMock());
        //    var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(typeToAnalyze));
        //    var epa = epaGenerator.GenerateEpa(typeDefinition, A.Dummy<IEpaBuilder>());
        //    epa.Wait();

        //    Assert.IsFalse(epa.IsFaulted);
        //    Assert.IsFalse(epa.IsCanceled);
        //}
    }
}
