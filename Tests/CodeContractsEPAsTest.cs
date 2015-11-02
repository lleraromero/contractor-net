using Analysis.Cci;
using Contractor.Core;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.IO;

namespace Tests
{
    [TestClass]
    public class CodeContractsEPAsTest
    {
        //private const string InputFilePath = @"..\..\..\Examples\obj\Debug\Decl\Examples.dll";
        //protected static IAssemblyXXX inputAssembly;

        //[ClassInitialize()]
        //public static void GenerateEPAs(TestContext tc)
        //{
        //    Configuration.Initialize();
        //    Configuration.TempPath = Directory.GetParent(tc.TestDir).ToString();
        //    Configuration.InlineMethodsBody = true;

        //    var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, InputFilePath));

        //    inputAssembly = new CciDecompiler().Decompile(ExamplesPath, null);
        //}

        //[TestMethod]
        //public void TestVendingMachine()
        //{
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

        //private void GenerateEpa(string typeToAnalyze)
        //{
        //    throw new NotImplementedException();
        //    //var epaGenerator = new EpaGenerator(inputAssembly, new AnalyzerMock());
        //    //epaGenerator.GenerateEpa(typeToAnalyze);
        //}
    }
}
