using System;
using System.Diagnostics;
using System.IO;
using System.Threading;
using BytecodeTranslator;

namespace Analyzer.Corral
{
    internal class BctRunner
    {
        protected CancellationToken token;

        public BctRunner(CancellationToken token)
        {
            this.token = token;
        }

        public TimeSpan Run(string[] args)
        {
            // Check if the user stopped the analysis
            token.ThrowIfCancellationRequested();

            var timer = Stopwatch.StartNew();

            // I need to change the current directory so BCT can write the output in the correct folder
            var tmp = Environment.CurrentDirectory;
            Environment.CurrentDirectory = Path.GetDirectoryName(args[0]);
            var boogieTranslator = new BCT();

            if (boogieTranslator.Main(args) != 0)
            {
                Logger.Log(LogLevel.Fatal, "Error translating the query assembly to boogie");
                Logger.Log(LogLevel.Info, string.Format("args: {0}, {1}", args));
                throw new Exception("Error translating the query assembly to boogie");
            }
            Environment.CurrentDirectory = tmp;

            timer.Stop();
            return timer.Elapsed;
        }
    }
}