using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using Contractor.Core;
using Log;

namespace Analyzer.Corral
{
    internal class CorralRunner : ISolver
    {
        protected readonly string corralArguments;

        public CorralRunner(string defaultArgs)
        {
            corralArguments = defaultArgs;
        }

        public QueryResult Execute(FileInfo queryAssembly, Query query)
        {
            var tmpDir = Path.Combine(Configuration.TempPath, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tmpDir);

            var args = string.Format("{0} /main:{1} {2}", queryAssembly.FullName, BctTranslator.CreateUniqueMethodName(query), corralArguments);
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

            return ParseResultKind(output.ToString(), query);
        }

        protected QueryResult ParseResultKind(string message, Query query)
        {
            message = message.ToLower();

            if (message.Contains("true bug"))
            {
                return QueryResult.Reachable;
            }

            if (message.Contains("has no bugs"))
            {
                return QueryResult.Unreachable;
            }

            if (message.Contains("recursion bound reached"))
            {
                return QueryResult.MaybeReachable;
            }

            throw new NotImplementedException("The result was not understood");
        }
    }
}