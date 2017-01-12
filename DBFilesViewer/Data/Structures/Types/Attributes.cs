using System;
using System.Drawing;
using System.Windows.Forms;

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

        public delegate Form OpenFormDelegate(uint recordKey);

        /// <summary>
        /// This delegate is called when the button is clicked - it expects
        /// a form to be returned.
        /// </summary>
        /// <remarks>
        /// Should you want it to return void, you will need to implement another type of button,
        /// until the C# committee decides to implement generic attributes. Which is really never
        /// gonna happen.
        /// </remarks>
        public OpenFormDelegate OpenForm { get; set; }
    }

    /// <summary>
    /// A specialization of <see cref="OptionalButtonAttribute"/> designed to open
    /// models.
    /// </summary>
    public sealed class ViewModelButtonAttribute : OptionalButtonAttribute
    {
        public ViewModelButtonAttribute(Type delegateType, string delegateName, params Type[] args)
        {
            ImageOnly = false;
            Text = "Model viewer";

            OpenForm = (OpenFormDelegate)Delegate.CreateDelegate(typeof(OpenFormDelegate), delegateType.GetMethod(delegateName, args));
        }
    }
}
