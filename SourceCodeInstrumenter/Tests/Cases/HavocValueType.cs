﻿using BinarySample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class HavocValueType
    {
        public static void Main(string[] args)
        {
            object para = new object();
            object parb = new object();
            int pepe = Binary.TestScalarRet(para, parb);
            int jose = pepe;
        }
    }
}
