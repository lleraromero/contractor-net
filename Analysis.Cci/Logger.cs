using NLog.Config;
using NLog.Targets;
using System;
using System.Diagnostics.Contracts;
using System.IO;

namespace Contractor.Core
{
    internal enum LogLevel { Fatal, Error, Warn, Info, Debug, Trace };

    internal class Logger
    {
        public static void SetUpLogger(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename));

            // Step 1. Create configuration object 
            var config = new LoggingConfiguration();

            // Step 2. Create targets and add them to the configuration 
            var fileTarget = new FileTarget();
            config.AddTarget("file", fileTarget);

            // Step 3. Set target properties 
            fileTarget.FileName = filename;
            fileTarget.Layout = "${longdate} | ${level} | ${newline}${message}";

            // Step 4. Define rules
            var rule = new LoggingRule("*", NLog.LogLevel.Trace, fileTarget);
            config.LoggingRules.Add(rule);

            // Step 5. Activate the configuration
            NLog.LogManager.Configuration = config;
        }

        public static void Log(LogLevel level, Exception ex)
        {
            Contract.Requires(ex != null);

            NLog.LogManager.GetCurrentClassLogger().Log(ToLibraryLogLevel(level), ex);
        }

        public static void Log(LogLevel level, string message)
        {
            Contract.Requires(message != null);

            NLog.LogManager.GetCurrentClassLogger().Log(ToLibraryLogLevel(level), message);
        }

        private static NLog.LogLevel ToLibraryLogLevel(LogLevel level)
        {
            switch (level)
            {
                case LogLevel.Fatal:
                    return NLog.LogLevel.Fatal;
                case LogLevel.Error:
                    return NLog.LogLevel.Error;
                case LogLevel.Warn:
                    return NLog.LogLevel.Warn;
                case LogLevel.Info:
                    return NLog.LogLevel.Info;
                case LogLevel.Debug:
                    return NLog.LogLevel.Debug;
                case LogLevel.Trace:
                    return NLog.LogLevel.Trace;
                default:
                    throw new NotImplementedException("Unknown LogLevel");
            }
        }
    }
}