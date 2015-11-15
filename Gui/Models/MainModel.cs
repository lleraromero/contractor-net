using System;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Analysis.Cci;
using Analyzer.Corral;
using Contractor.Core;
using Contractor.Core.Model;

namespace Contractor.Gui.Models
{
    internal class MainModel : IMainModel
    {
        protected FileInfo inputFile;
        protected FileInfo contractFile;
        protected IAssemblyXXX inputAssembly;

        protected CancellationTokenSource cancellationSource;
        protected CciDecompiler decompiler;
        protected Epa generatedEpa;

        public MainModel()
        {
            decompiler = new CciDecompiler();
        }

        public Epa GeneratedEpa
        {
            get { return generatedEpa; }
        }

        public IAssemblyXXX InputAssembly
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

            var analyzer = GetAnalyzer(analysisEventArgs.TypeToAnalyze, analysisEventArgs.Engine, cancellationSource.Token);
            var epaGenerator = GetEpaGenerator(analyzer);

            var selectedMethods = from m in analysisEventArgs.SelectedMethods select m.ToString();
            return await epaGenerator.GenerateEpa(analysisEventArgs.TypeToAnalyze, selectedMethods);
        }

        public async Task LoadAssembly(FileInfo inputFileInfo)
        {
            inputFile = inputFileInfo;
            inputAssembly = await Task.Run(() => decompiler.Decompile(inputFile.FullName, null));
        }

        public async Task LoadContracts(FileInfo contractFileInfo)
        {
            contractFile = contractFileInfo;
            inputAssembly = await Task.Run(() => decompiler.Decompile(inputFile.FullName, contractFile.FullName));
        }

        protected EpaGenerator GetEpaGenerator(IAnalyzer analyzer)
        {
            var epaGenerator = new EpaGenerator(inputAssembly, analyzer);
            epaGenerator.StateAdded += OnStateAdded;
            epaGenerator.TransitionAdded += OnTransitionAdded;
            return epaGenerator;
        }

        protected void OnStateAdded(object sender, StateAddedEventArgs e)
        {
            generatedEpa = e.EpaBuilder.Build();
            if (StateAdded != null)
            {
                StateAdded(sender, e);
            }
        }

        protected void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            generatedEpa = e.EpaBuilder.Build();
            if (TransitionAdded != null)
            {
                TransitionAdded(sender, e);
            }
        }

        protected IAnalyzer GetAnalyzer(TypeDefinition typeToAnalyze, string engine, CancellationToken cancellationToken)
        {
            IAnalyzer analyzer;
            switch (engine)
            {
                case "CodeContracts":
                    throw new NotImplementedException();
                case "Corral":
                    analyzer = new CorralAnalyzer(decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly, inputFile.FullName,
                        typeToAnalyze, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return analyzer;
        }
    }
}