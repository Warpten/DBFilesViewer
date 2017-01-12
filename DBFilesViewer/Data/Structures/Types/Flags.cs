using System;
using DBFilesClient.NET.Types;

namespace DBFilesViewer.Data.Structures.Types
{
    /// <summary>
    /// This is a hack and should be replaced ASAP.
    /// </summary>
    /// <typeparam name="T"></typeparam>
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
