namespace Contractor.Gui
{
	partial class TitleBar
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

		#region Component Designer generated code

		/// <summary> 
		/// Required method for Designer support - do not modify 
		/// the contents of this method with the code editor.
		/// </summary>
		private void InitializeComponent()
		{
			System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TitleBar));
			this.buttonClose = new System.Windows.Forms.Button();
			this.labelTitle = new System.Windows.Forms.Label();
			this.SuspendLayout();
			// 
			// buttonClose
			// 
			this.buttonClose.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.buttonClose.BackColor = System.Drawing.Color.Transparent;
			this.buttonClose.FlatAppearance.BorderColor = System.Drawing.SystemColors.GradientActiveCaption;
			this.buttonClose.FlatAppearance.BorderSize = 0;
			this.buttonClose.FlatAppearance.MouseDownBackColor = System.Drawing.Color.White;
			this.buttonClose.FlatAppearance.MouseOverBackColor = System.Drawing.Color.AliceBlue;
			this.buttonClose.FlatStyle = System.Windows.Forms.FlatStyle.Flat;
			this.buttonClose.Image = ((System.Drawing.Image)(resources.GetObject("buttonClose.Image")));
			this.buttonClose.Location = new System.Drawing.Point(173, 2);
			this.buttonClose.Margin = new System.Windows.Forms.Padding(0);
			this.buttonClose.Name = "buttonClose";
			this.buttonClose.Size = new System.Drawing.Size(16, 14);
			this.buttonClose.TabIndex = 1;
			this.buttonClose.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
			this.buttonClose.UseVisualStyleBackColor = false;
			this.buttonClose.Click += new System.EventHandler(this.buttonClose_Click);
			// 
			// labelTitle
			// 
			this.labelTitle.BackColor = System.Drawing.Color.Transparent;
			this.labelTitle.CausesValidation = false;
			this.labelTitle.Dock = System.Windows.Forms.DockStyle.Fill;
			this.labelTitle.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.labelTitle.ForeColor = System.Drawing.SystemColors.ActiveCaptionText;
			this.labelTitle.Location = new System.Drawing.Point(0, 0);
			this.labelTitle.Name = "labelTitle";
			this.labelTitle.Padding = new System.Windows.Forms.Padding(1, 1, 1, 2);
			this.labelTitle.Size = new System.Drawing.Size(191, 18);
			this.labelTitle.TabIndex = 0;
			this.labelTitle.Text = "Title";
			this.labelTitle.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
			// 
			// TitleBar
			// 
			this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.BackColor = System.Drawing.SystemColors.ActiveCaption;
			this.Controls.Add(this.buttonClose);
			this.Controls.Add(this.labelTitle);
			this.Name = "TitleBar";
			this.Size = new System.Drawing.Size(191, 18);
			this.Paint += new System.Windows.Forms.PaintEventHandler(this.TitleBar_Paint);
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonClose;
		private System.Windows.Forms.Label labelTitle;
	}
}
