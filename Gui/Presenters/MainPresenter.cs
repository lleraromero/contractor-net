using System;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Analysis.Cci;
using Analyzer.Corral;
using Contractor.Core;
using Contractor.Core.Model;
using Contractor.Gui.Models;

namespace Contractor.Gui.Presenters
{
    internal class MainPresenter
    {
        protected IMainScreen screen;
        protected IMainModel model;

        protected FileInfo inputFile;
        protected FileInfo contractFile;
        protected IAssemblyXXX inputAssembly;

        protected CciDecompiler decompiler;
        protected CancellationTokenSource cancellationSource;

        protected SynchronizationContext syncContext;

        protected Epa generatedEpa;

        public MainPresenter(SynchronizationContext syncContext, IMainScreen screen)
        {
            Configuration.Initialize();

            this.syncContext = syncContext;

            this.screen = screen;
            this.screen.LoadAssembly += ScreenOnLoadAssembly; 
            this.screen.LoadContracts += ScreenOnLoadContracts;
            this.screen.StartAnalysis += async (sender, analysisEventArgs) => await StartAnalisis(analysisEventArgs);
            this.screen.StopAnalysis += (sender, args) => cancellationSource.Cancel();
            this.screen.ExportGraph += (sender, outputFileInfo) => ExportGraph(outputFileInfo, generatedEpa, new EpaBinarySerializer());
            this.screen.GenerateAssembly += (sender, outputFileInfo) => GenerateAssembly(outputFileInfo, generatedEpa);

            model = new MainModel();

            decompiler = new CciDecompiler();
        }

        private async void ScreenOnLoadAssembly(object sender, FileInfo fileInfo)
        {
            inputFile = fileInfo;
            UpdateStatus("Decompiling assembly {0}...", inputFile.Name);
            inputAssembly = await Task.Run(() => decompiler.Decompile(inputFile.FullName, null));
            screen.ShowTypes(inputAssembly);
            UpdateStatus("Ready");
        }

        private async void ScreenOnLoadContracts(object sender, FileInfo fileInfo)
        {
            contractFile = fileInfo;
            UpdateStatus("Loading contracts from assembly {0}...", contractFile.Name);
            inputAssembly = await Task.Run(() =>  decompiler.Decompile(inputFile.FullName, contractFile.FullName));
            UpdateStatus("Ready");
        }


        protected async Task StartAnalisis(AnalysisEventArgs analysisEventArgs)
        {
            screen.DisableInterfaceWhileAnalyzing();
            cancellationSource = new CancellationTokenSource();

            var analyzer = GetAnalyzer(analysisEventArgs.TypeToAnalyze, cancellationSource.Token);
            var epaGenerator = GetEpaGenerator(analyzer);
            
            await GenerateGraph(epaGenerator, analysisEventArgs);

            screen.EnableInterfaceAfterAnalysis();
        }

        protected EpaGenerator GetEpaGenerator(IAnalyzer analyzer)
        {
            var epaGenerator = new EpaGenerator(inputAssembly, analyzer);
            epaGenerator.StateAdded += (sender, args) => syncContext.Post(_ => screen.StateAdded(args), null);
            epaGenerator.TransitionAdded += (sender, args) => syncContext.Post(_ => screen.TransitionAdded(args), null);
            return epaGenerator;
        }        

        protected IAnalyzer GetAnalyzer(TypeDefinition typeToAnalyze, CancellationToken cancellationToken)
        {
            IAnalyzer analyzer;
            switch (screen.Engine)
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

        protected async Task GenerateGraph(EpaGenerator epaGenerator, AnalysisEventArgs analysisEventArgs)
        {
            var typeToAnalyze = analysisEventArgs.TypeToAnalyze;

            var selectedMethods = from m in analysisEventArgs.SelectedMethods select m.ToString();

            UpdateStatus("Generating EPA for {0}...", typeToAnalyze.Name);

            try
            {
                var analysisResult = await epaGenerator.GenerateEpa(typeToAnalyze, selectedMethods);   
                ShowAnalysisResult(typeToAnalyze, analysisResult);
            }
            catch (OperationCanceledException)
            {
                UpdateStatus("Analysis for {0} aborted", typeToAnalyze.Name);
            }
            catch (Exception ex)
            {
                UpdateStatus("Analysis for {0} aborted", typeToAnalyze.Name);
                HandleException(ex);
            }
        }

        protected void ShowAnalysisResult(TypeDefinition typeToAnalyze, TypeAnalysisResult analysisResult)
        {
            var totalTime = analysisResult.TotalDuration;
            var statesCount = analysisResult.EPA.States.Count();
            var transitionsCount = analysisResult.EPA.Transitions.Count();

            UpdateStatus("Analysis for {0} done in {1:%m} minutes {1:%s} seconds ({2} states, {3} transitions)", typeToAnalyze.Name, totalTime, statesCount,
                transitionsCount);
        }

        protected void HandleException(Exception ex)
        {
            //textboxOutput.AppendText(GetEnvironmentInfo());
            //textboxOutput.AppendText(ex.ToString());
            //textboxOutput.AppendText(Environment.NewLine);
            //textboxOutput.AppendText(Environment.NewLine);

            screen.EnableInterfaceAfterAnalysis();

            var msg = string.Format("{0}\n\nFor more information check the output window.", ex.Message);
            MessageBox.Show(msg, "Oops! Something went wrong :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected void UpdateStatus(string format, params object[] args)
        {
            screen.UpdateStatus(string.Format(format, args));
        }

        private void GenerateAssembly(FileInfo outputFile, Epa epa)
        {
            UpdateStatus("Generating assembly {0}...", outputFile);

            try
            {
                throw new NotSupportedException();
                //_EpaGenerator.GenerateOutputAssembly(fileName);
                //new Instrumenter().GenerateOutputAssembly(fileName, epa);
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected void ExportGraph(FileInfo outputFile, Epa epa, ISerializer serializer)
        {
            Contract.Requires(outputFile != null && !outputFile.Exists);
            Contract.Requires(epa != null);
            Contract.Requires(serializer != null);

            UpdateStatus("Exporting graph to {0}...", outputFile);
            try
            {
                using (var fileStream = outputFile.OpenWrite())
                {
                    serializer.Serialize(fileStream, epa);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        //private void StartBackgroundTask(string message, params object[] args)
        //{
        //    progressBar.Visible = true;
        //    UpdateStatus(string.Format(message, args));
        //}

        //private void EndBackgroundTask(string message = null, params object[] args)
        //{
        //    progressBar.Visible = false;

        //    if (message == null)
        //    {
        //        statusLabel.Text = "Ready";
        //    }
        //    else
        //    {
        //        UpdateStatus(string.Format(message, args));
        //    }
        //}
    }
}