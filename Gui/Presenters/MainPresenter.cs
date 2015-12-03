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

        protected SynchronizationContext syncContext;

        public MainPresenter(SynchronizationContext syncContext, IMainScreen screen, IMainModel model)
        {
            this.syncContext = syncContext;

            this.model = model;
            this.model.StateAdded += OnStateAdded;
            this.model.TransitionAdded += OnTransitionAdded;

            this.screen = screen;
            this.screen.LoadAssembly += ScreenOnLoadAssembly; 
            this.screen.LoadContracts += ScreenOnLoadContracts;
            this.screen.StartAnalysis += async (sender, analysisEventArgs) => await StartAnalisis(analysisEventArgs);
            this.screen.StopAnalysis += (sender, args) => this.model.Stop();
            this.screen.ExportGraph += ScreenOnExportGraph;
            this.screen.GenerateAssembly += (sender, outputFileInfo) => GenerateAssembly(outputFileInfo, this.model.GeneratedEpa);
        }

        protected void ScreenOnExportGraph(object sender, FileInfo outputFileInfo)
        {
            ISerializer serializer;
            switch (outputFileInfo.Extension)
            {
                case ".xml":
                    serializer = new EpaXmlSerializer();
                    break;
                case ".png":
                    serializer = new EpaBinarySerializer();
                    break;
                default:
                    throw new NotSupportedException("Unexpected file type");
            }

            ExportGraph(outputFileInfo, this.model.GeneratedEpa, serializer);
        }

        protected void OnStateAdded(object sender, StateAddedEventArgs e)
        {
            syncContext.Post(_ => screen.StateAdded(e), null);
        }

        protected void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            syncContext.Post(_ => screen.TransitionAdded(e), null);
        }

        protected async void ScreenOnLoadAssembly(object sender, FileInfo fileInfo)
        {
            UpdateStatus("Decompiling assembly {0}...", fileInfo.Name);
            await model.LoadAssembly(fileInfo);
            screen.ShowTypes(model.InputAssembly);
            UpdateStatus("Ready");
        }

        protected async void ScreenOnLoadContracts(object sender, FileInfo fileInfo)
        {
            UpdateStatus("Loading contracts from assembly {0}...", fileInfo.Name);
            await model.LoadContracts(fileInfo);
            UpdateStatus("Ready");
        }

        protected async Task StartAnalisis(AnalysisEventArgs analysisEventArgs)
        {
            screen.DisableInterfaceWhileAnalyzing();

            var typeToAnalyze = analysisEventArgs.TypeToAnalyze;
            UpdateStatus("Generating EPA for {0}...", typeToAnalyze.Name);

            try
            {
                var analysisResult = await model.Start(analysisEventArgs);
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

            
            screen.EnableInterfaceAfterAnalysis();
        }

        protected void ShowAnalysisResult(TypeDefinition typeToAnalyze, TypeAnalysisResult analysisResult)
        {
            var totalTime = analysisResult.TotalTime;
            var statesCount = analysisResult.Epa.States.Count();
            var transitionsCount = analysisResult.Epa.Transitions.Count();

            UpdateStatus("Analysis for {0} done in {1:%m} minutes {1:%s} seconds ({2} states, {3} transitions)", typeToAnalyze.Name, totalTime, statesCount,
                transitionsCount);

            UpdateOutput(analysisResult.ToString());
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
            UpdateOutput(format, args);
        }

        protected void UpdateOutput(string format, params object[] args)
        {
            screen.UpdateOutput(string.Format(format, args));
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
            Contract.Requires(outputFile != null);
            Contract.Requires(epa != null);
            Contract.Requires(serializer != null);

            UpdateStatus("Exporting graph to {0}...", outputFile);
            try
            {
                using (var fileStream = File.Create(outputFile.FullName))
                {
                    serializer.Serialize(fileStream, epa);
                }
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
            UpdateStatus("Done!", outputFile);
        }
    }
}