using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases.Solver
{
    class ChainNews
    {
        public static void Main(string[] args)
        {
           int CANT_NEWS = 10;
            
           var v1 = new B();
           for (int i = 0; i < CANT_NEWS; i++)
           {
               v1 = new B(v1);
           }

            
            //var v1 = new B();
            //var v2 = new B(v1);
            //var v3 = new B(v2);
            //var v4 = new B(v3);
            //var v5 = new B(v4);
            //var v6 = new B(v5);
            //var v7 = new B(v6);
            //var v8 = new B(v7);
            //var v9 = new B(v8);
            //var v10 = new B(v9);

            //B[] miArray = new B[CANT_NEWS];
            //miArray[0] = new B();
            //for (int i = 1; i < CANT_NEWS; i++)
            //{
            //    miArray[i] = new B(miArray[i - 1]);
            //}
            
           
            return;
        }

        class B
        {
            public B miProperty;

            public B()
            {

            }

            public B(B _b)
            {
                this.miProperty = _b;
            }
        }
        
    }
}
