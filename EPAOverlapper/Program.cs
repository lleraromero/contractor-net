using Contractor.Core;
using System.IO;

namespace EPAOverlapper
{
    class Program
    {
        static void Main(string[] args)
        {
            using (var stream = File.OpenRead(@"C:\Users\lean\Desktop\diego.xml"))
            using (var stream2 = File.OpenRead(@"C:\Users\lean\Desktop\ListItr-Guido.xml"))
            {
                var epa = new EpaXmlSerializer().Deserialize(stream, @"D:\contractor-net\Examples\obj\Debug\Decl\Examples.dll");
                var epa2 = new EpaXmlSerializer().Deserialize(stream2, @"D:\contractor-net\Examples\obj\Debug\Decl\Examples.dll");

                using (var streamo = File.Create(@"C:\Users\lean\Desktop\diego.png"))
                {
                    // e2 has to be a subgraph of e1
                    (new EpaBinarySerializer()).SerializeOverlapped(streamo, epa, epa2);
                }
            }            
        }
    }
}
