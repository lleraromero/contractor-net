﻿using BinarySample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class NonInstrumentedNestedInside
    {
        public static void Main(string[] args)
        {
            object a = new object();
            object b = Binary.Test(Binary.Test(a));
        }
    }
}
