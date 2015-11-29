using Contractor.Core.Properties;
using System;
using System.IO;
using System.Text;

namespace Contractor.Core
{
    public static class Configuration
    {
        public static string TempPath;
        public static string CheckerFileName;
        public static string CheckerArguments;
        public static bool InlineMethodsBody;

        private static string windows;
        private static string programFiles;
        private static string codeContracts;

        public static void Initialize()
        {
            windows = Environment.GetFolderPath(Environment.SpecialFolder.Windows);
            programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFiles);

            if (Environment.Is64BitOperatingSystem)
                programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);

            InlineMethodsBody = true;
            CheckerFileName = ExpandVariables(Resources.CheckerFileName);
            CheckerArguments = Resources.CheckerArguments;
            var dependenciesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Dependencies"));

            TempPath = Path.Combine(Path.GetTempPath(), "Contractor");
            Directory.CreateDirectory(TempPath);

            codeContracts = Environment.GetEnvironmentVariable("CodeContractsInstallDir");
            if (string.IsNullOrEmpty(codeContracts))
            {
                var msg = new StringBuilder();
                msg.AppendLine("The environment variable %CodeContractsInstallDir% does not exist.");
                msg.AppendLine("Please make sure that Code Contracts is installed correctly.");
                msg.AppendLine("This might be because the system was not restarted after Code Contracts installation.");

                throw new DirectoryNotFoundException(msg.ToString());
            }
        }

        public static string ExpandVariables(string text)
        {
            text = text.Replace("@codeContracts", codeContracts);
            text = text.Replace("@windows", windows);
            text = text.Replace("@programFiles", programFiles);
            text = text.Replace("\t", string.Empty);
            text = text.Replace("\n", string.Empty);
            text = text.Replace("\r", string.Empty);
            text = text.Replace("\\\\", "\\");
            return text;
        }
    }
}
