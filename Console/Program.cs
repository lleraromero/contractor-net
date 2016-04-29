using System;
using System.Collections.Generic;
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
            var tempPath = ConfigurationManager.AppSettings["WorkingDir"];
            var graphPath = @"C:\Users\lean\Desktop\EPAs";
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
                "-t", "Examples.Door",
                "-b", "Corral",
                "--ga",
                "-o", @"C:\Users\lean\Desktop\EPAs\strengthenedAssembly.dll",
            };
#endif
            var options = new Options();
            if (!Parser.Default.ParseArgumentsStrict(args, options))
            {
                throw new FormatException("Args parsing error!");
            }

            System.Console.WriteLine(options.InputAssembly);

            var decompiler = new CciAssemblyPersister();
            var inputAssembly = decompiler.Load(options.InputAssembly, null);
            var typeToAnalyze = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));

            var analysisResult = GenerateEpa(inputAssembly, typeToAnalyze, options);

            System.Console.WriteLine(analysisResult.ToString());

            var epa = analysisResult.Epa;

            SaveEpasAsImages(epa, new DirectoryInfo(options.GraphDirectory));

            if (options.XML)
            {
                SaveEpasAsXml(epa, new DirectoryInfo(options.GraphDirectory));
            }

            if (options.GenerateStrengthenedAssembly)
            {
                var strengthenedAssembly = GenerateStrengthenedAssembly(epa, inputAssembly) as CciAssembly;
                decompiler.Save(strengthenedAssembly, options.OutputAssembly);
            }
            
            System.Console.WriteLine("Done!");
            System.Console.ReadLine();
            return 0;
        }

        protected static TypeAnalysisResult GenerateEpa(CciAssembly inputAssembly, ITypeDefinition typeToAnalyze, Options options)
        {
            System.Console.WriteLine("Starting analysis for type {0}", typeToAnalyze);

            var cancellationSource = new CancellationTokenSource();

            var workingDir = CreateOrCleanupWorkingDirectory();

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
                    analyzerFactory = new CodeContractsAnalyzerFactory(workingDir, cccheckArgs, string.Empty, queryGenerator, inputAssembly,
                        options.InputAssembly,
                        typeToAnalyze, cancellationSource.Token);
                    break;

                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    Contract.Assert(corralDefaultArgs != null);
                    analyzerFactory = new CorralAnalyzerFactory(corralDefaultArgs, workingDir, queryGenerator, inputAssembly,
                        options.InputAssembly, typeToAnalyze, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var generator = new EpaGenerator(analyzerFactory, options.Cutter);

            var epaBuilder = new EpaBuilder(typeToAnalyze);

            var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
            epaBuilderObservable.TransitionAdded += OnTransitionAdded;
            TypeAnalysisResult analysisResult;
            if (!options.Methods.Equals("All"))
            {
                var selectedMethods = options.Methods.Split(';');
                analysisResult = generator.GenerateEpa(typeToAnalyze, selectedMethods, epaBuilderObservable).Result;
            }
            else
            {
                analysisResult = generator.GenerateEpa(typeToAnalyze, epaBuilderObservable).Result;
            }

            return analysisResult;
        }

        protected static DirectoryInfo CreateOrCleanupWorkingDirectory()
        {
            var workingDir = new DirectoryInfo(ConfigurationManager.AppSettings["WorkingDir"]);
            if (workingDir.Exists && workingDir.CreationTimeUtc < DateTime.UtcNow.AddDays(-3))
            {
                workingDir.Delete(true);
            }
            workingDir.Create();
            return workingDir;
        }

        protected static void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            System.Console.WriteLine("======================================================");
            System.Console.WriteLine("States number:" + e.EpaBuilder.States.Count);
            System.Console.WriteLine("Transitions number:" + e.EpaBuilder.Transitions.Count);
            System.Console.WriteLine("New transition:" + e.Transition);
        }

        protected static void SaveEpasAsImages(Epa epa, DirectoryInfo outputDir)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputDir != null);

            SaveEpaAs<EpaBinarySerializer>(epa, outputDir, "png");
        }

        protected static void SaveEpasAsXml(Epa epa, DirectoryInfo outputDir)
        {
            Contract.Requires(epa != null);
            Contract.Requires(outputDir.Exists);

            SaveEpaAs<EpaXmlSerializer>(epa, outputDir, "xml");
        }

        protected static void SaveEpaAs<T>(Epa epa, DirectoryInfo outputDir, string fileNameExtension) where T : ISerializer, new()
        {
            var typeName = epa.Type.ToString().Replace('.', '_');
            var safeFileName = GetSafeFilename(string.Format("{0}\\{1}.{2}", outputDir.FullName, typeName, fileNameExtension));
            using (var stream = File.Create(safeFileName))
            {
                new T().Serialize(stream, epa);
            }
        }

        protected static IAssembly GenerateStrengthenedAssembly(Epa epa, CciAssembly assembly)
        {
            Contract.Requires(epa != null);
            Contract.Requires(assembly != null);

            System.Console.WriteLine("Generating strengthened output assembly");
            var instrumenter = new Instrumenter.Instrumenter();
            return instrumenter.InstrumentType(assembly, epa);
        }

        protected static string GetSafeFilename(string filename)
        {
            Contract.Requires(!string.IsNullOrEmpty(filename));
            var safeFileName = RemoveCharsFrom(filename, Path.GetInvalidFileNameChars());
            safeFileName = RemoveCharsFrom(safeFileName, Path.GetInvalidPathChars());
            return safeFileName;
        }

        protected static string RemoveCharsFrom(string fileName, IEnumerable<char> charsToRemove)
        {
            return charsToRemove.Aggregate(fileName, (current, c) => current.Replace(c.ToString(), ""));
        }
    }
}