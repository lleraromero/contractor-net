using Analysis.Cci;
using Analyzer.Corral;
using Contractor.Core;
using System;
using System.Collections.Generic;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;

namespace Contractor.Console
{
    class Program
    {
        private Options options;

        public static int Main(string[] args)
        {
            Configuration.Initialize();
#if DEBUG
            var GraphPath = Path.Combine(Configuration.TempPath, "Graph");
            if (!Directory.Exists(GraphPath))
                Directory.CreateDirectory(GraphPath);

            var ExamplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));

            args = new string[]
            {
                "-i", ExamplesPath,
                "-g", GraphPath,
                "-tmp", Configuration.TempPath,
                "-il=true",
                "-t", "Examples.Linear",
                "-b", "Corral"
            };
#endif
            var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();
            var options = new Options();

            System.Console.WriteLine();
            System.Console.WriteLine("Contractor.NET Version {0}", assemblyName.Version);
            System.Console.WriteLine("Copyright (C) LaFHIS - UBA. All rights reserved.");
            System.Console.WriteLine();

            args = args ?? new string[] { };
            options.Parse(args);

            if (!args.Any() || options.HelpRequested)
            {
                System.Console.WriteLine("usage: <general-option>*");
                System.Console.WriteLine();
                System.Console.WriteLine("where <general-option> is one of");
                options.PrintOptions(string.Empty);
                return -1;
            }
            else if (options.HasErrors)
            {
                options.PrintErrorsAndExit(System.Console.Error);
            }

            try
            {
                var program = new Program(options);

                // epas is a mapping between Typename and the result of the analysis.
                Dictionary<string, TypeAnalysisResult> epas = program.Execute(program.options.backend);

                // Save each EPA as an image in the Graph folder
                foreach (var result in epas)
                {
                    var typeName = result.Key.Replace('.', '_');
                    using (var stream = File.Create(string.Format("{0}\\{1}.png", options.graph, typeName)))
                    {
                        (new EpaBinarySerializer()).Serialize(stream, result.Value.EPA);
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

        public Program(Options options)
        {
            Contract.Requires(options != null);

            this.options = options;

            var wrkdir = Directory.GetCurrentDirectory();

            if (string.IsNullOrEmpty(options.output))
                options.output = Path.Combine(wrkdir, "Output");

            if (string.IsNullOrEmpty(options.graph))
                options.graph = Path.Combine(wrkdir, "Graph");

            if (!string.IsNullOrEmpty(options.temp))
                Configuration.TempPath = options.temp;

            if (!string.IsNullOrEmpty(options.cccheck))
                Configuration.CheckerFileName = options.cccheck;

            if (!string.IsNullOrEmpty(options.cccheckArgs))
                Configuration.CheckerArguments = options.cccheckArgs;

            Configuration.InlineMethodsBody = options.inline;
            Directory.CreateDirectory(options.graph);
            Directory.CreateDirectory(Configuration.TempPath);
        }

        public Dictionary<string, TypeAnalysisResult> Execute(string backend)
        {
            Contract.Assert(!string.IsNullOrEmpty(options.type));
            Contract.Assert(!string.IsNullOrEmpty(options.input) && File.Exists(options.input));

            System.Console.WriteLine("Starting analysis for type {0}", options.type);

            var decompiler = new CciDecompiler();
            var inputAssembly = decompiler.Decompile(options.input, null);
            string typeToAnalyze = inputAssembly.Types().First(t => t.Equals(options.type));
            var cancellationSource = new CancellationTokenSource();

            IAnalyzer analyzer = null;
            switch (backend)
            {
                case "CodeContracts":
                    throw new NotImplementedException();
                case "Corral":
                    analyzer = new CorralAnalyzer(decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly, options.input, typeToAnalyze, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var generator = new EpaGenerator(inputAssembly, analyzer);

            //if (string.IsNullOrEmpty(options.type))
            //    epas = generator.GenerateEpas(cancellationSource.Token);
            //else

            var analysisResult = generator.GenerateEpa(options.type);

            System.Console.WriteLine(analysisResult.ToString());
            var epas = new Dictionary<string, TypeAnalysisResult>() { { options.type, analysisResult } };

            if (options.generateAssembly)
            {
                System.Console.WriteLine("Generating strengthened output assembly");
                //new Instrumenter().GenerateOutputAssembly(options.output, analysisResult.EPA);
            }

            return epas;
        }
    }
}
