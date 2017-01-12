using System;
using System.Windows.Forms;
using DBFilesViewer.Graphics.Files.Models;
using DBFilesViewer.Graphics.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;
using Device = SharpDX.Direct3D11.Device;
using DeviceContext = SharpDX.Direct3D11.DeviceContext;

namespace DBFilesViewer.Graphics
{
    /// <summary>
    /// Encapsulates the logic for creating a DirectX device, and drawing to it.
    /// </summary>
    public class GxContext : IDisposable
    {
        public TextureManager TextureManager { get; private set; }
        public GraphicsDispatcher Dispatcher { get; private set; }

        private readonly Factory1 mFactory;
        private readonly UserControl mWindow;
        private SwapChainDescription mSwapChainDesc;
        private SwapChain mSwapChain;
        private Output mOutput;
        private RenderTargetView mRenderTarget;
        private DepthStencilView mDepthBuffer;
        private Texture2D mDepthTexture;
        private bool mHasMultisample;

        public Device Device { get; private set; }
        public DeviceContext Context { get; private set; }
        public Adapter1 Adapter { get; private set; }
        public ViewportF Viewport => Context.Rasterizer.GetViewports<ViewportF>()[0];

        public SampleDescription Multisampling => mSwapChainDesc.SampleDescription;
        public Format BackBufferFormat => mSwapChainDesc.ModeDescription.Format;

        public event Action<float, float> Resize;

        public ModelShaders ModelShaders { get; private set; }

        public GxContext(UserControl window)
        {
            mWindow = window;
            mFactory = new Factory1();
            if (mFactory.Adapters1.Length == 0)
                throw new InvalidOperationException(
                    "Sorry, but DirectX returned that there is no graphics card installed on your system. Please check if all your drivers are up to date!");

            Adapter = mFactory.GetAdapter1(0);
            if (Adapter.Outputs.Length == 0)
                throw new InvalidOperationException(
                    "Sorry, but DirectX returned that there is no output (monitor) assigned to the graphics card: \"" +
                    Adapter.Description.Description
                    +
                    "\". Please check if your drivers are OK and if your graphics card and monitor show up in the device manager.");

            mOutput = Adapter.Outputs[0];
        }

        public void BeginFrame()
        {
            Context.ClearDepthStencilView(mDepthBuffer, DepthStencilClearFlags.Depth | DepthStencilClearFlags.Stencil,
               1.0f, 0);
            Context.ClearRenderTargetView(mRenderTarget, Color4.Black);
        }

        public void EndFrame()
        {
            mSwapChain.Present(1, PresentFlags.None);
        }

        public void InitContext()
        {
            var modeDesc = new ModeDescription
            {
                Format = Format.R8G8B8A8_UNorm,
                Height = mWindow.ClientSize.Height,
                Width = mWindow.ClientSize.Width,
                RefreshRate = new Rational(60, 1),
                Scaling = DisplayModeScaling.Unspecified,
                ScanlineOrdering = DisplayModeScanlineOrder.Unspecified
            };

            mSwapChainDesc = new SwapChainDescription()
            {
                BufferCount = 1,
                Flags = SwapChainFlags.None,
                IsWindowed = true,
                OutputHandle = mWindow.Handle,
                SwapEffect = SwapEffect.Discard,
                Usage = Usage.RenderTargetOutput
            };

            mOutput.GetClosestMatchingMode(null, modeDesc, out modeDesc);
            modeDesc.Width = mWindow.ClientSize.Width;
            modeDesc.Height = mWindow.ClientSize.Height;
            mSwapChainDesc.ModeDescription = modeDesc;

#if DEBUG
            Device = new Device(Adapter, DeviceCreationFlags.Debug);
#else
            Device = new Device(Adapter);
#endif

            BuildMultisample();

            mSwapChain = new SwapChain(mFactory, Device, mSwapChainDesc);

            Context = Device.ImmediateContext;

            InitRenderTarget();
            InitDepthBuffer();

            Context.OutputMerger.SetTargets(mDepthBuffer, mRenderTarget);
            Context.Rasterizer.SetViewport(new Viewport(0, 0, mWindow.ClientSize.Width, mWindow.ClientSize.Height));

            Texture.InitDefaultTexture(this);
            TextureManager = new TextureManager(this);

            mWindow.Resize += OnResize;

            ModelShaders = new ModelShaders();
            ModelShaders.Initialize(Context);

            Dispatcher = new GraphicsDispatcher();
            Dispatcher.AssignToThread();
        }

