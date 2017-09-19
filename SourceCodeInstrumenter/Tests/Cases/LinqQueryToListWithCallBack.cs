using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class LinqQueryToListWithCallBack
    {
        static void Main(string[] args)
        {
            IList<int> list = new List<int>();
            list.Add(1);
            int y = 2;
            Modifier m = new Modifier();
            IList<int> mapApplied = list.Select(x => m.modify(x,y)).ToList();
            int z = mapApplied.First();
        }
        public class Modifier
        {
            static int f = 1;
            public int modify(int x, int y)
            {
                return x + y;
            }
        }
    }
}
