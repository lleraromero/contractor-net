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
if((2) < 0 || (2) >= array.Length)
throw new IndexOutOfRangeException();
        int i = array[2];
    }
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }

        [TestMethod]
        public void NestedIndexOutOfRange()
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
        int i=0;
        while(i<3){
            if(i!=0){
                var j= array[i];
            }
            i++;
        }
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
        int i=0;
        while(i<3){
            if(i!=0){
if((i) < 0 || (i) >= array.Length)
throw new IndexOutOfRangeException();
                var j= array[i];
            }
            i++;
        }
    }
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }
    }
}
