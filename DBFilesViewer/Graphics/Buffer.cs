﻿using System;
using DBFilesViewer.Utils;
using SharpDX;
using SharpDX.Direct3D11;

namespace DBFilesViewer.Graphics
{
    public abstract class Buffer<T> : IDisposable where T : struct
    {
        private BufferDescription mDescription;
        private GxContext mContext;

        public SharpDX.Direct3D11.Buffer Native { get; private set; }

        protected Buffer(GxContext context, BindFlags binding)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] {0} created.", GetType().Name);
#endif

            mContext = context;

            mDescription = new BufferDescription
            {
                BindFlags = binding,
                CpuAccessFlags = CpuAccessFlags.None,
                OptionFlags = ResourceOptionFlags.None,
                SizeInBytes = 0,
                StructureByteStride = 0,
                Usage = ResourceUsage.Default
            };
        }

        ~Buffer()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] Disposing {0} {1}.", GetType().Name, disposing ? "manually" : "from destructor");
#endif
            if (Native != null)
            {
                Native.Dispose();
                Native = null;
            }

            mContext = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void UpdateData(T data)
        {
            Resize(SizeCache<T>.Size, data);
        }

        public void UpdateData(T[] data)
        {
            Resize(data.Length * SizeCache<T>.Size, data);
        }

        private void Resize(int length, T value)
        {
            if (length > mDescription.SizeInBytes)
            {
                mDescription.SizeInBytes = length;
                Native?.Dispose();

                using (var strm = new DataStream(length, true, true))
                {
                    strm.Write(value);
                    strm.Position = 0;
                    Native = new SharpDX.Direct3D11.Buffer(mContext.Device, strm, mDescription);
                }
            }
            else
                mContext.Context.UpdateSubresource(ref value, Native);
        }

        private void Resize(int length, T[] data)
        {
            if (length > mDescription.SizeInBytes)
            {
                mDescription.SizeInBytes = length;
                Native?.Dispose();

                if (data != null)
                {
                    using (var strm = new DataStream(length, true, true))
                    {
                        strm.WriteRange(data);
                        strm.Position = 0;
                        Native = new SharpDX.Direct3D11.Buffer(mContext.Device, strm, mDescription);
                    }
                }
                else
                    Native = new SharpDX.Direct3D11.Buffer(mContext.Device, mDescription);
            }
            else
                mContext.Context.UpdateSubresource(data, Native);
        }
    }
}
