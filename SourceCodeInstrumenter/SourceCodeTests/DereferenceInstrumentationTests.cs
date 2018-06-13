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
        public void StaticDereferenceMathAbs()
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
        public void StaticDereferenceSystem()
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

        [TestMethod]
        public void StaticDereferenceThreading
            ()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    [NonSerialized()]
    private Object _syncRoot;
    public virtual Object SyncRoot {
			get {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
				return _syncRoot;
			}
	}
}
            ";

            string oracle = @"
using System;
public class Example   
{
    [NonSerialized()]
    private Object _syncRoot;
    public virtual Object SyncRoot {
			get {
                if (_syncRoot == null)
                {
                    System.Threading.Interlocked.CompareExchange<Object>(ref _syncRoot, new Object(), null);
                }
				return _syncRoot;
			}
	}
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }

        [TestMethod]
        public void NestedDereference
            ()
        {
            string fileToInstrument = @"
using System;
public class Example   
{
    private Object _syncRoot;
    public Object SyncRoot {
			get {
                if (_syncRoot == null)
                {
                   _syncRoot.Equals(null);
                }
				return _syncRoot;
			}
	}
}
            ";

            string oracle = @"
using System;
public class Example   
{
    private Object _syncRoot;
    public Object SyncRoot {
			get {
                if (_syncRoot == null)
                {
                    if(_syncRoot==null) throw new NullReferenceException();
                    _syncRoot.Equals(null);
                }
				return _syncRoot;
			}
	}
}
            ";

            string instrumented = TestUtil.CallInstrumenter(fileToInstrument);
            TestUtil.AssertEquals(ref oracle, ref instrumented);
        }

    }
}
