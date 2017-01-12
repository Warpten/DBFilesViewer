using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DBFilesClient.NET.Types;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.Data.Structures.Types
{
    /// <summary>
    /// Represents a direct link into CASC Root, using FileDataID.
    /// </summary>
    [Editor(typeof(TextureEditorControl), typeof(UITypeEditor))]
    public sealed class TextureFile : IObjectType<uint>
    {
        public TextureFile(uint underlyingValue) : base(underlyingValue)
        {
        }

        private class TextureEditorControl : UITypeEditor
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
                var objectType = value as TextureFile;

                if (edSvc != null && objectType != null)
                    using (var form = new TexturePreviewForm((uint)objectType.Key))
                        edSvc.ShowDialog(form);

                return value; // can also replace the wrapper object here
            }
        }
    }
}
