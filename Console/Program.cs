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
            //var tempPath = ConfigurationManager.AppSettings["WorkingDir"];
            //var graphPath = Path.Combine(tempPath, "Graph");
            //if (!Directory.Exists(graphPath))
            //{
            //    Directory.CreateDirectory(graphPath);
            //}

            //var examplesPath = Path.GetFullPath(Path.Combine(Environment.CurrentDirectory, @"..\..\..\Examples\obj\Debug\Decl\Examples.dll"));

            //args = new[]
            //{
            //    "-i", examplesPath,
            //    "-g", graphPath,
            //    "--tmp", tempPath,
            //    "-t", "Examples.ICSE2011.ListItr",
            //    "-b", "Corral",
            //    "--xml"
            //};
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

            System.Console.WriteLine(options.InputAssembly);
            System.Diagnostics.Debugger.Break();

            try
            {
                var analysisResult = GenerateEpa(options);
               
                var epa = analysisResult.Epa;
                
                SaveEpasAsImages(epa, new DirectoryInfo(options.GraphDirectory));
                
                if (options.XML)
                {
                    SaveEpasAsXML(epa, new DirectoryInfo(options.GraphDirectory));
                }

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

            //OnInitialStateAdded(this, epaBuilder);
            var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
            epaBuilderObservable.TransitionAdded += OnTransitionAdded;

            var analysisResult = generator.GenerateEpa(typeDefinition, epaBuilderObservable).Result;
            return analysisResult;
        }

        private static void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            System.Console.WriteLine("======================================================");
            System.Console.WriteLine("States number:"+ e.EpaBuilder.States.Count);
            System.Console.WriteLine("Transitions number:"+ e.EpaBuilder.Transitions.Count);
            System.Console.WriteLine("New transition:");
            System.Console.WriteLine(e.Transition);
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

        protected static void SaveEpasAsXML(Epa epa, DirectoryInfo outputDir)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputDir != null);

            var typeName = epa.Type.ToString().Replace('.', '_');
            using (var stream = File.Create(string.Format("{0}\\{1}.png", outputDir.FullName, typeName)))
            {
                new EpaXmlSerializer().Serialize(stream, epa);
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