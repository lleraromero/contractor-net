using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using Log;

namespace Analyzer.Corral
{
    internal class BctRunner
    {
        public void Run(string[] args)
        {
            Contract.Requires(args.Length > 0);

            var tmpDir = Path.GetDirectoryName(args[0]);
            Contract.Assert(!string.IsNullOrEmpty(tmpDir) && Directory.Exists(tmpDir));

            MyLogger.LogBCT(string.Join(" ", args));

            using (var bct = new Process())
            {
                bct.StartInfo = new ProcessStartInfo
                {
                    FileName = AppDomain.CurrentDomain.BaseDirectory + @"..\..\..\Dependencies\BCT\BytecodeTranslator.exe",
                    //FileName = @"C:\Users\Administrador\Documents\Visual Studio 2013\Projects\BCT\bytecodetranslator\Binaries\BytecodeTranslator.exe",
                    Arguments = string.Join(" ", args),
                    WorkingDirectory = tmpDir,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                bct.OutputDataReceived += (sender, e) => { Logger.Log(LogLevel.Debug, "BCT: " + e.Data); };
                bct.ErrorDataReceived += (sender, e) => { Logger.Log(LogLevel.Fatal, "BCT: " + e.Data); };
                //Console.WriteLine(string.Join(" ", args));
                //Console.WriteLine(tmpDir);
                bct.Start();
                bct.BeginErrorReadLine();
                bct.BeginOutputReadLine();
                bct.WaitForExit();

                if (bct.ExitCode != 0)
                {
                    Logger.Log(LogLevel.Fatal, "BCT: Error translating the query assembly to boogie");

                    MyLogger.LogBCTBreakingQuery(string.Join(" ", string.Join(" ", args)));

                    Logger.Log(LogLevel.Info, string.Format("BCT: args: {0}, {1}", args));
                    throw new Exception("BCT: Error translating the query assembly to boogie");
                }
            }
        }
    }
}