namespace DBFilesViewer.UI.Forms
{
    partial class FindFileDialog
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindFileDialog));
            this.button1 = new System.Windows.Forms.Button();
            this.textBox1 = new System.Windows.Forms.TextBox();
            this._fileExistsPanel = new System.Windows.Forms.Panel();
            this.pictureBox2 = new System.Windows.Forms.PictureBox();
            this.label2 = new System.Windows.Forms.Label();
            this._viewFileButton = new System.Windows.Forms.Button();
            this._fileNotFoundPanel = new System.Windows.Forms.Panel();
            this.pictureBox1 = new System.Windows.Forms.PictureBox();
            this.label1 = new System.Windows.Forms.Label();
            this.comboBox1 = new System.Windows.Forms.ComboBox();
            this._fileExistsPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).BeginInit();
            this._fileNotFoundPanel.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).BeginInit();
            this.SuspendLayout();
            // 
            // button1
            // 
            this.button1.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.button1.AutoSize = true;
            this.button1.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Zoom;
            this.button1.Image = global::DBFilesViewer.Properties.Resources.Search1;
            this.button1.Location = new System.Drawing.Point(410, 12);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(35, 36);
            this.button1.TabIndex = 1;
            this.button1.TextImageRelation = System.Windows.Forms.TextImageRelation.ImageAboveText;
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.OnClick);
            // 
            // textBox1
            // 
            this.textBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.textBox1.Location = new System.Drawing.Point(103, 21);
            this.textBox1.Name = "textBox1";
            this.textBox1.Size = new System.Drawing.Size(301, 20);
            this.textBox1.TabIndex = 0;
            // 
            // _fileExistsPanel
            // 
            this._fileExistsPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(11)))), ((int)(((byte)(143)))), ((int)(((byte)(68)))));
            this._fileExistsPanel.Controls.Add(this.pictureBox2);
            this._fileExistsPanel.Controls.Add(this.label2);
            this._fileExistsPanel.Location = new System.Drawing.Point(12, 54);
            this._fileExistsPanel.Name = "_fileExistsPanel";
            this._fileExistsPanel.Size = new System.Drawing.Size(356, 30);
            this._fileExistsPanel.TabIndex = 3;
            this._fileExistsPanel.Visible = false;
            this._fileExistsPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintSuccessBox);
            // 
            // pictureBox2
            // 
            this.pictureBox2.Image = global::DBFilesViewer.Properties.Resources.Checkmark;
            this.pictureBox2.Location = new System.Drawing.Point(5, 5);
            this.pictureBox2.Name = "pictureBox2";
            this.pictureBox2.Size = new System.Drawing.Size(21, 20);
            this.pictureBox2.TabIndex = 1;
            this.pictureBox2.TabStop = false;
            // 
            // label2
            // 
            this.label2.Location = new System.Drawing.Point(32, 5);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(223, 20);
            this.label2.TabIndex = 0;
            this.label2.Text = "File found";
            this.label2.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // _viewFileButton
            // 
            this._viewFileButton.Location = new System.Drawing.Point(374, 58);
            this._viewFileButton.Name = "_viewFileButton";
            this._viewFileButton.Size = new System.Drawing.Size(71, 23);
            this._viewFileButton.TabIndex = 2;
            this._viewFileButton.Text = "Visualize";
            this._viewFileButton.UseVisualStyleBackColor = true;
            this._viewFileButton.Visible = false;
            this._viewFileButton.Click += new System.EventHandler(this.OpenFileAsHex);
            // 
            // _fileNotFoundPanel
            // 
            this._fileNotFoundPanel.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(244)))), ((int)(((byte)(94)))), ((int)(((byte)(55)))));
            this._fileNotFoundPanel.Controls.Add(this.pictureBox1);
            this._fileNotFoundPanel.Controls.Add(this.label1);
            this._fileNotFoundPanel.Location = new System.Drawing.Point(12, 54);
            this._fileNotFoundPanel.Name = "_fileNotFoundPanel";
            this._fileNotFoundPanel.Size = new System.Drawing.Size(433, 30);
            this._fileNotFoundPanel.TabIndex = 4;
            this._fileNotFoundPanel.Visible = false;
            this._fileNotFoundPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.OnPaintErrorBox);
            // 
            // pictureBox1
            // 
            this.pictureBox1.Image = Properties.Resources.Delete;
            this.pictureBox1.Location = new System.Drawing.Point(5, 5);
            this.pictureBox1.Name = "pictureBox1";
            this.pictureBox1.Size = new System.Drawing.Size(21, 20);
            this.pictureBox1.TabIndex = 1;
            this.pictureBox1.TabStop = false;
            // 
            // label1
            // 
            this.label1.BackColor = System.Drawing.Color.Transparent;
            this.label1.Location = new System.Drawing.Point(32, 5);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(223, 20);
            this.label1.TabIndex = 0;
            this.label1.Text = "File not found.";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // comboBox1
            // 
            this.comboBox1.FormattingEnabled = true;
            this.comboBox1.Items.AddRange(new object[] {
            "Path",
            "FileDataID"});
            this.comboBox1.Location = new System.Drawing.Point(12, 21);
            this.comboBox1.Name = "comboBox1";
            this.comboBox1.Size = new System.Drawing.Size(85, 21);
            this.comboBox1.TabIndex = 5;
            // 
            // FindFileDialog
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(453, 93);
            this.Controls.Add(this.comboBox1);
            this.Controls.Add(this._viewFileButton);
            this.Controls.Add(this._fileNotFoundPanel);
            this.Controls.Add(this._fileExistsPanel);
            this.Controls.Add(this.textBox1);
            this.Controls.Add(this.button1);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MaximumSize = new System.Drawing.Size(469, 132);
            this.MinimizeBox = false;
            this.MinimumSize = new System.Drawing.Size(469, 132);
            this.Name = "FindFileDialog";
            this.SizeGripStyle = System.Windows.Forms.SizeGripStyle.Hide;
            this.Text = "FindFileDialog";
            this._fileExistsPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox2)).EndInit();
            this._fileNotFoundPanel.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.pictureBox1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox textBox1;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.Panel _fileExistsPanel;
        private System.Windows.Forms.PictureBox pictureBox2;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Panel _fileNotFoundPanel;
        private System.Windows.Forms.PictureBox pictureBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button _viewFileButton;
        private System.Windows.Forms.ComboBox comboBox1;
    }
}