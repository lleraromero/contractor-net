namespace Contractor.Gui
{
	partial class OptionsDialog
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
			this.buttonCancel = new System.Windows.Forms.Button();
			this.buttonOk = new System.Windows.Forms.Button();
			this.groupBox1 = new System.Windows.Forms.GroupBox();
			this.textboxCheckerArguments = new System.Windows.Forms.TextBox();
			this.label1 = new System.Windows.Forms.Label();
			this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
			this.checkboxCollapseTransitions = new System.Windows.Forms.CheckBox();
			this.checkboxInlineMethodsBody = new System.Windows.Forms.CheckBox();
			this.checkboxUnprovenTransitions = new System.Windows.Forms.CheckBox();
			this.checkboxStatesDescriptions = new System.Windows.Forms.CheckBox();
			this.groupBox1.SuspendLayout();
			this.tableLayoutPanel1.SuspendLayout();
			this.SuspendLayout();
			// 
			// buttonCancel
			// 
			this.buttonCancel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.buttonCancel.Location = new System.Drawing.Point(355, 237);
			this.buttonCancel.Margin = new System.Windows.Forms.Padding(6, 3, 3, 3);
			this.buttonCancel.Name = "buttonCancel";
			this.buttonCancel.Size = new System.Drawing.Size(75, 23);
			this.buttonCancel.TabIndex = 2;
			this.buttonCancel.Text = "&Cancel";
			// 
			// buttonOk
			// 
			this.buttonOk.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
			this.buttonOk.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.buttonOk.Location = new System.Drawing.Point(268, 237);
			this.buttonOk.Margin = new System.Windows.Forms.Padding(3, 3, 6, 3);
			this.buttonOk.Name = "buttonOk";
			this.buttonOk.Size = new System.Drawing.Size(75, 23);
			this.buttonOk.TabIndex = 1;
			this.buttonOk.Text = "&OK";
			// 
			// groupBox1
			// 
			this.tableLayoutPanel1.SetColumnSpan(this.groupBox1, 2);
			this.groupBox1.Controls.Add(this.textboxCheckerArguments);
			this.groupBox1.Controls.Add(this.label1);
			this.groupBox1.Dock = System.Windows.Forms.DockStyle.Fill;
			this.groupBox1.Location = new System.Drawing.Point(3, 59);
			this.groupBox1.Margin = new System.Windows.Forms.Padding(3, 9, 3, 3);
			this.groupBox1.Name = "groupBox1";
			this.groupBox1.Padding = new System.Windows.Forms.Padding(9);
			this.groupBox1.Size = new System.Drawing.Size(415, 154);
			this.groupBox1.TabIndex = 4;
			this.groupBox1.TabStop = false;
			this.groupBox1.Text = "Code Contracts";
			// 
			// textboxCheckerArguments
			// 
			this.textboxCheckerArguments.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.textboxCheckerArguments.Location = new System.Drawing.Point(12, 46);
			this.textboxCheckerArguments.Multiline = true;
			this.textboxCheckerArguments.Name = "textboxCheckerArguments";
			this.textboxCheckerArguments.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
			this.textboxCheckerArguments.Size = new System.Drawing.Size(391, 84);
			this.textboxCheckerArguments.TabIndex = 1;
			// 
			// label1
			// 
			this.label1.AutoSize = true;
			this.label1.Location = new System.Drawing.Point(9, 25);
			this.label1.Margin = new System.Windows.Forms.Padding(0, 0, 0, 3);
			this.label1.Name = "label1";
			this.label1.Size = new System.Drawing.Size(140, 15);
			this.label1.TabIndex = 0;
			this.label1.Text = "Static checker arguments";
			// 
			// tableLayoutPanel1
			// 
			this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)
						| System.Windows.Forms.AnchorStyles.Left)
						| System.Windows.Forms.AnchorStyles.Right)));
			this.tableLayoutPanel1.ColumnCount = 2;
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
			this.tableLayoutPanel1.Controls.Add(this.checkboxCollapseTransitions, 1, 0);
			this.tableLayoutPanel1.Controls.Add(this.checkboxInlineMethodsBody, 0, 0);
			this.tableLayoutPanel1.Controls.Add(this.groupBox1, 0, 2);
			this.tableLayoutPanel1.Controls.Add(this.checkboxUnprovenTransitions, 0, 1);
			this.tableLayoutPanel1.Controls.Add(this.checkboxStatesDescriptions, 1, 1);
			this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 12);
			this.tableLayoutPanel1.Margin = new System.Windows.Forms.Padding(3, 3, 3, 6);
			this.tableLayoutPanel1.Name = "tableLayoutPanel1";
			this.tableLayoutPanel1.RowCount = 3;
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle());
			this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
			this.tableLayoutPanel1.Size = new System.Drawing.Size(421, 216);
			this.tableLayoutPanel1.TabIndex = 0;
			// 
			// checkboxCollapseTransitions
			// 
			this.checkboxCollapseTransitions.AutoSize = true;
			this.checkboxCollapseTransitions.Location = new System.Drawing.Point(213, 3);
			this.checkboxCollapseTransitions.Name = "checkboxCollapseTransitions";
			this.checkboxCollapseTransitions.Size = new System.Drawing.Size(129, 19);
			this.checkboxCollapseTransitions.TabIndex = 1;
			this.checkboxCollapseTransitions.Text = "Collapse transitions";
			this.checkboxCollapseTransitions.UseVisualStyleBackColor = true;
			// 
			// checkboxInlineMethodsBody
			// 
			this.checkboxInlineMethodsBody.AutoSize = true;
			this.checkboxInlineMethodsBody.Location = new System.Drawing.Point(3, 3);
			this.checkboxInlineMethodsBody.Name = "checkboxInlineMethodsBody";
			this.checkboxInlineMethodsBody.Size = new System.Drawing.Size(135, 19);
			this.checkboxInlineMethodsBody.TabIndex = 0;
			this.checkboxInlineMethodsBody.Text = "Inline methods body";
			this.checkboxInlineMethodsBody.UseVisualStyleBackColor = true;
			// 
			// checkboxUnprovenTransitions
			// 
			this.checkboxUnprovenTransitions.AutoSize = true;
			this.checkboxUnprovenTransitions.Location = new System.Drawing.Point(3, 28);
			this.checkboxUnprovenTransitions.Name = "checkboxUnprovenTransitions";
			this.checkboxUnprovenTransitions.Size = new System.Drawing.Size(197, 19);
			this.checkboxUnprovenTransitions.TabIndex = 2;
			this.checkboxUnprovenTransitions.Text = "Distinguish unproven transitions";
			this.checkboxUnprovenTransitions.UseVisualStyleBackColor = true;
			// 
			// checkboxStatesDescriptions
			// 
			this.checkboxStatesDescriptions.AutoSize = true;
			this.checkboxStatesDescriptions.Location = new System.Drawing.Point(213, 28);
			this.checkboxStatesDescriptions.Name = "checkboxStatesDescriptions";
			this.checkboxStatesDescriptions.Size = new System.Drawing.Size(155, 19);
			this.checkboxStatesDescriptions.TabIndex = 3;
			this.checkboxStatesDescriptions.Text = "Show states descriptions";
			this.checkboxStatesDescriptions.UseVisualStyleBackColor = true;
			// 
			// OptionsDialog
			// 
			this.AcceptButton = this.buttonOk;
			this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
			this.CancelButton = this.buttonCancel;
			this.ClientSize = new System.Drawing.Size(445, 272);
			this.Controls.Add(this.buttonCancel);
			this.Controls.Add(this.tableLayoutPanel1);
			this.Controls.Add(this.buttonOk);
			this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "OptionsDialog";
			this.Padding = new System.Windows.Forms.Padding(9);
			this.ShowIcon = false;
			this.ShowInTaskbar = false;
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
			this.Text = "Options";
			this.groupBox1.ResumeLayout(false);
			this.groupBox1.PerformLayout();
			this.tableLayoutPanel1.ResumeLayout(false);
			this.tableLayoutPanel1.PerformLayout();
			this.ResumeLayout(false);

		}

		#endregion

		private System.Windows.Forms.Button buttonCancel;
		private System.Windows.Forms.Button buttonOk;
		private System.Windows.Forms.GroupBox groupBox1;
		private System.Windows.Forms.Label label1;
		private System.Windows.Forms.TextBox textboxCheckerArguments;
		private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
		private System.Windows.Forms.CheckBox checkboxCollapseTransitions;
		private System.Windows.Forms.CheckBox checkboxInlineMethodsBody;
		private System.Windows.Forms.CheckBox checkboxUnprovenTransitions;
		private System.Windows.Forms.CheckBox checkboxStatesDescriptions;


	}
}