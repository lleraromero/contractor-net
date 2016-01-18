using System;
using System.IO;
using Contractor.Core;

namespace EPAOverlapper
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var workingDir = Environment.GetFolderPath(Environment.SpecialFolder.Desktop);
            var baseEPAFile = Path.Combine(workingDir, @"ListItr-guido-contractor-net.xml");
            var topEPAFile = Path.Combine(workingDir, @"ListItr-guido-contractor-net.xml");

            using (var stream = File.OpenRead(baseEPAFile))
            using (var stream2 = File.OpenRead(topEPAFile))
            {
                var epa = new EpaXmlSerializer().Deserialize(stream);
                var epa2 = new EpaXmlSerializer().Deserialize(stream2);

                var baseFile = baseEPAFile.Split('\\');
                var topFile = topEPAFile.Split('\\');
                var destinationFile = Path.Combine(workingDir, string.Format(@"{0}-{1}-{2}.png", baseFile[baseFile.Length - 1],
                    topFile[topFile.Length - 1],
                    DateTime.Now.ToString("dd_MM_yyyy_HHMMss")));
                using (var streamo = File.Create(destinationFile))
                {
                    // e2 has to be a subgraph of e1
                    new EpaOverlapperBinarySerializer().SerializeOverlapped(streamo, epa, epa2);
                }
            }
        }
    }
}