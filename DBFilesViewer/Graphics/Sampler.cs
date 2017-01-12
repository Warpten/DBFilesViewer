using System;
using SharpDX.Direct3D11;

namespace DBFilesViewer.Graphics
{
    public class Sampler : IDisposable
    {
        private SamplerState mState;
        private SamplerStateDescription mDescription;
        private GxContext mContext;
        private bool mChanged;

        ~Sampler()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] Disposing {0} {1}.", GetType().Name, disposing ? "manually" : "from destructor");
#endif
            if (mState != null)
            {
                mState.Dispose();
                mState = null;
            }

            mContext = null;
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public TextureAddressMode AddressU
        {
            set
            {
                mDescription.AddressU = value;
                mChanged = true;
            }
            get { return mDescription.AddressU; }
        }

        public TextureAddressMode AddressV
        {
            set
            {
                mDescription.AddressV = value;
                mChanged = true;
            }
            get { return mDescription.AddressV; }
        }

        public TextureAddressMode AddressW
        {
            set
            {
                mDescription.AddressW = value;
                mChanged = true;
            }
            get { return mDescription.AddressW; }
        }

        public Filter Filter
        {
            set { mDescription.Filter = value; mChanged = true; }
            get { return mDescription.Filter; }
        }

        public int MaximumAnisotropy
        {
            set { mDescription.MaximumAnisotropy = value; mChanged = true; }
            get { return mDescription.MaximumAnisotropy; }
        }

        public Sampler(GxContext context)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] {0} created.", GetType().Name);
#endif

            mContext = context;
            mDescription = new SamplerStateDescription
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Wrap,
                BorderColor = SharpDX.Color4.Black,
                ComparisonFunction = Comparison.Always,
                Filter = Filter.MinMagMipLinear,
                MaximumAnisotropy = 0,
                MaximumLod = float.MaxValue,
                MinimumLod = float.MinValue,
                MipLodBias = 0.0f,
            };

            mChanged = true;
        }

        public SamplerState Native
        {
            get
            {
                if (!mChanged) return mState;
                mState?.Dispose();

                mState = new SamplerState(mContext.Device, mDescription);
                mChanged = false;

                return mState;
            }
        }

    }
}
