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
            errorList.Add("OverflowException");
            errorList.Add("IndexOutOfRangeException");
            errorList.Add("NullReferenceException");
            errorList.Add("DivideByZeroException");
            errorList.Add("IllegalStateException");
            errorList.Add("ConcurrentModificationException");
            errorList.Add("NoSuchElementException");
            errorList.Add("ArgumentNullException");
            errorList.Add("ArgumentException");
            errorList.Add("ArgumentOutOfRangeException");
            errorList.Add("InvalidOperationException");
            errorList.Add("IOException");
            errorList.Add("EndOfStreamException");
            errorList.Add("ObjectDisposedException");
            errorList.Add("NotSupportedException");
            errorList.Add("NotImplementedException");
            errorList.Add("AssertFailedException");
        }
    }
}
