using System;
using System.Collections.Generic;
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
using Configuration = Contractor.Core.Configuration;

namespace Contractor.Console
{
    internal class Program
    {
        public static int Main(string[] args)
        {
            Configuration.Initialize();

            var options = new Options();
#if DEBUG
            var graphPath = Path.Combine(Configuration.TempPath, "Graph");
            if (!Directory.Exists(graphPath))
            {
                Directory.CreateDirectory(graphPath);
            }

            var examplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));

            args = new[]
            {
                "-i", examplesPath,
                "-g", graphPath,
                "--tmp", Configuration.TempPath,
                "-t", "Examples.Linear"
            };
#endif
            if (!Parser.Default.ParseArguments(args, options))
            {
                // TODO: imprimir errores
                System.Console.ReadLine();
                return -1;
            }

            try
            {
                // epas is a mapping between Typename and the result of the analysis.
                var epas = Execute(options);

                // Save each EPA as an image in the Graph folder
                foreach (var result in epas)
                {
                    var typeName = result.Key.Replace('.', '_');
                    using (var stream = File.Create(string.Format("{0}\\{1}.png", options.GraphDirectory, typeName)))
                    {
                        new EpaBinarySerializer().Serialize(stream, result.Value.Epa);
                    }
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

        private static Dictionary<string, TypeAnalysisResult> Execute(Options options)
        {
            Contract.Assert(!string.IsNullOrEmpty(options.TypeToAnalyze));
            Contract.Assert(!string.IsNullOrEmpty(options.InputAssembly) && File.Exists(options.InputAssembly));

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
                    analyzer = new CorralAnalyzer(corralDefaultArgs, decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly,
                        options.InputAssembly, typeToAnalyze, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var generator = new EpaGenerator(analyzer);

            var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
            var epaBuilder = new EpaBuilder(typeDefinition);
            var analysisResult = generator.GenerateEpa(typeDefinition, epaBuilder).Result;

            System.Console.WriteLine(analysisResult.ToString());
            var epas = new Dictionary<string, TypeAnalysisResult> {{options.TypeToAnalyze, analysisResult}};

            if (options.GenerateStrengthenedAssembly)
            {
                System.Console.WriteLine("Generating strengthened output assembly");
                //new Instrumenter().GenerateOutputAssembly(options.output, analysisResult.EPA);
            }

            return epas;
        }
    }
}