﻿using System;
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
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
