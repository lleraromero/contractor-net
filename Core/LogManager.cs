using System;
using System.Diagnostics.Contracts;

namespace Contractor.Core
{
    internal enum LogLevel { Fatal, Error, Warn, Info, Debug, Trace };

    internal sealed class LogManager
    {
        public static void Log(LogLevel level, Exception ex)
        {
            Contract.Requires(ex != null);

            NLog.LogManager.GetCurrentClassLogger().Log(ToLibraryLogLevel(level), ex);
        }

        public static void Log(LogLevel level, string message)
        {
            Contract.Requires(!string.IsNullOrEmpty(message));

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