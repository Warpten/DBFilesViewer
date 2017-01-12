using System;
using System.ComponentModel;

namespace DBFilesViewer.UI.Controls
{
    public class HexConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(string) || base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(string) || base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string) && value is uint)
                return $"0x{value:X8}";

            return base.ConvertTo(context, culture, value, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
        {
            var stringValue = value as string;
            if (stringValue != null)
            {
                if (stringValue.StartsWith("0x", StringComparison.OrdinalIgnoreCase))
                    stringValue = stringValue.Substring(2);

                return uint.Parse(stringValue, System.Globalization.NumberStyles.HexNumber, culture);
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
