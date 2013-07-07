namespace Contractor.Gui
{
	partial class AboutDialog
	{
		/// <summary>
		/// Required designer variable.
		/// </summary>
		private System.ComponentModel.IContainer components = null;

		/// <summary>
		/// Clean up any resources being used.
		/// </summary>
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
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AboutDialog));
			this.tableLayoutPanel = new System.Windows.Forms.TableLayoutPanel();
			this.labelDescription = new System.Windows.Forms.Label();
			this.logoPictureBox = new System.Windows.Forms.PictureBox();
			this.labelProductName = new System.Windows.Forms.Label();
			this.labelVersion = new System.Windows.Forms.Label();
			this.labelCopyright = new System.Windows.Forms.Label();
			this.buttonOk = new System.Windows.Forms.Button();
			this.richtextboxLicense = new System.Windows.Forms.RichTextBox();
			this.linklabelUrl = new System.Windows.Forms.LinkLabel();
			this.tableLayoutPanel.SuspendLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).BeginInit();
			this.SuspendLayout();
			// 
			// tableLayoutPanel
			// 
			this.tableLayoutPanel.ColumnCount = 2;
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle());
			this.tableLayoutPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel.Controls.Add(this.labelDescription, 0, 4);
			this.tableLayoutPanel.Controls.Add(this.logoPictureBox, 0, 0);
			this.tableLayoutPanel.Controls.Add(this.labelProductName, 1, 0);
			this.tableLayoutPanel.Controls.Add(this.labelVersion, 1, 1);
			this.tableLayoutPanel.Controls.Add(this.labelCopyright, 1, 2);
			this.tableLayoutPanel.Controls.Add(this.buttonOk, 1, 6);
			this.tableLayoutPanel.Controls.Add(this.richtextboxLicense, 1, 5);
			this.tableLayoutPanel.Controls.Add(this.linklabelUrl, 1, 3);
			this.tableLayoutPanel.Dock = System.Windows.Forms.DockStyle.Fill;
			this.tableLayoutPanel.Location = new System.Drawing.Point(9, 9);
			this.tableLayoutPanel.Name = "tableLayoutPanel";
			this.tableLayoutPanel.RowCount = 7;
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 10F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel.Size = new System.Drawing.Size(451, 271);
			this.tableLayoutPanel.TabIndex = 0;
			// 
			// labelDescription
			// 
			this.labelDescription.AutoSize = true;
			this.labelDescription.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelDescription.Location = new System.Drawing.Point(134, 95);
			this.labelDescription.Margin = new System.Windows.Forms.Padding(3);
			this.labelDescription.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelDescription.Name = "labelDescription";
			this.labelDescription.Size = new System.Drawing.Size(314, 17);
			this.labelDescription.TabIndex = 4;
			this.labelDescription.Text = "Description";
			this.labelDescription.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// logoPictureBox
			// 
			this.logoPictureBox.BackColor = System.Drawing.Color.White;
			this.logoPictureBox.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
			this.logoPictureBox.Dock = System.Windows.Forms.DockStyle.Fill;
			this.logoPictureBox.Image = ((System.Drawing.Image)(resources.GetObject("logoPictureBox.Image")));
			this.logoPictureBox.Location = new System.Drawing.Point(3, 3);
			this.logoPictureBox.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.logoPictureBox.Name = "logoPictureBox";
			this.tableLayoutPanel.SetRowSpan(this.logoPictureBox, 6);
			this.logoPictureBox.Size = new System.Drawing.Size(122, 228);
			this.logoPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.CenterImage;
			this.logoPictureBox.TabIndex = 12;
			this.logoPictureBox.TabStop = false;
			// 
			// labelProductName
			// 
			this.labelProductName.AutoSize = true;
			this.labelProductName.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelProductName.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelProductName.Location = new System.Drawing.Point(134, 3);
			this.labelProductName.Margin = new System.Windows.Forms.Padding(3);
			this.labelProductName.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelProductName.Name = "labelProductName";
			this.labelProductName.Size = new System.Drawing.Size(314, 17);
			this.labelProductName.TabIndex = 0;
			this.labelProductName.Text = "Product Name";
			this.labelProductName.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelVersion
			// 
			this.labelVersion.AutoSize = true;
			this.labelVersion.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelVersion.Location = new System.Drawing.Point(134, 26);
			this.labelVersion.Margin = new System.Windows.Forms.Padding(3);
			this.labelVersion.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelVersion.Name = "labelVersion";
			this.labelVersion.Size = new System.Drawing.Size(314, 17);
			this.labelVersion.TabIndex = 1;
			this.labelVersion.Text = "Version";
			this.labelVersion.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// labelCopyright
			// 
			this.labelCopyright.AutoSize = true;
			this.labelCopyright.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelCopyright.Location = new System.Drawing.Point(134, 49);
			this.labelCopyright.Margin = new System.Windows.Forms.Padding(3);
			this.labelCopyright.MaximumSize = new System.Drawing.Size(0, 17);
			this.labelCopyright.Name = "labelCopyright";
			this.labelCopyright.Size = new System.Drawing.Size(314, 17);
			this.labelCopyright.TabIndex = 2;
			this.labelCopyright.Text = "Copyright";
			this.labelCopyright.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(373, 245);
			this.buttonOk.Margin = new System.Windows.Forms.Padding(3, 6, 3, 3);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 6;
			this.buttonOk.Text = "&OK";
			// 
			// richtextboxLicense
			// 
			this.richtextboxLicense.Dock = System.Windows.Forms.DockStyle.Fill;
			this.richtextboxLicense.Location = new System.Drawing.Point(134, 118);
			this.richtextboxLicense.Name = "richtextboxLicense";
			this.richtextboxLicense.ReadOnly = true;
			this.richtextboxLicense.Size = new System.Drawing.Size(314, 113);
			this.richtextboxLicense.TabIndex = 5;
			this.richtextboxLicense.Text = "License file not found.";
			// 
			// linklabelUrl
			// 
			this.linklabelUrl.AutoSize = true;
			this.linklabelUrl.Dock = System.Windows.Forms.DockStyle.Fill;
			this.linklabelUrl.LinkBehavior = System.Windows.Forms.LinkBehavior.HoverUnderline;
			this.linklabelUrl.Location = new System.Drawing.Point(134, 72);
			this.linklabelUrl.Margin = new System.Windows.Forms.Padding(3);
			this.linklabelUrl.MaximumSize = new System.Drawing.Size(0, 17);
			this.linklabelUrl.Name = "linklabelUrl";
			this.linklabelUrl.Size = new System.Drawing.Size(314, 17);
			this.linklabelUrl.TabIndex = 3;
			this.linklabelUrl.TabStop = true;
			this.linklabelUrl.Text = "http://lafhis.dc.uba.ar/contractor.net";
			this.linklabelUrl.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			this.linklabelUrl.LinkClicked += new System.Windows.Forms.LinkLabelLinkClickedEventHandler(this.OnUrlLinkClicked);
			// 
			// AboutDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonOk;
			this.ClientSize = new System.Drawing.Size(469, 289);
			this.Controls.Add(this.tableLayoutPanel);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "AboutDialog";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "About Contractor.NET";
			this.tableLayoutPanel.ResumeLayout(false);
			this.tableLayoutPanel.PerformLayout();
			((System.ComponentModel.ISupportInitialize)(this.logoPictureBox)).EndInit();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel;
		private System.Windows.Forms.PictureBox logoPictureBox;
		private System.Windows.Forms.Label labelProductName;
		private System.Windows.Forms.Label labelVersion;
		private System.Windows.Forms.Label labelCopyright;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.Label labelDescription;
		private System.Windows.Forms.RichTextBox richtextboxLicense;
		private System.Windows.Forms.LinkLabel linklabelUrl;
	}
}
