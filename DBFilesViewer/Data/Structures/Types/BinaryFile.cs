using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DBFilesClient.NET.Types;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.Data.Structures.Types
{
    /// <summary>
    /// Use this type when you know that your field designates a file within the CASC file system
    /// but you don't know the actual file type.
    /// 
    /// This is mostly of use to programmers or dataminers.
    /// </summary>
    [Editor(typeof(FileEditorControl), typeof(UITypeEditor))]
    public sealed class BinaryFile : IObjectType<uint>
    {
        public BinaryFile(uint underlyingValue) : base(underlyingValue)
        {
        }

        private class FileEditorControl : UITypeEditor
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
                var objectType = value as BinaryFile;

                if (edSvc != null && objectType != null)
                    using (var form = new HexViewForm(objectType.Key))
                        edSvc.ShowDialog(form);

                return value; // can also replace the wrapper object here
            }
        }
    }
}
