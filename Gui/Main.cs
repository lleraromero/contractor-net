using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Contracts;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.Drawing.Text;
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
using Contractor.Utils;
using Microsoft.Cci;
using Microsoft.Msagl.Drawing;
using Microsoft.Msagl.GraphViewerGdi;
using Action = System.Action;
using Color = System.Drawing.Color;

namespace Contractor.Gui
{
    internal partial class Main : Form
    {
        private Thread _AnalisisThread;
        private INamedTypeDefinition _AnalizedType;
        private readonly AssemblyInfo _AssemblyInfo;
        private CancellationTokenSource _cancellationSource;
        private string _ContractReferenceAssemblyFileName;
        private EpaGenerator _EpaGenerator;
        private Graph _Graph;
        private TypeAnalysisResult _LastResult;
        private Options _Options;
        private IViewerNode _SelectedGraphNode;

        public Main()
        {
            InitializeComponent();

            graphViewer.OutsideAreaBrush = Brushes.White;
            splitcontainerOutput.Panel2Collapsed = true;
            treeviewTypes.Sorted = true;
            treeviewTypes.ShowPlusMinus = true;
            cmbBackend.Items.Add("CodeContracts");
            cmbBackend.Items.Add("Corral");
            cmbBackend.SelectedIndex = 1;

            var host = new PeReader.DefaultHost();
            _AssemblyInfo = new AssemblyInfo(host);
            _Options = new Options();
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

        private void OnAbout(object sender, EventArgs e)
        {
            var aboutDialog = new AboutDialog();
            aboutDialog.ShowDialog(this);
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

        private void OnTreeNodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        {
            if (!buttonStartAnalysis.Enabled) return;
            StartAnalisis();
        }

        private void OnAfterSelectTreeNode(object sender, TreeViewEventArgs e)
        {
            UpdateStartAnalisisCommand();
            listboxMethods.Items.Clear();
            toolstripMethods.Enabled = false;

            if (e.Node.Tag is INamedTypeDefinition)
            {
                toolstripMethods.Enabled = true;

                var selectedType = e.Node.Tag as INamedTypeDefinition;
                LoadTypeMethods(selectedType);
            }
        }

        private void OnCheckAllMethods(object sender, EventArgs e)
        {
            for (var i = 0; i < listboxMethods.Items.Count; ++i)
                listboxMethods.SetItemChecked(i, true);
        }

        private void OnUncheckAllMethods(object sender, EventArgs e)
        {
            for (var i = 0; i < listboxMethods.Items.Count; ++i)
                listboxMethods.SetItemChecked(i, false);
        }

        private void OnGraphChanged(object sender, EventArgs e)
        {
            UpdateCommands();
        }

        private void OnLoadAssembly(object sender, EventArgs e)
        {
            var result = loadAssemblyDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = loadAssemblyDialog.FileName;
                loadAssemblyDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                UnloadAssembly();
                Task.Factory.StartNew(() => LoadAssembly(fileName));
            }
        }

        private void OnLoadContracts(object sender, EventArgs e)
        {
            var result = loadContractsDialog.ShowDialog(this);

            if (result == DialogResult.OK)
            {
                var fileName = loadContractsDialog.FileName;
                loadContractsDialog.InitialDirectory = Path.GetDirectoryName(fileName);

                _ContractReferenceAssemblyFileName = fileName;

                buttonLoadContracts.Checked = true;

                var name = Path.GetFileName(fileName);
                SetBackgroundStatus("Using contract reference assembly {0}", name);
            }
        }

        private void OnStartAnalysis(object sender, EventArgs e)
        {
            StartAnalisis();
        }

        private void OnStopAnalysis(object sender, EventArgs e)
        {
            if (_AnalisisThread != null && _AnalisisThread.IsAlive)
            {
                _cancellationSource.Cancel();

                buttonStopAnalysis.Enabled = false;

                var typeFullName = _AnalizedType.GetDisplayName();
                statusLabel.Text = string.Format("Aborting analysis for {0}...", typeFullName);
            }
        }

        private void OnZoomIn(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.ZoomInPressed();
        }

        private void OnZoomOut(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.ZoomOutPressed();
        }

        private void OnZoomBestFit(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.FitGraphBoundingBox();
            graphViewer.ZoomF = 1.0;
        }

        private void OnResetLayout(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.Graph = graphViewer.Graph;
        }

        private void OnPan(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.PanButtonPressed = !graphViewer.PanButtonPressed;

            buttonPan.Checked = graphViewer.PanButtonPressed;
        }

        private void OnUndo(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.Undo();

            buttonUndo.Enabled = graphViewer.CanUndo();

            buttonRedo.Enabled = true;
        }

        private void OnRedo(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;
            graphViewer.Redo();

            buttonUndo.Enabled = true;

            buttonRedo.Enabled = graphViewer.CanRedo();
        }

        private void OnExportGraph(object sender, EventArgs e)
        {
            if (graphViewer.Graph == null) return;

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
            if (graphViewer.Graph == null) return;
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

            var n = _Graph.AddNode(e.EpaAndState.Item2.Name);

            n.UserData = e.EpaAndState;
            n.DrawNodeDelegate += OnDrawNode;
            n.Attr.Shape = Shape.Circle;
            n.Attr.LabelMargin = 7;
            n.Label.FontName = "Cambria";
            n.Label.FontSize = 6;

            if (_Options.StateDescription)
            {
                n.LabelText = string.Join(Environment.NewLine, e.EpaAndState.Item2.ToString());
            }
            else
            {
                n.LabelText = string.Format("S{0}", _Graph.NodeCount);
            }

            UpdateAnalysisProgress();
        }

        private void OnTransitionAdded(object sender, TransitionAddedEventArgs e)
        {
            if (InvokeRequired)
            {
                BeginInvoke(new EventHandler<TransitionAddedEventArgs>(OnTransitionAdded), sender, e);
                return;
            }

            var label = e.Transition.Action.ToString();
            var createEdge = true;
            var lineStyle = e.Transition.IsUnproven ? Style.Dashed : Style.Solid;

            if (_Options.CollapseTransitions)
            {
                var n = _Graph.FindNode(e.SourceState.Name);

                if (_Options.UnprovenTransitions && e.Transition.IsUnproven)
                    label = string.Format("{0}?", label);

                if (n != null)
                {
                    var edges = n.OutEdges.Union(n.SelfEdges);

                    foreach (var ed in edges)
                    {
                        if (ed.Target == e.Transition.TargetState.Name && ed.Attr.Styles.Contains(lineStyle))
                        {
                            ed.LabelText = string.Format("{0}{1}{2}", ed.LabelText, Environment.NewLine, label);
                            createEdge = false;
                            break;
                        }
                    }
                }
            }

            if (createEdge)
            {
                var edge = _Graph.AddEdge(e.SourceState.Name, label, e.Transition.TargetState.Name);

                edge.Label.FontName = "Cambria";
                edge.Label.FontSize = 6;
                edge.Attr.AddStyle(lineStyle);
            }

            UpdateAnalysisProgress();
        }

        private bool OnDrawNode(Node node, object graphics)
        {
            var g = graphics as Graphics;
            var w = node.Attr.Width;
            var h = node.Attr.Height;
            var x = node.Attr.Pos.X - w/2.0;
            var y = node.Attr.Pos.Y - h/2.0;

            g.FillEllipse(Brushes.AliceBlue, (float) x, (float) y, (float) w, (float) h);

            var penWidth = _SelectedGraphNode != null && _SelectedGraphNode.Node == node ? 2f : 1f;
            using (var pen = new Pen(Color.Black, penWidth))
                g.DrawEllipse(pen, (float) x, (float) y, (float) w, (float) h);

            var epaAndState = node.UserData as Tuple<EpaBuilder, State>;
            if (epaAndState.Item1.Initial.Equals(epaAndState.Item2))
            {
                const double offset = 3.1;
                x += offset/2.0;
                y += offset/2.0;
                w -= offset;
                h -= offset;

                g.DrawEllipse(Pens.Black, (float) x, (float) y, (float) w, (float) h);
            }

            using (var m = g.Transform)
            using (var saveM = m.Clone())
            {
                var c = (float) (2.0*node.Label.Center.Y);
                x = node.Label.Center.X;
                y = node.Label.Center.Y;

                using (var m2 = new Matrix(1f, 0f, 0f, -1f, 0f, c))
                    m.Multiply(m2);

                g.Transform = m;

                using (var font = new Font(node.Label.FontName, node.Label.FontSize))
                using (var format = new StringFormat(StringFormat.GenericTypographic))
                {
                    format.Alignment = StringAlignment.Center;
                    format.LineAlignment = StringAlignment.Center;

                    g.DrawString(node.LabelText, font, Brushes.Black, (float) x, (float) y, format);
                }

                g.Transform = saveM;
            }

            return true;
        }

        private void OnNodeMarkedForDragging(object sender, EventArgs e)
        {
            _SelectedGraphNode = sender as IViewerNode;
            var state = _SelectedGraphNode.Node.UserData as Tuple<EpaBuilder, State>;
            var info = GetStateInfo(state.Item1, state.Item2);

            richtextboxInformation.Rtf = info;
            titlebarProperties.Text = "State Info";
            listboxMethods.Visible = false;
            toolstripMethods.Visible = false;
        }

        private void OnNodeUnmarkedForDragging(object sender, EventArgs e)
        {
            _SelectedGraphNode = null;

            titlebarProperties.Text = "Methods";
            toolstripMethods.Visible = true;
            listboxMethods.Visible = true;
            richtextboxInformation.Clear();

            OnObjectUnmarkedForDragging(sender, e);
        }

        private void OnObjectUnmarkedForDragging(object sender, EventArgs e)
        {
            buttonUndo.Enabled = graphViewer.CanUndo();
            buttonRedo.Enabled = graphViewer.CanRedo();
        }

        #endregion Event Handlers

        #region Private Methods

        private void StartAnalisis()
        {
            var parameters = GetParameters();

            _AnalizedType = treeviewTypes.SelectedNode.Tag as INamedTypeDefinition;

            var backend = (string) parameters["backend"];

            var decompiler = new CciDecompiler();
            var inputAssembly = decompiler.Decompile(_AssemblyInfo.FileName, _ContractReferenceAssemblyFileName);
            var typeFullName = TypeHelper.GetTypeName(_AnalizedType, NameFormattingOptions.None);
            var typeToAnalyze = inputAssembly.Types().First(t => t.Equals(typeFullName));
            _cancellationSource = new CancellationTokenSource();

            IAnalyzer analyzer = null;
            switch (backend)
            {
                case "CodeContracts":
                    throw new NotImplementedException();
                case "Corral":
                    analyzer = new CorralAnalyzer(decompiler.CreateQueryGenerator(), inputAssembly as CciAssembly, _AssemblyInfo.FileName,
                        typeToAnalyze, _cancellationSource.Token);
                    break;
                default:
                    throw new NotSupportedException();
            }

            _EpaGenerator = new EpaGenerator(inputAssembly, analyzer);
            _EpaGenerator.StateAdded += OnStateAdded;
            _EpaGenerator.TransitionAdded += OnTransitionAdded;

            _AnalisisThread = new Thread(GenerateGraph);
            _AnalisisThread.Name = "GenerateGraph";
            _AnalisisThread.IsBackground = true;
            _AnalisisThread.Start();
        }

        private Dictionary<string, object> GetParameters()
        {
            var parameters = new Dictionary<string, object>();
            parameters.Add("backend", cmbBackend.SelectedItem);

            return parameters;
        }

        private void GenerateGraph()
        {
            BeginInvoke(new Action(UpdateAnalysisInitialize));
            try
            {
                var fileName = Path.GetFileName(_AssemblyInfo.FileName);
                BeginInvoke(new System.Action<string, string>(SetBackgroundStatus), "Decompiling assembly {0}...", fileName);

                if (_ContractReferenceAssemblyFileName != null)
                {
                    fileName = Path.GetFileName(_ContractReferenceAssemblyFileName);
                    BeginInvoke(new System.Action<string, string>(SetBackgroundStatus), "Loading contracts from assembly {0}...", fileName);
                }

                var typeFullName = TypeHelper.GetTypeName(_AnalizedType, NameFormattingOptions.None);
                var selectedMethods = listboxMethods.CheckedItems.Cast<string>();

                BeginInvoke(new System.Action<string, string>(SetBackgroundStatus), "Generating contractor graph for {0}...", typeFullName);
                var typeAnalysisResult = _EpaGenerator.GenerateEpa(typeFullName, selectedMethods);
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
            var typeFullName = _AnalizedType.GetDisplayName();
            StartBackgroundTask("Initializing analysis for {0}...", typeFullName);

            _Graph = new Graph();
            _Graph.Attr.OptimizeLabelPositions = true;
            _Graph.Attr.LayerDirection = LayerDirection.LR;
            graphViewer.Graph = _Graph;

            _SelectedGraphNode = null;
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

            var typeFullName = _AnalizedType.GetDisplayName();

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

            _AnalisisThread = null;
            _Graph = null;

            buttonStopAnalysis.Enabled = false;
            buttonLoadAssembly.Enabled = true;
            buttonLoadContracts.Enabled = true;
            buttonExportGraph.Enabled = true;
            buttonGenerateAssembly.Enabled = true;

            UpdateStartAnalisisCommand();
        }

        private void OutputTypeAnalysisResult(TypeAnalysisResult analysisResult)
        {
            textboxOutput.AppendText(analysisResult.ToString());
            textboxOutput.AppendText(Environment.NewLine);
        }

        private void UpdateAnalysisProgress()
        {
            var typeFullName = _AnalizedType.GetDisplayName();
            var msg = "Performing analysis for {0}: {1} states, {2} transitions";
            statusLabel.Text = string.Format(msg, typeFullName, _Graph.NodeCount, _Graph.EdgeCount);

            //_Graph.Attr.AspectRatio = (double)graphViewer.ClientSize.Width / graphViewer.ClientSize.Height;
            graphViewer.Graph = _Graph;
            graphViewer.Enabled = true;

            foreach (var obj in graphViewer.Entities)
            {
                if (obj is IViewerNode)
                {
                    obj.MarkedForDraggingEvent += OnNodeMarkedForDragging;
                    obj.UnmarkedForDraggingEvent += OnNodeUnmarkedForDragging;
                }
                else
                {
                    obj.UnmarkedForDraggingEvent += OnObjectUnmarkedForDragging;
                }
            }
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
            using (var sw = File.CreateText(fileName))
            {
                var typeFullName = _AnalizedType.GetDisplayName();
                var nodes = graphViewer.Graph.GeometryGraph.CollectAllNodes();
                var initialNodes = nodes.Where(n => epa.Initial.Equals((n.UserData as Node).UserData as State));
                var otherNodes = nodes.Where(n => !epa.Initial.Equals((n.UserData as Node).UserData as State));

                sw.WriteLine("digraph \"{0}\"", typeFullName);
                sw.WriteLine("{");
                sw.WriteLine("\trankdir=LR;");
                sw.WriteLine("\tnode [style = filled, fillcolor = aliceblue, fontname = \"{0}\"];", "Cambria");

                sw.WriteLine();
                sw.WriteLine("\tnode [shape = doublecircle];");

                foreach (var n in initialNodes)
                {
                    var node = n.UserData as Node;
                    var name = node.LabelText.Replace(sw.NewLine, @"\n");

                    sw.WriteLine("\tnode [label = \"{0}\"]; \"{1}\";", name, node.Id);
                }

                sw.WriteLine();
                sw.WriteLine("\tnode [shape = circle];");

                foreach (var n in otherNodes)
                {
                    var node = n.UserData as Node;
                    var name = node.LabelText.Replace(sw.NewLine, @"\n");

                    sw.WriteLine("\tnode [label = \"{0}\"]; \"{1}\";", name, node.Id);
                }

                sw.WriteLine();
                sw.WriteLine("\tedge [fontname = \"{0}\"];", "Cambria");
                sw.WriteLine();

                foreach (var edge in graphViewer.Graph.Edges)
                {
                    var from = edge.SourceNode.Id;
                    var to = edge.TargetNode.Id;
                    var label = edge.LabelText.Replace(sw.NewLine, @"\n");

                    sw.WriteLine("\tedge [label = \"{0}\"] \"{1}\" -> \"{2}\";", label, from, to);
                }

                sw.WriteLine("}");
            }
        }

        private void ExportVectorGraph(string fileName)
        {
            var scale = 6.0f;
            var w = (int) (graphViewer.Graph.Width*scale);
            var h = (int) (graphViewer.Graph.Height*scale);

            using (var temp = CreateGraphics())
            {
                var hdc = temp.GetHdc();

                using (var img = new Metafile(fileName, hdc, EmfType.EmfOnly))
                {
                    temp.ReleaseHdc(hdc);

                    using (var g = Graphics.FromImage(img))
                    {
                        g.SmoothingMode = SmoothingMode.HighQuality;
                        g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
                        g.CompositingQuality = CompositingQuality.HighQuality;
                        g.InterpolationMode = InterpolationMode.HighQualityBicubic;

                        DrawGraph(g, w, h, scale);
                    }
                }
            }
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
            var graph = graphViewer.Graph;

            var num1 = (float) (0.5*w - scale*(graph.Left + 0.5*graph.Width));
            var num2 = (float) (0.5*h + scale*(graph.Bottom + 0.5*graph.Height));

            using (var brush = new SolidBrush(Draw.MsaglColorToDrawingColor(graph.Attr.BackgroundColor)))
                g.FillRectangle(brush, 0, 0, w, h);

            using (var matrix = new Matrix(scale, 0f, 0f, -scale, num1, num2))
            {
                g.Transform = matrix;
                Draw.DrawPrecalculatedLayoutObject(g, graphViewer.ViewerGraph);
            }
        }

        private void UpdateCommands()
        {
            var graphGenerated = graphViewer.Graph != null;
            var analisisRunning = _AnalisisThread != null;

            buttonExportGraph.Enabled = graphGenerated && !analisisRunning;
            buttonGenerateAssembly.Enabled = graphGenerated && !analisisRunning;
            buttonPan.Enabled = graphGenerated;
            buttonResetLayout.Enabled = graphGenerated;
            buttonZoomBestFit.Enabled = graphGenerated;
            buttonZoomIn.Enabled = graphGenerated;
            buttonZoomOut.Enabled = graphGenerated;
            buttonRedo.Enabled = false;
            buttonUndo.Enabled = false;
        }

        private void UpdateStartAnalisisCommand()
        {
            var selectedNode = treeviewTypes.SelectedNode;
            var isClassNode = selectedNode != null && selectedNode.Tag is INamedTypeDefinition;
            var analisisRunning = _AnalisisThread != null;

            buttonStartAnalysis.Enabled = isClassNode && !analisisRunning;
        }

        private void LoadAssembly(string fileName)
        {
            BeginInvoke((Action) delegate
            {
                var name = Path.GetFileName(fileName);
                StartBackgroundTask("Loading assembly {0}...", name);
            });

            _AssemblyInfo.Load(fileName);
            var root = LoadAssemblyTypes();

            BeginInvoke((Action) delegate
            {
                treeviewTypes.BeginUpdate();
                treeviewTypes.Nodes.Add(root);
                treeviewTypes.EndUpdate();

                buttonLoadContracts.Enabled = true;

                buttonLoadContracts.Checked = false;

                EndBackgroundTask();
            });
        }

        private void UnloadAssembly()
        {
            treeviewTypes.Nodes.Clear();
            listboxMethods.Items.Clear();
            graphViewer.Enabled = false;
            graphViewer.Graph = null;
            _ContractReferenceAssemblyFileName = null;
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

        private TreeNode LoadAssemblyTypes()
        {
            var namespaces = new Dictionary<INamespaceDefinition, TreeNode>();
            var types = _AssemblyInfo.Module.GetAnalyzableTypes();
            var assemblyNode = new TreeNode
            {
                Text = _AssemblyInfo.Module.Name.Value,
                ImageKey = "assembly",
                SelectedImageKey = "assembly"
            };

            foreach (var type in types)
            {
                TreeNode namespaceNode;
                var containingNamespace = type.ContainingUnitNamespace;

                if (namespaces.ContainsKey(containingNamespace))
                {
                    namespaceNode = namespaces[containingNamespace];
                }
                else
                {
                    var namespaceName = containingNamespace.ToString();
                    namespaceNode = CreateTreeNode(assemblyNode, namespaceName, "namespace");
                    namespaces.Add(containingNamespace, namespaceNode);
                }

                var typeName = type.GetDisplayName();
                var typeNode = CreateTreeNode(namespaceNode, typeName, "class");
                typeNode.Tag = type;

                if (!type.IsPublic)
                {
                    typeNode.ForeColor = Color.Gray;
                }
            }

            return assemblyNode;
        }

        private void LoadTypeMethods(INamedTypeDefinition type)
        {
            var methods = type.GetPublicInstanceMethods();
            listboxMethods.BeginUpdate();

            foreach (var method in methods)
            {
                var name = method.GetDisplayName();
                listboxMethods.Items.Add(name, true);
            }

            listboxMethods.EndUpdate();
        }

        private TreeNode CreateTreeNode(TreeNode rootNode, string name, string image)
        {
            var node = rootNode.Nodes.Add(name);
            node.ImageKey = image;
            node.SelectedImageKey = image;
            return node;
        }

        #endregion Private Methods
    }
}