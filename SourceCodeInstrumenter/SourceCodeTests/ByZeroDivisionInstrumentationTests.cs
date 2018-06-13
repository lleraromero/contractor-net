using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeTests
{
    [TestClass]
    public class ByZeroDivisionInstrumentationTests
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
if(0 == 0 )
throw new DivideByZeroException();

        _size /= 0;
    }
    public void m()
	{
if(0 == 0 )
throw new DivideByZeroException();

        _size /= 0;
    }
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
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
if(0 == 0 )
throw new DivideByZeroException();

        _size = 10/0;
    }
    public void m()
	{
if(0 == 0 )
throw new DivideByZeroException();

        _size = 10/0;
    }
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }
    }
}
