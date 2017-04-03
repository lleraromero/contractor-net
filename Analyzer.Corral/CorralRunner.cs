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
        protected readonly DirectoryInfo workingDir;
        protected BoggieHardcoderForExceptionSupport boogieHarcoder;

        public CorralRunner(string defaultArgs, DirectoryInfo workingDir, BoggieHardcoderForExceptionSupport boogieHarcoder)
        {
            corralArguments = defaultArgs;
            this.workingDir = workingDir;
            this.boogieHarcoder = boogieHarcoder;
        }

        public QueryResult Execute(FileInfo queryAssembly, Query query)
        {
            var tmpDir = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tmpDir);

            var args = string.Format("{0} /main:{1} {2}", queryAssembly.FullName, BctMethodNamer.CreateUniqueMethodName(query), corralArguments);
            var output = new StringBuilder();

            MyLogger.LogCorral(args);

            //THIS SHOULD BE NOT HERE.  
            //var boogieHarcoder = new BoggieHardcoderForExceptionSupport();
            this.boogieHarcoder.hardcodeExceptionsToFile(queryAssembly.FullName);
            //--
            using (var corral = new Process())
            {
                corral.StartInfo = new ProcessStartInfo
                {
                    FileName = @"..\..\..\Dependencies\Corral\corral.exe",
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

        public QueryResult ExecuteAfterHarcodeExceptions(FileInfo queryAssembly, Query query)
        {
            var tmpDir = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tmpDir);

            var args = string.Format("{0} /main:{1} {2}", queryAssembly.FullName, BctMethodNamer.CreateUniqueMethodName(query), corralArguments);
            var output = new StringBuilder();

            MyLogger.LogCorral(args);

            var boogieHarcoder = new BoggieHardcoderForExceptionSupport();
            boogieHarcoder.hardcodeExceptionsToFile(queryAssembly.FullName);

            using (var corral = new Process())
            {
                corral.StartInfo = new ProcessStartInfo
                {
                    FileName = @"..\..\..\Dependencies\Corral\corral.exe",
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

            if (message.Contains("reached recursion bound"))
            {
                return QueryResult.MaybeReachable;
            }

            if (message.Contains("true bug"))
            {
                return QueryResult.Reachable;
            }

            if (message.Contains("has no bugs"))
            {
                return QueryResult.Unreachable;
            }

            throw new NotImplementedException("The result was not understood");
        }
    }
}