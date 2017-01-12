using System.ComponentModel;
using System.Drawing.Design;
using System.Windows.Forms.Design;
using DBFilesClient.NET.Types;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.Data.Structures.Types
{
    [Editor(typeof(ModelFileEditorControl), typeof(UITypeEditor))]
    public sealed class ModelFile : IObjectType<uint>
    {
        public ModelFile(uint underlyingValue) : base(underlyingValue)
        {
        }

        private class ModelFileEditorControl : UITypeEditor
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
                var objectType = value as ModelFile;

                if (edSvc != null && objectType != null)
                {
                    using (var form = new ModelRenderForm())
                    {
                        form.LoadModel(objectType.Key);
                        edSvc.ShowDialog(form);
                    }
                }

                return value; // can also replace the wrapper object here
            }
        }
    }
}
