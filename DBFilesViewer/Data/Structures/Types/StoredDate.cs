using System;
using System.ComponentModel;
using System.Globalization;
using DBFilesClient.NET.Types;

namespace DBFilesViewer.Data.Structures.Types
{
    [TypeConverter(typeof (TimeStampConverter))]
    public class TimeStamp : IObjectType<int>
    {
        public TimeStamp(int underlyingValue) : base(underlyingValue)
        {
        }

        public DateTime Time => new DateTime(2000, 1, 1).Add(new TimeSpan(0, 0, Key));

        private class TimeStampConverter : ExpandableObjectConverter
        {
            public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
            {
                var instance = value as TimeStamp;
                if (destinationType == typeof (string) && instance != null)
                    return instance.Time.ToString(CultureInfo.CurrentCulture);

                return base.ConvertTo(context, culture, value, destinationType);
            }
        }
    }

    /// <summary>
    /// Represents time durations as string.
    /// TODO Inaccurate for now (See HolidaysEntry)
    /// </summary>
    public class TimeDuration : IObjectType<int>
    {
        public TimeDuration(int underlyingValue) : base(underlyingValue)
        {
        }

        public TimeSpan TimeSpan => new TimeSpan(0, 0, Key);

        public override string ToString() => TimeSpan.ToString();
    }
}
