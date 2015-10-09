using Contractor.Core;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace EPAOverlapper
{
    class Program
    {
        static void Main(string[] args)
        {
            //string workingDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            //string baseEPAFile = Path.Combine(workingDir, @"Base.xml");
            //string topEPAFile = Path.Combine(workingDir, @"ListItr-PerfectInv.xml");

            //using (var stream = File.OpenRead(baseEPAFile))
            //using (var stream2 = File.OpenRead(topEPAFile))
            //{
            //    string inputAssemblyPath = Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll");
            //    var epa = new EpaXmlSerializer().Deserialize(stream, inputAssemblyPath);
            //    var epa2 = new EpaXmlSerializer().Deserialize(stream2, inputAssemblyPath);

            //    string[] baseFile = baseEPAFile.Split('\\');
            //    string[] topFile = topEPAFile.Split('\\');
            //    string destinationFile = Path.Combine(workingDir, string.Format(@"{0}-{1}-{2}.png", baseFile[baseFile.Length - 1],
            //                                                                                        topFile[topFile.Length - 1],
            //                                                                                        DateTime.Now.ToString("dd_MM_yyyy_HHMMss")));
            //    using (var streamo = File.Create(destinationFile))
            //    {
            //        // e2 has to be a subgraph of e1
            //        (new EpaBinarySerializer()).SerializeOverlapped(streamo, epa, epa2);
            //    }
            //}
        }
    }
}
