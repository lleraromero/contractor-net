using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeTests
{
    public class TestUtil
    {
        public static void AssertEquals(ref string oracle, ref string instrumented)
        {
            oracle = oracle.Where(x => !x.Equals(" ") && !x.Equals("  ")).ToString();
            instrumented = instrumented.Where(x => !x.Equals(" ") && !x.Equals("  ")).ToString();
            Assert.AreEqual(oracle, instrumented);
        }

        public static string CallInstrumenter(string fileToInstrument)
        {
            //write test to file
            string[] lines = { fileToInstrument };
            File.WriteAllLines(@"../../../ExampleToTest/StackMscorlib/Example.cs", lines);

            //call instrumenter
            //string outputPath = "../../../ExampleToTest/Instrumented";
            string outputPath = "C:\\Users\\Fernan\\Documents\\contractor-net\\SourceCodeInstrumenter\\ExampleToTest\\Instrumented";
            //string[] args = { "../../../ExampleToTest/ExampleToTest.sln", "Test", outputPath, "Example.cs" };
            string sln = "C:\\Users\\Fernan\\\\Documents\\contractor-net\\SourceCodeInstrumenter\\ExampleToTest\\ExampleToTest.sln";
            string[] args = { sln, "StackMscorlib", outputPath, "Example.cs" };
            DC.Slicer.Program.Main(args);

            //get instrumented file
            string instrumented = File.ReadAllText(@"../../../ExampleToTest/Instrumented/StackMscorlib/Example.cs");
            return instrumented;
        }

    }
}
