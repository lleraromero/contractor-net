namespace Contractor.Gui
{
	partial class Main
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
		/// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
		protected override void Dispose(bool disposing)
		{
			if (disposing && (components != null))
			{
				components.Dispose();
			}
			base.Dispose(disposing);
		}

		#region Windows Form Designer generated code

		/// <summary>
		/// Required method for Designer support - do not modify
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Main));
			this.imagelist = new System.Windows.Forms.ImageList(this.components);
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.exportGraphDialog = new System.Windows.Forms.SaveFileDialog();
			this.generateOutputDialog = new System.Windows.Forms.SaveFileDialog();
			this.statusStrip = new System.Windows.Forms.StatusStrip();
			this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
			this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
			this.toolStrip = new System.Windows.Forms.ToolStrip();
			this.buttonOptions = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonLoadAssembly = new System.Windows.Forms.ToolStripButton();
			this.buttonExportGraph = new System.Windows.Forms.ToolStripButton();
			this.buttonGenerateAssembly = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonStartAnalysis = new System.Windows.Forms.ToolStripButton();
			this.buttonStopAnalysis = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonUndo = new System.Windows.Forms.ToolStripButton();
			this.buttonRedo = new System.Windows.Forms.ToolStripButton();
			this.toolStripSeparator3 = new System.Windows.Forms.ToolStripSeparator();
			this.buttonResetLayout = new System.Windows.Forms.ToolStripButton();
			this.buttonZoomIn = new System.Windows.Forms.ToolStripButton();
			this.buttonZoomOut = new System.Windows.Forms.ToolStripButton();
			this.buttonZoomBestFit = new System.Windows.Forms.ToolStripButton();
			this.buttonPan = new System.Windows.Forms.ToolStripButton();
			this.menuStrip = new System.Windows.Forms.MenuStrip();
			this.menuitemFile = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemLoadAssembly = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemExportGraph = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemGenerateAssembly = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator6 = new System.Windows.Forms.ToolStripSeparator();
			this.menuitemQuit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemEdit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemUndo = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemRedo = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemView = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemResetLayout = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemZoomIn = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemZoomOut = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemZoomBestFit = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemPan = new System.Windows.Forms.ToolStripMenuItem();
			this.toolStripSeparator7 = new System.Windows.Forms.ToolStripSeparator();
			this.menuitemOutput = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemOptions = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemAnalyze = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemStartAnalysis = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemStopAnalysis = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemHelp = new System.Windows.Forms.ToolStripMenuItem();
			this.menuitemAbout = new System.Windows.Forms.ToolStripMenuItem();
			this.loadAssemblyDialog = new System.Windows.Forms.OpenFileDialog();
			this.textboxOutput = new System.Windows.Forms.TextBox();
			this.splitcontainerV = new System.Windows.Forms.SplitContainer();
			this.splitcontainerH = new System.Windows.Forms.SplitContainer();
			this.treeviewTypes = new System.Windows.Forms.TreeView();
			this.richtextboxInformation = new System.Windows.Forms.RichTextBox();
			this.splitcontainerOutput = new System.Windows.Forms.SplitContainer();
			this.graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
			this.titlebarStateInfo = new Contractor.Gui.TitleBar();
			this.titlebarOutput = new Contractor.Gui.TitleBar();
			this.statusStrip.SuspendLayout();
			this.toolStrip.SuspendLayout();
			this.menuStrip.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerV)).BeginInit();
			this.splitcontainerV.Panel1.SuspendLayout();
			this.splitcontainerV.Panel2.SuspendLayout();
			this.splitcontainerV.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerH)).BeginInit();
			this.splitcontainerH.Panel1.SuspendLayout();
			this.splitcontainerH.Panel2.SuspendLayout();
			this.splitcontainerH.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerOutput)).BeginInit();
			this.splitcontainerOutput.Panel1.SuspendLayout();
			this.splitcontainerOutput.Panel2.SuspendLayout();
			this.splitcontainerOutput.SuspendLayout();
			this.SuspendLayout();
			// 
			// imagelist
			// 
			this.imagelist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelist.ImageStream")));
			this.imagelist.TransparentColor = System.Drawing.Color.White;
			this.imagelist.Images.SetKeyName(0, "assembly");
			this.imagelist.Images.SetKeyName(1, "namespace");
			this.imagelist.Images.SetKeyName(2, "class");
			this.imagelist.Images.SetKeyName(3, "collapsed");
			this.imagelist.Images.SetKeyName(4, "expanded");
			this.imagelist.Images.SetKeyName(5, "none");
			this.imagelist.Images.SetKeyName(6, "close");
			// 
			// exportGraphDialog
			// 
			this.exportGraphDialog.DefaultExt = "png";
			this.exportGraphDialog.Filter = "PNG Files (*.png)|*.png|JPG Files (*.jpg)|*.jpg|BMP Files (*.bmp)|*.bmp|GIF Files" +
				" (*.gif)|*.gif|EMF Files (*.emf)|*.emf|WMF Files (*.wmf)|*.wmf|XML Files (*.xml)" +
				"|*.xml|Graphviz Files (*.gv)|*.gv";
			this.exportGraphDialog.Title = "Export Graph...";
			// 
			// generateOutputDialog
			// 
			this.generateOutputDialog.DefaultExt = "dll";
			this.generateOutputDialog.Filter = "Dynamic Link Libraries (*.dll)|*.dll|Executable Files (*.exe)|*.exe";
			this.generateOutputDialog.Title = "Generate Assembly...";
			// 
			// statusStrip
			// 
			this.statusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.statusLabel,
            this.progressBar});
			this.statusStrip.Location = new System.Drawing.Point(0, 347);
			this.statusStrip.Name = "statusStrip";
			this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
			this.statusStrip.Size = new System.Drawing.Size(532, 22);
			this.statusStrip.TabIndex = 4;
			this.statusStrip.Text = "statusStrip1";
			// 
			// statusLabel
			// 
			this.statusLabel.Name = "statusLabel";
			this.statusLabel.Size = new System.Drawing.Size(517, 17);
			this.statusLabel.Spring = true;
			this.statusLabel.Text = "Ready";
			this.statusLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// progressBar
			// 
			this.progressBar.Name = "progressBar";
			this.progressBar.Size = new System.Drawing.Size(150, 16);
			this.progressBar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressBar.Visible = false;
			// 
			// toolStrip
			// 
			this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOptions,
            this.toolStripSeparator4,
            this.buttonLoadAssembly,
            this.buttonExportGraph,
            this.buttonGenerateAssembly,
            this.toolStripSeparator1,
            this.buttonStartAnalysis,
            this.buttonStopAnalysis,
            this.toolStripSeparator2,
            this.buttonUndo,
            this.buttonRedo,
            this.toolStripSeparator3,
            this.buttonResetLayout,
            this.buttonZoomIn,
            this.buttonZoomOut,
            this.buttonZoomBestFit,
            this.buttonPan});
			this.toolStrip.Location = new System.Drawing.Point(0, 24);
			this.toolStrip.Name = "toolStrip";
			this.toolStrip.Size = new System.Drawing.Size(532, 25);
			this.toolStrip.TabIndex = 1;
			// 
			// buttonOptions
			// 
			this.buttonOptions.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonOptions.Image = ((System.Drawing.Image)(resources.GetObject("buttonOptions.Image")));
			this.buttonOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonOptions.Name = "buttonOptions";
			this.buttonOptions.Size = new System.Drawing.Size(23, 22);
			this.buttonOptions.Text = "Options";
			this.buttonOptions.Click += new System.EventHandler(this.OnOptions);
			// 
			// toolStripSeparator4
			// 
			this.toolStripSeparator4.Name = "toolStripSeparator4";
			this.toolStripSeparator4.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonLoadAssembly
			// 
			this.buttonLoadAssembly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonLoadAssembly.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoadAssembly.Image")));
			this.buttonLoadAssembly.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonLoadAssembly.Name = "buttonLoadAssembly";
			this.buttonLoadAssembly.Size = new System.Drawing.Size(23, 22);
			this.buttonLoadAssembly.Text = "Load Assembly";
			this.buttonLoadAssembly.Click += new System.EventHandler(this.OnLoadAssembly);
			// 
			// buttonExportGraph
			// 
			this.buttonExportGraph.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonExportGraph.Enabled = false;
			this.buttonExportGraph.Image = ((System.Drawing.Image)(resources.GetObject("buttonExportGraph.Image")));
			this.buttonExportGraph.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonExportGraph.Name = "buttonExportGraph";
			this.buttonExportGraph.Size = new System.Drawing.Size(23, 22);
			this.buttonExportGraph.Text = "Export Graph";
			this.buttonExportGraph.Click += new System.EventHandler(this.OnExportGraph);
			// 
			// buttonGenerateAssembly
			// 
			this.buttonGenerateAssembly.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonGenerateAssembly.Enabled = false;
			this.buttonGenerateAssembly.Image = ((System.Drawing.Image)(resources.GetObject("buttonGenerateAssembly.Image")));
			this.buttonGenerateAssembly.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonGenerateAssembly.Name = "buttonGenerateAssembly";
			this.buttonGenerateAssembly.Size = new System.Drawing.Size(23, 22);
			this.buttonGenerateAssembly.Text = "Generate Assembly";
			this.buttonGenerateAssembly.Click += new System.EventHandler(this.OnGenerateAssembly);
			// 
			// toolStripSeparator1
			// 
			this.toolStripSeparator1.Name = "toolStripSeparator1";
			this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonStartAnalysis
			// 
			this.buttonStartAnalysis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonStartAnalysis.Enabled = false;
			this.buttonStartAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("buttonStartAnalysis.Image")));
			this.buttonStartAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonStartAnalysis.Name = "buttonStartAnalysis";
			this.buttonStartAnalysis.Size = new System.Drawing.Size(23, 22);
			this.buttonStartAnalysis.Text = "Start Analysis";
			this.buttonStartAnalysis.Click += new System.EventHandler(this.OnStartAnalysis);
			// 
			// buttonStopAnalysis
			// 
			this.buttonStopAnalysis.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonStopAnalysis.Enabled = false;
			this.buttonStopAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("buttonStopAnalysis.Image")));
			this.buttonStopAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonStopAnalysis.Name = "buttonStopAnalysis";
			this.buttonStopAnalysis.Size = new System.Drawing.Size(23, 22);
			this.buttonStopAnalysis.Text = "Stop Analysis";
			this.buttonStopAnalysis.Click += new System.EventHandler(this.OnStopAnalysis);
			// 
			// toolStripSeparator2
			// 
			this.toolStripSeparator2.Name = "toolStripSeparator2";
			this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonUndo
			// 
			this.buttonUndo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonUndo.Enabled = false;
			this.buttonUndo.Image = ((System.Drawing.Image)(resources.GetObject("buttonUndo.Image")));
			this.buttonUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonUndo.Name = "buttonUndo";
			this.buttonUndo.Size = new System.Drawing.Size(23, 22);
			this.buttonUndo.Text = "Undo";
			this.buttonUndo.Click += new System.EventHandler(this.OnUndo);
			// 
			// buttonRedo
			// 
			this.buttonRedo.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonRedo.Enabled = false;
			this.buttonRedo.Image = ((System.Drawing.Image)(resources.GetObject("buttonRedo.Image")));
			this.buttonRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonRedo.Name = "buttonRedo";
			this.buttonRedo.Size = new System.Drawing.Size(23, 22);
			this.buttonRedo.Text = "Redo";
			this.buttonRedo.Click += new System.EventHandler(this.OnRedo);
			// 
			// toolStripSeparator3
			// 
			this.toolStripSeparator3.Name = "toolStripSeparator3";
			this.toolStripSeparator3.Size = new System.Drawing.Size(6, 25);
			// 
			// buttonResetLayout
			// 
			this.buttonResetLayout.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonResetLayout.Enabled = false;
			this.buttonResetLayout.Image = ((System.Drawing.Image)(resources.GetObject("buttonResetLayout.Image")));
			this.buttonResetLayout.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonResetLayout.Name = "buttonResetLayout";
			this.buttonResetLayout.Size = new System.Drawing.Size(23, 22);
			this.buttonResetLayout.Text = "Reset Layout";
			this.buttonResetLayout.Click += new System.EventHandler(this.OnResetLayout);
			// 
			// buttonZoomIn
			// 
			this.buttonZoomIn.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonZoomIn.Enabled = false;
			this.buttonZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomIn.Image")));
			this.buttonZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonZoomIn.Name = "buttonZoomIn";
			this.buttonZoomIn.Size = new System.Drawing.Size(23, 22);
			this.buttonZoomIn.Text = "Zoom In";
			this.buttonZoomIn.Click += new System.EventHandler(this.OnZoomIn);
			// 
			// buttonZoomOut
			// 
			this.buttonZoomOut.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonZoomOut.Enabled = false;
			this.buttonZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomOut.Image")));
			this.buttonZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonZoomOut.Name = "buttonZoomOut";
			this.buttonZoomOut.Size = new System.Drawing.Size(23, 22);
			this.buttonZoomOut.Text = "Zoom Out";
			this.buttonZoomOut.Click += new System.EventHandler(this.OnZoomOut);
			// 
			// buttonZoomBestFit
			// 
			this.buttonZoomBestFit.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonZoomBestFit.Enabled = false;
			this.buttonZoomBestFit.Image = ((System.Drawing.Image)(resources.GetObject("buttonZoomBestFit.Image")));
			this.buttonZoomBestFit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonZoomBestFit.Name = "buttonZoomBestFit";
			this.buttonZoomBestFit.Size = new System.Drawing.Size(23, 22);
			this.buttonZoomBestFit.Text = "Zoom Best Fit";
			this.buttonZoomBestFit.Click += new System.EventHandler(this.OnZoomBestFit);
			// 
			// buttonPan
			// 
			this.buttonPan.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
			this.buttonPan.Enabled = false;
			this.buttonPan.Image = ((System.Drawing.Image)(resources.GetObject("buttonPan.Image")));
			this.buttonPan.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.buttonPan.Name = "buttonPan";
			this.buttonPan.Size = new System.Drawing.Size(23, 22);
			this.buttonPan.Text = "Pan";
			this.buttonPan.Click += new System.EventHandler(this.OnPan);
			// 
			// menuStrip
			// 
			this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemFile,
            this.menuitemEdit,
            this.menuitemView,
            this.menuitemAnalyze,
            this.menuitemHelp});
			this.menuStrip.Location = new System.Drawing.Point(0, 0);
			this.menuStrip.Name = "menuStrip";
			this.menuStrip.Size = new System.Drawing.Size(532, 24);
			this.menuStrip.TabIndex = 0;
			this.menuStrip.Text = "menuStrip1";
			// 
			// menuitemFile
			// 
			this.menuitemFile.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemLoadAssembly,
            this.menuitemExportGraph,
            this.menuitemGenerateAssembly,
            this.toolStripSeparator6,
            this.menuitemQuit});
			this.menuitemFile.Name = "menuitemFile";
			this.menuitemFile.Size = new System.Drawing.Size(37, 20);
			this.menuitemFile.Text = "&File";
			// 
			// menuitemLoadAssembly
			// 
			this.menuitemLoadAssembly.Image = ((System.Drawing.Image)(resources.GetObject("menuitemLoadAssembly.Image")));
			this.menuitemLoadAssembly.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemLoadAssembly.Name = "menuitemLoadAssembly";
			this.menuitemLoadAssembly.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.O)));
			this.menuitemLoadAssembly.Size = new System.Drawing.Size(256, 22);
			this.menuitemLoadAssembly.Text = "&Load Assembly...";
			this.menuitemLoadAssembly.Click += new System.EventHandler(this.OnLoadAssembly);
			// 
			// menuitemExportGraph
			// 
			this.menuitemExportGraph.Enabled = false;
			this.menuitemExportGraph.Image = ((System.Drawing.Image)(resources.GetObject("menuitemExportGraph.Image")));
			this.menuitemExportGraph.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemExportGraph.Name = "menuitemExportGraph";
			this.menuitemExportGraph.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.S)));
			this.menuitemExportGraph.Size = new System.Drawing.Size(256, 22);
			this.menuitemExportGraph.Text = "&Export Graph...";
			this.menuitemExportGraph.Click += new System.EventHandler(this.OnExportGraph);
			// 
			// menuitemGenerateAssembly
			// 
			this.menuitemGenerateAssembly.Enabled = false;
			this.menuitemGenerateAssembly.Image = ((System.Drawing.Image)(resources.GetObject("menuitemGenerateAssembly.Image")));
			this.menuitemGenerateAssembly.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemGenerateAssembly.Name = "menuitemGenerateAssembly";
			this.menuitemGenerateAssembly.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Shift)
						| System.Windows.Forms.Keys.S)));
			this.menuitemGenerateAssembly.Size = new System.Drawing.Size(256, 22);
			this.menuitemGenerateAssembly.Text = "&Generate Assembly...";
			this.menuitemGenerateAssembly.Click += new System.EventHandler(this.OnGenerateAssembly);
			// 
			// toolStripSeparator6
			// 
			this.toolStripSeparator6.Name = "toolStripSeparator6";
			this.toolStripSeparator6.Size = new System.Drawing.Size(253, 6);
			// 
			// menuitemQuit
			// 
			this.menuitemQuit.Name = "menuitemQuit";
			this.menuitemQuit.ShortcutKeyDisplayString = "";
			this.menuitemQuit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Alt | System.Windows.Forms.Keys.F4)));
			this.menuitemQuit.Size = new System.Drawing.Size(256, 22);
			this.menuitemQuit.Text = "&Quit";
			this.menuitemQuit.Click += new System.EventHandler(this.OnQuit);
			// 
			// menuitemEdit
			// 
			this.menuitemEdit.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemUndo,
            this.menuitemRedo});
			this.menuitemEdit.Name = "menuitemEdit";
			this.menuitemEdit.Size = new System.Drawing.Size(39, 20);
			this.menuitemEdit.Text = "&Edit";
			// 
			// menuitemUndo
			// 
			this.menuitemUndo.Enabled = false;
			this.menuitemUndo.Image = ((System.Drawing.Image)(resources.GetObject("menuitemUndo.Image")));
			this.menuitemUndo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemUndo.Name = "menuitemUndo";
			this.menuitemUndo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Z)));
			this.menuitemUndo.Size = new System.Drawing.Size(144, 22);
			this.menuitemUndo.Text = "&Undo";
			this.menuitemUndo.Click += new System.EventHandler(this.OnUndo);
			// 
			// menuitemRedo
			// 
			this.menuitemRedo.Enabled = false;
			this.menuitemRedo.Image = ((System.Drawing.Image)(resources.GetObject("menuitemRedo.Image")));
			this.menuitemRedo.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemRedo.Name = "menuitemRedo";
			this.menuitemRedo.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Y)));
			this.menuitemRedo.Size = new System.Drawing.Size(144, 22);
			this.menuitemRedo.Text = "&Redo";
			this.menuitemRedo.Click += new System.EventHandler(this.OnRedo);
			// 
			// menuitemView
			// 
			this.menuitemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemResetLayout,
            this.menuitemZoomIn,
            this.menuitemZoomOut,
            this.menuitemZoomBestFit,
            this.menuitemPan,
            this.toolStripSeparator7,
            this.menuitemOutput,
            this.menuitemOptions});
			this.menuitemView.Name = "menuitemView";
			this.menuitemView.Size = new System.Drawing.Size(44, 20);
			this.menuitemView.Text = "&View";
			// 
			// menuitemResetLayout
			// 
			this.menuitemResetLayout.Enabled = false;
			this.menuitemResetLayout.Image = ((System.Drawing.Image)(resources.GetObject("menuitemResetLayout.Image")));
			this.menuitemResetLayout.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemResetLayout.Name = "menuitemResetLayout";
			this.menuitemResetLayout.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
			this.menuitemResetLayout.Size = new System.Drawing.Size(210, 22);
			this.menuitemResetLayout.Text = "&Reset Layout";
			this.menuitemResetLayout.Click += new System.EventHandler(this.OnResetLayout);
			// 
			// menuitemZoomIn
			// 
			this.menuitemZoomIn.Enabled = false;
			this.menuitemZoomIn.Image = ((System.Drawing.Image)(resources.GetObject("menuitemZoomIn.Image")));
			this.menuitemZoomIn.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemZoomIn.Name = "menuitemZoomIn";
			this.menuitemZoomIn.ShortcutKeyDisplayString = "Ctrl++";
			this.menuitemZoomIn.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Oemplus)));
			this.menuitemZoomIn.Size = new System.Drawing.Size(210, 22);
			this.menuitemZoomIn.Text = "Zoom &In";
			this.menuitemZoomIn.Click += new System.EventHandler(this.OnZoomIn);
			// 
			// menuitemZoomOut
			// 
			this.menuitemZoomOut.Enabled = false;
			this.menuitemZoomOut.Image = ((System.Drawing.Image)(resources.GetObject("menuitemZoomOut.Image")));
			this.menuitemZoomOut.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemZoomOut.Name = "menuitemZoomOut";
			this.menuitemZoomOut.ShortcutKeyDisplayString = "Ctrl+-";
			this.menuitemZoomOut.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.OemMinus)));
			this.menuitemZoomOut.Size = new System.Drawing.Size(210, 22);
			this.menuitemZoomOut.Text = "Zoom &Out";
			this.menuitemZoomOut.Click += new System.EventHandler(this.OnZoomOut);
			// 
			// menuitemZoomBestFit
			// 
			this.menuitemZoomBestFit.Enabled = false;
			this.menuitemZoomBestFit.Image = ((System.Drawing.Image)(resources.GetObject("menuitemZoomBestFit.Image")));
			this.menuitemZoomBestFit.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemZoomBestFit.Name = "menuitemZoomBestFit";
			this.menuitemZoomBestFit.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.F)));
			this.menuitemZoomBestFit.Size = new System.Drawing.Size(210, 22);
			this.menuitemZoomBestFit.Text = "Zoom Best &Fit";
			this.menuitemZoomBestFit.Click += new System.EventHandler(this.OnZoomBestFit);
			// 
			// menuitemPan
			// 
			this.menuitemPan.Enabled = false;
			this.menuitemPan.Image = ((System.Drawing.Image)(resources.GetObject("menuitemPan.Image")));
			this.menuitemPan.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemPan.Name = "menuitemPan";
			this.menuitemPan.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.P)));
			this.menuitemPan.Size = new System.Drawing.Size(210, 22);
			this.menuitemPan.Text = "&Pan";
			this.menuitemPan.Click += new System.EventHandler(this.OnPan);
			// 
			// toolStripSeparator7
			// 
			this.toolStripSeparator7.Name = "toolStripSeparator7";
			this.toolStripSeparator7.Size = new System.Drawing.Size(207, 6);
			// 
			// menuitemOutput
			// 
			this.menuitemOutput.CheckOnClick = true;
			this.menuitemOutput.Name = "menuitemOutput";
			this.menuitemOutput.ShortcutKeys = ((System.Windows.Forms.Keys)(((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Alt)
						| System.Windows.Forms.Keys.O)));
			this.menuitemOutput.Size = new System.Drawing.Size(210, 22);
			this.menuitemOutput.Text = "O&utput Panel";
			this.menuitemOutput.Click += new System.EventHandler(this.OnOutput);
			// 
			// menuitemOptions
			// 
			this.menuitemOptions.Image = ((System.Drawing.Image)(resources.GetObject("menuitemOptions.Image")));
			this.menuitemOptions.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemOptions.Name = "menuitemOptions";
			this.menuitemOptions.Size = new System.Drawing.Size(210, 22);
			this.menuitemOptions.Text = "&Op&tions...";
			this.menuitemOptions.Click += new System.EventHandler(this.OnOptions);
			// 
			// menuitemAnalyze
			// 
			this.menuitemAnalyze.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemStartAnalysis,
            this.menuitemStopAnalysis});
			this.menuitemAnalyze.Name = "menuitemAnalyze";
			this.menuitemAnalyze.Size = new System.Drawing.Size(60, 20);
			this.menuitemAnalyze.Text = "&Analyze";
			// 
			// menuitemStartAnalysis
			// 
			this.menuitemStartAnalysis.Enabled = false;
			this.menuitemStartAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("menuitemStartAnalysis.Image")));
			this.menuitemStartAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemStartAnalysis.Name = "menuitemStartAnalysis";
			this.menuitemStartAnalysis.ShortcutKeys = System.Windows.Forms.Keys.F5;
			this.menuitemStartAnalysis.Size = new System.Drawing.Size(163, 22);
			this.menuitemStartAnalysis.Text = "&Start Analysis";
			this.menuitemStartAnalysis.Click += new System.EventHandler(this.OnStartAnalysis);
			// 
			// menuitemStopAnalysis
			// 
			this.menuitemStopAnalysis.Enabled = false;
			this.menuitemStopAnalysis.Image = ((System.Drawing.Image)(resources.GetObject("menuitemStopAnalysis.Image")));
			this.menuitemStopAnalysis.ImageTransparentColor = System.Drawing.Color.Magenta;
			this.menuitemStopAnalysis.Name = "menuitemStopAnalysis";
			this.menuitemStopAnalysis.ShortcutKeys = System.Windows.Forms.Keys.F6;
			this.menuitemStopAnalysis.Size = new System.Drawing.Size(163, 22);
			this.menuitemStopAnalysis.Text = "S&top Analysis";
			this.menuitemStopAnalysis.Click += new System.EventHandler(this.OnStopAnalysis);
			// 
			// menuitemHelp
			// 
			this.menuitemHelp.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemAbout});
			this.menuitemHelp.Name = "menuitemHelp";
			this.menuitemHelp.Size = new System.Drawing.Size(44, 20);
			this.menuitemHelp.Text = "&Help";
			// 
			// menuitemAbout
			// 
			this.menuitemAbout.Name = "menuitemAbout";
			this.menuitemAbout.Size = new System.Drawing.Size(201, 22);
			this.menuitemAbout.Text = "&About Contractor.NET...";
			this.menuitemAbout.Click += new System.EventHandler(this.OnAbout);
			// 
			// loadAssemblyDialog
			// 
			this.loadAssemblyDialog.DefaultExt = "dll";
			this.loadAssemblyDialog.Filter = "Dynamic Link Libraries (*.dll)|*.dll|Executable Files (*.exe)|*.exe";
			this.loadAssemblyDialog.Title = "Load Assembly...";
			// 
			// textboxOutput
			// 
			this.textboxOutput.BackColor = System.Drawing.SystemColors.Window;
			this.textboxOutput.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.textboxOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.textboxOutput.Font = new System.Drawing.Font("Consolas", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.textboxOutput.Location = new System.Drawing.Point(0, 19);
			this.textboxOutput.Multiline = true;
			this.textboxOutput.Name = "textboxOutput";
			this.textboxOutput.ReadOnly = true;
			this.textboxOutput.ScrollBars = System.Windows.Forms.ScrollBars.Both;
			this.textboxOutput.Size = new System.Drawing.Size(329, 124);
			this.textboxOutput.TabIndex = 1;
			this.textboxOutput.WordWrap = false;
			// 
			// splitcontainerV
			// 
			this.splitcontainerV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitcontainerV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitcontainerV.Location = new System.Drawing.Point(0, 49);
			this.splitcontainerV.Name = "splitcontainerV";
			// 
			// splitcontainerV.Panel1
			// 
			this.splitcontainerV.Panel1.Controls.Add(this.splitcontainerH);
			this.splitcontainerV.Panel1MinSize = 150;
			// 
			// splitcontainerV.Panel2
			// 
			this.splitcontainerV.Panel2.Controls.Add(this.splitcontainerOutput);
			this.splitcontainerV.Panel2MinSize = 200;
			this.splitcontainerV.Size = new System.Drawing.Size(532, 298);
			this.splitcontainerV.SplitterDistance = 197;
			this.splitcontainerV.TabIndex = 0;
			this.splitcontainerV.TabStop = false;
			// 
			// splitcontainerH
			// 
			this.splitcontainerH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitcontainerH.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitcontainerH.Location = new System.Drawing.Point(0, 0);
			this.splitcontainerH.Name = "splitcontainerH";
			this.splitcontainerH.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitcontainerH.Panel1
			// 
			this.splitcontainerH.Panel1.Controls.Add(this.treeviewTypes);
			// 
			// splitcontainerH.Panel2
			// 
			this.splitcontainerH.Panel2.Controls.Add(this.richtextboxInformation);
			this.splitcontainerH.Panel2.Controls.Add(this.titlebarStateInfo);
			this.splitcontainerH.Size = new System.Drawing.Size(197, 298);
			this.splitcontainerH.SplitterDistance = 149;
			this.splitcontainerH.TabIndex = 0;
			this.splitcontainerH.TabStop = false;
			// 
			// treeviewTypes
			// 
			this.treeviewTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeviewTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeviewTypes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.treeviewTypes.HideSelection = false;
			this.treeviewTypes.ImageIndex = 0;
			this.treeviewTypes.ImageList = this.imagelist;
			this.treeviewTypes.Indent = 18;
			this.treeviewTypes.Location = new System.Drawing.Point(0, 0);
			this.treeviewTypes.Name = "treeviewTypes";
			this.treeviewTypes.SelectedImageIndex = 0;
			this.treeviewTypes.ShowLines = false;
			this.treeviewTypes.ShowNodeToolTips = true;
			this.treeviewTypes.ShowPlusMinus = false;
			this.treeviewTypes.Size = new System.Drawing.Size(195, 147);
			this.treeviewTypes.StateImageList = this.imagelist;
			this.treeviewTypes.TabIndex = 0;
			this.treeviewTypes.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnBeforeCollapseTreeNode);
			this.treeviewTypes.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnBeforeExpandTreeNode);
			this.treeviewTypes.BeforeSelect += new System.Windows.Forms.TreeViewCancelEventHandler(this.OnBeforeSelectTreeNode);
			this.treeviewTypes.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeNodeMouseClick);
			this.treeviewTypes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeNodeMouseDoubleClick);
			// 
			// richtextboxInformation
			// 
			this.richtextboxInformation.BackColor = System.Drawing.SystemColors.Info;
			this.richtextboxInformation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richtextboxInformation.DetectUrls = false;
			this.richtextboxInformation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richtextboxInformation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.richtextboxInformation.Location = new System.Drawing.Point(0, 19);
			this.richtextboxInformation.Name = "richtextboxInformation";
			this.richtextboxInformation.ReadOnly = true;
			this.richtextboxInformation.ShowSelectionMargin = true;
			this.richtextboxInformation.Size = new System.Drawing.Size(195, 124);
			this.richtextboxInformation.TabIndex = 1;
			this.richtextboxInformation.TabStop = false;
			this.richtextboxInformation.Text = "";
			// 
			// splitcontainerOutput
			// 
			this.splitcontainerOutput.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitcontainerOutput.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitcontainerOutput.Location = new System.Drawing.Point(0, 0);
			this.splitcontainerOutput.Name = "splitcontainerOutput";
			this.splitcontainerOutput.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitcontainerOutput.Panel1
			// 
			this.splitcontainerOutput.Panel1.Controls.Add(this.graphViewer);
			// 
			// splitcontainerOutput.Panel2
			// 
			this.splitcontainerOutput.Panel2.Controls.Add(this.textboxOutput);
			this.splitcontainerOutput.Panel2.Controls.Add(this.titlebarOutput);
			this.splitcontainerOutput.Size = new System.Drawing.Size(331, 298);
			this.splitcontainerOutput.SplitterDistance = 149;
			this.splitcontainerOutput.TabIndex = 0;
			this.splitcontainerOutput.TabStop = false;
			// 
			// graphViewer
			// 
			this.graphViewer.AsyncLayout = false;
			this.graphViewer.AutoScroll = true;
			this.graphViewer.BackwardEnabled = false;
			this.graphViewer.BuildHitTree = true;
			this.graphViewer.CurrentLayoutMethod = Microsoft.Msagl.GraphViewerGdi.LayoutMethod.SugiyamaScheme;
			this.graphViewer.Dock = System.Windows.Forms.DockStyle.Fill;
			this.graphViewer.Enabled = false;
			this.graphViewer.ForwardEnabled = false;
			this.graphViewer.Graph = null;
			this.graphViewer.LayoutAlgorithmSettingsButtonVisible = true;
			this.graphViewer.LayoutEditingEnabled = true;
			this.graphViewer.Location = new System.Drawing.Point(0, 0);
			this.graphViewer.MouseHitDistance = 0.05D;
			this.graphViewer.Name = "graphViewer";
			this.graphViewer.NavigationVisible = true;
			this.graphViewer.NeedToCalculateLayout = true;
			this.graphViewer.PanButtonPressed = false;
			this.graphViewer.SaveAsImageEnabled = true;
			this.graphViewer.SaveAsMsaglEnabled = true;
			this.graphViewer.SaveButtonVisible = true;
			this.graphViewer.SaveGraphButtonVisible = true;
			this.graphViewer.SaveInVectorFormatEnabled = true;
			this.graphViewer.Size = new System.Drawing.Size(329, 147);
			this.graphViewer.TabIndex = 0;
			this.graphViewer.ToolBarIsVisible = false;
			this.graphViewer.ZoomF = 1D;
			this.graphViewer.ZoomFraction = 0.5D;
			this.graphViewer.ZoomWindowThreshold = 0.05D;
			this.graphViewer.GraphChanged += new System.EventHandler(this.OnGraphChanged);
			// 
			// titlebarStateInfo
			// 
			this.titlebarStateInfo.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.titlebarStateInfo.BackColorStyle = Contractor.Gui.BackColorStyle.Gradient;
			this.titlebarStateInfo.DarkBackColor = System.Drawing.SystemColors.ActiveCaption;
			this.titlebarStateInfo.Dock = System.Windows.Forms.DockStyle.Top;
			this.titlebarStateInfo.LightBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.titlebarStateInfo.Location = new System.Drawing.Point(0, 0);
			this.titlebarStateInfo.Name = "titlebarStateInfo";
			this.titlebarStateInfo.ShowBottomBorder = true;
			this.titlebarStateInfo.ShowCloseButton = false;
			this.titlebarStateInfo.Size = new System.Drawing.Size(195, 19);
			this.titlebarStateInfo.TabIndex = 0;
			this.titlebarStateInfo.TabStop = false;
			this.titlebarStateInfo.Text = "State Info";
			// 
			// titlebarOutput
			// 
			this.titlebarOutput.BackColor = System.Drawing.SystemColors.ActiveBorder;
			this.titlebarOutput.BackColorStyle = Contractor.Gui.BackColorStyle.Gradient;
			this.titlebarOutput.DarkBackColor = System.Drawing.SystemColors.ActiveCaption;
			this.titlebarOutput.Dock = System.Windows.Forms.DockStyle.Top;
			this.titlebarOutput.LightBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
			this.titlebarOutput.Location = new System.Drawing.Point(0, 0);
			this.titlebarOutput.Name = "titlebarOutput";
			this.titlebarOutput.ShowBottomBorder = true;
			this.titlebarOutput.ShowCloseButton = true;
			this.titlebarOutput.Size = new System.Drawing.Size(329, 19);
			this.titlebarOutput.TabIndex = 0;
			this.titlebarOutput.Text = "Output";
			this.titlebarOutput.Close += new System.EventHandler(this.OnOutputClose);
			// 
			// Main
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.ClientSize = new System.Drawing.Size(532, 369);
			this.Controls.Add(this.splitcontainerV);
			this.Controls.Add(this.toolStrip);
			this.Controls.Add(this.menuStrip);
			this.Controls.Add(this.statusStrip);
			this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
			this.MainMenuStrip = this.menuStrip;
			this.Name = "Main";
			this.Text = "Contractor.NET";
			this.Load += new System.EventHandler(this.OnLoad);
			this.statusStrip.ResumeLayout(false);
			this.statusStrip.PerformLayout();
			this.toolStrip.ResumeLayout(false);
			this.toolStrip.PerformLayout();
			this.menuStrip.ResumeLayout(false);
			this.menuStrip.PerformLayout();
			this.splitcontainerV.Panel1.ResumeLayout(false);
			this.splitcontainerV.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerV)).EndInit();
			this.splitcontainerV.ResumeLayout(false);
			this.splitcontainerH.Panel1.ResumeLayout(false);
			this.splitcontainerH.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerH)).EndInit();
			this.splitcontainerH.ResumeLayout(false);
			this.splitcontainerOutput.Panel1.ResumeLayout(false);
			this.splitcontainerOutput.Panel2.ResumeLayout(false);
			this.splitcontainerOutput.Panel2.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerOutput)).EndInit();
			this.splitcontainerOutput.ResumeLayout(false);
			this.ResumeLayout(false);
			this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.SaveFileDialog exportGraphDialog;
		private System.Windows.Forms.SaveFileDialog generateOutputDialog;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStrip toolStrip;
		private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem menuitemFile;
		private System.Windows.Forms.ToolStripMenuItem menuitemEdit;
		private System.Windows.Forms.ToolStripMenuItem menuitemHelp;
		private System.Windows.Forms.ToolStripMenuItem menuitemAbout;
		private System.Windows.Forms.ToolStripMenuItem menuitemQuit;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
		private System.Windows.Forms.ToolStripProgressBar progressBar;
		private System.Windows.Forms.ToolStripButton buttonStopAnalysis;
		private System.Windows.Forms.ToolStripButton buttonExportGraph;
		private System.Windows.Forms.ToolStripButton buttonGenerateAssembly;
		private System.Windows.Forms.ToolStripButton buttonUndo;
		private System.Windows.Forms.ToolStripButton buttonOptions;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ToolStripButton buttonRedo;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator3;
		private System.Windows.Forms.ToolStripButton buttonResetLayout;
		private System.Windows.Forms.ToolStripButton buttonZoomIn;
		private System.Windows.Forms.ToolStripButton buttonZoomOut;
		private System.Windows.Forms.ToolStripButton buttonZoomBestFit;
		private System.Windows.Forms.ToolStripButton buttonPan;
		private System.Windows.Forms.ImageList imagelist;
		private System.Windows.Forms.ToolStripButton buttonStartAnalysis;
		private System.Windows.Forms.ToolStripButton buttonLoadAssembly;
		private System.Windows.Forms.OpenFileDialog loadAssemblyDialog;
		private System.Windows.Forms.ToolStripMenuItem menuitemView;
		private System.Windows.Forms.ToolStripMenuItem menuitemOutput;
		private System.Windows.Forms.ToolStripMenuItem menuitemOptions;
		private System.Windows.Forms.ToolStripMenuItem menuitemLoadAssembly;
		private System.Windows.Forms.ToolStripMenuItem menuitemExportGraph;
		private System.Windows.Forms.ToolStripMenuItem menuitemGenerateAssembly;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator6;
		private System.Windows.Forms.ToolStripMenuItem menuitemAnalyze;
		private System.Windows.Forms.ToolStripMenuItem menuitemStartAnalysis;
		private System.Windows.Forms.ToolStripMenuItem menuitemStopAnalysis;
		private System.Windows.Forms.ToolStripMenuItem menuitemUndo;
		private System.Windows.Forms.ToolStripMenuItem menuitemRedo;
		private System.Windows.Forms.ToolStripMenuItem menuitemZoomIn;
		private System.Windows.Forms.ToolStripMenuItem menuitemZoomOut;
		private System.Windows.Forms.ToolStripMenuItem menuitemZoomBestFit;
		private System.Windows.Forms.ToolStripMenuItem menuitemPan;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator7;
		private System.Windows.Forms.ToolStripMenuItem menuitemResetLayout;
		private TitleBar titlebarOutput;
		private System.Windows.Forms.TextBox textboxOutput;
		private System.Windows.Forms.SplitContainer splitcontainerV;
		private System.Windows.Forms.SplitContainer splitcontainerH;
		private System.Windows.Forms.TreeView treeviewTypes;
		private System.Windows.Forms.RichTextBox richtextboxInformation;
		private TitleBar titlebarStateInfo;
		private System.Windows.Forms.SplitContainer splitcontainerOutput;
		private Microsoft.Msagl.GraphViewerGdi.GViewer graphViewer;
	}
}

