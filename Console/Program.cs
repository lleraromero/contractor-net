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
using System.Collections.Generic;

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
            //System.Diagnostics.Debugger.Break();
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
                Dictionary<string, ErrorInstrumentator.MethodInfo> methodsInfo=null;
                //var xmlPath = options.InputAssembly.Substring(0, options.InputAssembly.LastIndexOf('\\'));
                //xmlPath = xmlPath.Substring(0, xmlPath.LastIndexOf('\\'));
                //xmlPath = xmlPath.Substring(0, xmlPath.LastIndexOf('\\'));
                //xmlPath = xmlPath.Substring(0, xmlPath.LastIndexOf('\\')) + "\\methodExceptions.xml";
                //using (var stream = File.OpenRead(xmlPath))
                //{
                //    methodsInfo = ErrorInstrumentator.XmlInstrumentationInfoSerializer.Deserialize(stream);
                //}
                
                var analysisResult = GenerateEpa(options,methodsInfo);

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
                System.Console.WriteLine(ex.StackTrace);
                System.Console.WriteLine(ex.TargetSite);
                System.Console.WriteLine(ex.Source);
                System.Console.WriteLine(ex.InnerException);
            }

            System.Console.WriteLine("Done!");
#if DEBUG
            //System.Console.WriteLine("Press any key to continue");
            //System.Console.ReadKey();
#endif
            return 0;
        }

        protected static TypeAnalysisResult GenerateEpa(Options options, Dictionary<string, ErrorInstrumentator.MethodInfo> methodsInfo)
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

            List<string> errorList = new List<string>();
            errorList.Add("Ok");
            if (options.ErrorList.Equals("All"))
            {
                errorList.Add("Exception");
                errorList.Add("OverflowException");
                errorList.Add("IndexOutOfRangeException");
                errorList.Add("NullReferenceException");
                errorList.Add("IllegalStateException");
                errorList.Add("ConcurrentModificationException");
                errorList.Add("NoSuchElementException");
            }
            else
            {
                errorList.AddRange(options.ErrorList.Split(';'));
            }

            var queryGenerator = new CciQueryGenerator(errorList);

            IAnalyzer analyzer;
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
                    analyzer = new CodeContractsAnalyzer(workingDir, cccheckArgs, string.Empty, queryGenerator, inputAssembly as CciAssembly, options.InputAssembly,
                        typeToAnalyze, cancellationSource.Token);
                    break;
                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    Contract.Assert(corralDefaultArgs != null);
                    analyzer = new CorralAnalyzer(corralDefaultArgs, workingDir, queryGenerator, inputAssembly as CciAssembly,
                        options.InputAssembly, typeToAnalyze, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }
            var oc = options.OutputConditions.Split(',');
            if (options.OutputConditions.Equals("none") || !oc.Contains("exitCode"))
            {
                var generator = new EpaGenerator(analyzer, options.Cutter);

                var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
                var epaBuilder = new EpaBuilder(typeDefinition);

                //OnInitialStateAdded(this, epaBuilder);
                var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
                epaBuilderObservable.TransitionAdded += OnTransitionAdded;
                TypeAnalysisResult analysisResult;
                if (!options.Methods.Equals("All"))
                {
                    var selectedMethods = options.Methods.Split(';');
                    analysisResult = generator.GenerateEpa(typeDefinition, selectedMethods, epaBuilderObservable).Result;
                }
                else
                {
                    analysisResult = generator.GenerateEpa(typeDefinition, epaBuilderObservable).Result;
                }
                return analysisResult;
            }
            else
            {
                //if (oc.Contains("exitCode"))
                //{
                
                var generator = new EpaOGenerator(analyzer, options.Cutter, errorList);

                    var typeDefinition = inputAssembly.Types().First(t => t.Name.Equals(options.TypeToAnalyze));
                    var epaBuilder = new EpaBuilder(typeDefinition);

                    //OnInitialStateAdded(this, epaBuilder);
                    var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
                    epaBuilderObservable.TransitionAdded += OnTransitionAdded;
                    TypeAnalysisResult analysisResult;
                    if (!options.Methods.Equals("All"))
                    {
                        var selectedMethods = options.Methods.Split(';');
                        analysisResult = generator.GenerateEpa(typeDefinition, selectedMethods, epaBuilderObservable, methodsInfo).Result;
                    }
                    else
                    {
                        analysisResult = generator.GenerateEpa(typeDefinition, epaBuilderObservable, methodsInfo).Result;
                    }
                    return analysisResult;
                //}
            }

            
        }

        private static void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            System.Console.WriteLine("======================================================");
            System.Console.WriteLine("States number:" + e.EpaBuilder.States.Count);
            System.Console.WriteLine("Transitions number:" + e.EpaBuilder.Transitions.Count);
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