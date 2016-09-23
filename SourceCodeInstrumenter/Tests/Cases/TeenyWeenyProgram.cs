using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Tests.Util;

namespace Tests.Cases
{
    class TeenyWeenyProgram
    {
        static void Main(string[] args)
        {
        }

        [SlicingResult(Criteria = 3, Sliced = new[] {3, 8, 9})]
        [SlicingResult(Criteria = 2, Sliced = new[] {7, 11})]
        public object Result { get; set; }
    }
}
