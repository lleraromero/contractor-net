using System;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Windows.Forms;
using Contractor.Core;
using Contractor.Core.Model;
using Contractor.Gui.Models;
using Contractor.Gui.Presenters;

namespace Contractor.Gui.Views
{
    internal partial class Main : Form, IMainScreen
    {
        protected readonly MainPresenter mainPresenter;
        protected EpaViewerPresenter epaViewerPresenter;
        protected MethodFilterPresenter methodFilterPresenter;
        protected TypesViewerPresenter typesViewerPresenter;

        protected ITypeDefinition selectedType;
        private Options options;

        public Main()
        {
            InitializeComponent();

            splitcontainerOutput.Panel2Collapsed = true;

            //Load Engines
            cmbBackend.Items.Add("CodeContracts");
            cmbBackend.Items.Add("Corral");
            cmbBackend.SelectedIndex = 1;

            cmbConditions.Items.Add("EPA");
            cmbConditions.Items.Add("EPA-I");
            cmbConditions.Items.Add("EPA-O");
            cmbConditions.Items.Add("EPA-I/O");
            cmbConditions.SelectedIndex = 2;

            cmbExc.Items.Add("All");
            cmbExc.Items.Add("Exc");
            cmbExc.SelectedIndex = 0;

            epaViewerPresenter = new EpaViewerPresenter(epaViewer, new EpaViewer(), SynchronizationContext.Current);
            epaViewerPresenter.StateSelected += ShowStateInformation;
            typesViewerPresenter = new TypesViewerPresenter(typesViewer, new TypesViewer(), SynchronizationContext.Current);
            typesViewerPresenter.TypeSelected += TypesViewerPresenterOnTypeSelected;
            typesViewerPresenter.StartAnalysis += TypesViewerPresenterOnStartAnalysis;
            methodFilterPresenter = new MethodFilterPresenter(methodFilter);

            options = new Options();
            mainPresenter = new MainPresenter(SynchronizationContext.Current, this, new MainModel());
        }

        public string Engine
        {
            get { return (string) cmbBackend.SelectedItem; }
        }

        public string Conditions
        {
            get { return (string)cmbConditions.SelectedItem; }
        }

        public string Exceptions
        {
            get { return (string)cmbExc.SelectedItem; }
        }

        public event EventHandler<FileInfo> LoadAssembly;
        public event EventHandler<FileInfo> LoadContracts;
        public event EventHandler<AnalysisEventArgs> StartAnalysis;
        public event EventHandler StopAnalysis;
        public event EventHandler<FileInfo> ExportGraph;
        public event EventHandler<FileInfo> GenerateAssembly;

        public void UpdateStatus(string msg)
        {
            statusLabel.Text = msg;
        }

        public void UpdateOutput(string msg)
        {
            textboxOutput.AppendText(msg);
            textboxOutput.AppendText(Environment.NewLine);
        }

        public void StateAdded(StateAddedEventArgs e)
        {
            epaViewerPresenter.AddState(e.State);
            UpdateProgress(e);
        }

        public void TransitionAdded(TransitionAddedEventArgs e)
        {
            epaViewerPresenter.AddTransition(e.Transition);
            UpdateProgress(e);
        }

        public void ShowTypes(IAssembly inputAssembly)
        {
            typesViewerPresenter.ShowTypes(inputAssembly);
        }

        public void DisableInterfaceWhileAnalyzing()
        {
            buttonOptions.Enabled = false;
            buttonLoadAssembly.Enabled = false;
            buttonLoadContracts.Enabled = false;
            buttonExportGraph.Enabled = false;
            buttonGenerateAssembly.Enabled = false;
            buttonStartAnalysis.Enabled = false;
            cmbBackend.Enabled = false;
            cmbConditions.Enabled = false;
            cmbExc.Enabled = false;
            typesViewer.Enabled = false;
            methodFilter.Enabled = false;

            buttonStopAnalysis.Enabled = true;
            pgrAnalizando.Visible = true;
        }

