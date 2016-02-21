using System;
using System.Diagnostics;
using System.IO;
using NLog;

namespace Experiments
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var tempPath = @"C:\Users\lean\AppData\Local\Temp\Contractor";
            var graphPath = @"D:\tesis-experiments\EPAs";
            if (!Directory.Exists(graphPath))
            {
                Directory.CreateDirectory(graphPath);
            }

            var examplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));
            var examples = new[]
            {
                "Examples.Linear",
                "Examples.VendingMachine",
                "Examples.FiniteStack",
                "Examples.GenericStackSet`1",
                "Examples.Door",
                "Examples.ATM",

                "Examples.ICSE2011.LinkedList",
                "Examples.ICSE2011.ListItr",
                "Examples.ICSE2011.PipedOutputStream"
            };

            var backend = new[] { "Corral", "CodeContracts" };

            Write("======== Starting Experiments ========");
            var stopwatch = Stopwatch.StartNew();
            var finishedSuccessfully = true;
            for (var i = 0; i < backend.Length; i++)
            {
                Write(string.Format("******** Using {0} ********", backend[i]));
                Directory.CreateDirectory(Path.Combine(graphPath, backend[i]));
                for (var j = 0; j < examples.Length; j++)
                {
                    args = new[]
                    {
                        "-i", examplesPath,
                        "-g", Path.Combine(graphPath, backend[i]),
                        "--tmp", tempPath,
                        "-t", examples[j],
                        "-b", backend[i]
                    };

                    using (var consoleFrontend = new Process())
                    {
                        consoleFrontend.StartInfo = new ProcessStartInfo
                        {
                            FileName = Path.Combine(Environment.CurrentDirectory, @"..\..\..\Console\bin\Debug\Contractor.Console.exe"),
                            Arguments = string.Join(" ", args),
                            WorkingDirectory = Path.Combine(Environment.CurrentDirectory, @"..\..\..\Console\bin\Debug\"),
                            CreateNoWindow = false,
                            WindowStyle = ProcessWindowStyle.Normal,
                            RedirectStandardOutput = true,
                            RedirectStandardError = true,
                            UseShellExecute = false
                        };

                        consoleFrontend.OutputDataReceived += (sender, e) => { Write(e.Data); };
                        consoleFrontend.ErrorDataReceived += (sender, e) => { Write(e.Data); };
                        consoleFrontend.Start();
                        consoleFrontend.BeginErrorReadLine();
                        consoleFrontend.BeginOutputReadLine();
                        consoleFrontend.WaitForExit();

                        if (consoleFrontend.ExitCode != 0)
                        {
                            finishedSuccessfully = false;
                            Write(string.Format("Error processing {0} with {1}", examples[j], backend[i]));
                        }
                    }
                }
            }

            stopwatch.Stop();
            Write(finishedSuccessfully ? "All the experiments finished OK! :)" : "There were some errors :(");
            Write("Running time: " + stopwatch.Elapsed);
            Console.ReadLine();
        }

        public static void Write(string msg)
        {
            LogManager.GetCurrentClassLogger().Info(msg);
            Console.WriteLine(msg);
        }
    }
}