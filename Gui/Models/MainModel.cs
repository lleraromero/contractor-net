using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Cci;
using Analyzer.CodeContracts;
using Analyzer.Corral;
using Contractor.Core;
using Contractor.Core.Model;
using System.Collections.Generic;

namespace Contractor.Gui.Models
{
    internal class MainModel : IMainModel
    {
        protected FileInfo inputFile;
        protected FileInfo contractFile;
        protected IAssembly inputAssembly;

        //**************************************************
        protected Dictionary<string, ErrorInstrumentator.MethodInfo> methodsInfo;

        protected CancellationTokenSource cancellationSource;
        protected CciAssemblyPersister assemblyPersister;
        protected Epa generatedEpa;
        protected const int maxDegreeOfParallelism=4;

        public MainModel()
        {
            assemblyPersister = new CciAssemblyPersister();

            StateAdded += (sender, args) => { };
            TransitionAdded += (sender, args) => { };
        }

        public Epa GeneratedEpa
        {
            get { return generatedEpa; }
        }

        public IAssembly InputAssembly
        {
            get { return inputAssembly; }
        }

        public event EventHandler<StateAddedEventArgs> StateAdded;
        public event EventHandler<TransitionAddedEventArgs> TransitionAdded;

        public void Stop()
        {
            cancellationSource.Cancel();
        }

        public async Task<TypeAnalysisResult> Start(AnalysisEventArgs analysisEventArgs)
        {
            cancellationSource = new CancellationTokenSource();
            var selectedMethods = from m in analysisEventArgs.SelectedMethods select m.ToString();

            var exceptionExtractor = new ExceptionExtractor(inputFile.FullName);
            //process all methods to analyze
            //foreach (var m in analysisEventArgs.SelectedMethods.Select(sm=>sm.Method)){
            //    exceptionExtractor.Process(m);
            //}
            exceptionExtractor.Process(analysisEventArgs.SelectedMethods);
            var allExceptions = exceptionExtractor.GetAllExceptions;
            var exceptionsByMethod = exceptionExtractor.GetExceptionsByMethods;

            ImplementedExceptions.CreateInstance(allExceptions);

            List<string> errorList = new List<string>();
            errorList.Add("Ok");
            if (analysisEventArgs.Exceptions.Equals("All"))
            {
                //ImplementedExceptions.AddAllExceptionsTo(errorList);
                errorList.AddRange(allExceptions);
            }
            else
            {
                errorList.Add("System.Exception");
            }

            var analyzer = GetAnalyzerFactory(analysisEventArgs.TypeToAnalyze, analysisEventArgs.Engine, cancellationSource.Token, errorList, analysisEventArgs.Conditions.Contains("I"));
            
            /*using (StreamWriter writer = new StreamWriter("selectedMethods.txt", true))
            {
                foreach(var m in selectedMethods)
                    writer.Write(m+";");
            }*/ 

            var epaBuilder = new EpaBuilder(analysisEventArgs.TypeToAnalyze);
            OnInitialStateAdded(this, epaBuilder,analysisEventArgs.SelectedMethods);
            var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
            epaBuilderObservable.TransitionAdded += OnTransitionAdded;

            if (analysisEventArgs.Conditions.Equals("EPA") || analysisEventArgs.Conditions.Equals("EPA-I"))
            {
                var epaGenerator = new EpaGenerator(analyzer, -1, false, maxDegreeOfParallelism);
                return await epaGenerator.GenerateEpa(analysisEventArgs.TypeToAnalyze, selectedMethods, epaBuilderObservable);
            }
            else if (analysisEventArgs.Conditions.Equals("EPA-O") || analysisEventArgs.Conditions.Equals("EPA-I/O"))
            {
                errorList = errorList.Select(x => x.Split('.').Last()).ToList();
                var epaGenerator = new EpaOGenerator(analyzer, -1, errorList, false, maxDegreeOfParallelism, exceptionsByMethod);
                return await epaGenerator.GenerateEpa(analysisEventArgs.TypeToAnalyze, selectedMethods, epaBuilderObservable);
            }else{
                throw new NotImplementedException("Unknown abstraction level option");
            }
        }

        public async Task LoadAssembly(FileInfo inputFileInfo)
        {
            inputFile = inputFileInfo;

            Log.MyLogger.LogStartAnalysis(inputFile.FullName);
            //*************************************************
            //cargamos del XML
            var xmlPath = inputFile.DirectoryName + "\\methodExceptions.xml";
            if (File.Exists(xmlPath))
            {
                using (var stream = File.OpenRead(xmlPath))
                {
                    methodsInfo = ErrorInstrumentator.XmlInstrumentationInfoSerializer.Deserialize(stream);
                }
            }
            inputAssembly = await Task.Run(() => assemblyPersister.Load(inputFile.FullName, null));
        }

