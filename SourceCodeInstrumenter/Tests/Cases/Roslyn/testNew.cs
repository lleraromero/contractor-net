﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases.Roslyn
{
    class TestNew
    {


        public static void Main(string[] args)
        {
            Casa casa = new Casa();
            Console.WriteLine(casa.duenho);
        }

        class Casa
        {
            public string duenho = "valor";
            public Casa()
            {
                duenho = "Christian";
            }
        }
    }
}
