﻿using BinarySample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class NonInstrumentedNestedOutside
    {
        public static void Main(string[] args)
        {
            object a = new object();
            object ret = Binary.Test(a);
            object b = Binary.Test(ret);
        }
    }
}
