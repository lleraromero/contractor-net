using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class ClassConstrWithoutInstructions
    {
        public static void Main(string[] args)
        {
            K instanciaK = new K("valor");
        }
        public class K
        {
            public K(string se)
            {
            }
        }
    }
}
