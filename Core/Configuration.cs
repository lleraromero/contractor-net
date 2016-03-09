﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using Contractor.Core.Properties;
using NLog.Config;
using NLog.Targets;

namespace Contractor.Core
{
    public static class Configuration
    {
        public static string TempPath;
        public static string CheckerFileName;
        public static string CheckerArguments;
        public static string CorralArguments;
        public static bool InlineMethodsBody;
        public static bool TransitionsWithConditions;

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
            //TODO: ask for check box value
            TransitionsWithConditions = true;
            CheckerFileName = ExpandVariables(Resources.CheckerFileName);
            CheckerArguments = Resources.CheckerArguments;
            var dependenciesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Dependencies"));
            CorralArguments = Resources.CorralArguments;

#if DEBUG
            TempPath = Path.Combine(Directory.GetCurrentDirectory(), "Temp");

            if (!Directory.Exists(TempPath))
                Directory.CreateDirectory(TempPath);
#else
			TempPath = Path.GetTempPath();
#endif

            codeContracts = Environment.GetEnvironmentVariable("CodeContractsInstallDir");
            if (string.IsNullOrEmpty(codeContracts))
            {
                var msg = new StringBuilder();
                msg.AppendLine("The environment variable %CodeContractsInstallDir% does not exist.");
                msg.AppendLine("Please make sure that Code Contracts is installed correctly.");
                msg.AppendLine("This might be because the system was not restarted after Code Contracts installation.");

                throw new DirectoryNotFoundException(msg.ToString());
            }

            SetUpLogger();
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

        private static void SetUpLogger()
        {
            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = Path.Combine(TempPath, "contractor.out");
            fileTarget.Layout = "${longdate} | ${level} | ${newline}${message}";

            // Step 4. Define rules
            var rule = new LoggingRule("*", NLog.LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule);

            // Step 5. Activate the configuration
            NLog.LogManager.Configuration = config;
        }
    }
}
