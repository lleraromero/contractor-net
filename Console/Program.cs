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
using System.Diagnostics;

namespace Contractor.Console
{
    public class Program
    {
        public static int Main(string[] args)
        {
#if DEBUG
            //var tempPath = ConfigurationManager.AppSettings["WorkingDir"];
            //var graphPath = @"C:\Users\lean\Desktop\EPAs";
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
            //    "-t", "Examples.Door",
            //    "-b", "Corral",
            //    "--ga",
            //    "-o", @"C:\Users\lean\Desktop\EPAs\strengthenedAssembly.dll",
            //};
#endif
            var options = new Options();
            System.Action myact = () =>
            {
            };
            if (!Parser.Default.ParseArgumentsStrict(args, options, myact))
            {
                System.Console.WriteLine("Args parsing error!");
                return -1;
            }

            System.Console.WriteLine(options.InputAssembly);

            var decompiler = new CciAssemblyPersister();
            var inputAssembly = decompiler.Load(options.InputAssembly, null);
            var typeToAnalyze = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));

            if (options.PrintMethodList)
            {
                var methodList= new StringBuilder();
                methodList.Append(@"""");

                foreach (var constructor in typeToAnalyze.Constructors()){
                    methodList.Append(constructor.ToString().Replace(" ", ""));
                    methodList.Append(@",");
                }

                foreach (var action in typeToAnalyze.Actions())
                {
                    methodList.Append(action.ToString().Replace(" ", ""));
                    methodList.Append(@",");
                }
                methodList.Remove(methodList.Length-1,1);
                methodList.Append(@"""");
                System.Console.WriteLine(methodList.ToString());
                System.Console.WriteLine("#constructors: "+typeToAnalyze.Constructors().Count());
                System.Console.WriteLine("#methods: " + typeToAnalyze.Actions().Count());
                System.Console.WriteLine("#total: " + (typeToAnalyze.Constructors().Count() + typeToAnalyze.Actions().Count()));
                System.Console.WriteLine("#maybe async: " + (typeToAnalyze.Actions().Where(a => a.ToString().Contains("Async") || a.ToString().Contains("Begin") || a.ToString().Contains("End"))).Count());
                return 0;
            }

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
            //System.Console.ReadLine();
            return 0;
        }

        protected static TypeAnalysisResult GenerateEpa(CciAssembly inputAssembly, ITypeDefinition typeToAnalyze, Options options)
        {
            System.Console.WriteLine("Starting analysis for type {0}", typeToAnalyze);

            Log.MyLogger.LogStartAnalysis(options.TypeToAnalyze);
            var cancellationSource = new CancellationTokenSource();

            var workingDir = CreateOrCleanupWorkingDirectory();

            List<string> errorList = new List<string>();
            errorList.Add("Ok");
            if (options.ErrorList.Equals("All"))
            {
                ImplementedExceptions.AddAllExceptionsTo(errorList);
                //errorList.Add("Exception");
                errorList.Add("System.Exception");
            }
            else
            {
                errorList.AddRange(options.ErrorList.Split(';'));
            }
            //var queryGenerator = new CciQueryGenerator(errorList.Select(x => x.Split('.').Last()).ToList());
            //var queryGenerator = new CciQueryGenerator(errorList);

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
                    analyzerFactory = new CodeContractsAnalyzerFactory(workingDir, cccheckArgs, string.Empty, errorList.Select(x => x.Split('.').Last()).ToList(), inputAssembly,
                        options.InputAssembly,
                        typeToAnalyze, cancellationSource.Token);
                    break;

                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    Contract.Assert(corralDefaultArgs != null);
                    analyzerFactory = new CorralAnalyzerFactory(corralDefaultArgs, workingDir, errorList.Select(x => x.Split('.').Last()).ToList(), inputAssembly,
                        options.InputAssembly, typeToAnalyze, cancellationSource.Token, errorList, options.MaxDegreeOfParallelism);
                    break;
                default:
                    throw new NotSupportedException();
            }

