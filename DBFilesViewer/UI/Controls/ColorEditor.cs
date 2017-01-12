using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using RelDBC.Utils;

namespace RelDBC.Controls
{
    public class ColorEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.DropDown;
        }

        // No editing
        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            if (value == null)
                return null;

            if (value.GetType() != typeof (PackedColor))
                return value;

            var packedColor = value as PackedColor;
            if (packedColor == null)
                return value;

            return $"R: {packedColor.Color.R:X2} G: {packedColor.Color.G:X2} B: {packedColor.Color.B:X2}";
        }

        public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

        public override void PaintValue(PaintValueEventArgs e)
        {
            using (var brush = new SolidBrush(((PackedColor)e.Value).Color))
                e.Graphics.FillRectangle(brush, e.Bounds);

            e.Graphics.SmoothingMode = SmoothingMode.None;
            e.Graphics.DrawRectangle(Pens.DarkSlateGray, e.Bounds);
        }
    }
}
