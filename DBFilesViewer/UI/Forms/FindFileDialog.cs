using System;
using System.Drawing;
using System.Windows.Forms;
using DBFilesViewer.Data.IO.CASC;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A simple utility form designed to let you visualize binary data for a file,
    /// if it exists with the CASC file system.
    /// </summary>
    public partial class FindFileDialog : Form
    {
        public FindFileDialog()
        {
            InitializeComponent();
        }

        ~FindFileDialog()
        {
            _fileStream = null;
        }

        private string _fileName;
        private BLTE _fileStream;

        public void OnClick(object sender, EventArgs args)
        {
            _fileName = textBox1.Text;
            if ((string) comboBox1.SelectedItem == "Path")
                _fileStream = Manager.OpenFile(_fileName);
            else
                _fileStream = Manager.OpenFile(Convert.ToUInt32(_fileName));

            _fileExistsPanel.Visible = _fileStream != null;
            _viewFileButton.Visible = _fileStream != null;

            _fileNotFoundPanel.Visible = _fileStream == null;
        }

        private void OnPaintSuccessBox(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, _fileExistsPanel.ClientRectangle, Color.FromArgb(39, 174, 96), ButtonBorderStyle.Solid);
        }

        private void OnPaintErrorBox(object sender, PaintEventArgs e)
        {
            ControlPaint.DrawBorder(e.Graphics, _fileNotFoundPanel.ClientRectangle, Color.FromArgb(200, 54, 16), ButtonBorderStyle.Solid);
        }

        private void OpenFileAsHex(object sender, EventArgs e)
        {
            if (_fileStream != null)
                new HexViewForm(_fileStream).ShowDialog();
        }
    }
}
