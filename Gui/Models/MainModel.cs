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
            List<string> errorList = new List<string>();
            errorList.Add("Ok");
            errorList.Add("OverflowException");
            errorList.Add("IndexOutOfRangeException");
            errorList.Add("NullReferenceException");
            errorList.Add("IllegalStateException");
            errorList.Add("ConcurrentModificationException");
            errorList.Add("NoSuchElementException");
            errorList.Add("Exception");
            var analyzer = GetAnalyzer(analysisEventArgs.TypeToAnalyze, analysisEventArgs.Engine, cancellationSource.Token,errorList);

            var selectedMethods = from m in analysisEventArgs.SelectedMethods select m.ToString();

            var epaBuilder = new EpaBuilder(analysisEventArgs.TypeToAnalyze);
            OnInitialStateAdded(this, epaBuilder);
            var epaBuilderObservable = new ObservableEpaBuilder(epaBuilder);
            epaBuilderObservable.TransitionAdded += OnTransitionAdded;
            
            if(analysisEventArgs.Conditions.Equals("EPA")){
                var epaGenerator = new EpaGenerator(analyzer, -1);
                return await epaGenerator.GenerateEpa(analysisEventArgs.TypeToAnalyze, selectedMethods, epaBuilderObservable);
            }
            else if (analysisEventArgs.Conditions.Equals("EPA-O"))
            {
                var epaGenerator = new EpaOGenerator(analyzer, -1,errorList);
                return await epaGenerator.GenerateEpa(analysisEventArgs.TypeToAnalyze, selectedMethods, epaBuilderObservable,methodsInfo);
            }else{
                throw new NotImplementedException();
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

        protected void OnInitialStateAdded(object sender, IEpaBuilder epaBuilder)
        {
            generatedEpa = epaBuilder.Build();
            var stateAddedEventArg = new StateAddedEventArgs(epaBuilder.Type, epaBuilder, generatedEpa.Initial);
            StateAdded(sender, stateAddedEventArg);
        }

        protected void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            generatedEpa = e.EpaBuilder.Build();
            TransitionAdded(sender, e);
        }

        protected IAnalyzer GetAnalyzer(ITypeDefinition typeToAnalyze, string engine, CancellationToken cancellationToken, List<string> errorList)
        {
            var workingDir = new DirectoryInfo(ConfigurationManager.AppSettings["WorkingDir"]);
            workingDir.Create();

            var queryGenerator = new CciQueryGenerator(errorList);

            IAnalyzer analyzer;
            switch (engine)
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
                    analyzer = new CodeContractsAnalyzer(workingDir, cccheckArgs, string.Empty, queryGenerator, inputAssembly as CciAssembly, inputFile.FullName,
                        typeToAnalyze, cancellationToken);
                    break;
                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];        
                    analyzer = new CorralAnalyzer(corralDefaultArgs, workingDir, queryGenerator, inputAssembly as CciAssembly, inputFile.FullName,
                        typeToAnalyze, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return analyzer;
        }
    }
}