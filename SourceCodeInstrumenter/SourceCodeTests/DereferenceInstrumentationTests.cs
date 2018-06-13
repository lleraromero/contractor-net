using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeTests
{
    [TestClass]
    public class DereferenceInstrumentationTests
    {
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

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
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

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }
    }
}
