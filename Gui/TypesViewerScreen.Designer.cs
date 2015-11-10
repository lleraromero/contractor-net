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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(TypesViewerScreen));
            this.trvTypes = new System.Windows.Forms.TreeView();
            this.imlTreeviewImages = new System.Windows.Forms.ImageList(this.components);
            this.SuspendLayout();
            // 
            // trvTypes
            // 
            this.trvTypes.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.trvTypes.Dock = System.Windows.Forms.DockStyle.Fill;
            this.trvTypes.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.trvTypes.HideSelection = false;
            this.trvTypes.ImageIndex = 0;
            this.trvTypes.ImageList = this.imlTreeviewImages;
            this.trvTypes.Indent = 18;
            this.trvTypes.Location = new System.Drawing.Point(0, 0);
            this.trvTypes.Name = "trvTypes";
            this.trvTypes.SelectedImageIndex = 0;
            this.trvTypes.ShowLines = false;
            this.trvTypes.ShowNodeToolTips = true;
            this.trvTypes.ShowPlusMinus = false;
            this.trvTypes.Size = new System.Drawing.Size(288, 223);
            this.trvTypes.TabIndex = 1;
            this.trvTypes.AfterSelect += new System.Windows.Forms.TreeViewEventHandler(this.trvTypes_AfterSelect);
            this.trvTypes.NodeMouseDoubleClick += new System.Windows.Forms.TreeNodeMouseClickEventHandler(this.trvTypes_NodeMouseDoubleClick);
            // 
            // imlTreeviewImages
            // 
            this.imlTreeviewImages.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("imlTreeviewImages.ImageStream")));
            this.imlTreeviewImages.TransparentColor = System.Drawing.Color.White;
            this.imlTreeviewImages.Images.SetKeyName(0, "assembly");
            this.imlTreeviewImages.Images.SetKeyName(1, "namespace");
            this.imlTreeviewImages.Images.SetKeyName(2, "class");
            this.imlTreeviewImages.Images.SetKeyName(3, "collapsed");
            this.imlTreeviewImages.Images.SetKeyName(4, "expanded");
            this.imlTreeviewImages.Images.SetKeyName(5, "none");
            this.imlTreeviewImages.Images.SetKeyName(6, "method");
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
        private System.Windows.Forms.ImageList imlTreeviewImages;
    }
}
