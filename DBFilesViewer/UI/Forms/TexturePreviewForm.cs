using System.Drawing;
using System.IO;
using System.Windows.Forms;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Graphics.Scene;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A simple form that lets the user visualize textures.
    /// </summary>
    public partial class TexturePreviewForm : Form
    {
        public TexturePreviewForm()
        {
            InitializeComponent();
        }

        public TexturePreviewForm(uint fileDataID) : this(Manager.OpenFile(fileDataID)) { }

        public TexturePreviewForm(Stream imageStream) : this()
        {
            var bitmap = TextureReader.GetBitmap(imageStream);
            ClientSize = new Size(bitmap.Size.Width + 18, bitmap.Size.Height + 30 + 10);
            pictureBox1.Image = bitmap;
        }

        private void ExportTexture(object sender, System.EventArgs e)
        {
            var dialog = new SaveFileDialog
            {
                DefaultExt = ".png",
                Filter = @"JPEG|*.jpg;*.jpeg|PNG|*.png|BMP|*.bmp",
                Title = @"Save as ..."
            };

            if (dialog.ShowDialog() != DialogResult.OK || string.IsNullOrEmpty(dialog.FileName))
                return;

            pictureBox1.Image.Save(dialog.FileName);
        }
    }
}
