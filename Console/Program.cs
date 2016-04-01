using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using Analysis.Cci;
using Analyzer.CodeContracts;
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
            //var graphPath = @"D:\tesis-experiments\EPAs";
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
            //    "-t", "Examples.VendingMachine",
            //    "-b", "Corral"
            //};
#endif
            var options = new Options();
            if (!Parser.Default.ParseArgumentsStrict(args, options))
            {
                System.Console.WriteLine("Args parsing error!");
#if DEBUG
                //System.Console.ReadKey();
#endif
                return -1;
            }

            System.Console.WriteLine(options.InputAssembly);

            try
            {
                var analysisResult = GenerateEpa(options);

                System.Console.WriteLine(analysisResult.ToString());

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
                return -1;
            }

            System.Console.WriteLine("Done!");
            //System.Console.ReadLine();
            return 0;
        }

        protected static TypeAnalysisResult GenerateEpa(Options options)
        {
            Contract.Requires(!string.IsNullOrEmpty(options.TypeToAnalyze));
            Contract.Requires(!string.IsNullOrEmpty(options.InputAssembly) && File.Exists(options.InputAssembly));

            System.Console.WriteLine("Starting analysis for type {0}", options.TypeToAnalyze);

            var decompiler = new CciAssemblyPersister();
            var inputAssembly = decompiler.Load(options.InputAssembly, null);
            var typeToAnalyze = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
            var cancellationSource = new CancellationTokenSource();

            var workingDir = new DirectoryInfo(ConfigurationManager.AppSettings["WorkingDir"]);
            workingDir.Create();

            var queryGenerator = new CciQueryGenerator();

            IAnalyzerFactory analyzerFactory;
            switch (options.Backend)
            {
                case "CodeContracts":
                    var codeContracts = Environment.GetEnvironmentVariable("CodeContractsInstallDir");
                    if (string.IsNullOrEmpty(codeContracts))
                    {
                        var msg = new StringBuilder();
                        msg.AppendLine("The environment variable %CodeContractsInstallDir% does not exist.");
                        msg.AppendLine("Please make sure that Code Contracts is installed correctly.");
                        msg.AppendLine("This might be because the system was not restarted after Code Contracts installation.");

                        throw new DirectoryNotFoundException(msg.ToString());
                    }
                    var cccheckArgs = ConfigurationManager.AppSettings["CccheckArgs"];
                    Contract.Assert(cccheckArgs != null);
                    var cccheck = new FileInfo(ConfigurationManager.AppSettings["CccheckFullName"]);
                    Contract.Assert(cccheck.Exists);
                    analyzerFactory = new CodeContractsAnalyzerFactory(workingDir, cccheckArgs, string.Empty, queryGenerator, inputAssembly as CciAssembly, options.InputAssembly,
                        typeToAnalyze, cancellationSource.Token);
                    break;

                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    Contract.Assert(corralDefaultArgs != null);
                    analyzerFactory = new CorralAnalyzerFactory(corralDefaultArgs, workingDir, queryGenerator, inputAssembly as CciAssembly,
                        options.InputAssembly, typeToAnalyze, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var generator = new EpaGenerator(analyzerFactory,options.Cutter);

            var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
            var epaBuilder = new EpaBuilder(typeDefinition);

            ////OnInitialStateAdded(this, epaBuilder);
            //var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
            //epaBuilderObservable.TransitionAdded += OnTransitionAdded;
            //TypeAnalysisResult analysisResult;
            //if (!options.Methods.Equals("All"))
            //{
            //    var selectedMethods = options.Methods.Split(';');
            //    analysisResult = generator.GenerateEpa(typeDefinition, selectedMethods,epaBuilderObservable).Result;
            //}
            //else
            //{
            //    analysisResult = generator.GenerateEpa(typeDefinition, epaBuilderObservable).Result;
            //}

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

        protected static void SaveEpasAsXML(Epa epa, DirectoryInfo outputDir)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputDir.Exists);

            var typeName = epa.Type.ToString().Replace('.', '_');
            using (var stream = File.Create(string.Format("{0}\\{1}.xml", outputDir.FullName, typeName)))
            {
                new EpaXmlSerializer().Serialize(stream, epa);
            }
        }

        protected static void GenerateStrengthenedAssembly(Epa epa, FileInfo outputFile)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputFile.Exists);

            throw new NotSupportedException();
            // TODO (lleraromero): Volver a habilitar el instrumenter
            //System.Console.WriteLine("Generating strengthened output assembly");
            //new Instrumenter().GenerateOutputAssembly(options.output, analysisResult.EPA);
        }
    }
}