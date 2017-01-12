using System.Windows.Forms;
using DBFilesViewer.UI.Controls;

namespace DBFilesViewer.UI.Forms
{
    partial class ModelRenderForm : Form
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
            this._renderTimer = new System.Windows.Forms.Timer(this.components);
            this.modelRenderControl1 = new ModelRenderControl();
            this.SuspendLayout();
            // 
            // modelRenderControl1
            // 
            this.modelRenderControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.modelRenderControl1.Location = new System.Drawing.Point(0, 0);
            this.modelRenderControl1.Name = "modelRenderControl1";
            this.modelRenderControl1.Size = new System.Drawing.Size(481, 290);
            this.modelRenderControl1.TabIndex = 0;
            // 
            // ModelRenderForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(481, 290);
            this.Controls.Add(this.modelRenderControl1);
            this.Name = "ModelRenderForm";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.OnClose);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Timer _renderTimer;
        private Controls.ModelRenderControl modelRenderControl1;
    }
}
