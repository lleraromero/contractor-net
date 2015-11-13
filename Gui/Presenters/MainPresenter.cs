using System;
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
    internal interface IMainPresenter
    {
        void StartApplication();
    }

    internal class MainPresenter : IMainPresenter
    {
        protected IMainScreen screen;
        protected IMainModel model;

        protected FileInfo inputFile;
        protected FileInfo contractFile;
        protected IAssemblyXXX inputAssembly;

        protected CciDecompiler decompiler;
        protected CancellationTokenSource cancellationSource;

        protected MethodFilterPresenter methodFilterPresenter;
        protected EpaViewerPresenter epaViewerPresenter;

        public MainPresenter(IMainScreen screen, IMainModel model)
        {
            this.screen = screen;
            this.model = model;

            methodFilterPresenter = new MethodFilterPresenter(screen.MethodFilter);
        }

        public void StartApplication()
        {
            screen.StartApplication();
        }

        protected async void StartAnalisisAsync(TypeDefinition typeToAnalyze)
        {
            cancellationSource = new CancellationTokenSource();

            var analyzer = GetAnalyzer(typeToAnalyze, cancellationSource.Token);
            var epaGenerator = GetEpaGenerator(analyzer);

            await Task.Run(() => GenerateGraph(epaGenerator, typeToAnalyze));
        }

        protected EpaGenerator GetEpaGenerator(IAnalyzer analyzer)
        {
            var epaGenerator = new EpaGenerator(inputAssembly, analyzer);
            epaGenerator.StateAdded += EpaGeneratorOnStateAdded;
            epaGenerator.TransitionAdded += EpaGeneratorOnTransitionAdded;
            return epaGenerator;
        }

        protected void EpaGeneratorOnStateAdded(object sender, StateAddedEventArgs e)
        {
            var stateAdded = e.EpaAndState.Item2;
            epaViewerPresenter.AddState(stateAdded);

            var epa = e.EpaAndState.Item1;
            UpdateStatus("Performing analysis for {0}: {1} states, {2} transitions", epa.Type.Name, epa.States.Count, epa.Transitions.Count);
        }

        protected void EpaGeneratorOnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            epaViewerPresenter.AddTransition(e.Transition);
            //var msg = "Performing analysis for {0}: {1} states, {2} transitions";
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

        protected void GenerateGraph(EpaGenerator epaGenerator, TypeDefinition typeToAnalyze)
        {
            UpdateStatus("Initializing analysis for {0}...", typeToAnalyze.Name);
            try
            {
                UpdateStatus("Decompiling assembly {0}...", inputFile.Name);

                if (contractFile != null)
                {
                    UpdateStatus("Loading contracts from assembly {0}...", contractFile.Name);
                }

                var selectedMethods = from m in methodFilterPresenter.SelectedMethods() select m.ToString();

                UpdateStatus("Generating contractor graph for {0}...", typeToAnalyze.Name);

                var analysisResult = epaGenerator.GenerateEpa(typeToAnalyze, selectedMethods);

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

            UpdateStatus("Analysis for {0} done in {1}: {2} states, {3} transitions", typeToAnalyze.Name, totalTime, statesCount,
                transitionsCount);
        }

        protected void HandleException(Exception ex)
        {
            //textboxOutput.AppendText(GetEnvironmentInfo());
            //textboxOutput.AppendText(ex.ToString());
            //textboxOutput.AppendText(Environment.NewLine);
            //textboxOutput.AppendText(Environment.NewLine);

            var msg = string.Format("{0}\n\nFor more information check the output window.", ex.Message);
            MessageBox.Show(msg, "Oops! Something went wrong :(", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected void UpdateStatus(string format, params object[] args)
        {
            screen.UpdateStatus(string.Format(format, args));
        }
    }
}