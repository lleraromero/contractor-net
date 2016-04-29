namespace Contractor.Gui.Views
{
    partial class MethodFilterScreen
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MethodFilterScreen));
            this.lsbMethods = new System.Windows.Forms.CheckedListBox();
            this.toolstrip = new System.Windows.Forms.ToolStrip();
            this.toolStripLabel1 = new System.Windows.Forms.ToolStripLabel();
            this.btnAll = new System.Windows.Forms.ToolStripButton();
            this.btnNone = new System.Windows.Forms.ToolStripButton();
            this.toolstrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // lsbMethods
            // 
            this.lsbMethods.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.lsbMethods.CheckOnClick = true;
            this.lsbMethods.Dock = System.Windows.Forms.DockStyle.Fill;
            this.lsbMethods.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lsbMethods.FormattingEnabled = true;
            this.lsbMethods.IntegralHeight = false;
            this.lsbMethods.Location = new System.Drawing.Point(0, 25);
            this.lsbMethods.Name = "lsbMethods";
            this.lsbMethods.Size = new System.Drawing.Size(283, 225);
            this.lsbMethods.Sorted = true;
            this.lsbMethods.TabIndex = 3;
            // 
            // toolstrip
            // 
            this.toolstrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.toolstrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.toolStripLabel1,
            this.btnAll,
            this.btnNone});
            this.toolstrip.Location = new System.Drawing.Point(0, 0);
            this.toolstrip.Name = "toolstrip";
            this.toolstrip.ShowItemToolTips = false;
            this.toolstrip.Size = new System.Drawing.Size(283, 25);
            this.toolstrip.TabIndex = 4;
            this.toolstrip.Text = "toolStrip1";
            // 
            // toolStripLabel1
            // 
            this.toolStripLabel1.Name = "toolStripLabel1";
            this.toolStripLabel1.Size = new System.Drawing.Size(43, 22);
            this.toolStripLabel1.Text = "Check:";
            // 
            // btnAll
            // 
            this.btnAll.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnAll.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnAll.Image = ((System.Drawing.Image)(resources.GetObject("btnAll.Image")));
            this.btnAll.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnAll.Name = "btnAll";
            this.btnAll.Size = new System.Drawing.Size(25, 22);
            this.btnAll.Text = "All";
            this.btnAll.Click += new System.EventHandler(this.btnAll_Click);
            // 
            // btnNone
            // 
            this.btnNone.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.btnNone.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.btnNone.Image = ((System.Drawing.Image)(resources.GetObject("btnNone.Image")));
            this.btnNone.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.btnNone.Name = "btnNone";
            this.btnNone.Size = new System.Drawing.Size(41, 22);
            this.btnNone.Text = "None";
            this.btnNone.Click += new System.EventHandler(this.btnNone_Click);
            // 
            // MethodFilterScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.lsbMethods);
            this.Controls.Add(this.toolstrip);
            this.Name = "MethodFilterScreen";
            this.Size = new System.Drawing.Size(283, 250);
            this.toolstrip.ResumeLayout(false);
            this.toolstrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckedListBox lsbMethods;
        private System.Windows.Forms.ToolStrip toolstrip;
        private System.Windows.Forms.ToolStripLabel toolStripLabel1;
        private System.Windows.Forms.ToolStripButton btnAll;
        private System.Windows.Forms.ToolStripButton btnNone;
    }
}
