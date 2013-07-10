using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Msagl.Drawing;
using EnvDTE;
using EnvDTE80;
using Microsoft.VisualStudio.Shell;
using System.IO;
using Contractor.Core;
using System.ComponentModel.Design;
using System.Threading;
using System.Drawing.Imaging;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using Microsoft.Msagl.GraphViewerGdi;
using System.Xml;
using Contractor.VSExtension.Properties;
using System.Diagnostics;

namespace Contractor.VSExtension
{
	public partial class ContractorExplorerControl : UserControl
	{
		private static string environmentInfo;
		private readonly static DTE2 dte;
		private readonly static SolutionEvents solutionEvents;
		private readonly static ProjectItemsEvents solutionItemsEvents;
		private readonly static DocumentEvents documentEvents;

		private MenuCommand cmdRefresh;
		private MenuCommand cmdStopAnalysis;
		private MenuCommand cmdZoomIn;
		private MenuCommand cmdZoomOut;
		private MenuCommand cmdZoomBestFit;
		private MenuCommand cmdPan;
		private MenuCommand cmdExportGraph;
		private MenuCommand cmdGenerateOutputAssembly;
		private MenuCommand cmdResetLayout;
		private MenuCommand cmdUndo;
		private MenuCommand cmdRedo;

		private System.Threading.Thread thread;
		private EpaGenerator generator;
		private string typeFullName;
		private Graph graph;
		private IViewerNode selectedNode;

		static ContractorExplorerControl()
		{
			dte = Package.GetGlobalService(typeof(DTE)) as DTE2;
			dte.SuppressUI = false;

			solutionEvents = dte.Events.SolutionEvents;
			solutionItemsEvents = dte.Events.SolutionItemsEvents;
			documentEvents = dte.Events.DocumentEvents;
		}

		public ContractorExplorerControl()
		{
			InitializeComponent();
			this.splitcontainerV.SplitterWidth = 3;
		}

		private void ContractorExplorerControl_Load(object sender, EventArgs e)
		{
			graphViewer.OutsideAreaBrush = System.Drawing.Brushes.White;

			fillProjects();

			solutionEvents.Opened += solutionEvents_Opened;
			solutionEvents.AfterClosing += solutionEvents_AfterClosing;
			solutionEvents.ProjectAdded += solutionEvents_ProjectAdded;
			solutionEvents.ProjectRemoved += solutionEvents_ProjectRemoved;
			solutionEvents.ProjectRenamed += solutionEvents_ProjectRenamed;

			solutionItemsEvents.ItemAdded += solutionItemsEvents_ItemAdded;
			solutionItemsEvents.ItemRemoved += solutionItemsEvents_ItemRemoved;
			solutionItemsEvents.ItemRenamed += solutionItemsEvents_ItemRenamed;

			documentEvents.DocumentSaved += documentEvents_DocumentSaved;
		}

		public Options Options { get; set; }

