using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Tests.Cases
{
    class TryCatchStatement
    {
        static void Main()
        {
            string s = null; // For demonstration purposes.
            try
            {
                s = ProcessString(s);
            }

            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            var t = s;
        }

        static String ProcessString(string s)
        {
            if (s == null)
            {
                throw new ArgumentNullException();
            }
            else
            {
                return s + "hola";
            }

        }
    }
}