        public async Task LoadContracts(FileInfo contractFileInfo)
        {
            contractFile = contractFileInfo;
            inputAssembly = await Task.Run(() => assemblyPersister.Load(inputFile.FullName, contractFile.FullName));
        }

        protected void OnInitialStateAdded(object sender, IEpaBuilder epaBuilder, IEnumerable<Contractor.Core.Model.Action> selectedMethods)
        {
            generatedEpa = epaBuilder.Build();
            Core.StateAddedEventArgs stateAddedEventArg;
            if((epaBuilder.Type.Constructors().Except(selectedMethods).Any())){ // if selected constructors
                var constructors=(epaBuilder.Type.Constructors().Intersect(selectedMethods));
                var initialState = new State(new HashSet<Core.Model.Action>(constructors), new HashSet<Core.Model.Action>());
                stateAddedEventArg = new StateAddedEventArgs(epaBuilder.Type, epaBuilder, initialState);
            }
            else
            {
                stateAddedEventArg = new StateAddedEventArgs(epaBuilder.Type, epaBuilder, generatedEpa.Initial);
            }
            StateAdded(sender, stateAddedEventArg);
        }

        protected void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            generatedEpa = e.EpaBuilder.Build();
            TransitionAdded(sender, e);
        }

        protected IAnalyzerFactory GetAnalyzerFactory(ITypeDefinition typeToAnalyze, string engine, CancellationToken cancellationToken,List<string> errorList,bool inputConditionOption)
        {
            var workingDir = new DirectoryInfo(ConfigurationManager.AppSettings["WorkingDir"]);
            if (workingDir.Exists && workingDir.CreationTimeUtc < DateTime.UtcNow.AddDays(-3))
            {
                workingDir.Delete(true);
            }
            workingDir.Create();

            //var queryGenerator = new CciQueryGenerator(errorList.Select(x => x.Split('.').Last()).ToList());

            IAnalyzerFactory analyzerFactory;
            switch (engine)
            {
                case "CodeContracts":
                    var cccheckArgs = CheckCodeContractsExists();
                    if (!inputConditionOption)
                    {
                        analyzerFactory = new CodeContractsAnalyzerFactory(workingDir, cccheckArgs, string.Empty, errorList.Select(x => x.Split('.').Last()).ToList(),
                            inputAssembly as CciAssembly, inputFile.FullName,
                            typeToAnalyze, cancellationToken);
                    }
                    else
                    {
                        analyzerFactory = new CodeContractsWithConditionsAnalyzerFactory(workingDir, cccheckArgs, string.Empty, errorList.Select(x => x.Split('.').Last()).ToList(),
                            inputAssembly as CciAssembly, inputFile.FullName,
                            typeToAnalyze, cancellationToken);
                    }
                    break;
                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    if (!inputConditionOption)
                    {
                        analyzerFactory = new CorralAnalyzerFactory(corralDefaultArgs, workingDir, errorList.Select(x => x.Split('.').Last()).ToList(), inputAssembly as CciAssembly,
                            inputFile.FullName,
                            typeToAnalyze, cancellationToken, errorList, maxDegreeOfParallelism);
                    }
                    else
                    {
                        analyzerFactory = new CorralAnalyzerWithConditionsFactory(corralDefaultArgs, workingDir, errorList.Select(x => x.Split('.').Last()).ToList(), inputAssembly as CciAssembly,
                            inputFile.FullName,
                            typeToAnalyze, cancellationToken, errorList, maxDegreeOfParallelism);
                        cccheckArgs = CheckCodeContractsExists();
                        var csCheckeFac=new CodeContractsWithConditionsAnalyzerFactory(workingDir, cccheckArgs, string.Empty, errorList.Select(x => x.Split('.').Last()).ToList(),
                            inputAssembly as CciAssembly, inputFile.FullName,
                            typeToAnalyze, cancellationToken);
                        var csChecker = csCheckeFac.CreateAnalyzer();
                        var csGenerator = CSGenerator.Instance((Analyzer.CodeContracts.AnalyzerWithCondition)csChecker);
                    }
                    break;
                default:
                    throw new NotSupportedException();
            }

            return analyzerFactory;
        }

        private static string CheckCodeContractsExists()
        {
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
            return cccheckArgs;
        }
        
    }
}