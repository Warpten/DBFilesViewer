using System;
using SharpDX.Direct3D11;

namespace DBFilesViewer.Graphics
{
    /// <summary>
    /// A wrapper class for blending bound to a specific graphic context instead of an IntPtr.
    /// </summary>
    public class BlendState : IDisposable
    {
        private SharpDX.Direct3D11.BlendState mState;
        private BlendStateDescription mDescription;
        private bool mChanged;
        private GxContext mContext;

        public object Tag { get; set; }

        public bool BlendEnabled
        {
            get { return mDescription.RenderTarget[0].IsBlendEnabled; }
            set { mDescription.RenderTarget[0].IsBlendEnabled = value; mChanged = true; }
        }

        public SharpDX.Direct3D11.BlendState Native
        {
            get
            {
                if (mChanged != true) return mState;

                mState?.Dispose();

                mState = new SharpDX.Direct3D11.BlendState(mContext.Device, mDescription);
                mChanged = false;

                return mState;
            }
        }

        public BlendOption SourceBlend
        {
            get { return mDescription.RenderTarget[0].SourceBlend; }
            set { mDescription.RenderTarget[0].SourceBlend = value; mChanged = true; }
        }

        public BlendOption DestinationBlend
        {
            get { return mDescription.RenderTarget[0].DestinationBlend; }
            set { mDescription.RenderTarget[0].DestinationBlend = value; mChanged = true; }
        }

        public BlendOption SourceAlphaBlend
        {
            get { return mDescription.RenderTarget[0].SourceAlphaBlend; }
            set { mDescription.RenderTarget[0].SourceAlphaBlend = value; mChanged = true; }
        }

        public BlendOption DestinationAlphaBlend
        {
            get { return mDescription.RenderTarget[0].DestinationAlphaBlend; }
            set { mDescription.RenderTarget[0].DestinationAlphaBlend = value; mChanged = true; }
        }

        public BlendState(GxContext context)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] {0} created.", GetType().Name);
#endif
            mContext = context;
            mDescription = new BlendStateDescription
            {
                AlphaToCoverageEnable = false,
                IndependentBlendEnable = false
            };

            mDescription.RenderTarget[0] = new RenderTargetBlendDescription
            {
                IsBlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                BlendOperation = BlendOperation.Add,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                AlphaBlendOperation = BlendOperation.Add,
                RenderTargetWriteMask = ColorWriteMaskFlags.All
            };

            mChanged = true;
        }

        ~BlendState()
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
    }
}