		public void InitializeToolBar(OleMenuCommandService mcs)
		{
			if (mcs == null) return;

			CommandID commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.RefreshButton);
			cmdRefresh = new MenuCommand(toolbar_refresh, commandId);
			mcs.AddCommand(cmdRefresh);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.StopAnalysisButton);
			cmdStopAnalysis = new MenuCommand(toolbar_stopAnalysis, commandId);
			cmdStopAnalysis.Enabled = false;
			mcs.AddCommand(cmdStopAnalysis);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ZoomInButton);
			cmdZoomIn = new MenuCommand(toolbar_zoomIn, commandId);
			cmdZoomIn.Enabled = false;
			mcs.AddCommand(cmdZoomIn);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ZoomOutButton);
			cmdZoomOut = new MenuCommand(toolbar_zoomOut, commandId);
			cmdZoomOut.Enabled = false;
			mcs.AddCommand(cmdZoomOut);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ZoomBestFitButton);
			cmdZoomBestFit = new MenuCommand(toolbar_zoomBestFit, commandId);
			cmdZoomBestFit.Enabled = false;
			mcs.AddCommand(cmdZoomBestFit);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.PanButton);
			cmdPan = new MenuCommand(toolbar_pan, commandId);
			cmdPan.Enabled = false;
			mcs.AddCommand(cmdPan);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ExportGraphButton);
			cmdExportGraph = new MenuCommand(toolbar_exportGraph, commandId);
			cmdExportGraph.Enabled = false;
			mcs.AddCommand(cmdExportGraph);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.GenerateOutputAssemblyButton);
			cmdGenerateOutputAssembly = new MenuCommand(toolbar_generateOutputAssembly, commandId);
			cmdGenerateOutputAssembly.Enabled = false;
			mcs.AddCommand(cmdGenerateOutputAssembly);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.ResetLayoutButton);
			cmdResetLayout = new MenuCommand(toolbar_resetLayout, commandId);
			cmdResetLayout.Enabled = false;
			mcs.AddCommand(cmdResetLayout);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.UndoButton);
			cmdUndo = new MenuCommand(toolbar_undo, commandId);
			cmdUndo.Enabled = false;
			mcs.AddCommand(cmdUndo);

			commandId = new CommandID(GuidList.guidVSExtensionCmdSet, PkgCmdIDList.RedoButton);
			cmdRedo = new MenuCommand(toolbar_redo, commandId);
			cmdRedo.Enabled = false;
			mcs.AddCommand(cmdRedo);
		}

		#region DTE EventHandlers

		private void solutionEvents_Opened()
		{
			fillProjects();
		}

		private void solutionEvents_AfterClosing()
		{
			stopAnalysis();
			treeviewTypes.Nodes.Clear();
			graphViewer.Graph = null;
			graphViewer.Enabled = false;

			cmdExportGraph.Enabled = false;
			cmdGenerateOutputAssembly.Enabled = false;
			cmdPan.Enabled = false;
			cmdResetLayout.Enabled = false;
			cmdZoomBestFit.Enabled = false;
			cmdZoomIn.Enabled = false;
			cmdZoomOut.Enabled = false;
			cmdRedo.Enabled = false;
			cmdUndo.Enabled = false;

			if (generator != null)
			{
				generator.Dispose();
				generator = null;
			}
		}

		private void solutionEvents_ProjectAdded(Project Project)
		{
			fillProjects();
		}

		private void solutionEvents_ProjectRemoved(Project Project)
		{
			fillProjects();
		}

		private void solutionEvents_ProjectRenamed(Project Project, string OldName)
		{
			fillProjects();
		}

		private void solutionItemsEvents_ItemAdded(ProjectItem ProjectItem)
		{
			fillProjects();
		}

		private void solutionItemsEvents_ItemRemoved(ProjectItem ProjectItem)
		{
			fillProjects();
		}

		private void solutionItemsEvents_ItemRenamed(ProjectItem ProjectItem, string OldName)
		{
			fillProjects();
		}

		private void documentEvents_DocumentSaved(Document Document)
		{
			fillProjects();
		}

		#endregion

		private void fillProjects()
		{
			treeviewTypes.Nodes.Clear();

			for (int i = 1; i <= dte.Solution.Projects.Count; ++i)
			{
				var prj = dte.Solution.Projects.Item(i);

				if (prj.Kind == Constants.vsProjectKindSolutionItems ||
					prj.Kind == Constants.vsProjectKindUnmodeled ||
					prj.Kind == Constants.vsProjectKindMisc)
					continue;

				var node = treeviewTypes.Nodes.Add(prj.Name);

				switch (prj.CodeModel.Language)
				{
					case CodeModelLanguageConstants.vsCMLanguageCSharp:
						node.ImageKey = "csharpProject";
						break;

					case CodeModelLanguageConstants.vsCMLanguageVB:
						node.ImageKey = "vbProject";
						break;

					case CodeModelLanguageConstants.vsCMLanguageVC:
					case CodeModelLanguageConstants.vsCMLanguageMC:
						node.ImageKey = "vcProject";
						break;

					default:
						node.ImageKey = "unknownProject";
						break;
				}

				node.SelectedImageKey = node.ImageKey;
				node.StateImageKey = "collapsed";

				for (int j = 1; j <= prj.ProjectItems.Count; ++j)
				{
					var codemodel = prj.ProjectItems.Item(j).FileCodeModel;

					if (codemodel == null)
						continue;

					fillNamespaces(codemodel.CodeElements, node);
				}
			}
		}

		private void fillNamespaces(CodeElements elements, TreeNode node)
		{
			for (int i = 1; i <= elements.Count; ++i)
			{
				var name = elements.Item(i) as CodeNamespace;

				if (name == null)
					continue;

				fillNamespaces(name.Members, node);
				fillTypes(name.Members, node);
			}
		}

		private void fillTypes(CodeElements elements, TreeNode node)
		{
			for (int i = 1; i <= elements.Count; ++i)
			{
				var type = elements.Item(i) as CodeClass;

				if (type == null)
					continue;

				TreeNode child;
				string name = type.Name;

				if (node.Nodes.ContainsKey(type.Namespace.FullName))
				{
					child = node.Nodes[type.Namespace.FullName];
				}
				else
				{
					child = node.Nodes.Add(type.Namespace.FullName, type.Namespace.FullName);
					child.ImageKey = "namespace";
					child.SelectedImageKey = child.ImageKey;
					child.StateImageKey = "collapsed";
				}

				if (type.Parent is CodeClass)
					name = string.Format("{0}.{1}", (type.Parent as CodeClass).Name, type.Name);
				
				child = child.Nodes.Add(name);
				child.ImageKey = "class";
				child.SelectedImageKey = child.ImageKey;
				child.StateImageKey = "none";
				child.Tag = type;

				fillTypes(type.Members, node);
			}
		}

		private void treeviewTypes_BeforeCollapse(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.StateImageKey = "collapsed";
		}

		private void treeviewTypes_BeforeExpand(object sender, TreeViewCancelEventArgs e)
		{
			e.Node.StateImageKey = "expanded";
		}

		private void treeviewTypes_NodeMouseClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			Rectangle rect = e.Node.Bounds;
			rect.Width = imagelist.ImageSize.Width;
			rect.X -= imagelist.ImageSize.Width * 2 + 3;

			if (rect.Contains(e.Location))
				e.Node.Toggle();
		}

		private void treeviewTypes_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
		{
			CodeClass type = e.Node.Tag as CodeClass;
			if (type == null) return;

			stopAnalysis();

			Project prj = type.ProjectItem.ContainingProject;
			string fullPath = prj.Properties.Item("FullPath").Value.ToString();
			string outputFileName = prj.Properties.Item("OutputFileName").Value.ToString();
			string outputPath = prj.ConfigurationManager.ActiveConfiguration.Properties.Item("OutputPath").Value.ToString();
			string cfgName = dte.Solution.SolutionBuild.ActiveConfiguration.Name;

			dte.Solution.SolutionBuild.BuildProject(cfgName, prj.UniqueName, true);

			if (dte.ToolWindows.ErrorList.ErrorItems.Count > 0)
			{
				var msg = string.Format("Build for project {0} failed.", prj.Name);
				MessageBox.Show(msg, Resources.ToolWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
				return;
			}

			typeFullName = type.FullName;
			exportGraphDialog.FileName = string.Format("{0}.png", typeFullName);
			generateOutputDialog.FileName = outputFileName;

			//Obtenemos el output original por si el code contracts le borra los contratos
			var outputWithContractsFileName = Path.Combine(fullPath, outputPath.Replace("bin", "obj"), "Decl", outputFileName);
			outputFileName = Path.Combine(fullPath, outputPath, outputFileName);

			if (File.Exists(outputWithContractsFileName))
				outputFileName = outputWithContractsFileName;

			if (!File.Exists(outputFileName))
			{
				var msg = string.Format("Can not find the assembly:\n{0}", outputFileName);
				MessageBox.Show(msg, Resources.ToolWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
				return;
			}

			if (generator != null)
				generator.Dispose();

			generator = new EpaGenerator();
			generator.LoadAssembly(outputFileName);
			generator.TypeAnalysisStarted += typeAnalysisStarted;
			generator.TypeAnalysisDone += typeAnalysisDone;
			generator.StateAdded += stateAdded;
			generator.TransitionAdded += transitionAdded;

			thread = new System.Threading.Thread(generateGraph);
			thread.Name = "generateGraph";
			thread.IsBackground = true;
			thread.Start();
		}

		private void generateGraph()
		{
			try
			{
				generator.GenerateEpa(typeFullName);
			}
			catch (ThreadAbortException)
			{
				this.Invoke(new Action<TypeAnalysisResult>(updateAnalysisEnd), (object)null);
			}
			catch (Exception ex)
			{
				this.Invoke(new Action<Exception>(HandleException), ex);
				this.Invoke(new Action<TypeAnalysisResult>(updateAnalysisEnd), (object)null);
			}
			finally
			{
				generator.UnloadAssembly();
			}
		}

		private void typeAnalysisStarted(object sender, TypeAnalysisStartedEventArgs e)
		{
			this.Invoke(new Action(updateAnalysisStart));
		}

		private void typeAnalysisDone(object sender, TypeAnalysisDoneEventArgs e)
		{
			this.Invoke(new Action<TypeAnalysisResult>(updateAnalysisEnd), e.AnalysisResult);
		}

		private void stateAdded(object sender, StateAddedEventArgs e)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new EventHandler<StateAddedEventArgs>(stateAdded), sender, e);
				return;
			}

			var n = graph.AddNode(e.State.Name);

			n.UserData = e.State;
			n.DrawNodeDelegate += OnDrawNode;
			n.Attr.Shape = Shape.Circle;
			n.Attr.LabelMargin = 7;
			n.Label.FontName = "Cambria";
			n.Label.FontSize = 6;

			if (this.Options.StateDescription)
			{
				n.LabelText = string.Join<string>(Environment.NewLine, e.State.EnabledActions);
			}
			else
			{
				n.LabelText = string.Format("S{0}", graph.NodeCount);
			}

			updateAnalysisProgress();
		}

		private void transitionAdded(object sender, TransitionAddedEventArgs e)
		{
			if (this.InvokeRequired)
			{
				this.Invoke(new EventHandler<TransitionAddedEventArgs>(transitionAdded), sender, e);
				return;
			}

			var label = e.Transition.Name;
			bool createEdge = true;

			if (this.Options.CollapseTransitions)
			{
				var n = graph.FindNode(e.Transition.SourceState.Name);

				if (this.Options.UnprovenTransitions && e.Transition.IsUnproven)
					label = string.Format("{0}?", label);

				if (n != null)
				{
					var edges = n.OutEdges.Union(n.SelfEdges);

					foreach (var ed in edges)
						if (ed.Target == e.Transition.TargetState.Name)
						{
							ed.LabelText = string.Format("{0}{1}{2}", ed.LabelText, Environment.NewLine, label);
							createEdge = false;
							break;
						}
				}
			}

			if (createEdge)
			{
				var edge = graph.AddEdge(e.Transition.SourceState.Name, label, e.Transition.TargetState.Name);

				edge.Label.FontName = "Cambria";
				edge.Label.FontSize = 6;
			}

			updateAnalysisProgress();
		}

		private bool OnDrawNode(Node node, object graphics)
		{
			var g = graphics as Graphics;
			double w = node.Attr.Width;
			double h = node.Attr.Height;
			double x = node.Attr.Pos.X - (w / 2.0);
			double y = node.Attr.Pos.Y - (h / 2.0);

			g.FillEllipse(Brushes.AliceBlue, (float)x, (float)y, (float)w, (float)h);

			float penWidth = (selectedNode != null && selectedNode.Node == node ? 2f : 1f);
			using (Pen pen = new Pen(System.Drawing.Color.Black, penWidth))
				g.DrawEllipse(pen, (float)x, (float)y, (float)w, (float)h);

			if ((node.UserData as IState).IsInitial)
			{
				const double offset = 3.1;
				x += offset / 2.0;
				y += offset / 2.0;
				w -= offset;
				h -= offset;

				g.DrawEllipse(Pens.Black, (float)x, (float)y, (float)w, (float)h);
			}

			using (Matrix m = g.Transform)
			using (Matrix saveM = m.Clone())
			{
				float c = (float)(2.0 * node.Label.Center.Y);
				x = node.Label.Center.X;
				y = node.Label.Center.Y;

				using (Matrix m2 = new Matrix(1f, 0f, 0f, -1f, 0f, c))
					m.Multiply(m2);

				g.Transform = m;

				using (Font font = new Font(node.Label.FontName, node.Label.FontSize))
				using (StringFormat format = new StringFormat(StringFormat.GenericTypographic))
				{
					format.Alignment = StringAlignment.Center;
					format.LineAlignment = StringAlignment.Center;

					g.DrawString(node.LabelText, font, Brushes.Black, (float)x, (float)y, format);
				}

				g.Transform = saveM;
			}

			return true;
		}

		private void saveStart(string fileName)
		{
			dte.StatusBar.Text = string.Format("Saving {0}...", fileName);
			dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationSave);
		}

		private void saveEnd(string fileName)
		{
			dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationSave);
			dte.StatusBar.Text = string.Format("{0} saved", fileName);
		}

		private void updateAnalysisStart()
		{
			dte.StatusBar.Text = string.Format("Generating contractor graph for {0}", typeFullName);
			dte.StatusBar.Animate(true, vsStatusAnimation.vsStatusAnimationGeneral);

			this.graph = new Graph();
			this.graph.Attr.OptimizeLabelPositions = true;
			this.graph.Attr.LayerDirection = LayerDirection.LR;
			graphViewer.Graph = this.graph;
			progressbar.Visible = true;

			selectedNode = null;
			richtextboxInformation.Clear();

			cmdStopAnalysis.Enabled = true;
		}

		private void updateAnalysisEnd(TypeAnalysisResult analysisResult)
		{
			var msg = string.Format("Analysis for {0}", typeFullName);

			if (analysisResult == null)
			{
				msg = string.Format("{0} aborted", msg);
			}
			else
			{
				var seconds = Math.Ceiling(analysisResult.TotalDuration.TotalSeconds);
				var statesCount = analysisResult.States.Count;
				var transitionsCount = analysisResult.Transitions.Count;

				msg = string.Format("{0} done in {1} seconds: {2} states, {3} transitions", msg, seconds, statesCount, transitionsCount);
			}

			dte.StatusBar.Animate(false, vsStatusAnimation.vsStatusAnimationGeneral);
			dte.StatusBar.Text = msg;

			this.thread = null;
			this.graph = null;
			progressbar.Visible = false;

			cmdStopAnalysis.Enabled = false;
		}

		private void updateAnalysisProgress()
		{
			var msg = "Performing analysis for {0}: {1} states, {2} transitions";
			dte.StatusBar.Text = string.Format(msg, typeFullName, graph.NodeCount, graph.EdgeCount);

			//graph.Attr.AspectRatio = (double)graphViewer.ClientSize.Width / graphViewer.ClientSize.Height;
			graphViewer.Graph = this.graph;
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

		private void OnNodeMarkedForDragging(object sender, EventArgs e)
		{
			selectedNode = sender as IViewerNode;
			var state = selectedNode.Node.UserData as IState;
			var info = new StringBuilder();
			info.Append(@"{\rtf1\ansi\fs18");

			if (state.IsInitial)
			{
				info.Append(@" \b Initial State \b0 \fs8\par\par\fs18");
			}

			if (state.EnabledActions.Any())
			{
				info.Append(@" \b Enabled Actions \b0 \par ");
				var text = string.Join<string>(@" \par ", state.EnabledActions);
				info.Append(text);
			}

			if (state.DisabledActions.Any())
			{
				info.Append(@" \fs8\par\par\fs18 \b Disabled Actions \b0 \par ");
				var text = string.Join<string>(@" \par ", state.DisabledActions);
				info.Append(text);
			}

			info.Append(@"}");
			richtextboxInformation.Rtf = info.ToString();
		}

		private void OnNodeUnmarkedForDragging(object sender, EventArgs e)
		{
			selectedNode = null;
			richtextboxInformation.Clear();

			this.OnObjectUnmarkedForDragging(sender, e);
		}

		private void OnObjectUnmarkedForDragging(object sender, EventArgs e)
		{
			cmdUndo.Enabled = graphViewer.CanUndo();
			cmdRedo.Enabled = graphViewer.CanRedo();
		}

		private void toolbar_refresh(object sender, EventArgs e)
		{
			fillProjects();
		}

		private void toolbar_stopAnalysis(object sender, EventArgs e)
		{
			stopAnalysis();
		}

		private void stopAnalysis()
		{
			if (this.thread != null && this.thread.IsAlive)
				this.thread.Abort();
		}

		private void toolbar_zoomIn(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.ZoomInPressed();
		}

		private void toolbar_zoomOut(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.ZoomOutPressed();
		}

		private void toolbar_zoomBestFit(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.FitGraphBoundingBox();
			graphViewer.ZoomF = 1.0;
		}

		private void toolbar_resetLayout(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.Graph = graphViewer.Graph;
		}

		private void toolbar_pan(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.PanButtonPressed = !graphViewer.PanButtonPressed;
			cmdPan.Checked = graphViewer.PanButtonPressed;
		}

		private void toolbar_undo(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.Undo();
			cmdUndo.Enabled = graphViewer.CanUndo();
			cmdRedo.Enabled = true;
		}

		private void toolbar_redo(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			graphViewer.Redo();
			cmdUndo.Enabled = true;
			cmdRedo.Enabled = graphViewer.CanRedo();
		}

		private void toolbar_exportGraph(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			var result = exportGraphDialog.ShowDialog(this);

			if (result == DialogResult.OK)
			{
				var fileName = exportGraphDialog.FileName;
				saveStart(fileName);

				var ext = Path.GetExtension(fileName);

				switch (ext.ToLower())
				{
					case ".xml":
						exportXmlGraph(fileName);
						break;

					case ".gv":
						exportGraphvizGraph(fileName);
						break;

					case ".emf":
					case ".wmf":
						exportVectorGraph(fileName);
						break;

					default:
						exportImageGraph(fileName);
						break;
				}

				saveEnd(fileName);
			}
		}

		private void exportXmlGraph(string fileName)
		{
			using (XmlTextWriter xml = new XmlTextWriter(fileName, Encoding.UTF8))
			{
				var nodes = graphViewer.Graph.GeometryGraph.CollectAllNodes();

				xml.Formatting = Formatting.Indented;
				xml.WriteStartDocument();
				xml.WriteStartElement("graph");
				xml.WriteAttributeString("type", typeFullName);
			    xml.WriteStartElement("states");

			    foreach (var n in nodes)
				{
					var node = n.UserData as Node;
					var state = node.UserData as IState;
					var name = node.LabelText.Replace(Environment.NewLine, @"\n");

					xml.WriteStartElement("state");
					xml.WriteAttributeString("name", node.Id);

					if (state.IsInitial)
					{
						xml.WriteAttributeString("initial", "true");
					}

					xml.WriteString(name);
					xml.WriteEndElement();
				}

				xml.WriteEndElement();
				xml.WriteStartElement("transitions");

				foreach (var edge in graphViewer.Graph.Edges)
				{
					var from = edge.SourceNode.Id;
					var to = edge.TargetNode.Id;
					var label = edge.LabelText.Replace(Environment.NewLine, @"\n");

					xml.WriteStartElement("transition");
					xml.WriteAttributeString("from", from);
					xml.WriteAttributeString("to", to);
					xml.WriteString(label);
					xml.WriteEndElement();
				}

			    xml.WriteEndElement();
				xml.WriteEndElement();
			}
		}

		private void exportGraphvizGraph(string fileName)
		{
			using (var sw = File.CreateText(fileName))
			{
				var nodes = graphViewer.Graph.GeometryGraph.CollectAllNodes();
				var initialNodes = nodes.Where(n => ((n.UserData as Node).UserData as IState).IsInitial);
				var otherNodes = nodes.Where(n => !((n.UserData as Node).UserData as IState).IsInitial);

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

		private void exportVectorGraph(string fileName)
		{
			float scale = 6.0f;
			var w = (int)(graphViewer.Graph.Width * scale);
			var h = (int)(graphViewer.Graph.Height * scale);

			using (Graphics temp = base.CreateGraphics())
			{
				IntPtr hdc = temp.GetHdc();

				using (Metafile img = new Metafile(fileName, hdc, EmfType.EmfOnly))
				{
					temp.ReleaseHdc(hdc);
					
					using (Graphics g = Graphics.FromImage(img))
					{
						g.SmoothingMode = SmoothingMode.HighQuality;
						g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
						g.CompositingQuality = CompositingQuality.HighQuality;
						g.InterpolationMode = InterpolationMode.HighQualityBicubic;

						drawGraph(g, w, h, scale);
					}
				}
			}
		}

		private void exportImageGraph(string fileName)
		{
			float scale = 6.0f;
			var w = (int)(graphViewer.Graph.Width * scale);
			var h = (int)(graphViewer.Graph.Height * scale);

			using (Image img = new Bitmap(w, h, PixelFormat.Format32bppPArgb))
			using (Graphics g = Graphics.FromImage(img))
			{
				g.SmoothingMode = SmoothingMode.HighQuality;
				g.TextRenderingHint = TextRenderingHint.AntiAliasGridFit;
				g.CompositingQuality = CompositingQuality.HighQuality;
				g.InterpolationMode = InterpolationMode.HighQualityBicubic;

				drawGraph(g, w, h, scale);
				img.Save(fileName);
			}
		}

		private void drawGraph(Graphics g, int w, int h, float scale)
		{
			var graph = graphViewer.Graph;

			float num1 = (float)((0.5 * w) - (scale * (graph.Left + (0.5 * graph.Width))));
			float num2 = (float)((0.5 * h) + (scale * (graph.Bottom + (0.5 * graph.Height))));

			using (SolidBrush brush = new SolidBrush(Draw.MsaglColorToDrawingColor(graph.Attr.BackgroundColor)))
				g.FillRectangle(brush, 0, 0, w, h);

			using (Matrix matrix = new Matrix(scale, 0f, 0f, -scale, num1, num2))
			{
				g.Transform = matrix;
				Draw.DrawPrecalculatedLayoutObject(g, graphViewer.ViewerGraph);
			}
		}

		private void toolbar_generateOutputAssembly(object sender, EventArgs e)
		{
			if (graphViewer.Graph == null) return;
			var result = generateOutputDialog.ShowDialog(this);

			if (result == DialogResult.OK)
			{
				try
				{
					var fileName = generateOutputDialog.FileName;
					saveStart(fileName);
					generator.GenerateOutputAssembly(fileName);
					saveEnd(fileName);
				}
				catch (Exception ex)
				{
					HandleException(ex);
				}
				finally
				{
					generator.UnloadAssembly();
				}
			}
		}

		private void graphViewer_GraphChanged(object sender, EventArgs e)
		{
			bool enabled = (graphViewer.Graph != null);

			cmdExportGraph.Enabled = enabled;
			cmdGenerateOutputAssembly.Enabled = enabled;
			cmdPan.Enabled = enabled;
			cmdResetLayout.Enabled = enabled;
			cmdZoomBestFit.Enabled = enabled;
			cmdZoomIn.Enabled = enabled;
			cmdZoomOut.Enabled = enabled;

			cmdRedo.Enabled = false;
			cmdUndo.Enabled = false;
		}

		public static void HandleException(Exception ex)
		{
			OutputWindowPane output = null;

			try
			{
				// If the pane exists already, return it.
				output = dte.ToolWindows.OutputWindow.OutputWindowPanes.Item(Resources.ToolWindowTitle);
			}
			catch (ArgumentException)
			{
				// Create a new pane.
				output = dte.ToolWindows.OutputWindow.OutputWindowPanes.Add(Resources.ToolWindowTitle);
			}

			output.OutputString(getEnvironmentInfo());
			output.OutputString(ex.ToString());
			output.OutputString(Environment.NewLine);

			string msg = string.Format("{0}\n\nFor more information check the {1} output window.", ex.Message, Resources.ToolWindowTitle);
			MessageBox.Show(msg, Resources.ToolWindowTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
			output.Activate();
		}

		private static string getEnvironmentInfo()
		{
			if (string.IsNullOrEmpty(environmentInfo))
			{
				StringBuilder sb = new StringBuilder();

				sb.AppendLine("========================================");

				sb.Append("Operating System: ");
				sb.Append(Environment.OSVersion.ToString());
				sb.AppendLine(Environment.Is64BitOperatingSystem ? " (64 bits)" : " (32 bits)");

				sb.Append("CLR Version: ");
				sb.AppendLine(Environment.Version.ToString());

				//string visualStudioVersion = dte.RegistryRoot;
				//visualStudioVersion = visualStudioVersion.Substring(visualStudioVersion.LastIndexOf('\\') + 1);

				sb.Append("Visual Studio: ");
				sb.Append(dte.Edition);
				sb.Append(" Edition ");
				sb.AppendLine(Application.ProductVersion);

				string codeContractsVersion = "Code Contracts is not installed.";
				string checkerFileName = Contractor.Core.Configuration.CheckerFileName;

				if (!string.IsNullOrEmpty(checkerFileName))
				{
					var vi = FileVersionInfo.GetVersionInfo(checkerFileName);
					codeContractsVersion = vi.ProductVersion;
				}

				sb.Append("Code Contracts Version: ");
				sb.AppendLine(codeContractsVersion);

				var assemblyName = System.Reflection.Assembly.GetExecutingAssembly().GetName();

				sb.Append("Contractor.NET Version: ");
				sb.AppendLine(assemblyName.Version.ToString());

				sb.AppendLine("----------------------------------------");
				environmentInfo = sb.ToString();
			}

			return environmentInfo;
		}
	}
}
