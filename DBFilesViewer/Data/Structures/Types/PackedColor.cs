using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.Drawing.Drawing2D;
using System.Globalization;
using DBFilesClient.NET.Types;

namespace DBFilesViewer.Data.Structures.Types
{
    [Editor(typeof (ColorEditor), typeof (UITypeEditor))]
    [TypeConverter(typeof (PackedColorConverter))]
    public class PackedColor : IObjectType<uint>
    {
        private byte[] _bytes;

        public PackedColor(uint underlyingValue) : base(underlyingValue)
        {
            _bytes = BitConverter.GetBytes(underlyingValue);
        }

        private Color Color => Color.FromArgb(0xFF, _bytes[2], _bytes[1], _bytes[0]);

        //! TODO: Declare a type that isnt showing an expandable arrow
        private class PackedColorConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                var instance = value as PackedColor;
                if (destinationType == typeof(string) && instance != null)
                    return $"R: {instance._bytes[2]} G: {instance._bytes[1]} B: {instance._bytes[0]}";

                return base.ConvertTo(context, culture, value, destinationType);
            }

            public override PropertyDescriptorCollection GetProperties(ITypeDescriptorContext context, object value, Attribute[] attributes)
            {
                return new PropertyDescriptorCollection(new PropertyDescriptor[0]);
            }
        }

        #region Interface editor

        private class ColorEditor : UITypeEditor
        {
            public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
            {
                return UITypeEditorEditStyle.None;
            }

            // No editing
            public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
            {
                return null;
            }

            public override bool GetPaintValueSupported(ITypeDescriptorContext context) => true;

            public override void PaintValue(PaintValueEventArgs e)
            {
                e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;
                using (var brush = new SolidBrush(((PackedColor)e.Value).Color))
                    e.Graphics.FillRectangle(brush, e.Bounds.X + 0.5f, e.Bounds.Y + 0.5f, e.Bounds.Width - 1.0f, e.Bounds.Height - 1.0f);

                e.Graphics.DrawRectangle(Pens.DarkSlateGray, e.Bounds.X + 0.5f, e.Bounds.Y + 0.5f, e.Bounds.Width - 1.0f, e.Bounds.Height - 1.0f);
                e.Graphics.SmoothingMode = SmoothingMode.None;
            }
        }
        #endregion
    }
}
