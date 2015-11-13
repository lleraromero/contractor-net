using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using Analysis.Cci;
using Analyzer.Corral;
using Contractor.Core;
using Contractor.Core.Model;
using Action = System.Action;

namespace Contractor.Gui
{
    interface IMainScreen
    {
        void StartApplication();
        IEpaViewerScreen EpaViewer { get; }
        ITypesViewerScreen TypesViewer { get; }
        IMethodFilterScreen MethodFilter { get; }
        string Engine { get; }
        void UpdateStatus(string msg);
    }
    internal partial class Main : Form, IMainScreen
    {
        private TypeAnalysisResult _LastResult;
        private Options _Options;

        protected EpaViewerPresenter epaViewerPresenter;
        protected MethodFilterPresenter methodFilterPresenter;
        protected TypeDefinition selectedType;
        protected SynchronizationContext syncContext;
        protected TypesViewerPresenter typesViewerPresenter;

        protected string inputFile;
        protected string contractFile;

        protected CciDecompiler decompiler;
        protected CancellationTokenSource cancellationSource;

        public void StartApplication()
        {
            Application.Run(this);
        }

        public IEpaViewerScreen EpaViewer { get { return epaViewer; } }
        public ITypesViewerScreen TypesViewer { get { return typesViewer; } }
        public IMethodFilterScreen MethodFilter { get { return methodFilter; } }
        public string Engine { get { return (string)cmbBackend.SelectedItem; } }

        public void UpdateStatus(string msg)
        {
            statusLabel.Text = msg;

            textboxOutput.AppendText(msg);
            textboxOutput.AppendText(Environment.NewLine);
        }

        public Main()
        {
            InitializeComponent();

            splitcontainerOutput.Panel2Collapsed = true;
            cmbBackend.Items.Add("CodeContracts");
            cmbBackend.Items.Add("Corral");
            cmbBackend.SelectedIndex = 1;

            _Options = new Options();

            epaViewerPresenter = new EpaViewerPresenter(epaViewer, new EpaViewer());

            syncContext = SynchronizationContext.Current;
            typesViewerPresenter = new TypesViewerPresenter(typesViewer, new TypesViewer(), syncContext);
            typesViewerPresenter.TypeSelected += TypesViewerPresenterOnTypeSelected;
            typesViewerPresenter.StartAnalysis += (sender, typeDefinition) => { StartAnalisisAsync(typeDefinition); };

            methodFilterPresenter = new MethodFilterPresenter(methodFilter);

            decompiler = new CciDecompiler();
        }

        protected void TypesViewerPresenterOnTypeSelected(object sender, TypeDefinition typeDefinition)
        {
            selectedType = typeDefinition;

            methodFilterPresenter.LoadMethods(typeDefinition);

            buttonStartAnalysis.Enabled = true;
        }

        #region Event Handlers

        private void OnLoad(object sender, EventArgs e)
        {
            try
            {
                Configuration.Initialize();
            }
            catch (Exception ex)
            {
                HandleException(ex);
            }
        }

        protected void OnAbout(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        private void OnOptions(object sender, EventArgs e)
        {
            var optionsDialog = new OptionsDialog(_Options);
            var result = optionsDialog.ShowDialog(this);

            if (result == DialogResult.OK)
                _Options = optionsDialog.Options;
        }

        private void OnOutput(object sender, EventArgs e)
        {
            splitcontainerOutput.Panel2Collapsed = !menuitemOutput.Checked;
        }

        private void OnOutputClose(object sender, EventArgs e)
        {
            splitcontainerOutput.Panel2Collapsed = true;
            menuitemOutput.Checked = false;
        }

        private void OnLoadAssembly(object sender, EventArgs e)
        {
            var result = loadAssemblyDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var fileName = loadAssemblyDialog.FileName;
                loadAssemblyDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                inputFile = fileName;

                Task.Run(() => LoadAssembly(fileName));
            }
        }

        private void OnLoadContracts(object sender, EventArgs e)
        {
            var result = loadContractsDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = loadContractsDialog.FileName;
                loadContractsDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                contractFile = fileName;

                buttonLoadContracts.Checked = true;

                var name = Path.GetFileName(fileName);
                SetBackgroundStatus("Using contract reference assembly {0}", name);
            }
        }

        protected void OnStartAnalysis(object sender, EventArgs e)
        {
            StartAnalisisAsync(selectedType);
        }

        protected void OnStopAnalysis(object sender, EventArgs e)
        {
            cancellationSource.Cancel();

            buttonStopAnalysis.Enabled = false;

            var typeFullName = selectedType.ToString();
            statusLabel.Text = string.Format("Aborting analysis for {0}...", typeFullName);
        }

