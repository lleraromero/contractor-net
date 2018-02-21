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
using System.Compiler.Analysis;
using Microsoft.Research.CodeAnalysis;
using System.Collections;

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
                    Logger.Log(LogLevel.Info, "=============== Code Contracts ===============");
                    Logger.Log(LogLevel.Debug, "Args: " + codeContracts.StartInfo.Arguments);
                    Logger.Log(LogLevel.Debug, output.ToString());
                    throw new Exception("Error executing Code Contracts");
                }

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
                    // In order to avoid problems with methods that have a prefix in common we find the longest string that matches
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
                if (codeContractsConclusions[query].Contains(ResultKind.FalseRequires))
                {
                    queryResults.Add(query, QueryResult.Unreachable);
                }
                else if (codeContractsConclusions[query].Contains(ResultKind.UnsatisfiableRequires))
                {
                    queryResults.Add(query, QueryResult.Unreachable);
                }
                else if (codeContractsConclusions[query].Contains(ResultKind.FalseEnsures))
                {
                    queryResults.Add(query, QueryResult.Reachable);
                }
                else if (codeContractsConclusions[query].Contains(ResultKind.UnprovenEnsures))
                {
                    queryResults.Add(query, QueryResult.MaybeReachable);
                }
                else 
                {
                    queryResults.Add(query, QueryResult.Unreachable);
                }
            }

            return queryResults;
        }

        protected ResultKind ParseResultKind(string message)
        {
            if (message.Contains("requires is false"))
            {
                Logger.Log(LogLevel.Warn, "Code Contracts: Requires must be true");
                return ResultKind.FalseRequires;
            }

            if (message.Contains("Requires (including invariants) are unsatisfiable"))
            {
                Logger.Log(LogLevel.Warn, "Code Contracts: Requires (including invariants) are unsatisfiable");
                return ResultKind.UnsatisfiableRequires;
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

            if (message.Contains("Suggested requires: ") || message.Contains("Suggested assume: "))
            {
                return ResultKind.SuggestedRequires;
            }

            if (message.Contains("Missing precondition in an externally visible method"))
            {
                return ResultKind.MissingRequires;
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
            ValidEnsures,
            SuggestedRequires,
            MissingRequires
        }

        internal Dictionary<Query, List<string>> ExecuteWithConditions(FileInfo queryAssembly, IReadOnlyCollection<TransitionQuery> queries)
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

            /*
                var cccheckArgs = Configuration.CheckerArguments;
                cccheckArgs = cccheckArgs.Replace("\"@assemblyName\"", queryAssemblyName);
                cccheckArgs = cccheckArgs.Replace("@fullTypeName", typeName);
                //cccheckArgs += " -infer=requires -suggest=requires";
                //cccheckArgs += " -nologo -nopex -remote  -suggest=!! -premode combined -suggest codefixes -framework:v4.5 -warninglevel full  -maxwarnings 10000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false  -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires";
                cccheckArgs += " -nobox -nologo -nopex -remote  -suggest=!! -premode combined -suggest codefixes -framework:v4.0 -warninglevel full  -maxwarnings 100000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false  -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -missingPublicEnsuresAreErrors  -suggest calleeassumes -suggest assumes -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires -infer methodensures -infer objectinvariants";
                
            */
            //var args = new StringBuilder(ccCheckDefaultArgs);
            var args = new StringBuilder();
            args.AppendFormat("-typeNameSelect={0}", typeToAnalyze.Name);
            //args.AppendFormat(" -libPaths:\"{0}\"", libPaths);
            //args.AppendFormat(" \"{0}\"", queryAssembly.FullName);
            args.AppendFormat(" -libPaths:{0}", libPaths);
            args.AppendFormat(" {0}", queryAssembly.FullName);
            //args.Append(" -nobox -nologo -nopex -suggest=!! -premode combined -suggest codefixes -warninglevel full -framework:v4.0 -maxwarnings 100000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -missingPublicEnsuresAreErrors  -suggest calleeassumes -suggest assumes -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires -infer methodensures -infer objectinvariants");
            args.Append(" -remote -nobox -nologo -nopex -suggest=!! -premode combined -suggest codefixes -framework:v4.0 -warninglevel full -maxwarnings 100000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -missingPublicEnsuresAreErrors -suggest calleeassumes -suggest assumes -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires -infer methodensures -infer objectinvariants");
            /**************************************************************************************************************
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
                    Logger.Log(LogLevel.Info, "=============== Code Contracts ===============");
                    Logger.Log(LogLevel.Debug, "Args: " + codeContracts.StartInfo.Arguments);
                    Logger.Log(LogLevel.Debug, output.ToString());
                    throw new Exception("Error executing Code Contracts");
                }
                
                Contract.Assert(output.Length > 11, "It seems that Code Contracts didn't analyze any methods");
            }
            ******************************************************************************************/
            string outputString;
            using (var output = new StringWriter())
            {
                //var cccheckArgs = Configuration.CheckerArguments;
                //cccheckArgs = cccheckArgs.Replace("\"@assemblyName\"", queryAssemblyName);
                //cccheckArgs = cccheckArgs.Replace("@fullTypeName", typeName);
                //cccheckArgs += " -infer=requires -suggest=requires";
                //cccheckArgs += " -nologo -nopex -remote  -suggest=!! -premode combined -suggest codefixes -framework:v4.5 -warninglevel full  -maxwarnings 10000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false  -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires";
                //cccheckArgs += " -nobox -nologo -nopex -remote  -suggest=!! -premode combined -suggest codefixes -framework:v4.0 -warninglevel full  -maxwarnings 100000 -nonnull -bounds: -arrays -wp=true -bounds:type=subpolyhedra,reduction=simplex,diseq=false  -arrays -adaptive -arithmetic -enum -check assumptions -suggest asserttocontracts -check conditionsvalidity -missingPublicRequiresAreErrors -missingPublicEnsuresAreErrors  -suggest calleeassumes -suggest assumes -suggest requires -infer autopropertiesensures -suggest necessaryensures -suggest objectinvariants -suggest readonlyfields  -infer requires -infer methodensures -infer objectinvariants";
                // If the user didn't stop the analysis, execute cccheck
                //if (!base.token.IsCancellationRequested)
                //{
                    //var cccheckTime = Stopwatch.StartNew();

                    //var args = cccheckArgs.Split(' ');
                    //args[args.Length - 2] = args[args.Length - 2].Replace("@libPaths", libPaths);

                    IOutputFullResultsFactory<System.Compiler.Method, System.Compiler.AssemblyNode> outputFactory =
                        new FullTextWriterOutputFactory<System.Compiler.Method, System.Compiler.AssemblyNode>(output);
                    var argsToCCCheck = args.ToString().Split(' ');
                    var errorCode = Clousot.ClousotMain(argsToCCCheck, CCIMDDecoder.Value, CCIContractDecoder.Value, new Hashtable(), outputFactory);

                    //cccheckTime.Stop();

                    if (errorCode != 0)
                    {
                        Logger.Log(LogLevel.Info, "=============== Code Contracts ===============");
                        //Logger.Log(LogLevel.Debug, "Args: " + codeContracts.StartInfo.Arguments);
                        Logger.Log(LogLevel.Debug, output.ToString());
                        throw new Exception("Error executing Code Contracts");
                    }

                //}

                outputString = output.ToString();
                //Contract.Assert(output.Length > 11, "It seems that Code Contracts didn't analyze any methods");
            }

            Directory.Delete(tmpDir, true);

            //var codeContractsConclusions = new Dictionary<Query, HashSet<ResultKind>>();
            var result = new Dictionary<Query, List<string>>();

            using (var reader = new StringReader(outputString))
            {
                for (var ccMessage = reader.ReadLine(); ccMessage != null; ccMessage = reader.ReadLine())
                {
                    var candidateQueries = queries.Where(q => ccMessage.Contains(q.Method.Method.Name.Value));
                    if (!candidateQueries.Any())
                    {
                        continue;
                    }
                    // In order to avoid problems with methods that have a prefix in common we find the longest string that matches
                    var currentQuery =
                        candidateQueries.First(
                            q => candidateQueries.All(q2 => q2.Method.Method.Name.Value.Length <= q.Method.Method.Name.Value.Length));

                    var message = ccMessage.Substring(ccMessage.IndexOf(':') + 1).Trim();
                    var conclusion = ParseResultKind(message);

                    if (conclusion == ResultKind.SuggestedRequires || conclusion == ResultKind.MissingRequires)
                    {
                        if (!result.ContainsKey(currentQuery))
                            result.Add(currentQuery, new List<string>());
                        var condition = CSGenerator.parseCondition(message);
                        result[currentQuery].Add(condition);
                    }
                }
            }

            return result;

            }
        }
}