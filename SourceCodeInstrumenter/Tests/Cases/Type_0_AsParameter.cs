using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class Type_0_AsParameter
    {
        public static void Main(string[] args)
        {
            A a = new A();
            int x = a.DoSomething(new Type[0]);
            int y = x;
        }
        public class A
        {
            public int DoSomething(Type[] arrType)
            {
                int x = 0;
                if (arrType.Count() == 0)
                {
                    x = 3;
                }
                return x;
            }
        }
    }
}
