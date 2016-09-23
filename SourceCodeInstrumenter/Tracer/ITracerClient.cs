﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Tracer
{
    interface ITracerClient
    {
        void Initialize();
        void Trace(int fileId, int traceType, int spanStart, int spanEnd);
    }
}
