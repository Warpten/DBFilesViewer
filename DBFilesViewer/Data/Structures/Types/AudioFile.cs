using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DBFilesClient.NET.Types;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.Data.Structures.Types
{
    /// <summary>
    /// Describes an audio file. Trying to edit the entry through model
    /// will open <see cref="AudioPlayerForm"/>. 
    /// </summary>
    [Editor(typeof(AudioFileEditorControl), typeof(UITypeEditor))]
    public sealed class AudioFile : IObjectType<uint>
    {
        public AudioFile(uint underlyingValue) : base(underlyingValue)
        {
        }

        public class AudioFileEditorControl : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.Modal;
            }

            public override object EditValue(ITypeDescriptorContext context, System.IServiceProvider provider, object value)
            {
                if (context?.Instance == null || provider == null || context.PropertyDescriptor == null)
                    return null;

                var edSvc = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
                var objectType = value as AudioFile;

                if (edSvc != null && objectType != null)
                    using (var form = new AudioPlayerForm(objectType.Key))
                        edSvc.ShowDialog(form);

                return value; // can also replace the wrapper object here
            }
        }
    }
}
