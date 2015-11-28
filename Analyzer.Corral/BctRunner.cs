using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;

namespace Analyzer.Corral
{
    internal class BctRunner
    {
        public void Run(string[] args)
        {
            var tmpDir = Path.GetDirectoryName(args[0]);
            Contract.Assert(!string.IsNullOrEmpty(tmpDir) && Directory.Exists(tmpDir));

            var output = new StringBuilder();

            using (var bct = new Process())
            {
                bct.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Users\lean\Desktop\bct-contractor\Binaries\BytecodeTranslator.exe",
                    Arguments = string.Join(" ", args),
                    WorkingDirectory = tmpDir,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                Logger.Log(LogLevel.Info, "=============== BCT ===============");
                bct.OutputDataReceived += (sender, e) =>
                {
                    output.AppendLine(e.Data);
                    Logger.Log(LogLevel.Debug, e.Data);
                };
                bct.ErrorDataReceived += (sender, e) => { Logger.Log(LogLevel.Fatal, e.Data); };
                bct.Start();
                bct.BeginErrorReadLine();
                bct.BeginOutputReadLine();
                bct.WaitForExit();

                if (bct.ExitCode != 0)
                {
                    Logger.Log(LogLevel.Fatal, "Error translating the query assembly to boogie");
                    Logger.Log(LogLevel.Info, string.Format("args: {0}, {1}", args));
                    throw new Exception("Error translating the query assembly to boogie");
                }
            }
        }
    }
}