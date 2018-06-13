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
    public class IndexAccessInstrumentationTests
    {
        
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

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }

        
    }
}
