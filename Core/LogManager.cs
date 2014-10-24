using System;
using System.Diagnostics.Contracts;

namespace Contractor.Core
{
    public enum LogLevel { Fatal, Error, Warn, Info, Debug, Trace };

    internal class LogManager
    {
        public static void Log(LogLevel level, object message)
        {
            if (message == null)
                return;

            switch (level)
            {
                case LogLevel.Fatal:
                    NLog.LogManager.GetCurrentClassLogger().Fatal(message.ToString());
                    break;

                case LogLevel.Error:
                    NLog.LogManager.GetCurrentClassLogger().Error(message.ToString());
                    break;

                case LogLevel.Warn:
                    NLog.LogManager.GetCurrentClassLogger().Warn(message.ToString());
                    break;

                case LogLevel.Info:
                    NLog.LogManager.GetCurrentClassLogger().Info(message.ToString());
                    break;

                case LogLevel.Debug:
                    NLog.LogManager.GetCurrentClassLogger().Debug(message.ToString());
                    break;

                case LogLevel.Trace:
                    NLog.LogManager.GetCurrentClassLogger().Trace(message.ToString());
                    break;

                default:
                    throw new NotImplementedException("Unknown LogLevel");
            }
        }
    }
}