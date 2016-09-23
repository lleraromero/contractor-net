using BinarySample;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class OverrideToStringTest
    {
        public static void Main(string[] args)
        {
            Cliente c = new Cliente();
            string a = "asda" + c;
        }
    }

    public class Cliente
    {
        public string Val { get; set; }
        public override string ToString()
        {
            return "mi string";
        }
    }
}
