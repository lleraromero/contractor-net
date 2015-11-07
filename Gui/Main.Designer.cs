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
            this.exportGraphDialog = new System.Windows.Forms.SaveFileDialog();
            this.generateOutputDialog = new System.Windows.Forms.SaveFileDialog();
            this.statusStrip = new System.Windows.Forms.StatusStrip();
            this.statusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.progressBar = new System.Windows.Forms.ToolStripProgressBar();
            this.toolStrip = new System.Windows.Forms.ToolStrip();
            this.buttonOptions = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator4 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonLoadAssembly = new System.Windows.Forms.ToolStripButton();
            this.buttonLoadContracts = new System.Windows.Forms.ToolStripButton();
            this.buttonExportGraph = new System.Windows.Forms.ToolStripButton();
            this.buttonGenerateAssembly = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.buttonStartAnalysis = new System.Windows.Forms.ToolStripButton();
            this.buttonStopAnalysis = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.cmbBackend = new System.Windows.Forms.ToolStripComboBox();
            this.menuStrip = new System.Windows.Forms.MenuStrip();
            this.menuitemView = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemOutput = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemHelp = new System.Windows.Forms.ToolStripMenuItem();
            this.menuitemAbout = new System.Windows.Forms.ToolStripMenuItem();
            this.loadAssemblyDialog = new System.Windows.Forms.OpenFileDialog();
            this.textboxOutput = new System.Windows.Forms.TextBox();
            this.splitcontainerV = new System.Windows.Forms.SplitContainer();
            this.splitcontainerH = new System.Windows.Forms.SplitContainer();
            this.treeviewTypes = new System.Windows.Forms.TreeView();
            this.listboxMethods = new System.Windows.Forms.CheckedListBox();
            this.richtextboxInformation = new System.Windows.Forms.RichTextBox();
            this.toolstripMethods = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.buttonCheckAllMethods = new System.Windows.Forms.ToolStripButton();
            this.buttonUncheckAllMethods = new System.Windows.Forms.ToolStripButton();
            this.titlebarProperties = new Contractor.Gui.TitleBar();
            this.splitcontainerOutput = new System.Windows.Forms.SplitContainer();
            this.epaViewer = new Contractor.Gui.EpaViewerScreen();
            this.titlebarOutput = new Contractor.Gui.TitleBar();
            this.loadContractsDialog = new System.Windows.Forms.OpenFileDialog();
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
            this.toolstripMethods.SuspendLayout();
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
            this.imagelist.Images.SetKeyName(6, "method");
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
            this.statusStrip.Location = new System.Drawing.Point(0, 431);
            this.statusStrip.Name = "statusStrip";
            this.statusStrip.RenderMode = System.Windows.Forms.ToolStripRenderMode.ManagerRenderMode;
            this.statusStrip.Size = new System.Drawing.Size(775, 22);
            this.statusStrip.TabIndex = 4;
            this.statusStrip.Text = "statusStrip1";
            // 
            // statusLabel
            // 
            this.statusLabel.Name = "statusLabel";
            this.statusLabel.Size = new System.Drawing.Size(760, 17);
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
            this.toolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.buttonOptions,
            this.toolStripSeparator4,
            this.buttonLoadAssembly,
            this.buttonLoadContracts,
            this.buttonExportGraph,
            this.buttonGenerateAssembly,
            this.toolStripSeparator1,
            this.buttonStartAnalysis,
            this.buttonStopAnalysis,
            this.toolStripSeparator2,
            this.cmbBackend});
            this.toolStrip.Location = new System.Drawing.Point(0, 24);
            this.toolStrip.Name = "toolStrip";
            this.toolStrip.Size = new System.Drawing.Size(775, 25);
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
            // buttonLoadContracts
            // 
            this.buttonLoadContracts.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.buttonLoadContracts.Enabled = false;
            this.buttonLoadContracts.Image = ((System.Drawing.Image)(resources.GetObject("buttonLoadContracts.Image")));
            this.buttonLoadContracts.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonLoadContracts.Name = "buttonLoadContracts";
            this.buttonLoadContracts.Size = new System.Drawing.Size(23, 22);
            this.buttonLoadContracts.Text = "Load Contracts";
            this.buttonLoadContracts.Click += new System.EventHandler(this.OnLoadContracts);
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
            // cmbBackend
            // 
            this.cmbBackend.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbBackend.Name = "cmbBackend";
            this.cmbBackend.Size = new System.Drawing.Size(110, 25);
            // 
            // menuStrip
            // 
            this.menuStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemView,
            this.menuitemHelp});
            this.menuStrip.Location = new System.Drawing.Point(0, 0);
            this.menuStrip.Name = "menuStrip";
            this.menuStrip.Size = new System.Drawing.Size(775, 24);
            this.menuStrip.TabIndex = 0;
            this.menuStrip.Text = "menuStrip1";
            // 
            // menuitemView
            // 
            this.menuitemView.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.menuitemOutput});
            this.menuitemView.Name = "menuitemView";
            this.menuitemView.Size = new System.Drawing.Size(44, 20);
            this.menuitemView.Text = "&View";
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
            this.textboxOutput.Size = new System.Drawing.Size(483, 166);
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
            this.splitcontainerV.Size = new System.Drawing.Size(775, 382);
            this.splitcontainerV.SplitterDistance = 286;
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
            this.splitcontainerH.Panel2.Controls.Add(this.listboxMethods);
            this.splitcontainerH.Panel2.Controls.Add(this.richtextboxInformation);
            this.splitcontainerH.Panel2.Controls.Add(this.toolstripMethods);
            this.splitcontainerH.Panel2.Controls.Add(this.titlebarProperties);
            this.splitcontainerH.Size = new System.Drawing.Size(286, 382);
            this.splitcontainerH.SplitterDistance = 191;
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
            this.treeviewTypes.Size = new System.Drawing.Size(284, 189);
            this.treeviewTypes.StateImageList = this.imagelist;
            this.treeviewTypes.TabIndex = 0;
            this.treeviewTypes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.OnAfterSelectTreeNode);
            this.treeviewTypes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.OnTreeNodeMouseDoubleClick);
            // 
            // listboxMethods
            // 
            this.listboxMethods.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.listboxMethods.CheckOnClick = true;
            this.listboxMethods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listboxMethods.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listboxMethods.FormattingEnabled = true;
            this.listboxMethods.IntegralHeight = false;
            this.listboxMethods.Location = new System.Drawing.Point(0, 44);
            this.listboxMethods.Name = "listboxMethods";
            this.listboxMethods.Size = new System.Drawing.Size(284, 141);
            this.listboxMethods.Sorted = true;
            this.listboxMethods.TabIndex = 2;
            // 
            // richtextboxInformation
            // 
            this.richtextboxInformation.BackColor = System.Drawing.SystemColors.Info;
            this.richtextboxInformation.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.richtextboxInformation.DetectUrls = false;
            this.richtextboxInformation.Dock = System.Windows.Forms.DockStyle.Fill;
            this.richtextboxInformation.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.richtextboxInformation.Location = new System.Drawing.Point(0, 44);
            this.richtextboxInformation.Name = "richtextboxInformation";
            this.richtextboxInformation.ReadOnly = true;
            this.richtextboxInformation.ShowSelectionMargin = true;
            this.richtextboxInformation.Size = new System.Drawing.Size(284, 141);
            this.richtextboxInformation.TabIndex = 1;
            this.richtextboxInformation.TabStop = false;
            this.richtextboxInformation.Text = "";
            // 
            // toolstripMethods
            // 
            this.toolstripMethods.Enabled = false;
            this.toolstripMethods.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolstripMethods.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.buttonCheckAllMethods,
            this.buttonUncheckAllMethods});
            this.toolstripMethods.Location = new System.Drawing.Point(0, 19);
            this.toolstripMethods.Name = "toolstripMethods";
            this.toolstripMethods.ShowItemToolTips = false;
            this.toolstripMethods.Size = new System.Drawing.Size(284, 25);
            this.toolstripMethods.TabIndex = 3;
            this.toolstripMethods.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(43, 22);
            this.toolStripLabel1.Text = "Check:";
            // 
            // buttonCheckAllMethods
            // 
            this.buttonCheckAllMethods.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonCheckAllMethods.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonCheckAllMethods.Image = ((System.Drawing.Image)(resources.GetObject("buttonCheckAllMethods.Image")));
            this.buttonCheckAllMethods.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonCheckAllMethods.Name = "buttonCheckAllMethods";
            this.buttonCheckAllMethods.Size = new System.Drawing.Size(25, 22);
            this.buttonCheckAllMethods.Text = "All";
            this.buttonCheckAllMethods.Click += new System.EventHandler(this.OnCheckAllMethods);
            // 
            // buttonUncheckAllMethods
            // 
            this.buttonUncheckAllMethods.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.buttonUncheckAllMethods.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.buttonUncheckAllMethods.Image = ((System.Drawing.Image)(resources.GetObject("buttonUncheckAllMethods.Image")));
            this.buttonUncheckAllMethods.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.buttonUncheckAllMethods.Name = "buttonUncheckAllMethods";
            this.buttonUncheckAllMethods.Size = new System.Drawing.Size(41, 22);
            this.buttonUncheckAllMethods.Text = "None";
            this.buttonUncheckAllMethods.Click += new System.EventHandler(this.OnUncheckAllMethods);
            // 
            // titlebarProperties
            // 
            this.titlebarProperties.BackColor = System.Drawing.SystemColors.ActiveBorder;
            this.titlebarProperties.BackColorStyle = Contractor.Gui.BackColorStyle.Gradient;
            this.titlebarProperties.DarkBackColor = System.Drawing.SystemColors.ActiveCaption;
            this.titlebarProperties.Dock = System.Windows.Forms.DockStyle.Top;
            this.titlebarProperties.LightBackColor = System.Drawing.SystemColors.GradientInactiveCaption;
            this.titlebarProperties.Location = new System.Drawing.Point(0, 0);
            this.titlebarProperties.Name = "titlebarProperties";
            this.titlebarProperties.ShowBottomBorder = true;
            this.titlebarProperties.ShowCloseButton = false;
            this.titlebarProperties.Size = new System.Drawing.Size(284, 19);
            this.titlebarProperties.TabIndex = 0;
            this.titlebarProperties.TabStop = false;
            this.titlebarProperties.Text = "Methods";
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
            this.splitcontainerOutput.Panel1.Controls.Add(this.epaViewer);
            // 
            // splitcontainerOutput.Panel2
            // 
            this.splitcontainerOutput.Panel2.Controls.Add(this.textboxOutput);
            this.splitcontainerOutput.Panel2.Controls.Add(this.titlebarOutput);
            this.splitcontainerOutput.Size = new System.Drawing.Size(485, 382);
            this.splitcontainerOutput.SplitterDistance = 191;
            this.splitcontainerOutput.TabIndex = 0;
            this.splitcontainerOutput.TabStop = false;
            // 
            // epaViewer
            // 
            this.epaViewer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.epaViewer.Location = new System.Drawing.Point(0, 0);
            this.epaViewer.Name = "epaViewer";
            this.epaViewer.Size = new System.Drawing.Size(483, 189);
            this.epaViewer.TabIndex = 1;
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
            this.titlebarOutput.Size = new System.Drawing.Size(483, 19);
            this.titlebarOutput.TabIndex = 0;
            this.titlebarOutput.Text = "Output";
            this.titlebarOutput.Close += new System.EventHandler(this.OnOutputClose);
            // 
            // loadContractsDialog
            // 
            this.loadContractsDialog.DefaultExt = "dll";
            this.loadContractsDialog.Filter = "Dynamic Link Libraries (*.dll)|*.dll|Executable Files (*.exe)|*.exe";
            this.loadContractsDialog.Title = "Load Contract Reference Assembly...";
            // 
            // Main
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(775, 453);
            this.Controls.Add(this.splitcontainerV);
            this.Controls.Add(this.toolStrip);
            this.Controls.Add(this.menuStrip);
            this.Controls.Add(this.statusStrip);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MainMenuStrip = this.menuStrip;
            this.Name = "Main";
            this.Text = "Contractor.NET";
            this.WindowState = System.Windows.Forms.FormWindowState.Maximized;
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
            this.splitcontainerH.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerH)).EndInit();
            this.splitcontainerH.ResumeLayout(false);
            this.toolstripMethods.ResumeLayout(false);
            this.toolstripMethods.PerformLayout();
            this.splitcontainerOutput.Panel1.ResumeLayout(false);
            this.splitcontainerOutput.Panel2.ResumeLayout(false);
            this.splitcontainerOutput.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.splitcontainerOutput)).EndInit();
            this.splitcontainerOutput.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

		}

		#endregion

		private System.Windows.Forms.SaveFileDialog exportGraphDialog;
		private System.Windows.Forms.SaveFileDialog generateOutputDialog;
		private System.Windows.Forms.StatusStrip statusStrip;
		private System.Windows.Forms.ToolStrip toolStrip;
        private System.Windows.Forms.MenuStrip menuStrip;
		private System.Windows.Forms.ToolStripMenuItem menuitemHelp;
        private System.Windows.Forms.ToolStripMenuItem menuitemAbout;
		private System.Windows.Forms.ToolStripStatusLabel statusLabel;
		private System.Windows.Forms.ToolStripProgressBar progressBar;
		private System.Windows.Forms.ToolStripButton buttonStopAnalysis;
		private System.Windows.Forms.ToolStripButton buttonExportGraph;
        private System.Windows.Forms.ToolStripButton buttonGenerateAssembly;
		private System.Windows.Forms.ToolStripButton buttonOptions;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator4;
		private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
		private System.Windows.Forms.ImageList imagelist;
		private System.Windows.Forms.ToolStripButton buttonStartAnalysis;
		private System.Windows.Forms.ToolStripButton buttonLoadAssembly;
		private System.Windows.Forms.OpenFileDialog loadAssemblyDialog;
		private System.Windows.Forms.ToolStripMenuItem menuitemView;
        private System.Windows.Forms.ToolStripMenuItem menuitemOutput;
		private TitleBar titlebarOutput;
		private System.Windows.Forms.TextBox textboxOutput;
		private System.Windows.Forms.SplitContainer splitcontainerV;
		private System.Windows.Forms.SplitContainer splitcontainerH;
		private System.Windows.Forms.TreeView treeviewTypes;
		private System.Windows.Forms.RichTextBox richtextboxInformation;
		private TitleBar titlebarProperties;
        private System.Windows.Forms.SplitContainer splitcontainerOutput;
		private System.Windows.Forms.CheckedListBox listboxMethods;
        private System.Windows.Forms.OpenFileDialog loadContractsDialog;
		private System.Windows.Forms.ToolStripButton buttonLoadContracts;
		private System.Windows.Forms.ToolStrip toolstripMethods;
		private System.Windows.Forms.ToolStripButton buttonCheckAllMethods;
		private System.Windows.Forms.ToolStripButton buttonUncheckAllMethods;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripComboBox cmbBackend;
        private EpaViewerScreen epaViewer;
	}
}

