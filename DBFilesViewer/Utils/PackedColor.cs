using System;
using System.ComponentModel;
using DBFilesClient.NET.Types;
using System.Drawing;
using System.Drawing.Design;
using RelDBC.Controls;

namespace RelDBC.Utils
{
    [Editor(typeof(ColorEditor), typeof(UITypeEditor))]
    public class PackedColor : IObjectType<uint>
    {
        private byte[] _bytes;

        public PackedColor(uint underlyingValue) : base(underlyingValue)
        {
            _bytes = BitConverter.GetBytes(underlyingValue);
        }

        public Color Color => Color.FromArgb(0xFF, _bytes[2], _bytes[1], _bytes[0]);
    }
}
