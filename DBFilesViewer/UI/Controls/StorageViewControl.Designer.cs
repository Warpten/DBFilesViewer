using System.Collections.Generic;
using BrightIdeasSoftware;

namespace DBFilesViewer.UI.Controls
{
    partial class StorageViewControl<T>
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
            _fileSummaryListView = new FastObjectListView<KeyValuePair<uint, T>>();
            _recordInspectListView = new FastObjectListView<uint>();
            _filterListView = new ObjectListView<Filter<T>>();

            _tabControl = new System.Windows.Forms.TabControl();
            tabPage3 = new System.Windows.Forms.TabPage();
            button1 = new System.Windows.Forms.Button();
            _filterValueComboBox = new System.Windows.Forms.ComboBox();
            _filterOpComboBox = new System.Windows.Forms.ComboBox();
            _columnComboBox = new System.Windows.Forms.ComboBox();
            tabPage1 = new System.Windows.Forms.TabPage();
            _inspectorTabPage = new System.Windows.Forms.TabPage();
            splitContainer1 = new System.Windows.Forms.SplitContainer();
            propertyGrid1 = new System.Windows.Forms.PropertyGrid();
            menuStrip1 = new System.Windows.Forms.MenuStrip();
            exportToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asCSVToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            asCStructureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            closeToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            selectColumnsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            _tabControl.SuspendLayout();
            tabPage3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_filterListView)).BeginInit();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_fileSummaryListView)).BeginInit();
            _inspectorTabPage.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).BeginInit();
            splitContainer1.Panel1.SuspendLayout();
            splitContainer1.Panel2.SuspendLayout();
            splitContainer1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(_recordInspectListView)).BeginInit();
            menuStrip1.SuspendLayout();
            SuspendLayout();
            // 
            // tabControl1
            // 
            _tabControl.Anchor = (((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            _tabControl.Controls.Add(tabPage3);
            _tabControl.Controls.Add(tabPage1);
            _tabControl.Controls.Add(_inspectorTabPage);
            _tabControl.Location = new System.Drawing.Point(0, 27);
            _tabControl.Name = "tabControl1";
            _tabControl.SelectedIndex = 0;
            _tabControl.Size = new System.Drawing.Size(704, 398);
            _tabControl.TabIndex = 0;
            // 
            // tabPage3
            // 
            tabPage3.Controls.Add(button1);
            tabPage3.Controls.Add(_filterValueComboBox);
            tabPage3.Controls.Add(_filterListView);
            tabPage3.Controls.Add(_filterOpComboBox);
            tabPage3.Controls.Add(_columnComboBox);
            tabPage3.Location = new System.Drawing.Point(4, 22);
            tabPage3.Name = "tabPage3";
            tabPage3.Size = new System.Drawing.Size(696, 372);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Search";
            tabPage3.UseVisualStyleBackColor = true;
            // 
            // button1
            // 
            button1.Anchor = (((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            button1.Location = new System.Drawing.Point(612, 11);
            button1.Name = "button1";
            button1.Size = new System.Drawing.Size(75, 23);
            button1.TabIndex = 3;
            button1.Text = "Add filter";
            button1.UseVisualStyleBackColor = true;
            button1.Click += new System.EventHandler(CreateFilter);
            // 
            // textBox1
            // 
            _filterValueComboBox.Anchor = ((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left)
                | System.Windows.Forms.AnchorStyles.Right);
            _filterValueComboBox.Location = new System.Drawing.Point(304, 14);
            _filterValueComboBox.Name = "textBox1";
            _filterValueComboBox.Size = new System.Drawing.Size(302, 20);
            _filterValueComboBox.TabIndex = 2;
            // 
            // objectListView1
            // 
            _filterListView.Anchor = (((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
                | System.Windows.Forms.AnchorStyles.Left) 
                | System.Windows.Forms.AnchorStyles.Right)));
            _filterListView.CellEditUseWholeCell = false;
            _filterListView.Cursor = System.Windows.Forms.Cursors.Default;
            _filterListView.GridLines = true;
            _filterListView.Location = new System.Drawing.Point(11, 41);
            _filterListView.Name = "objectListView1";
            _filterListView.Size = new System.Drawing.Size(676, 317);
            _filterListView.TabIndex = 1;
            _filterListView.UseCompatibleStateImageBehavior = false;
            _filterListView.View = System.Windows.Forms.View.Details;
            // 
            // comboBox2
            // 
            _filterOpComboBox.FormattingEnabled = true;
            _filterOpComboBox.Location = new System.Drawing.Point(177, 14);
            _filterOpComboBox.Name = "comboBox2";
            _filterOpComboBox.Size = new System.Drawing.Size(121, 21);
            _filterOpComboBox.TabIndex = 1;
            // 
            // comboBox1
            // 
            _columnComboBox.FormattingEnabled = true;
            _columnComboBox.Location = new System.Drawing.Point(11, 14);
            _columnComboBox.Name = "comboBox1";
            _columnComboBox.Size = new System.Drawing.Size(160, 21);
            _columnComboBox.TabIndex = 0;
            _columnComboBox.SelectionChangeCommitted += new System.EventHandler(OnFilterFieldSelected);
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(_fileSummaryListView);
            tabPage1.Location = new System.Drawing.Point(4, 22);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new System.Windows.Forms.Padding(3);
            tabPage1.Size = new System.Drawing.Size(696, 372);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "File summary";
            tabPage1.UseVisualStyleBackColor = true;
            // 
            // fastObjectListView1
            // 
            _fileSummaryListView.CellEditUseWholeCell = false;
            _fileSummaryListView.Dock = System.Windows.Forms.DockStyle.Fill;
            _fileSummaryListView.FullRowSelect = true;
            _fileSummaryListView.GridLines = true;
            _fileSummaryListView.Location = new System.Drawing.Point(3, 3);
            _fileSummaryListView.Name = "fastObjectListView1";
            _fileSummaryListView.SelectColumnsOnRightClick = false;
            _fileSummaryListView.SelectColumnsOnRightClickBehaviour = ColumnSelectBehaviour.None;
            _fileSummaryListView.ShowGroups = false;
            _fileSummaryListView.Size = new System.Drawing.Size(690, 366);
            _fileSummaryListView.TabIndex = 0;
            _fileSummaryListView.UseCompatibleStateImageBehavior = false;
            _fileSummaryListView.View = System.Windows.Forms.View.Details;
            _fileSummaryListView.VirtualMode = true;
            // 
            // tabPage2
            // 
            _inspectorTabPage.Controls.Add(splitContainer1);
            _inspectorTabPage.Location = new System.Drawing.Point(4, 22);
            _inspectorTabPage.Name = "tabPage2";
            _inspectorTabPage.Padding = new System.Windows.Forms.Padding(3);
            _inspectorTabPage.Size = new System.Drawing.Size(696, 372);
            _inspectorTabPage.TabIndex = 1;
            _inspectorTabPage.Text = "Record inspector";
            _inspectorTabPage.UseVisualStyleBackColor = true;
            // 
            // splitContainer1
            // 
            splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
            splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
            splitContainer1.IsSplitterFixed = true;
            splitContainer1.Location = new System.Drawing.Point(3, 3);
            splitContainer1.Name = "splitContainer1";
            // 
            // splitContainer1.Panel1
            // 
            splitContainer1.Panel1.Controls.Add(_recordInspectListView);
            // 
            // splitContainer1.Panel2
            // 
            splitContainer1.Panel2.Controls.Add(propertyGrid1);
            splitContainer1.Size = new System.Drawing.Size(690, 366);
            splitContainer1.SplitterDistance = 161;
            splitContainer1.TabIndex = 0;
            // 
            // fastObjectListView2
            // 
            _recordInspectListView.CellEditUseWholeCell = false;
            _recordInspectListView.Dock = System.Windows.Forms.DockStyle.Fill;
            _recordInspectListView.FullRowSelect = true;
            _recordInspectListView.GridLines = true;
            _recordInspectListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            _recordInspectListView.HideSelection = false;
            _recordInspectListView.Location = new System.Drawing.Point(0, 0);
            _recordInspectListView.Name = "fastObjectListView2";
            _recordInspectListView.OwnerDraw = false;
            _recordInspectListView.ShowGroups = false;
            _recordInspectListView.Size = new System.Drawing.Size(161, 366);
            _recordInspectListView.TabIndex = 1;
            _recordInspectListView.UseCompatibleStateImageBehavior = false;
            _recordInspectListView.View = System.Windows.Forms.View.Details;
            _recordInspectListView.VirtualMode = true;
            _recordInspectListView.SelectionChanged += new System.EventHandler(OnSelectionChanged);
            // 
            // propertyGrid1
            // 
            propertyGrid1.Dock = System.Windows.Forms.DockStyle.Fill;
            propertyGrid1.Font = new System.Drawing.Font("Microsoft Sans Serif", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            propertyGrid1.HelpVisible = false;
            propertyGrid1.Location = new System.Drawing.Point(0, 0);
            propertyGrid1.Name = "propertyGrid1";
            propertyGrid1.PropertySort = System.Windows.Forms.PropertySort.NoSort;
            propertyGrid1.Size = new System.Drawing.Size(525, 366);
            propertyGrid1.TabIndex = 0;
            // 
            // menuStrip1
            // 
            menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            exportToolStripMenuItem,
            closeToolStripMenuItem,
            selectColumnsToolStripMenuItem});
            menuStrip1.Location = new System.Drawing.Point(0, 0);
            menuStrip1.Name = "menuStrip1";
            menuStrip1.Size = new System.Drawing.Size(701, 24);
            menuStrip1.TabIndex = 1;
            menuStrip1.Text = "menuStrip1";
            // 
            // exportToolStripMenuItem
            // 
            exportToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            asCSVToolStripMenuItem,
            asCStructureToolStripMenuItem});
            exportToolStripMenuItem.Name = "exportToolStripMenuItem";
            exportToolStripMenuItem.Size = new System.Drawing.Size(64, 20);
            exportToolStripMenuItem.Text = "Export ...";
            // 
            // asCSVToolStripMenuItem
            // 
            asCSVToolStripMenuItem.Name = "asCSVToolStripMenuItem";
            asCSVToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            asCSVToolStripMenuItem.Text = "... as CSV";
            asCSVToolStripMenuItem.Click += new System.EventHandler(ExportCSV);
            // 
            // asCStructureToolStripMenuItem
            // 
            asCStructureToolStripMenuItem.Name = "asCStructureToolStripMenuItem";
            asCStructureToolStripMenuItem.Size = new System.Drawing.Size(158, 22);
            asCStructureToolStripMenuItem.Text = "... as C structure";
            asCStructureToolStripMenuItem.Click += new System.EventHandler(ExportStructure);
            // 
            // closeToolStripMenuItem
            // 
            closeToolStripMenuItem.Alignment = System.Windows.Forms.ToolStripItemAlignment.Right;
            closeToolStripMenuItem.Name = "closeToolStripMenuItem";
            closeToolStripMenuItem.Size = new System.Drawing.Size(48, 20);
            closeToolStripMenuItem.Text = "Close";
            closeToolStripMenuItem.Click += new System.EventHandler(CloseTab);
            // 
            // selectColumnsToolStripMenuItem
            // 
            selectColumnsToolStripMenuItem.Name = "selectColumnsToolStripMenuItem";
            selectColumnsToolStripMenuItem.Size = new System.Drawing.Size(111, 20);
            selectColumnsToolStripMenuItem.Text = "Select columns ...";
            selectColumnsToolStripMenuItem.Click += new System.EventHandler(SelectSummaryColumns);
            // 
            // StorageViewControl
            // 
            AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            Controls.Add(_tabControl);
            Controls.Add(menuStrip1);
            Name = "StorageViewControl";
            Size = new System.Drawing.Size(701, 422);
            _tabControl.ResumeLayout(false);
            tabPage3.ResumeLayout(false);
            tabPage3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(_filterListView)).EndInit();
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_fileSummaryListView)).EndInit();
            _inspectorTabPage.ResumeLayout(false);
            splitContainer1.Panel1.ResumeLayout(false);
            splitContainer1.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(splitContainer1)).EndInit();
            splitContainer1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(_recordInspectListView)).EndInit();
            menuStrip1.ResumeLayout(false);
            menuStrip1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TabControl _tabControl;
        private System.Windows.Forms.TabPage tabPage1;
        private System.Windows.Forms.TabPage _inspectorTabPage;
        private FastObjectListView<KeyValuePair<uint, T>> _fileSummaryListView;
        private System.Windows.Forms.SplitContainer splitContainer1;
        private FastObjectListView<uint> _recordInspectListView;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem exportToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem asCSVToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem closeToolStripMenuItem;
        private System.Windows.Forms.PropertyGrid propertyGrid1;
        private System.Windows.Forms.ToolStripMenuItem asCStructureToolStripMenuItem;
        private System.Windows.Forms.TabPage tabPage3;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.ComboBox _filterValueComboBox;
        private BrightIdeasSoftware.ObjectListView<Filter<T>> _filterListView;
        private System.Windows.Forms.ComboBox _filterOpComboBox;
        private System.Windows.Forms.ComboBox _columnComboBox;
        private System.Windows.Forms.ToolStripMenuItem selectColumnsToolStripMenuItem;
    }
}
