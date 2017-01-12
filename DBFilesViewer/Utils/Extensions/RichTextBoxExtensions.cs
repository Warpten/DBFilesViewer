using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace RelDBC.Utils.Extensions
{
    public static class RichTextBoxExtensions
    {
        public const string DefaultFamily = "Arial Unicode MS";
        public const float DefaultSize = 9f;

        public static void AppendFormatLine(this RichTextBox textbox, string format, params object[] arg0)
        {
            textbox.AppendText(string.Format(format, arg0) + Environment.NewLine);
        }

        public static void AppendFormat(this RichTextBox textbox, string format, params object[] arg0)
        {
            textbox.AppendText(string.Format(format, arg0));
        }

        public static void AppendLine(this RichTextBox textbox)
        {
            textbox.AppendText(Environment.NewLine);
        }

        public static void AppendLine(this RichTextBox textbox, string text)
        {
            textbox.AppendText(text + Environment.NewLine);
        }

        public static void Append(this RichTextBox textbox, object text)
        {
            textbox.AppendText(text.ToString());
        }

        public static void AppendFormatLineIfNotNull(this RichTextBox builder, string format, uint arg)
        {
            if (arg != 0)
            {
                builder.AppendFormatLine(format, arg);
            }
        }

        public static void AppendFormatLineIfNotNull(this RichTextBox builder, string format, float arg)
        {
            if (Math.Abs(arg) > 1.0E-5f)
            {
                builder.AppendFormatLine(format, arg);
            }
        }

        public static void AppendFormatLineIfNotNull(this RichTextBox builder, string format, string arg)
        {
            if (!string.IsNullOrEmpty(arg))
            {
                builder.AppendFormatLine(format, arg);
            }
        }

        public static void AppendFormatIfNotNull(this RichTextBox builder, string format, uint arg)
        {
            if (arg != 0)
            {
                builder.AppendFormat(format, arg);
            }
        }

        public static void AppendFormatIfNotNull(this RichTextBox builder, string format, float arg)
        {
            if (Math.Abs(arg) > 1.0E-5f)
            {
                builder.AppendFormat(format, arg);
            }
        }

        public static void SetStyle(this RichTextBox textbox, Color color, FontStyle style)
        {
            textbox.SelectionColor = color;
            textbox.SelectionFont = new Font(DefaultFamily, DefaultSize, style);
        }

        public static void SetBold(this RichTextBox textbox)
        {
            textbox.SelectionFont = new Font(DefaultFamily, DefaultSize, FontStyle.Bold);
        }

        public static void SetDefaultStyle(this RichTextBox textbox)
        {
            textbox.SelectionFont = new Font(DefaultFamily, DefaultSize, FontStyle.Regular);
            textbox.SelectionColor = Color.Black;
        }

        static string[] keywords = { "struct", "const",
            "QWORD", "DWORD", "WORD", "char",
            "QWORD*", "DWORD*", "WORD*", "char*",
        };

        public static void ColorizeCode(this RichTextBox rtb)
        {
            var text = rtb.Text;

            rtb.SelectAll();
            rtb.SelectionColor = rtb.ForeColor;

            foreach (var keyword in keywords)
            {
                var keywordPos = rtb.Find(keyword, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);

                while (keywordPos != -1)
                {
                    var newLinePos = text.LastIndexOf("\n", keywordPos, StringComparison.OrdinalIgnoreCase);

                    keywordPos = rtb.Find(keyword, keywordPos + rtb.SelectionLength, RichTextBoxFinds.MatchCase | RichTextBoxFinds.WholeWord);
                }
            }

            rtb.Select(0, 0);
        }
    }
}
