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
#if !DEBUG
            try
            {
#endif
                var program = new Program(options);

                EpaGenerator.Backend backend = EpaGenerator.Backend.Corral;
                if (program.options.backend.Equals("CodeContracts", StringComparison.InvariantCultureIgnoreCase))
                    backend = EpaGenerator.Backend.CodeContracts;

                // epas is a mapping between Typename and the result of the analysis.
                Dictionary<string, TypeAnalysisResult> epas = program.Execute(backend);

                // Save each EPA as an image in the Graph folder
                foreach (var result in epas)
                {
                    var typeName = result.Key.Replace('.', '_');
                    using (var stream = File.Create(string.Format("{0}\\{1}.png", options.graph, typeName)))
                    {
                        (new EpaBinarySerializer()).Serialize(stream, result.Value.EPA);
                    }
                }
#if !DEBUG
            }
            catch (Exception ex)
            {
                System.Console.WriteLine("Error: {0}", ex.Message);
            }
#endif
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

            if (!Directory.Exists(options.graph))
                Directory.CreateDirectory(options.graph);

            if (!Directory.Exists(Configuration.TempPath))
                Directory.CreateDirectory(Configuration.TempPath);
        }

        public Dictionary<string, TypeAnalysisResult> Execute(EpaGenerator.Backend backend)
        {
            var epas = new Dictionary<string, TypeAnalysisResult>();
            using (var generator = new EpaGenerator(backend))
            {
                generator.LoadAssembly(options.input);
                generator.TypeAnalysisStarted += (sender, e) => { System.Console.WriteLine("Starting analysis for type {0}", e.TypeFullName); };
                generator.TypeAnalysisDone += (sender, e) => { System.Console.WriteLine(e.AnalysisResult.ToString()); };

                var cancellationSource = new CancellationTokenSource();
                if (string.IsNullOrEmpty(options.type))
                    epas = generator.GenerateEpas(cancellationSource.Token);
                else
                    epas = new Dictionary<string, TypeAnalysisResult>() { { options.type, generator.GenerateEpa(options.type, cancellationSource.Token) } };

                if (options.generateAssembly)
                {
                    System.Console.WriteLine("Generating strengthened output assembly");
                    generator.GenerateOutputAssembly(options.output);
                }
            }
            return epas;
        }
    }
}
