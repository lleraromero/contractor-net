using Contractor.Core;
using System;
using System.IO;

namespace EPAOverlapper
{
    class Program
    {
        static void Main(string[] args)
        {
            string workingDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            string baseEPAFile = Path.Combine(workingDir, @"prueba.xml");
            string topEPAFile = Path.Combine(workingDir, @"epa-invariant\epas-different-inv\ListItr-Guido.xml");

            using (var stream = File.OpenRead(baseEPAFile))
            using (var stream2 = File.OpenRead(topEPAFile))
            {
                string inputAssemblyPath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll");
                var epa = new EpaXmlSerializer().Deserialize(stream, inputAssemblyPath);
                var epa2 = new EpaXmlSerializer().Deserialize(stream2, inputAssemblyPath);

                string destinationFile = Path.Combine(workingDir, @"ListItr-Overlapped-Test3.png");
                using (var streamo = File.Create(destinationFile))
                {
                    // e2 has to be a subgraph of e1
                    (new EpaBinarySerializer()).SerializeOverlapped(streamo, epa, epa2);
                }
            }            
        }
    }
}
