using System;

namespace DBFilesViewer.Utils
{
    public struct BitSet<T> where T : struct
    {
        private T _value;

        public T Value => _value;

        public BitSet(T defaultValue = default(T))
        {
            _value = defaultValue;
        }

        public unsafe bool this[int index]
        {
            get
            {
                if (index >= SizeCache<T>.BitSize)
                    throw new ArgumentOutOfRangeException();

                // Bit index in the byte
                var bitIndex = index % 8;
                var byteIndex = index / 8;

                var valuePtr = (byte*) SizeCache<T>.GetUnsafePtr(ref _value);
                return (valuePtr[byteIndex] & (1 << bitIndex)) != 0;
            }
            set
            {
                if (index >= SizeCache<T>.BitSize)
                    throw new ArgumentOutOfRangeException();

                var bitIndex = index % 8;
                var byteIndex = index / 8;

                var valuePtr = (byte*)SizeCache<T>.GetUnsafePtr(ref _value);
                if (value)
                    valuePtr[byteIndex] |= (byte)(1 << bitIndex);
                else
                    valuePtr[byteIndex] &= (byte)(~(1 << bitIndex));
            }
        }
    }
}