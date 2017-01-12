using System;
using System.Globalization;
using DBFilesClient.NET.Types;

namespace RelDBC.Utils
{
    public class TimeStamp : IObjectType<int>
    {
        public TimeStamp(int underlyingValue) : base(underlyingValue)
        {
        }

        public DateTime Time => new DateTime(2000, 1, 1).Add(new TimeSpan(0, 0, Key));

        public override string ToString() => Time.ToString(CultureInfo.CurrentCulture);
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
