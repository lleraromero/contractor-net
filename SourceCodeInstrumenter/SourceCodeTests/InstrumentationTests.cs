using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeTests
{
    [TestClass]
    public class InstrumentationTests
    {
        [TestMethod]
        public void DivideByZeroAssignExample()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    private int _size;
    public Example()
	{
        _size /= 0;
    }
    public void m()
    {
        _size /= 0;
    }
}
            ";

            string oracle = @"
using System;
public class Example   
{
    private int _size;
    public Example()
	{
if(0 == 0 ){
throw new DivideByZeroException();
}
        _size /= 0;
    }
    public void m()
	{
if(0 == 0 ){
throw new DivideByZeroException();
}
        _size /= 0;
    }
}
            ";

            string instrumented = CallInstrumenter(fileToInstrument);
            AssertEquals(ref oracle, ref instrumented);
        }

        [TestMethod]
        public void DivideByZeroExample()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    private int _size;
    public Example()
	{
        _size = 10/0;
    }
    public void m()
    {
        _size = 10/0;
    }
}
            ";

            string oracle = @"
using System;
public class Example   
{
    private int _size;
    public Example()
	{
if(0 == 0 ){
throw new DivideByZeroException();
}
        _size = 10/0;
    }
    public void m()
	{
if(0 == 0 ){
throw new DivideByZeroException();
}
        _size = 10/0;
    }
}
            ";

            string instrumented = CallInstrumenter(fileToInstrument);
            AssertEquals(ref oracle, ref instrumented);
        }

        [TestMethod]
        public void IndexOutOfRangeExample()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    private int[] array;
    public Example()
	{
        array = {1,2,3};
    }
    public void m()
    {
        int i = array[2];
    }
}
            ";

            string oracle = @"
using System;
public class Example   
{
    private int[] array;
    public Example()
	{
        array = {1,2,3};
    }
    public void m()
    {
if(2 < 0 || 2 > array.Length)
{
throw new IndexOutOfRange();
}
        int i = array[2];
    }
}
            ";

            string instrumented = CallInstrumenter(fileToInstrument);
            AssertEquals(ref oracle, ref instrumented);
        }

        [TestMethod]
        public void StaticDereference()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    private int n;
    public Example()
	{
        n=4;
    }
    public void m()
    {
        int j= Math.Abs(n);
    }
}
            ";

            string oracle = @"
using System;
public class Example   
{
    private int n;
    public Example()
	{
        n=4;
    }
    public void m()
    {
        int j= Math.Abs(n);
    }
}
            ";

            string instrumented = CallInstrumenter(fileToInstrument);
            AssertEquals(ref oracle, ref instrumented);
        }

        [TestMethod]
        public void StaticDereferenceNotConsidered()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    private int n;
    public Example()
	{
        n=4;
    }
    public void m()
    {
        System.Console.Beep();
    }
}
            ";

            string oracle = @"
using System;
public class Example   
{
    private int n;
    public Example()
	{
        n=4;
    }
    public void m()
    {
        System.Console.Beep();
    }
}
            ";

            string instrumented = CallInstrumenter(fileToInstrument);
            AssertEquals(ref oracle, ref instrumented);
        }

        private static void AssertEquals(ref string oracle, ref string instrumented)
        {
            oracle = oracle.Where(x => !x.Equals(" ") && !x.Equals("  ")).ToString();
            instrumented = instrumented.Where(x => !x.Equals(" ") && !x.Equals("  ")).ToString();
            Assert.AreEqual(oracle, instrumented);
        }

        private static string CallInstrumenter(string fileToInstrument)
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
