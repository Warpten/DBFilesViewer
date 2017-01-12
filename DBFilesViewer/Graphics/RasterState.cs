using System;
using SharpDX.Direct3D11;

namespace DBFilesViewer.Graphics
{
    /// <summary>
    /// A wrapper class designed to implement culling in DirectX.
    /// </summary>
    /// <remarks>
    /// https://bitbucket.org/mugadr_m/neo
    /// </remarks>
    public class RasterState : IDisposable
    {
        private RasterizerState mState;
        private RasterizerStateDescription mDescription;
        private bool mChanged;
        private GxContext mContext;

        public bool FarClipEnabled
        {
            get { return mDescription.IsDepthClipEnabled; }
            set
            {
                mDescription.IsDepthClipEnabled = value;
                mChanged = true;
            }
        }

        public bool CullEnabled
        {
            get { return mDescription.CullMode != CullMode.None; }
            set { mDescription.CullMode = value ? CullMode.Back : CullMode.None; mChanged = true; }
        }

        public bool CullCounterClock
        {
            get { return !mDescription.IsFrontCounterClockwise; }
            set { mDescription.IsFrontCounterClockwise = !value; mChanged = true; }
        }

        public bool Wireframe
        {
            get { return mDescription.FillMode == FillMode.Wireframe; }
            set { mDescription.FillMode = value ? FillMode.Wireframe : FillMode.Solid; mChanged = true; }
        }

        public RasterizerState Native
        {
            get
            {
                if (!mChanged) return mState;

                mState?.Dispose();
                mState = new RasterizerState(mContext.Device, mDescription);
                mChanged = false;

                return mState;
            }
        }

        public RasterState(GxContext context)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] {0} created.", GetType().Name);
#endif

            mContext = context;
            mDescription = new RasterizerStateDescription
            {
                CullMode = CullMode.None,
                DepthBias = 0,
                DepthBiasClamp = 0.0f,
                FillMode = FillMode.Solid,
                IsAntialiasedLineEnabled = false,
                IsDepthClipEnabled = true,
                IsFrontCounterClockwise = true,
                IsMultisampleEnabled = false,
                IsScissorEnabled = false,
                SlopeScaledDepthBias = 0.0f
            };

            mChanged = true;
        }

        ~RasterState()
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
