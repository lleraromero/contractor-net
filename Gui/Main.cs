using System;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
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
    internal partial class Main : Form
    {
        private CancellationTokenSource _cancellationSource;

        private TypeAnalysisResult _LastResult;
        private Options _Options;

        protected EpaViewerPresenter epaViewerPresenter;
        protected MethodFilterPresenter methodFilterPresenter;
        protected TypeDefinition selectedType;
        protected SynchronizationContext syncContext;
        protected TypesViewerPresenter typesViewerPresenter;

        protected CciDecompiler decompiler;
        protected string inputFile;
        protected string contractFile;

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
            if (result != DialogResult.OK)
            {
                return;
            }

            var fileName = loadAssemblyDialog.FileName;
            loadAssemblyDialog.InitialDirectory = Path.GetDirectoryName(fileName);

            inputFile = fileName;

            Task.Run(() => LoadAssembly(fileName));
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
            _cancellationSource.Cancel();

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
            var backend = (string) cmbBackend.SelectedItem;

            var inputAssembly = decompiler.Decompile(inputFile, contractFile);
            selectedType = inputAssembly.Types().First(t => t.Name.Equals(typeToAnalyze.Name));

            _cancellationSource = new CancellationTokenSource();

            IAnalyzer analyzer = null;
            switch (backend)
            {
                case "CodeContracts":
                    throw new NotImplementedException();
                case "Corral":
                    analyzer = new CorralAnalyzer(decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly, inputFile,
                        selectedType, _cancellationSource.Token);
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

                var inputAssembly = decompiler.Decompile(inputFile, contractFile);

                var typeAnalysisResult = epaGenerator.GenerateEpa(typeToAnalyze, selectedMethods);
                BeginInvoke(new Action<TypeAnalysisResult>(UpdateAnalysisEnd), typeAnalysisResult);
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action<Exception>(HandleException), ex);
                BeginInvoke(new Action<TypeAnalysisResult>(UpdateAnalysisEnd), (object) null);
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

            if (_cancellationSource.IsCancellationRequested)
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

            ////_Graph.Attr.AspectRatio = (double)graphViewer.ClientSize.Width / graphViewer.ClientSize.Height;
            //graphViewer.Graph = _Graph;
            //graphViewer.Enabled = true;

            //foreach (var obj in graphViewer.Entities)
            //{
            //    if (obj is IViewerNode)
            //    {
            //        obj.MarkedForDraggingEvent += OnNodeMarkedForDragging;
            //        obj.UnmarkedForDraggingEvent += OnNodeUnmarkedForDragging;
            //    }
            //    else
            //    {
            //        obj.UnmarkedForDraggingEvent += OnObjectUnmarkedForDragging;
            //    }
            //}
        }

        private void GenerateAssembly(string fileName)
        {
            BeginInvoke((Action) delegate
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

            BeginInvoke((Action) delegate { EndBackgroundTask(); });
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

        private string GetEnvironmentInfo()
        {
            var sb = new StringBuilder();
            sb.AppendLine("========================================");

            sb.Append("Operating System: ");
            sb.Append(Environment.OSVersion);
            sb.AppendLine(Environment.Is64BitOperatingSystem ? " (64 bits)" : " (32 bits)");

            sb.Append("CLR Version: ");
            sb.AppendLine(Environment.Version.ToString());

            var codeContractsVersion = "Code Contracts is not installed.";
            var checkerFileName = Configuration.CheckerFileName;

            if (!string.IsNullOrEmpty(checkerFileName) && File.Exists(checkerFileName))
            {
                var vi = FileVersionInfo.GetVersionInfo(checkerFileName);
                codeContractsVersion = vi.ProductVersion;
            }

            sb.Append("Code Contracts Version: ");
            sb.AppendLine(codeContractsVersion);

            var assemblyName = Assembly.GetExecutingAssembly().GetName();

            sb.Append("Contractor.NET Version: ");
            sb.AppendLine(assemblyName.Version.ToString());

            sb.AppendLine("----------------------------------------");
            return sb.ToString();
        }

        private string GetStateInfo(EpaBuilder epaBuilder, State state)
        {
            var info = new StringBuilder();
            info.Append(@"{\rtf1\ansi\fs8\par\fs18");

            if (epaBuilder.Initial.Equals(state))
            {
                info.Append(@" \b Initial State \b0 \fs8\par\par\fs18");
            }

            if (state.EnabledActions.Any())
            {
                info.Append(@" \b Enabled Actions \b0 \par ");
                var text = string.Join(@" \par ", state.EnabledActions);
                info.Append(text);
            }

            if (state.DisabledActions.Any())
            {
                info.Append(@" \fs8\par\par\fs18 \b Disabled Actions \b0 \par ");
                var text = string.Join(@" \par ", state.DisabledActions);
                info.Append(text);
            }

            info.Append(@"}");
            return info.ToString();
        }

        private void ExportGraph(string fileName, Epa epa)
        {
            Contract.Requires(!string.IsNullOrEmpty(fileName));
            Contract.Requires(epa != null);

            BeginInvoke((Action) delegate
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

                    case ".gv":
                        ExportGraphvizGraph(fileName, epa);
                        break;

                    case ".emf":
                    case ".wmf":
                        ExportVectorGraph(fileName);
                        break;

                    default:
                        ExportImageGraph(fileName, epa);
                        break;
                }
            }
            catch (Exception ex)
            {
                BeginInvoke(new Action<Exception>(HandleException), ex);
            }

            BeginInvoke((Action) delegate { EndBackgroundTask(); });
        }

        private void ExportGraphvizGraph(string fileName, Epa epa)
        {
            throw new NotImplementedException();
            //using (var sw = File.CreateText(fileName))
            //{
            //    var typeFullName = _AnalizedType.GetDisplayName();
            //    var nodes = graphViewer.Graph.GeometryGraph.CollectAllNodes();
            //    var initialNodes = nodes.Where(n => epa.Initial.Equals((n.UserData as Node).UserData as State));
            //    var otherNodes = nodes.Where(n => !epa.Initial.Equals((n.UserData as Node).UserData as State));

            //    sw.WriteLine("digraph \"{0}\"", typeFullName);
            //    sw.WriteLine("{");
            //    sw.WriteLine("\trankdir=LR;");
            //    sw.WriteLine("\tnode [style = filled, fillcolor = aliceblue, fontname = \"{0}\"];", "Cambria");

            //    sw.WriteLine();
            //    sw.WriteLine("\tnode [shape = doublecircle];");

            //    foreach (var n in initialNodes)
            //    {
            //        var node = n.UserData as Node;
            //        var name = node.LabelText.Replace(sw.NewLine, @"\n");

            //        sw.WriteLine("\tnode [label = \"{0}\"]; \"{1}\";", name, node.Id);
            //    }

            //    sw.WriteLine();
            //    sw.WriteLine("\tnode [shape = circle];");

            //    foreach (var n in otherNodes)
            //    {
            //        var node = n.UserData as Node;
            //        var name = node.LabelText.Replace(sw.NewLine, @"\n");

            //        sw.WriteLine("\tnode [label = \"{0}\"]; \"{1}\";", name, node.Id);
            //    }

            //    sw.WriteLine();
            //    sw.WriteLine("\tedge [fontname = \"{0}\"];", "Cambria");
            //    sw.WriteLine();

            //    foreach (var edge in graphViewer.Graph.Edges)
            //    {
            //        var from = edge.SourceNode.Id;
            //        var to = edge.TargetNode.Id;
            //        var label = edge.LabelText.Replace(sw.NewLine, @"\n");

            //        sw.WriteLine("\tedge [label = \"{0}\"] \"{1}\" -> \"{2}\";", label, from, to);
            //    }

            //    sw.WriteLine("}");
            //}
        }

        private void ExportVectorGraph(string fileName)
        {
            throw new NotImplementedException();
            //var scale = 6.0f;
            //var w = (int) (graphViewer.Graph.Width*scale);
            //var h = (int) (graphViewer.Graph.Height*scale);

            //using (var temp = CreateGraphics())
            //{
            //    var hdc = temp.GetHdc();

            //    using (var img = new Metafile(fileName, hdc, EmfType.EmfOnly))
            //    {
            //        temp.ReleaseHdc(hdc);

            //        using (var g = Graphics.FromImage(img))
            //        {
            //            g.SmoothingMode = SmoothingMode.HighQuality;
            //            g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
            //            g.CompositingQuality = CompositingQuality.HighQuality;
            //            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            //            DrawGraph(g, w, h, scale);
            //        }
            //    }
            //}
        }

        private void ExportImageGraph(string fileName, Epa epa)
        {
            using (var fileStream = File.Create(fileName))
            {
                new EpaBinarySerializer().Serialize(fileStream, epa);
            }
        }

        private void DrawGraph(Graphics g, int w, int h, float scale)
        {
            throw new NotImplementedException();
            //var graph = graphViewer.Graph;

            //var num1 = (float) (0.5*w - scale*(graph.Left + 0.5*graph.Width));
            //var num2 = (float) (0.5*h + scale*(graph.Bottom + 0.5*graph.Height));

            //using (var brush = new SolidBrush(Draw.MsaglColorToDrawingColor(graph.Attr.BackgroundColor)))
            //    g.FillRectangle(brush, 0, 0, w, h);

            //using (var matrix = new Matrix(scale, 0f, 0f, -scale, num1, num2))
            //{
            //    g.Transform = matrix;
            //    Draw.DrawPrecalculatedLayoutObject(g, graphViewer.ViewerGraph);
            //}
        }

        private void UpdateCommands()
        {
            //var graphGenerated = graphViewer.Graph != null;
            //var analisisRunning = _AnalisisThread != null;

            //buttonExportGraph.Enabled = graphGenerated && !analisisRunning;
            //buttonGenerateAssembly.Enabled = graphGenerated && !analisisRunning;
            //buttonPan.Enabled = graphGenerated;
            //buttonResetLayout.Enabled = graphGenerated;
            //buttonZoomBestFit.Enabled = graphGenerated;
            //buttonZoomIn.Enabled = graphGenerated;
            //buttonZoomOut.Enabled = graphGenerated;
            //buttonRedo.Enabled = false;
            //buttonUndo.Enabled = false;
        }

        private async void LoadAssembly(string fileName)
        {
            BeginInvoke((Action) delegate
            {
                var name = Path.GetFileName(fileName);
                StartBackgroundTask("Loading assembly {0}...", name);
            });

            var assembly = decompiler.Decompile(fileName, null);
            await Task.Run(() => typesViewerPresenter.ShowTypes(assembly));

            BeginInvoke((Action) delegate
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