            if (options.Query!=null)
            {
                var analysisTimer = Stopwatch.StartNew();
                var analyzer = analyzerFactory.CreateAnalyzer();
                var helper = new QueryHelper(analyzer, typeToAnalyze, errorList);
                var selectedMethods = options.Methods.Split(';');
                var transition = helper.computeQuery(options.Query, selectedMethods);
                var transitions = new List<Transition>();
                if (transition != null)
                {
                    transitions.Add(transition);
                }
                var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
                var epa = new Epa(typeDefinition, transitions);
                
                analysisTimer.Stop();
                return new TypeAnalysisResult(epa, analysisTimer.Elapsed, analyzerFactory.GeneratedQueriesCount, analyzerFactory.UnprovenQueriesCount);
            }

            var oc = options.OutputConditions.Split(',');
            if (options.OutputConditions.Equals("none") || !oc.Contains("exitCode"))
            {
                var generator = new EpaGenerator(analyzerFactory, options.Cutter,options.Dependencies,options.MaxDegreeOfParallelism);

                var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
                
                TypeAnalysisResult analysisResult;
                if (!options.Methods.Equals("All"))
                {
                    string[] selectedMethods = options.Methods.Split(';');
                    IEnumerable<string> sel = new List<string>(selectedMethods);
                    sel = sel.Select(a => a.Replace(" ", ""));
                    var selectedActions = typeDefinition.Constructors().Where(c => sel.Contains(c.ToString().Replace(" ", "")));
                    var epaBuilder = new EpaBuilder(typeDefinition, selectedActions);

                    //OnInitialStateAdded(this, epaBuilder);
                    var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
                    epaBuilderObservable.TransitionAdded += OnTransitionAdded;

                    analysisResult = generator.GenerateEpa(typeDefinition, selectedMethods, epaBuilderObservable).Result;
                }
                else
                {
                    var epaBuilder = new EpaBuilder(typeDefinition);

                    //OnInitialStateAdded(this, epaBuilder);
                    var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
                    epaBuilderObservable.TransitionAdded += OnTransitionAdded;
                    analysisResult = generator.GenerateEpa(typeDefinition, epaBuilderObservable).Result;
                }
                return analysisResult;
            }
            else
            {
                errorList = errorList.Select(x => x.Split('.').Last()).ToList();
                var generator = new EpaOGenerator(analyzerFactory, options.Cutter, errorList, options.Dependencies, options.MaxDegreeOfParallelism);

                    var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
                    
                    TypeAnalysisResult analysisResult;
                    if (!options.Methods.Equals("All"))
                    {
                        string[] selectedMethods = options.Methods.Split(';');
                        IEnumerable<string> sel = new List<string>(selectedMethods);
                        sel = sel.Select(a => a.Replace(" ", ""));
                        var selectedActions = typeDefinition.Constructors().Where(c => sel.Contains(c.ToString().Replace(" ", "")));
                        var epaBuilder = new EpaBuilder(typeDefinition, selectedActions);

                        //OnInitialStateAdded(this, epaBuilder);
                        var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
                        epaBuilderObservable.TransitionAdded += OnTransitionAdded;
                        analysisResult = generator.GenerateEpa(typeDefinition, selectedMethods, epaBuilderObservable).Result;
                    }
                    else
                    {
                        var epaBuilder = new EpaBuilder(typeDefinition);

                        //OnInitialStateAdded(this, epaBuilder);
                        var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
                        epaBuilderObservable.TransitionAdded += OnTransitionAdded;
                        analysisResult = generator.GenerateEpa(typeDefinition, epaBuilderObservable).Result;
                    }
                    return analysisResult;
                //}
            }

            
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

            var typeName = epa.Type.ToString().Replace('.', '_');
            typeName = typeName.Replace('<', '_');
            typeName = typeName.Replace('>', '_');
            using (var stream = File.Create(string.Format("{0}\\{1}.png", outputDir.FullName, typeName)))
            {
                new EpaBinarySerializer().Serialize(stream, epa);
            }
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