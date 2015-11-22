using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Contractor.Core;

namespace Analyzer.Corral
{
    internal class CorralRunnerParallel : ICorralRunner
    {
        protected CancellationToken token;
        protected IEnumerable<Query> queries;
        protected FileInfo boogieFilePath;

        public CorralRunnerParallel(CancellationToken token, IEnumerable<Query> queries, FileInfo boogieFilePath)
        {
            Contract.Requires(token != null);
            Contract.Requires(queries != null);
            Contract.Requires(boogieFilePath != null && boogieFilePath.Exists);

            this.token = token;
            this.queries = queries;
            this.boogieFilePath = boogieFilePath;
        }

        public IEnumerable<Query> Run()
        {
            var result = new ConcurrentBag<Query>();

            Parallel.ForEach(queries, query =>
            {
                // Check if the user stopped the analysis
                token.ThrowIfCancellationRequested();

                var tmpDir = Path.Combine(Configuration.TempPath, Guid.NewGuid().ToString());
                Directory.CreateDirectory(tmpDir);

                var args = string.Format("{0} /main:{1} {2}", boogieFilePath.FullName, BctTranslator.CreateUniqueMethodName(query),
                    Configuration.CorralArguments);
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

                    corral.OutputDataReceived += (sender, e) => { output.AppendLine(e.Data); };
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

                var queryResult = ParseResultKind(output.ToString(), query);
                result.Add(queryResult);

                Directory.Delete(tmpDir, true);
            });

            return result;
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