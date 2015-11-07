namespace Contractor.Gui
{
    partial class TypesViewerScreen
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
            this.trvTypes = new System.Windows.Forms.TreeView();
            this.SuspendLayout();
            // 
            // trvTypes
            // 
            this.trvTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.trvTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTypes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trvTypes.HideSelection = false;
            this.trvTypes.Indent = 18;
            this.trvTypes.Location = new System.Drawing.Point(0, 0);
            this.trvTypes.Name = "trvTypes";
            this.trvTypes.ShowLines = false;
            this.trvTypes.ShowNodeToolTips = true;
            this.trvTypes.ShowPlusMinus = false;
            this.trvTypes.Size = new System.Drawing.Size(288, 223);
            this.trvTypes.TabIndex = 1;
            // 
            // TypesViewerScreen
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.trvTypes);
            this.Name = "TypesViewerScreen";
            this.Size = new System.Drawing.Size(288, 223);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TreeView trvTypes;
    }
}
