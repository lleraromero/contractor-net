using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class KeywordYield
    {
        public static void Main(string[] args)
        {
            foreach (int i in Integers())
            {
                Console.WriteLine(i.ToString());
            }
            return;
        }
        public static IEnumerable<int> Integers()
        {
            yield return 1;
            yield return 2;
            yield return 4;
            yield return 8;
            yield return 16;
            yield return 16777216;
        }
    }
}
