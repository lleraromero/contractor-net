using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using Contractor.Core;
using Contractor.Core.Model;
using Log;

namespace Analyzer.CodeContracts
{
    public class CodeContractsRunner
    {
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

        public Dictionary<Query, QueryResult> Execute(FileInfo queryAssembly, IReadOnlyCollection<Query> queries)
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

            var args = new StringBuilder(ccCheckDefaultArgs);
            args.AppendFormat(" -typeNameSelect={0}", typeToAnalyze.Name);
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

                codeContracts.OutputDataReceived += (sender, e) => { output.AppendLine(e.Data); };
                codeContracts.ErrorDataReceived += (sender, e) => { output.AppendLine(e.Data); };
                codeContracts.Start();
                codeContracts.BeginErrorReadLine();
                codeContracts.BeginOutputReadLine();
                codeContracts.WaitForExit();

                if (codeContracts.ExitCode != 0)
                {
                    throw new Exception("Error executing Code Contracts");
                }

                Logger.Log(LogLevel.Info, "=============== Code Contracts ===============");
                Logger.Log(LogLevel.Debug, "Args: " + codeContracts.StartInfo.Arguments);
                Logger.Log(LogLevel.Debug, output.ToString());
                Contract.Assert(output.Length > 11, "It seems that Code Contracts didn't analyze any methods");
            }

            Directory.Delete(tmpDir, true);

            var codeContractsConclusions = new Dictionary<Query, HashSet<ResultKind>>();
            using (var reader = new StringReader(output.ToString()))
            {
                for (var ccMessage = reader.ReadLine(); ccMessage != null; ccMessage = reader.ReadLine())
                {
                    var candidateQueries = queries.Where(q => ccMessage.Contains(q.Method.Method.Name.Value));
                    if (!candidateQueries.Any())
                    {
                        continue;
                    }
                    var currentQuery =
                        candidateQueries.First(
                            q => candidateQueries.All(q2 => q2.Method.Method.Name.Value.Length <= q.Method.Method.Name.Value.Length));

                    var message = ccMessage.Substring(ccMessage.IndexOf(':') + 1).Trim();
                    var conclusion = ParseResultKind(message);

                    if (!codeContractsConclusions.ContainsKey(currentQuery))
                    {
                        codeContractsConclusions.Add(currentQuery, new HashSet<ResultKind>());
                    }

                    codeContractsConclusions[currentQuery].Add(conclusion);
                }
            }

            var queryResults = new Dictionary<Query, QueryResult>();
            foreach (var query in queries)
            {
                if (codeContractsConclusions[query].Contains(ResultKind.FalseEnsures))
                {
                    queryResults.Add(query, QueryResult.Reachable);
                }
                else if (codeContractsConclusions[query].Contains(ResultKind.UnprovenEnsures))
                {
                    queryResults.Add(query, QueryResult.MaybeReachable);
                }
                else if (codeContractsConclusions[query].Contains(ResultKind.ValidEnsures))
                {
                    queryResults.Add(query, QueryResult.Unreachable);
                }
                else
                {
                    throw new Exception("Code Contracs didn't provide a conclusion for this query");
                }
            }

            return queryResults;
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

            if (message.Contains("ensures is valid"))
            {
                return ResultKind.ValidEnsures;
            }

            return ResultKind.None;
        }

        protected enum ResultKind
        {
            None,
            UnsatisfiableRequires,
            FalseRequires,
            UnprovenEnsures,
            FalseEnsures,
            ValidEnsures
        }
    }
}