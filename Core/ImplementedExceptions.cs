using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contractor.Core
{
    public class ImplementedExceptions
    {
        public static void AddAllExceptionsTo(List<string> errorList)
        {
            errorList.Add("System.OverflowException");
            errorList.Add("System.IndexOutOfRangeException");
            errorList.Add("System.NullReferenceException");
            errorList.Add("System.DivideByZeroException");
            errorList.Add("System.IllegalStateException");
            errorList.Add("System.ConcurrentModificationException");
            errorList.Add("System.NoSuchElementException");
            errorList.Add("System.ArgumentNullException");
            errorList.Add("System.ArgumentException");
            errorList.Add("System.ArgumentOutOfRangeException");
            errorList.Add("System.InvalidOperationException");
            errorList.Add("System.IO.IOException");
            errorList.Add("System.IO.EndOfStreamException");
            errorList.Add("System.ObjectDisposedException");
            errorList.Add("System.NotSupportedException");
            errorList.Add("System.NotImplementedException");
            errorList.Add("System.InvalidCastException");
            errorList.Add("System.RankException");
            errorList.Add("AssertFailedException");
        }
    }
}
