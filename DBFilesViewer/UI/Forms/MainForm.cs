using System;
using System.Collections;
using System.Windows.Forms;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Data.IO.Files;
using DBFilesViewer.UI.Controls;
using DBFilesViewer.Utils.Extensions;

namespace DBFilesViewer.UI.Forms
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();

            var self = this;

            DBC.OnFilesLoaded += () =>
            {
                self.InvokeIfRequired(() =>
                {
                    _fileListBox.BeginUpdate();
                    _fileListBox.Items.Clear();

                    foreach (var storageInterface in DBC.Files)
                    {
                        _fileListBox.Items.Add(new CheckedListBoxEntry
                        {
                            RecordType = storageInterface.RecordType,
                            RecordCount = ((IDictionary)storageInterface).Count
                        });
                    }

                    _fileListBox.EndUpdate();
                });
            };
        }

        private class CheckedListBoxEntry
        {
            public Type RecordType { get; set; }
            public int RecordCount { get; set; }

            private string _prettyName;
            public string PrettyName
            {
                get
                {
                    if (_prettyName != null)
                        return _prettyName;

                    var lastIndex = RecordType.Name.LastIndexOf("Record", StringComparison.Ordinal);
                    if (lastIndex != -1)
                        _prettyName = RecordType.Name.Remove(lastIndex);
                    else
                        _prettyName = RecordType.Name;

                    return _prettyName;
                }
            }

            public override string ToString() => $"{PrettyName} ({RecordCount} entries)";
        }

        private static Type ViewControlType = typeof (StorageViewControl<>);

        private void OnFileDoubleClick(object sender, EventArgs e)
        {
            var listboxEntry = _fileListBox.SelectedItem as CheckedListBoxEntry;
            if (listboxEntry != null)
                FindTab(listboxEntry.RecordType);
        }

        private IStorageViewControl FindTab(Type boxRecordType)
        {
            foreach (TabPage tabPage in tabControl1.TabPages)
            {
                if ((Type) tabPage.Tag != boxRecordType)
                    continue;

                tabControl1.SelectedTab = tabPage;
                return tabPage.Controls[0] as IStorageViewControl;
            }

            var viewInstance = (IStorageViewControl)Activator.CreateInstance(ViewControlType.MakeGenericType(boxRecordType));
            viewInstance.OnClose += recordType =>
            {
                var tabFound = false;
                var i = 0;
                for (; !tabFound && i < tabControl1.TabPages.Count; ++i)
                {
                    if ((Type) tabControl1.TabPages[i].Tag == recordType)
                        tabFound = true;
                }

                if (tabFound)
                    tabControl1.TabPages.RemoveAt(i - 1);
            };
            viewInstance.OnReferenceSelected += (referenceType, referenceKey) =>
            {
                FindTab(referenceType).InspectEntry(referenceKey);
            };

            var newTabPage = new TabPage {
                Text = boxRecordType.Name.Replace("Entry", ""),
                Tag = boxRecordType
            };
            newTabPage.Controls.Add((UserControl)viewInstance);
            tabControl1.TabPages.Add(newTabPage);
            tabControl1.SelectedTab = newTabPage;
            return viewInstance;
        }

        private void OpenFromCASC(object sender, EventArgs e)
        {
            var self = this;
            DBC.OnLoadProgress += progressMessage => {
                self.InvokeIfRequired(() => { toolStripStatusLabel1.Text = $"Loaded {progressMessage}.db2 ..."; });
            };
            DBC.OnFilesLoaded += () => {
                self.InvokeIfRequired(() => {
                    toolStripStatusLabel1.Text = "Done.";
                    toolStripProgressBar1.Visible = false;
                });
            };
            Manager.OnLoadProgress += progressMessage => {
                self.InvokeIfRequired(() => { toolStripStatusLabel1.Text = progressMessage; });
            };
            Manager.OnLoadComplete += () => {
                self.InvokeIfRequired(() =>
                {
                    toolStripStatusLabel1.Text = @"Loading DB2s ...";
                    findFileToolStripMenuItem.Enabled = true;
                });
                DBC.InitializeFromCASC();
            };
            toolStripProgressBar1.Visible = true;
            Manager.Initialize();
        }

        private void OpenFromLocalFiles(object sender, EventArgs e)
        {
            toolStripStatusLabel1.Text = @"Loading DB2s ...";

            var self = this;
            DBC.OnLoadProgress += progressMessage =>
            {
                self.InvokeIfRequired(() => { toolStripStatusLabel1.Text = $"Loaded {progressMessage}.db2 ..."; });
            };
            DBC.OnFilesLoaded += () =>
            {
                self.InvokeIfRequired(() =>
                {
                    toolStripStatusLabel1.Text = "Done.";
                    toolStripProgressBar1.Visible = false;
                });
            };

            toolStripProgressBar1.Visible = true;
            DBC.InitializeLocal();
        }

        private void OpenFindFileDialog(object sender, EventArgs e)
        {
            new FindFileDialog().ShowDialog();
        }
    }
}
