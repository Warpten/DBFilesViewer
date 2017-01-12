using System;
using System.Windows.Forms;
using BrightIdeasSoftware;
using DBFilesViewer.Data.IO.Files;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A simple utility class that lets the user walk through all references to the currently
    /// inspected object in the Record inspector tab, and easily visualize the complete referencing record.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class ReferenceFinderForm<T> : Form where T : class, new()
    {
        public event Action<Type, uint> OnReferenceSelected;

        public ReferenceFinderForm()
        {
            InitializeComponent();
        }

        private void FireEvent(Type targetType, uint key)
        {
            OnReferenceSelected?.Invoke(targetType, key);
        }

        public void FindReferences(uint key)
        {
            fastObjectListView1.BeginUpdate();
            Generator<ReferenceInfo>.GenerateColumns(fastObjectListView1);

            var references = DBC.FindReferences<T>(key);
            fastObjectListView1.RebuildColumns();
            fastObjectListView1.SetObjects(references);
            fastObjectListView1.AutoSizeColumns();
            fastObjectListView1.CellClick += (_, a) =>
            {
                if (a.ClickCount != 2)
                    return;

                FireEvent(a.Model.StoreType, a.Model.ReferencerEntry);
                Close();
            };
            fastObjectListView1.EndUpdate();
        }
    }

    public class ReferenceInfo
    {
        [OLVColumn]
        public string ReferencingFile { get; set; }

        [OLVColumn]
        public string ReferencingField { get; set; }

        [OLVColumn]
        public uint ReferencerEntry { get; set; }

        [OLVIgnore]
        public Type StoreType { get; set; }
    }
}
