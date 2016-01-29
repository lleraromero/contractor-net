using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Text;
using Contractor.Core;
using Contractor.Core.Model;
using Log;

namespace Analyzer.CodeContracts
{
    internal class CodeContractsRunner : ISolver
    {
        protected enum ResultKind { None, UnsatisfiableRequires, FalseRequires, UnprovenEnsures, FalseEnsures }

        protected readonly DirectoryInfo workingDir;
        protected readonly string ccCheckDefaultArgs;
        protected readonly string libPaths;
        protected readonly ITypeDefinition typeToAnalyze;

        public CodeContractsRunner(DirectoryInfo workingDir, string ccCheckDefaultArgs, string libPaths, ITypeDefinition typeToAnalyze)
        {
            this.workingDir = workingDir;
            this.ccCheckDefaultArgs = ccCheckDefaultArgs;
            this.libPaths = libPaths;
            this.typeToAnalyze = typeToAnalyze;
        }

        public QueryResult Execute(FileInfo queryAssembly, Query query)
        {
            var tmpDir = Path.Combine(workingDir.FullName, Guid.NewGuid().ToString());
            Directory.CreateDirectory(tmpDir);

            //string libPaths;

            //if (inputAssembly.Module.TargetRuntimeVersion.StartsWith("v4.0"))
            //    libPaths = Configuration.ExpandVariables(Resources.Netv40);
            //else
            //    libPaths = Configuration.ExpandVariables(Resources.Netv35);

            //var inputAssemblyPath = Path.GetDirectoryName(inputAssembly.FileName);
            //libPaths = string.Format("{0};{1}", libPaths, inputAssemblyPath);

            //var typeName = this.typeToAnalyze.ToString();

            //if (this.typeToAnalyze.IsGeneric)
            //    typeName = string.Format("{0}`{1}", typeName, this.typeToAnalyze.GenericParameterCount);

            var args = new StringBuilder(ccCheckDefaultArgs);
            args.AppendFormat(" -typeNameSelect={0}", typeToAnalyze);
            args.AppendFormat(" -libPaths:\"{0}\"", libPaths);
            args.AppendFormat(" \"{0}\"", queryAssembly.FullName);

            var output = new StringBuilder();

            using (var codeContracts = new Process())
            {
                codeContracts.StartInfo = new ProcessStartInfo
                {
                    FileName = @"C:\Program Files (x86)\Microsoft\Contracts\Bin\cccheck.exe",
                    Arguments = args.ToString(),
                    WorkingDirectory = tmpDir,
                    CreateNoWindow = true,
                    WindowStyle = ProcessWindowStyle.Hidden,
                    RedirectStandardOutput = true,
                    RedirectStandardError = true,
                    UseShellExecute = false
                };

                Logger.Log(LogLevel.Info, "=============== Code Contracts ===============");
                codeContracts.OutputDataReceived += (sender, e) =>
                {
                    output.AppendLine(e.Data);
                    Logger.Log(LogLevel.Debug, e.Data);
                };
                codeContracts.ErrorDataReceived += (sender, e) => { Logger.Log(LogLevel.Fatal, e.Data); };
                codeContracts.Start();
                codeContracts.BeginErrorReadLine();
                codeContracts.BeginOutputReadLine();
                codeContracts.WaitForExit();

                if (codeContracts.ExitCode != 0)
                {
                    throw new Exception("Error executing Code Contracts");
                }
            }

            Directory.Delete(tmpDir, true);

            var codeContractsConclusions = new HashSet<ResultKind>();
            using (var reader = new StringReader(output.ToString()))
            {
                for (var ccMessage = reader.ReadLine(); ccMessage != null; ccMessage = reader.ReadLine())
                {
                    //var currentMethod = ccMessage.Substring(0, ccMessage.IndexOf('['));
                    ////Para el caso de .#ctor
                    //currentMethod = currentMethod.Replace("#", string.Empty);

                    var message = ccMessage.Substring(ccMessage.IndexOf(':') + 1).Trim();
                    codeContractsConclusions.Add(ParseResultKind(message));
                }
            }

            if (codeContractsConclusions.Contains(ResultKind.FalseEnsures))
            {
                return QueryResult.Reachable;
            }

            if (codeContractsConclusions.Contains(ResultKind.UnprovenEnsures))
            {
                return QueryResult.MaybeReachable;
            }

            return QueryResult.Unreachable;
        }

        protected ResultKind ParseResultKind(string message)
        {
            if (message.Contains("requires is false"))
            {
                throw new InvalidProgramException("Requires must be true");
            }

            if (message.Contains("Requires (including invariants) are unsatisfiable"))
            {
                throw new InvalidProgramException("Requires must be satisfiable");
            }

            if (message.Contains("ensures unproven"))
            {
                return ResultKind.UnprovenEnsures;
            }

            if (message.Contains("ensures unreachable"))
            {
                return ResultKind.UnprovenEnsures;
            }

            if (message.Contains("ensures is false"))
            {
                return ResultKind.FalseEnsures;
            }

            if (message.Contains("ensures (always false) may be reachable"))
            {
                return ResultKind.FalseEnsures;
            }

            return ResultKind.None;
        }
    }
}