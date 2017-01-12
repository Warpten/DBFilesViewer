using System;
using System.ComponentModel;
using System.Globalization;
using System.Reflection;

namespace DBFilesViewer.UI.Controls
{
    public class EnumConverter : System.ComponentModel.EnumConverter
    {
        private Type enumType;

        public EnumConverter(Type type) : base(type)
        {
            enumType = type;
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            var fieldName = Enum.GetName(enumType, value);
            if (string.IsNullOrEmpty(fieldName))
                return value.ToString();

            var fi = enumType.GetField(fieldName);
            var dna = fi.GetCustomAttribute<DescriptionAttribute>();

            return $"{dna?.Description ?? fi.Name} ({Convert.ChangeType(value, Enum.GetUnderlyingType(enumType))})";
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (string.IsNullOrEmpty(value?.ToString()))
                return value;

            foreach (var fi in enumType.GetFields())
            {
                var dna = fi.GetCustomAttribute<DescriptionAttribute>();
                if (dna != null && ((string)value == dna.Description))
                    return Enum.Parse(enumType, fi.Name);
            }
            return Enum.Parse(enumType, value.ToString().Split(' ')[0]);
        }
    }
}
