using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Analysis.Cci
{
    public class ExceptionEncoder
    {
        public static int ExceptionToInt(string exitCode){
            Contract.Requires(exitCode != null);
            switch (exitCode)
            {
                case "Ok":
                    return 0;
                case "NullReferenceException":
                    return 1;
                case "IndexOutOfRangeException":
                    return 2;
                case "DivideByZeroException":
                    return 3;
                case "OverflowException":
                    return 4;
                case "Exception":
                    return 5;
                case "IllegalStateException":
                    return 6;
                case "ConcurrentModificationException":
                    return 7;
                case "NoSuchElementException":
                    return 8;
                case "ArgumentNullException":
                    return 9;
                case "ArgumentException":
                    return 10;
                case "ArgumentOutOfRangeException":
                    return 11;
                case "InvalidOperationException":
                    return 12;

                default:
                    throw new NotSupportedException();
            }
        }
    }
}
