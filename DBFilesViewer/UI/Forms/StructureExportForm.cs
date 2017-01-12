using System;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using DBFilesViewer.Utils.Extensions;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A simple form, mostly of use to developpers, that exports the record structure
    /// as a simple C-style structure for IDA and other possible reverse engineering
    /// tools to understand.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public partial class StructureExportForm<T> : Form where T : class, new()
    {
        public StructureExportForm()
        {
            InitializeComponent();

            base.Text = $"Structure for {typeof (T).Name}.";

            richTextBox1.AppendFormatLine("struct {0}", typeof(T).Name);
            richTextBox1.AppendLine("{");
            foreach (var fieldInfo in typeof (T).GetFields())
            {
                var marshalAttr = fieldInfo.GetCustomAttribute<MarshalAsAttribute>();
                var fieldType = fieldInfo.FieldType;
                if (fieldType.IsArray)
                    fieldType = fieldType.GetElementType();

                if (Type.GetTypeCode(fieldType) == TypeCode.Object && fieldType.BaseType != null)
                    fieldType = fieldType.BaseType.GetGenericArguments()[0];

                switch (Type.GetTypeCode(fieldType))
                {
                    case TypeCode.Boolean:
                        richTextBox1.Append("    bool");
                        break;
                    case TypeCode.Char:
                    case TypeCode.SByte:
                    case TypeCode.Byte:
                        richTextBox1.Append("    _BYTE");
                        break;
                    case TypeCode.Int16:
                    case TypeCode.UInt16:
                        richTextBox1.Append("    _WORD");
                        break;
                    case TypeCode.Int32:
                    case TypeCode.UInt32:
                        richTextBox1.Append("    _DWORD");
                        break;
                    case TypeCode.Int64:
                    case TypeCode.UInt64:
                        richTextBox1.Append("    _QWORD");
                        break;
                    case TypeCode.Single:
                        richTextBox1.Append("    float");
                        break;
                    case TypeCode.Double:
                        richTextBox1.Append("    double");
                        break;
                    case TypeCode.String:
                        richTextBox1.Append("    const char*");
                        break;
                }

                richTextBox1.AppendFormat(" {0}", fieldInfo.Name);
                if (marshalAttr != null)
                    richTextBox1.AppendFormat("[{0}]", marshalAttr.SizeConst);
                richTextBox1.AppendLine(";");
            }
            richTextBox1.Append("};");
        }
    }
}
