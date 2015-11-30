﻿using System;
using System.Configuration;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Text;
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

            StateAdded += (sender, args) => { };
            TransitionAdded += (sender, args) => { };
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
            var epaGenerator = new EpaGenerator(analyzer);

            var selectedMethods = from m in analysisEventArgs.SelectedMethods select m.ToString();

            var epaBuilderObservable = new ObservableEpaBuilder(new EpaBuilder(analysisEventArgs.TypeToAnalyze));
            epaBuilderObservable.StateAdded += OnStateAdded;
            epaBuilderObservable.TransitionAdded += OnTransitionAdded;

            return await epaGenerator.GenerateEpa(analysisEventArgs.TypeToAnalyze, selectedMethods, epaBuilderObservable);
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

        protected void OnStateAdded(object sender, StateAddedEventArgs e)
        {
            generatedEpa = e.EpaBuilder.Build();
            StateAdded(sender, e);
        }

        protected void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            generatedEpa = e.EpaBuilder.Build();
            TransitionAdded(sender, e);
        }

        protected IAnalyzer GetAnalyzer(TypeDefinition typeToAnalyze, string engine, CancellationToken cancellationToken)
        {
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
                    var cccheck = new FileInfo(ConfigurationManager.AppSettings["CccheckFullName"]);
                    Contract.Assert(cccheck.Exists);
                    throw new NotImplementedException();
                case "Corral":
                    var corralDefaultArgs = ConfigurationManager.AppSettings["CorralDefaultArgs"];
                    var workingDir = new DirectoryInfo(ConfigurationManager.AppSettings["WorkingDir"]);
                    Contract.Assert(workingDir.Exists);
                    analyzer = new CorralAnalyzer(corralDefaultArgs, workingDir, decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly, inputFile.FullName,
                        typeToAnalyze, cancellationToken);
                    break;
                default:
                    throw new NotSupportedException();
            }

            return analyzer;
        }
    }
}