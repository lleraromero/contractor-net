namespace SliceBrowser
{
    partial class SliceBrowser
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
            this.sliceViewer = new ICSharpCode.TextEditor.TextEditorControl();
            this.back_button = new System.Windows.Forms.Button();
            this.loaded_files = new System.Windows.Forms.ComboBox();
            this.next_button = new System.Windows.Forms.Button();
            this.end_button = new System.Windows.Forms.Button();
            this.begin_button = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // sliceViewer
            // 
            this.sliceViewer.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.sliceViewer.IsReadOnly = false;
            this.sliceViewer.Location = new System.Drawing.Point(18, 105);
            this.sliceViewer.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.sliceViewer.Name = "sliceViewer";
            this.sliceViewer.Size = new System.Drawing.Size(1655, 730);
            this.sliceViewer.TabIndex = 0;
            this.sliceViewer.Load += new System.EventHandler(this.sliceViewer_Load);
            // 
            // back_button
            // 
            this.back_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.back_button.Location = new System.Drawing.Point(262, 56);
            this.back_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.back_button.Name = "back_button";
            this.back_button.Size = new System.Drawing.Size(112, 35);
            this.back_button.TabIndex = 1;
            this.back_button.Text = "Back";
            this.back_button.UseVisualStyleBackColor = true;
            this.back_button.Click += new System.EventHandler(this.back_button_Click);
            // 
            // loaded_files
            // 
            this.loaded_files.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.loaded_files.FormattingEnabled = true;
            this.loaded_files.Location = new System.Drawing.Point(18, 18);
            this.loaded_files.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.loaded_files.Name = "loaded_files";
            this.loaded_files.Size = new System.Drawing.Size(1655, 28);
            this.loaded_files.TabIndex = 3;
            this.loaded_files.SelectedIndexChanged += new System.EventHandler(this.loaded_files_SelectedIndexChanged);
            // 
            // next_button
            // 
            this.next_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.next_button.Location = new System.Drawing.Point(384, 56);
            this.next_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.next_button.Name = "next_button";
            this.next_button.Size = new System.Drawing.Size(112, 35);
            this.next_button.TabIndex = 4;
            this.next_button.Text = "Next";
            this.next_button.UseVisualStyleBackColor = true;
            this.next_button.Click += new System.EventHandler(this.next_button_Click);
            // 
            // end_button
            // 
            this.end_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.end_button.Location = new System.Drawing.Point(140, 56);
            this.end_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.end_button.Name = "end_button";
            this.end_button.Size = new System.Drawing.Size(112, 35);
            this.end_button.TabIndex = 5;
            this.end_button.Text = "End";
            this.end_button.UseVisualStyleBackColor = true;
            this.end_button.Click += new System.EventHandler(this.end_button_Click);
            // 
            // begin_button
            // 
            this.begin_button.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.begin_button.Location = new System.Drawing.Point(19, 56);
            this.begin_button.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.begin_button.Name = "begin_button";
            this.begin_button.Size = new System.Drawing.Size(112, 35);
            this.begin_button.TabIndex = 6;
            this.begin_button.Text = "Begin";
            this.begin_button.UseVisualStyleBackColor = true;
            this.begin_button.Click += new System.EventHandler(this.begin_button_Click);
            // 
            // SliceBrowser
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(9F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1691, 854);
            this.Controls.Add(this.begin_button);
            this.Controls.Add(this.end_button);
            this.Controls.Add(this.next_button);
            this.Controls.Add(this.loaded_files);
            this.Controls.Add(this.back_button);
            this.Controls.Add(this.sliceViewer);
            this.Margin = new System.Windows.Forms.Padding(4, 5, 4, 5);
            this.Name = "SliceBrowser";
            this.Text = "SliceBrowser";
            this.ResumeLayout(false);

        }

        #endregion

        private ICSharpCode.TextEditor.TextEditorControl sliceViewer;
        private System.Windows.Forms.Button back_button;
        private System.Windows.Forms.ComboBox loaded_files;
        private System.Windows.Forms.Button next_button;
        private System.Windows.Forms.Button end_button;
        private System.Windows.Forms.Button begin_button;
    }
}

