using System;
using System.Drawing;
using System.Threading.Tasks;
using System.Windows.Forms;
using DBFilesViewer.Data.IO.Files;
using DBFilesViewer.Graphics.Files.Terrain;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.Data.Structures.Types
{
    /// <summary>
    /// This attribute defines a set of buttons that will be added
    /// to the PropertyGrid when the record it is bound to is inspected.
    /// 
    /// See <see cref="CreatureDisplayInfoEntry"/> and <see cref="ItemDisplayInfoEntry"/>
    /// for example usages.
    /// </summary>
    public abstract class OptionalButtonAttribute : Attribute
    {
        /// <summary>
        /// If set to true, only the defined bitmap will render.
        /// </summary>
        public bool ImageOnly { get; set; } = true;

        public Bitmap Image
        {
            set
            {
                if (ImageOnly)
                    Button.Text = null;
                Button.Image = value;
            }
        }

        public string Text
        {
            set
            {
                if (!ImageOnly)
                    Button.Image = null;
                Button.Text = value;
            }
        }

        public ToolStripButton Button { get; } = new ToolStripButton { AutoSize = true };

        //! TODO This is not ideal in the least bit, but i don't want
        //! reflection
        public Func<object> TagGetter { get; set; }

        /// <summary>
        /// Called when the associated button is clicked.
        /// 
        /// To access the key of the record currently viewed in the details tab,
        /// see Button.Tag.
        /// </summary>
        public event EventHandler Click
        {
            add { Button.Click += value; }
            remove { Button.Click -= value; }
        }
    }

    /// <summary>
    /// A specialization of <see cref="OptionalButtonAttribute"/> designed to open models.
    /// </summary>
    public sealed class ViewCreatureModelButtonAttribute : OptionalButtonAttribute
    {
        public ViewCreatureModelButtonAttribute()
        {
            ImageOnly = false;
            Text = "Model viewer";

            Click += (sender, args) =>
            {
                var renderForm = new CreatureModelViewerForm();
                renderForm.OnAnimationsLoaded += animations => {
                    renderForm.SetAnimationSource(animations);
                };
                renderForm.LoadModel((uint)TagGetter());
                renderForm.ShowDialog();
            };
        }
    }

    /// <summary>
    /// A specialization of <see cref="OptionalButtonAttribute"/> designed to open models.
    /// </summary>
    public sealed class ViewItemModelButtonAttribute : OptionalButtonAttribute
    {
        public ViewItemModelButtonAttribute()
        {
            ImageOnly = false;
            Text = "Model viewer";

            Click += (sender, args) =>
            {
                var renderForm = new ItemModelViewerForm();
                renderForm.LoadModel((uint)TagGetter());
                renderForm.ShowDialog();
            };
        }
    }

    /// <summary>
    /// A specialization of <see cref="OptionalButtonAttribute"/> designed to open
    /// ADTs.
    /// </summary>
    public sealed class ViewTerrainButtonAttribute : OptionalButtonAttribute
    {
        public ViewTerrainButtonAttribute()
        {
            ImageOnly = false;
            Text = "Terrain viewer";

            Click += (sender, args) =>
            {
                Task.Factory.StartNew(() =>
                {
                    var terrain = new Terrain(DBC.Get<MapEntry>()[(uint)TagGetter()].Directory);
                });
            };
        }
    }
}
