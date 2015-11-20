using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using Contractor.Core;

namespace Analyzer.Corral
{
    internal class CorralRunner
    {
        protected CancellationToken token;
        protected Query result;

        public CorralRunner(CancellationToken token)
        {
            Contract.Requires(token != null);

            this.token = token;
        }

        public Query Result
        {
            get { return result; }
        }

        public TimeSpan Run(string args, Query query)
        {
            Contract.Requires(!string.IsNullOrEmpty(args));

            // Check if the user stopped the analysis
            token.ThrowIfCancellationRequested();

            var timer = Stopwatch.StartNew();

            var tmpDir = Path.Combine(Configuration.TempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tmpDir);

            var output = new StringBuilder();

            using (var corral = new Process())
            {
                corral.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\lean\Desktop\corral\bin\Debug\corral.exe",
                    Arguments = args,
                    WorkingDirectory = tmpDir,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                Logger.Log(LogLevel.Info, "=============== CORRAL ===============");
                corral.OutputDataReceived += (sender, e) =>
                {
                    output.AppendLine(e.Data);
                    Logger.Log(LogLevel.Debug, e.Data);
                };
                corral.ErrorDataReceived += (sender, e) => { Logger.Log(LogLevel.Fatal, e.Data); };
                corral.Start();
                corral.BeginErrorReadLine();
                corral.BeginOutputReadLine();
                corral.WaitForExit();

                if (corral.ExitCode != 0)
                {
                    throw new Exception("Error executing corral");
                }
            }

            Directory.Delete(tmpDir, true);
            timer.Stop();

            result = ParseResultKind(output.ToString(), query);

            return timer.Elapsed;
        }

        protected Query ParseResultKind(string message, Query query)
        {
            message = message.ToLower();

            if (message.Contains("true bug"))
            {
                return new ReachableQuery(query.Action);
            }

            if (message.Contains("has no bugs"))
            {
                return new UnreachableQuery(query.Action);
            }

            if (message.Contains("recursion bound reached"))
            {
                return new MayBeReachableQuery(query.Action);
            }

            throw new NotImplementedException("The result was not understood");
        }
    }
}