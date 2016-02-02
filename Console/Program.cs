using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using Analysis.Cci;
using Analyzer.Corral;
using CommandLine;
using Contractor.Core;
using Contractor.Core.Model;

namespace Contractor.Console
{
    internal class Program
    {
        public static int Main(string[] args)
        {
#if DEBUG
            var tempPath = ConfigurationManager.AppSettings["WorkingDir"];
            var graphPath = Path.Combine(tempPath, "Graph");
            if (!Directory.Exists(graphPath))
            {
                Directory.CreateDirectory(graphPath);
            }

            var examplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));

            args = new[]
            {
                "-i", examplesPath,
                "-g", graphPath,
                "--tmp", tempPath,
                "-t", "Examples.Linear"
            };
#endif
            var options = new Options();
            if (!Parser.Default.ParseArgumentsStrict(args, options))
            {
                System.Console.WriteLine("Args parsing error!");
#if DEBUG
                System.Console.ReadKey();
#endif
                return -1;
            }

            try
            {
                var analysisResult = GenerateEpa(options);
                var epa = analysisResult.Epa;

                SaveEpasAsImages(epa, new DirectoryInfo(options.GraphDirectory));

                if (options.GenerateStrengthenedAssembly)
                {
                    GenerateStrengthenedAssembly(epa, new FileInfo(options.OutputAssembly));
                }
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }

            System.Console.WriteLine("Done!");
#if DEBUG
            System.Console.WriteLine("Press any key to continue");
            System.Console.ReadKey();
#endif
            return 0;
        }

        protected static TypeAnalysisResult GenerateEpa(Options options)
        {
            Contract.Requires(!string.IsNullOrEmpty(options.TypeToAnalyze));
            Contract.Requires(!string.IsNullOrEmpty(options.InputAssembly) && File.Exists(options.InputAssembly));

            System.Console.WriteLine("Starting analysis for type {0}", options.TypeToAnalyze);

            var decompiler = new CciDecompiler();
            var inputAssembly = decompiler.Decompile(options.InputAssembly, null);
            var typeToAnalyze = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
            var cancellationSource = new CancellationTokenSource();

            IAnalyzer analyzer = null;
            switch (options.Backend)
            {
                case "CodeContracts":
                    throw new NotImplementedException();
                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    var workingDir = new DirectoryInfo(ConfigurationManager.AppSettings["WorkingDir"]);
                    workingDir.Create();

                    analyzer = new CorralAnalyzer(corralDefaultArgs, workingDir, decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly,
                        options.InputAssembly, typeToAnalyze, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var generator = new EpaGenerator(analyzer);

            var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
            var epaBuilder = new EpaBuilder(typeDefinition);
            var analysisResult = generator.GenerateEpa(typeDefinition, epaBuilder).Result;
            return analysisResult;
        }

        protected static void SaveEpasAsImages(Epa epa, DirectoryInfo outputDir)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputDir != null);

            var typeName = epa.Type.ToString().Replace('.', '_');
            using (var stream = File.Create(string.Format("{0}\\{1}.png", outputDir.FullName, typeName)))
            {
                new EpaBinarySerializer().Serialize(stream, epa);
            }
        }

        protected static void GenerateStrengthenedAssembly(Epa epa, FileInfo outputFile)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputFile != null);

            System.Console.WriteLine("Generating strengthened output assembly");
            //new Instrumenter().GenerateOutputAssembly(options.output, analysisResult.EPA);
        }
    }
}