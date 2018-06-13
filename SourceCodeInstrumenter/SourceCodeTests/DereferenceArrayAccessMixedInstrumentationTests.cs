using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SourceCodeTests
{
    [TestClass]
    public class DereferenceArrayAccessMixedInstrumentationTests
    {
        [TestMethod]
        public void DereferenceAndAccessArrayMixedTest()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    [NonSerialized()]
    private Object[] _array;   

        [Serializable()]
		private class StackEnumerator
		{
            private Example _s;

            public void m(index){
                var currentElement = _s._array[_index];
            }
        }
}
            ";

            string oracle = @"
using System;
public class Example   
{
    [NonSerialized()]
    private Object[] _array;   

        [Serializable()]
		private class StackEnumerator
		{
            private Example _s;

            public void m(index){
if((_index) < 0 || (_index) >= _s._array.Length)
throw new IndexOutOfRangeException();
					if(_s == null )
throw new NullReferenceException();
                var currentElement = _s._array[_index];
            }
        }
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }
        


    }
}