        private void OnExportGraph(object sender, EventArgs e)
        {
            if (_LastResult.EPA == null)
            {
                throw new InvalidDataException("Trying to export an unfinished EPA!");
            }

            var result = exportGraphDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = exportGraphDialog.FileName;
                exportGraphDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                Task.Factory.StartNew(() => ExportGraph(fileName, _LastResult.EPA));
            }
        }

        private void OnGenerateAssembly(object sender, EventArgs e)
        {
            var result = generateOutputDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = generateOutputDialog.FileName;
                generateOutputDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                Task.Factory.StartNew(() => GenerateAssembly(fileName));
            }
        }

        private void OnStateAdded(object sender, StateAddedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<StateAddedEventArgs>(OnStateAdded), sender, e);
                return;
            }

            epaViewerPresenter.AddState(e.EpaAndState.Item2);

            UpdateAnalysisProgress();
        }

        private void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<TransitionAddedEventArgs>(OnTransitionAdded), sender, e);
                return;
            }

            epaViewerPresenter.AddTransition(e.Transition);

            UpdateAnalysisProgress();
        }

        #endregion Event Handlers

        #region Private Methods

        protected async void StartAnalisisAsync(TypeDefinition typeToAnalyze)
        {
            var backend = (string)cmbBackend.SelectedItem;

            var inputAssembly = decompiler.Decompile(inputFile, contractFile);
            selectedType = inputAssembly.Types().First(t => t.Name.Equals(typeToAnalyze.Name));

            cancellationSource = new CancellationTokenSource();

            IAnalyzer analyzer;
            switch (backend)
            {
                case "CodeContracts":
                    throw new NotImplementedException();
                case "Corral":
                    analyzer = new CorralAnalyzer(decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly, inputFile,
                        selectedType, cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            var epaGenerator = new EpaGenerator(inputAssembly, analyzer);
            epaGenerator.StateAdded += OnStateAdded;
            epaGenerator.TransitionAdded += OnTransitionAdded;

            await Task.Run(() => GenerateGraph(epaGenerator, selectedType));
        }

        protected void GenerateGraph(EpaGenerator epaGenerator, TypeDefinition typeToAnalyze)
        {
            BeginInvoke(new Action(UpdateAnalysisInitialize));
            try
            {
                var fileName = Path.GetFileName(inputFile);
                BeginInvoke(new Action<string, string>(SetBackgroundStatus), "Decompiling assembly {0}...", fileName);

                if (contractFile != null)
                {
                    fileName = Path.GetFileName(contractFile);
                    BeginInvoke(new Action<string, string>(SetBackgroundStatus), "Loading contracts from assembly {0}...", fileName);
                }

                var typeFullName = selectedType.Name;
                var selectedMethods = from m in methodFilterPresenter.SelectedMethods() select m.ToString();

                BeginInvoke(new Action<string, string>(SetBackgroundStatus), "Generating contractor graph for {0}...", typeFullName);

                var typeAnalysisResult = epaGenerator.GenerateEpa(typeToAnalyze, selectedMethods);
                BeginInvoke(new Action<TypeAnalysisResult>(UpdateAnalysisEnd), typeAnalysisResult);
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action<Exception>(HandleException), ex);
                BeginInvoke(new Action<TypeAnalysisResult>(UpdateAnalysisEnd), (object)null);
            }
        }

        private void UpdateAnalysisInitialize()
        {
            var typeFullName = selectedType.ToString();
            StartBackgroundTask("Initializing analysis for {0}...", typeFullName);

            epaViewerPresenter.Reset();

            richtextboxInformation.Clear();

            buttonStartAnalysis.Enabled = false;
            buttonLoadAssembly.Enabled = false;
            buttonLoadContracts.Enabled = false;
            buttonExportGraph.Enabled = false;
            buttonGenerateAssembly.Enabled = false;
            buttonStopAnalysis.Enabled = true;
        }

        private void UpdateAnalysisEnd(TypeAnalysisResult analysisResult)
        {
            Contract.Requires(analysisResult != null);

            var typeFullName = selectedType.ToString();

            if (cancellationSource.IsCancellationRequested)
            {
                EndBackgroundTask("Analysis for {0} aborted", typeFullName);
            }
            else
            {
                var seconds = Math.Ceiling(analysisResult.TotalDuration.TotalSeconds);
                var statesCount = analysisResult.EPA.States.Count();
                var transitionsCount = analysisResult.EPA.Transitions.Count();

                EndBackgroundTask("Analysis for {0} done in {1} seconds: {2} states, {3} transitions", typeFullName, seconds, statesCount,
                    transitionsCount);
                OutputTypeAnalysisResult(analysisResult);

                _LastResult = analysisResult;
            }

            buttonStopAnalysis.Enabled = false;
            buttonLoadAssembly.Enabled = true;
            buttonLoadContracts.Enabled = true;
            buttonExportGraph.Enabled = true;
            buttonGenerateAssembly.Enabled = true;

            buttonStartAnalysis.Enabled = false;
        }

        private void OutputTypeAnalysisResult(TypeAnalysisResult analysisResult)
        {
            textboxOutput.AppendText(analysisResult.ToString());
            textboxOutput.AppendText(Environment.NewLine);
        }

        private void UpdateAnalysisProgress()
        {
            var typeFullName = selectedType.ToString();
            var msg = "Performing analysis for {0}: {1} states, {2} transitions";
            //statusLabel.Text = string.Format(msg, typeFullName, _Graph.NodeCount, _Graph.EdgeCount);
        }

        private void GenerateAssembly(string fileName)
        {
            BeginInvoke((Action)delegate
            {
                var name = Path.GetFileName(fileName);
                StartBackgroundTask("Generating assembly {0}...", name);
            });

            try
            {
                throw new NotSupportedException();
                //_EpaGenerator.GenerateOutputAssembly(fileName);
                //new Instrumenter().GenerateOutputAssembly(fileName, epa);
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action<Exception>(HandleException), ex);
            }

            BeginInvoke((Action)delegate { EndBackgroundTask(); });
        }

        public void HandleException(Exception ex)
        {
            textboxOutput.AppendText(GetEnvironmentInfo());
            textboxOutput.AppendText(ex.ToString());
            textboxOutput.AppendText(Environment.NewLine);
            textboxOutput.AppendText(Environment.NewLine);

            var msg = string.Format("{0}\n\nFor more information check the output window.", ex.Message);
            MessageBox.Show(msg, Text, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        protected string GetEnvironmentInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");
            sb.AppendLine(string.Format("Operating System: {0}", Environment.OSVersion));
            sb.AppendLine(Environment.Is64BitOperatingSystem ? " (64 bits)" : " (32 bits)");

            sb.AppendLine(string.Format("CLR Version: {0}", Environment.Version));

            var codeContractsVersion = "Code Contracts is not installed.";
            var checkerFileName = Configuration.CheckerFileName;
            if (!string.IsNullOrEmpty(checkerFileName) && File.Exists(checkerFileName))
            {
                codeContractsVersion = FileVersionInfo.GetVersionInfo(checkerFileName).ProductVersion;
            }

            sb.AppendLine(string.Format("Code Contracts Version: {0}", codeContractsVersion));

            var assemblyName = Assembly.GetExecutingAssembly().GetName();
            sb.AppendLine(string.Format("Contractor.NET Version: {0}", assemblyName.Version));

            sb.AppendLine("----------------------------------------");
            return sb.ToString();
        }

        private void ExportGraph(string fileName, Epa epa)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName));
            Contract.Requires(epa != null);

            BeginInvoke((Action)delegate
            {
                var name = Path.GetFileName(fileName);
                StartBackgroundTask("Exporting graph to {0}...", name);
            });

            try
            {
                var ext = Path.GetExtension(fileName);

                switch (ext.ToLower())
                {
                    case ".xml":
                        using (var stream = File.Create(fileName))
                        {
                            new EpaXmlSerializer().Serialize(stream, epa);
                        }
                        break;
                    default:
                        using (var fileStream = File.Create(fileName))
                        {
                            new EpaBinarySerializer().Serialize(fileStream, epa);
                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action<Exception>(HandleException), ex);
            }

            BeginInvoke((Action)delegate { EndBackgroundTask(); });
        }

        private async void LoadAssembly(string fileName)
        {
            BeginInvoke((Action)delegate
            {
                var name = Path.GetFileName(fileName);
                StartBackgroundTask("Loading assembly {0}...", name);
            });

            var assembly = decompiler.Decompile(fileName, null);
            await Task.Run(() => typesViewerPresenter.ShowTypes(assembly));

            BeginInvoke((Action)delegate
            {
                buttonLoadContracts.Enabled = true;

                buttonLoadContracts.Checked = false;

                EndBackgroundTask();
            });
        }

        private void StartBackgroundTask(string message, params object[] args)
        {
            progressBar.Visible = true;
            SetBackgroundStatus(message, args);
        }

        private void EndBackgroundTask(string message = null, params object[] args)
        {
            progressBar.Visible = false;

            if (message == null)
            {
                statusLabel.Text = "Ready";
            }
            else
            {
                SetBackgroundStatus(message, args);
            }
        }

        private void SetBackgroundStatus(string message, string arg)
        {
            SetBackgroundStatus(message, arg as object);
        }

        private void SetBackgroundStatus(string message, params object[] args)
        {
            message = string.Format(message, args);
            statusLabel.Text = message;

            textboxOutput.AppendText(message);
            textboxOutput.AppendText(Environment.NewLine);
        }

        #endregion Private Methods
    }
}