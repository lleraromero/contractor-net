using Contractor.Core.Properties;
using System;
using System.IO;
using System.Text;

namespace Contractor.Core
{
    public static class Configuration
    {
        public static string TempPath;
        public static bool InlineMethodsBody;

        public static void Initialize()
        {
            InlineMethodsBody = true;

            TempPath = Path.Combine(Path.GetTempPath(), "Contractor");
            Directory.CreateDirectory(TempPath);
        }
    }
}
