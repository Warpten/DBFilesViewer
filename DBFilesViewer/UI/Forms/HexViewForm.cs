using System.IO;
using System.Windows.Forms;
using Be.Windows.Forms;
using DBFilesViewer.Data.IO.CASC;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A simple binary visualizer for CASC files.
    /// This is mostly of use to developpers when datamining or reversing.
    /// </summary>
    public partial class HexViewForm : Form
    {
        public HexViewForm()
        {
            InitializeComponent();
        }

        public HexViewForm(Stream fileStream) : this()
        {
            if (fileStream != null)
                hexBox1.ByteProvider = new DynamicFileByteProvider(fileStream);
        }

        public HexViewForm(uint fileDataID) : this((Stream) Manager.OpenFile(fileDataID))
        {
            
        }
    }
}
