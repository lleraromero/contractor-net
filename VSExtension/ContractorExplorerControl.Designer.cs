using Microsoft.Msagl.GraphViewerGdi;

namespace Contractor.VSExtension
{
	partial class ContractorExplorerControl
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
			if (disposing)
			{
				if (generator != null)
					generator.Dispose();

				if (components != null)
					components.Dispose();
			}

			base.Dispose(disposing);
		}

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			this.components = new System.ComponentModel.Container();
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ContractorExplorerControl));
			this.splitcontainerH = new System.Windows.Forms.SplitContainer();
			this.splitcontainerV = new System.Windows.Forms.SplitContainer();
			this.treeviewTypes = new System.Windows.Forms.TreeView();
			this.imagelist = new System.Windows.Forms.ImageList(this.components);
			this.richtextboxInformation = new System.Windows.Forms.RichTextBox();
			this.graphViewer = new Microsoft.Msagl.GraphViewerGdi.GViewer();
			this.progressbar = new System.Windows.Forms.ProgressBar();
			this.tooltip = new System.Windows.Forms.ToolTip(this.components);
			this.exportGraphDialog = new System.Windows.Forms.SaveFileDialog();
			this.generateOutputDialog = new System.Windows.Forms.SaveFileDialog();
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerH)).BeginInit();
			this.splitcontainerH.Panel1.SuspendLayout();
			this.splitcontainerH.Panel2.SuspendLayout();
			this.splitcontainerH.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerV)).BeginInit();
			this.splitcontainerV.Panel1.SuspendLayout();
			this.splitcontainerV.Panel2.SuspendLayout();
			this.splitcontainerV.SuspendLayout();
			this.SuspendLayout();
			// 
			// splitcontainerH
			// 
			this.splitcontainerH.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitcontainerH.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitcontainerH.Location = new System.Drawing.Point(0, 0);
			this.splitcontainerH.Name = "splitcontainerH";
			// 
			// splitcontainerH.Panel1
			// 
			this.splitcontainerH.Panel1.Controls.Add(this.splitcontainerV);
			this.splitcontainerH.Panel1MinSize = 150;
			// 
			// splitcontainerH.Panel2
			// 
			this.splitcontainerH.Panel2.Controls.Add(this.graphViewer);
			this.splitcontainerH.Panel2.Controls.Add(this.progressbar);
			this.splitcontainerH.Panel2MinSize = 200;
			this.splitcontainerH.Size = new System.Drawing.Size(636, 217);
			this.splitcontainerH.SplitterDistance = 200;
			this.splitcontainerH.SplitterWidth = 3;
			this.splitcontainerH.TabIndex = 2;
			this.splitcontainerH.TabStop = false;
			// 
			// splitcontainerV
			// 
			this.splitcontainerV.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.splitcontainerV.Dock = System.Windows.Forms.DockStyle.Fill;
			this.splitcontainerV.Location = new System.Drawing.Point(0, 0);
			this.splitcontainerV.Name = "splitcontainerV";
			this.splitcontainerV.Orientation = System.Windows.Forms.Orientation.Horizontal;
			// 
			// splitcontainerV.Panel1
			// 
			this.splitcontainerV.Panel1.Controls.Add(this.treeviewTypes);
			// 
			// splitcontainerV.Panel2
			// 
			this.splitcontainerV.Panel2.Controls.Add(this.richtextboxInformation);
			this.splitcontainerV.Panel2.Padding = new System.Windows.Forms.Padding(3);
			this.splitcontainerV.Size = new System.Drawing.Size(200, 217);
			this.splitcontainerV.SplitterDistance = 132;
			this.splitcontainerV.SplitterWidth = 3;
			this.splitcontainerV.TabIndex = 0;
			// 
			// treeviewTypes
			// 
			this.treeviewTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.treeviewTypes.Dock = System.Windows.Forms.DockStyle.Fill;
			this.treeviewTypes.HideSelection = false;
			this.treeviewTypes.HotTracking = true;
			this.treeviewTypes.ImageIndex = 0;
			this.treeviewTypes.ImageList = this.imagelist;
			this.treeviewTypes.Indent = 18;
			this.treeviewTypes.Location = new System.Drawing.Point(0, 0);
			this.treeviewTypes.Name = "treeviewTypes";
			this.treeviewTypes.SelectedImageIndex = 0;
			this.treeviewTypes.ShowLines = false;
			this.treeviewTypes.ShowNodeToolTips = true;
			this.treeviewTypes.ShowPlusMinus = false;
			this.treeviewTypes.Size = new System.Drawing.Size(198, 130);
			this.treeviewTypes.StateImageList = this.imagelist;
			this.treeviewTypes.TabIndex = 3;
			this.treeviewTypes.BeforeCollapse += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeviewTypes_BeforeCollapse);
			this.treeviewTypes.BeforeExpand += new System.Windows.Forms.TreeViewCancelEventHandler(this.treeviewTypes_BeforeExpand);
			this.treeviewTypes.NodeMouseClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeviewTypes_NodeMouseClick);
			this.treeviewTypes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.treeviewTypes_NodeMouseDoubleClick);
			// 
			// imagelist
			// 
			this.imagelist.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imagelist.ImageStream")));
			this.imagelist.TransparentColor = System.Drawing.Color.White;
			this.imagelist.Images.SetKeyName(0, "csharpProject");
			this.imagelist.Images.SetKeyName(1, "vbProject");
			this.imagelist.Images.SetKeyName(2, "vcProject");
			this.imagelist.Images.SetKeyName(3, "unknownProject");
			this.imagelist.Images.SetKeyName(4, "namespace");
			this.imagelist.Images.SetKeyName(5, "class");
			this.imagelist.Images.SetKeyName(6, "collapsed");
			this.imagelist.Images.SetKeyName(7, "expanded");
			this.imagelist.Images.SetKeyName(8, "none");
			// 
			// richtextboxInformation
			// 
			this.richtextboxInformation.BackColor = System.Drawing.SystemColors.Control;
			this.richtextboxInformation.BorderStyle = System.Windows.Forms.BorderStyle.None;
			this.richtextboxInformation.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richtextboxInformation.Location = new System.Drawing.Point(3, 3);
			this.richtextboxInformation.Name = "richtextboxInformation";
			this.richtextboxInformation.ReadOnly = true;
			this.richtextboxInformation.Size = new System.Drawing.Size(192, 74);
			this.richtextboxInformation.TabIndex = 0;
			this.richtextboxInformation.Text = "";
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
			this.graphViewer.Size = new System.Drawing.Size(431, 197);
			this.graphViewer.TabIndex = 1;
			this.graphViewer.ToolBarIsVisible = false;
			this.graphViewer.ZoomF = 1D;
			this.graphViewer.ZoomFraction = 0.5D;
			this.graphViewer.ZoomWindowThreshold = 0.05D;
			this.graphViewer.GraphChanged += new System.EventHandler(this.graphViewer_GraphChanged);
			// 
			// progressbar
			// 
			this.progressbar.Dock = System.Windows.Forms.DockStyle.Bottom;
			this.progressbar.Location = new System.Drawing.Point(0, 197);
			this.progressbar.MarqueeAnimationSpeed = 20;
			this.progressbar.Name = "progressbar";
			this.progressbar.Size = new System.Drawing.Size(431, 18);
			this.progressbar.Style = System.Windows.Forms.ProgressBarStyle.Marquee;
			this.progressbar.TabIndex = 4;
			this.progressbar.Visible = false;
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
			// ContractorExplorerControl
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.Controls.Add(this.splitcontainerH);
			this.Name = "ContractorExplorerControl";
			this.Size = new System.Drawing.Size(636, 217);
			this.Load += new System.EventHandler(this.ContractorExplorerControl_Load);
			this.splitcontainerH.Panel1.ResumeLayout(false);
			this.splitcontainerH.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerH)).EndInit();
			this.splitcontainerH.ResumeLayout(false);
			this.splitcontainerV.Panel1.ResumeLayout(false);
			this.splitcontainerV.Panel2.ResumeLayout(false);
			((System.ComponentModel.ISupportInitialize)(this.splitcontainerV)).EndInit();
			this.splitcontainerV.ResumeLayout(false);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.SplitContainer splitcontainerH;
		private GViewer graphViewer;
		private System.Windows.Forms.ImageList imagelist;
		private System.Windows.Forms.ProgressBar progressbar;
		private System.Windows.Forms.ToolTip tooltip;
		private System.Windows.Forms.SaveFileDialog exportGraphDialog;
		private System.Windows.Forms.SaveFileDialog generateOutputDialog;
		private System.Windows.Forms.SplitContainer splitcontainerV;
		private System.Windows.Forms.TreeView treeviewTypes;
		private System.Windows.Forms.RichTextBox richtextboxInformation;

	}
}