        public void EnableInterfaceAfterAnalysis()
        {
            buttonOptions.Enabled = true;
            buttonLoadAssembly.Enabled = true;
            buttonLoadContracts.Enabled = true;
            buttonExportGraph.Enabled = true;
            buttonGenerateAssembly.Enabled = true;
            buttonStartAnalysis.Enabled = true;
            cmbBackend.Enabled = true;
            cmbConditions.Enabled = true;
            cmbExc.Enabled = true;
            typesViewer.Enabled = true;
            methodFilter.Enabled = true;

            buttonStopAnalysis.Enabled = false;
            pgrAnalizando.Visible = false;
        }

        private void ShowStateInformation(object sender, State e)
        {
            var info = new StringBuilder();
            info.Append(@"{\rtf1\ansi\fs8\par\fs18");

            if (e.EnabledActions.Any())
            {
                info.Append(@" \b Enabled Actions \b0 \par ");
                var text = string.Join(@" \par ", e.EnabledActions);
                info.Append(text);
            }

            if (e.DisabledActions.Any())
            {
                info.Append(@" \fs8\par\par\fs18 \b Disabled Actions \b0 \par ");
                var text = string.Join(@" \par ", e.DisabledActions);
                info.Append(text);
            }

            info.Append(@"}");

            richtextboxInformation.Rtf = info.ToString();
        }

        protected void UpdateProgress(TypeEventArgs e)
        {
            var status = string.Format("Performing analysis for {0}: {1} states, {2} transitions", e.EpaBuilder.Type.Name, e.EpaBuilder.States.Count,
                e.EpaBuilder.Transitions.Count);
            UpdateStatus(status);
            UpdateOutput(status);
        }

        protected void TypesViewerPresenterOnStartAnalysis(object sender, ITypeDefinition typeDefinition)
        {
            selectedType = typeDefinition;
            OnStartAnalysis(sender, new EventArgs());
        }

        protected void TypesViewerPresenterOnTypeSelected(object sender, ITypeDefinition typeDefinition)
        {
            selectedType = typeDefinition;
            methodFilterPresenter.LoadMethods(typeDefinition);
            buttonStartAnalysis.Enabled = true;
        }

        protected void OnAbout(object sender, EventArgs e)
        {
            new AboutDialog().ShowDialog(this);
        }

        private void OnOptions(object sender, EventArgs e)
        {
            var optionsDialog = new OptionsDialog(options);
            var result = optionsDialog.ShowDialog(this);

            if (result == DialogResult.OK)
                options = optionsDialog.Options;
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

        protected void OnLoadAssembly(object sender, EventArgs e)
        {
            var result = loadAssemblyDialog.ShowDialog(this);
            if (result == DialogResult.OK)
            {
                var fileName = loadAssemblyDialog.FileName;
                loadAssemblyDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                if (LoadAssembly != null)
                {
                    methodFilterPresenter.Reset();
                    LoadAssembly(sender, new FileInfo(fileName));
                    buttonStartAnalysis.Enabled = false;
                    buttonLoadContracts.Enabled = true;
                }
            }
        }

        protected void OnLoadContracts(object sender, EventArgs e)
        {
            var result = loadContractsDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = loadContractsDialog.FileName;
                loadContractsDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                if (LoadContracts != null)
                {
                    LoadContracts(sender, new FileInfo(fileName));
                }
            }
        }

        protected void OnStartAnalysis(object sender, EventArgs e)
        {
            if (StartAnalysis != null)
            {
                epaViewerPresenter.Reset();
                var analysisArgs = new AnalysisEventArgs(selectedType, methodFilterPresenter.SelectedMethods(), Engine, Conditions, Exceptions);
                StartAnalysis(sender, analysisArgs);
            }
        }

        protected void OnStopAnalysis(object sender, EventArgs e)
        {
            if (StopAnalysis != null)
            {
                var msg = "Stopping analysis...";
                UpdateStatus(msg);
                UpdateOutput(msg);
                StopAnalysis(sender, e);
            }
        }

        protected void OnExportGraph(object sender, EventArgs e)
        {
            var result = exportGraphDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = exportGraphDialog.FileName;
                exportGraphDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                if (ExportGraph != null)
                {
                    ExportGraph(sender, new FileInfo(fileName));
                }
            }
        }

        protected void OnGenerateAssembly(object sender, EventArgs e)
        {
            var result = generateOutputDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = generateOutputDialog.FileName;
                generateOutputDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                if (GenerateAssembly != null)
                {
                    GenerateAssembly(sender, new FileInfo(fileName));
                }
            }
        }
    }
}