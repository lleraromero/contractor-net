using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class ParenthesizedNulleExpression
    {
        static void Main(string[] args)
        {
            int? j = null;
            var g = (j);
            Console.WriteLine(g);
        }
    }
}
