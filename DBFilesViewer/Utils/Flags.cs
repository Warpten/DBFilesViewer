using System;
using DBFilesClient.NET.Types;

namespace RelDBC.Utils
{
    public class Flags<T> : IObjectType<T> where T : struct, IFormattable
    {
        public Flags(T underlyingValue) : base(underlyingValue)
        {
        }

        public override string ToString()
        {
            return $"0x{Key:X8}";
        }
    }
}