        private void OnResize(object sender, EventArgs args)
        {
            if (Device == null)
                return;

            Dispatcher.Enqueue(() =>
            {
                Context.OutputMerger.SetTargets((DepthStencilView) null, (RenderTargetView) null);
                mRenderTarget.Dispose();
                mDepthBuffer.Dispose();
                mDepthTexture.Dispose();

                mSwapChain.ResizeBuffers(0, 0, 0, Format.Unknown, SwapChainFlags.None);
                InitRenderTarget();
                InitDepthBuffer();

                Context.OutputMerger.SetTargets(mDepthBuffer, mRenderTarget);
                Context.Rasterizer.SetViewport(new Viewport(0, 0, mWindow.ClientSize.Width, mWindow.ClientSize.Height));

                Resize?.Invoke(mWindow.ClientSize.Width, mWindow.ClientSize.Height);
            });
        }

        private void BuildMultisample()
        {
#if DEBUG
            mHasMultisample = false;
            mSwapChainDesc.SampleDescription = new SampleDescription(1, 0);
#else
            var maxCount = 1;
            var maxQuality = 0;
            for (var i = 0; i <= Device.MultisampleCountMaximum; ++i)
            {
                var quality = Device.CheckMultisampleQualityLevels(Format.R8G8B8A8_UNorm, i);
                if (quality <= 0) continue;

                maxCount = i;
                maxQuality = quality - 1;
            }

            mSwapChainDesc.SampleDescription = new SampleDescription(maxCount, maxQuality);
            mHasMultisample = maxQuality > 0 || maxCount > 1;
#endif
        }

        private void InitRenderTarget()
        {
            using (var tex = mSwapChain.GetBackBuffer<Texture2D>(0))
            {
                var rtv = new RenderTargetViewDescription()
                {
                    Dimension = mHasMultisample ? RenderTargetViewDimension.Texture2DMultisampled : RenderTargetViewDimension.Texture2D,
                    Format = Format.R8G8B8A8_UNorm,
                };

                if (mHasMultisample)
                    rtv.Texture2DMS = new RenderTargetViewDescription.Texture2DMultisampledResource();
                else
                    rtv.Texture2D = new RenderTargetViewDescription.Texture2DResource { MipSlice = 0 };

                mRenderTarget = new RenderTargetView(Device, tex, rtv);
            }
        }

        private void InitDepthBuffer()
        {
            var texDesc = new Texture2DDescription()
            {
                ArraySize = 1,
                BindFlags = BindFlags.DepthStencil,
                CpuAccessFlags = CpuAccessFlags.None,
                Format = Format.D24_UNorm_S8_UInt,
                Height = mWindow.ClientSize.Height,
                Width = mWindow.ClientSize.Width,
                MipLevels = 1,
                OptionFlags = ResourceOptionFlags.None,
                SampleDescription = mSwapChainDesc.SampleDescription,
                Usage = ResourceUsage.Default
            };

            mDepthTexture = new Texture2D(Device, texDesc);

            var dsvd = new DepthStencilViewDescription
            {
                Dimension =
                    mHasMultisample
                        ? DepthStencilViewDimension.Texture2DMultisampled
                        : DepthStencilViewDimension.Texture2D,
                Format = Format.D24_UNorm_S8_UInt
            };

            if (mHasMultisample)
                dsvd.Texture2DMS = new DepthStencilViewDescription.Texture2DMultisampledResource();
            else
                dsvd.Texture2D = new DepthStencilViewDescription.Texture2DResource { MipSlice = 0 };

            mDepthBuffer = new DepthStencilView(Device, mDepthTexture, dsvd);
        }

        ~GxContext()
        {
            Dispose(false);
        }

        private void Dispose(bool disposing)
        {
            if (mDepthBuffer != null)
            {
                mDepthBuffer.Dispose();
                mDepthBuffer = null;
            }

            if (mRenderTarget != null)
            {
                mRenderTarget.Dispose();
                mRenderTarget = null;
            }

            if (mDepthTexture != null)
            {
                mDepthTexture.Dispose();
                mDepthTexture = null;
            }

            if (mSwapChain != null)
            {
                mSwapChain.Dispose();
                mSwapChain = null;
            }

            if (mOutput != null)
            {
                mOutput.Dispose();
                mOutput = null;
            }

            if (Adapter != null)
            {
                Adapter.Dispose();
                Adapter = null;
            }

            if (Device != null)
            {
                Device.Dispose();
                Device = null;
            }

            try
            {
                if (Context != null)
                {
                    Context.Dispose();
                    Context = null;
                }
            }
            catch { }

            if (TextureManager != null)
            {
                TextureManager.Dispose();
                TextureManager = null;
            }
        }

        public virtual void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
