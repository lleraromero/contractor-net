using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases.Roslyn
{
    class newWithNamedArgument
    {
        public static void Main(string[] args)
        {
            Casa casa = new Casa(nombre: "Christian");
            Console.WriteLine(casa.duenho);
        }

        class Casa
        {
            public string duenho;
            public Casa(string nombre)
            {
                duenho = nombre;
            }
        }
    }
}
