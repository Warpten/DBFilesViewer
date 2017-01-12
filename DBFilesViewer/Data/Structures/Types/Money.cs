using System;
using System.ComponentModel;
using System.Globalization;
using DBFilesClient.NET.Types;

namespace DBFilesViewer.Data.Structures.Types
{
    [TypeConverter(typeof (MoneyConverter))]
    public class Money : IObjectType<uint>
    {
        public Money(uint underlyingValue) : base(underlyingValue)
        {
            _gold   = Key / (100 * 100);
            _silver = (Key / 100) % 100;
            _copper = Key % 100;
        }

        private uint _gold;
        private uint _silver;
        private uint _copper;

        private class MoneyConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                var instance = value as Money;
                if (destinationType == typeof(string) && instance != null)
                    return $"{instance._gold}g {instance._silver}s {instance._copper}c";

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }
}
