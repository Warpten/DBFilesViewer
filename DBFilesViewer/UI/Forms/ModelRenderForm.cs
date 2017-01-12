using System;
using System.Collections.Generic;
using System.Windows.Forms;
using DBFilesViewer.Graphics;
using DBFilesViewer.Graphics.Files.Models.Animations;
using DBFilesViewer.Graphics.Scene;
using SharpDX;
using SharpDX.Direct3D11;
using SharpDX.DXGI;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A generic class designed to render models.
    /// 
    /// Provides a couple of events that let the owning object modify the way the model is loaded.
    /// See <see cref="CreatureModelViewerForm"/> for example use.
    /// </summary>
    public partial class ModelRenderForm
    {
        protected GxContext Context { get; }

        protected PerspectiveCamera Camera { get; }
        private CameraControl CameraControl { get; }

        private ConstantBuffer<Matrix> _matrixBuffer;
        private Texture2D _resolveTexture;
        private Texture2D _mapTexture;

        private List<ModelRenderer> Renderers { get; } = new List<ModelRenderer>();

        public ModelRenderForm()
        {
            InitializeComponent();

            Context = new GxContext(modelRenderControl1);
            Context.InitContext();

            _matrixBuffer = new ConstantBuffer<Matrix>(Context);

            Camera = new PerspectiveCamera();
            Camera.ViewChanged += ViewChanged;
            Camera.ProjectionChanged += ProjChanged;
            Camera.SetClip(0.2f, 1000.0f);
            Camera.SetParameters(new Vector3(10, 0, 0), Vector3.Zero, Vector3.UnitZ, Vector3.UnitY);
            CameraControl = new CameraControl(modelRenderControl1)
            {
                TurnFactor = 0.1f,
                SpeedFactor = 15.0f
            };

            Resize += OnResize;

            OnResize(this, null);

            modelRenderControl1.OnVerticalScroll += amount => Camera.MoveForward(amount / CameraControl.SpeedFactor * 1.25f);
            modelRenderControl1.OnRenderFrame += () =>
            {
                CameraControl.Update(Camera, false);
                Context.Dispatcher.Enqueue(OnRenderModel);
            };
            modelRenderControl1.Focus();
        }

        public virtual ModelRenderer LoadModel(uint fileDataID, bool clampCamera = true)
        {
            var renderer = new ModelRenderer(fileDataID, Context);

            var bboxMin = renderer.Model.MD20.BoundingBox.Minimum.Z;
            var bboxMax = renderer.Model.MD20.BoundingBox.Maximum.Z;
            if (clampCamera)
                Context.Dispatcher.Enqueue(() =>
                {
                    Camera.SetParameters(
                        new Vector3(renderer.Model.MD20.BoundingSphereRadius * 1.30f, 0, bboxMin + (bboxMax - bboxMin) / 2.0f),
                        new Vector3(0, 0, bboxMin + (bboxMax - bboxMin) / 2.0f), Vector3.UnitZ, Vector3.UnitY);
                });

            Renderers.Add(renderer);
            Text = $"{renderer.Model.Name} (FileDataID {fileDataID})";
            return renderer;
        }

        private void OnResize(object sender, EventArgs args)
        {
            Context.Dispatcher.Enqueue(() =>
            {
                var texDesc = new Texture2DDescription {
                    ArraySize = 1,
                    BindFlags = BindFlags.None,
                    CpuAccessFlags = CpuAccessFlags.None,
                    Format = Format.B8G8R8A8_UNorm,
                    Height = ClientSize.Height,
                    Width = ClientSize.Width,
                    Usage = ResourceUsage.Default,
                    SampleDescription = new SampleDescription(1, 0),
                    OptionFlags = ResourceOptionFlags.None,
                    MipLevels = 1
                };

                _resolveTexture?.Dispose();
                _resolveTexture = new Texture2D(Context.Device, texDesc);

                _mapTexture?.Dispose();

                texDesc.CpuAccessFlags = CpuAccessFlags.Read;
                texDesc.Usage = ResourceUsage.Staging;
                _mapTexture = new Texture2D(Context.Device, texDesc);

                Camera.SetAspect((float)ClientSize.Width / ClientSize.Height);
            });
        }

        private void ViewChanged(Camera cam, Matrix matView) => Context.Dispatcher.Enqueue(() => _matrixBuffer.UpdateData(cam.ViewProjection));

        private void ProjChanged(Camera cam, Matrix matProj) => Context.Dispatcher.Enqueue(() => _matrixBuffer.UpdateData(cam.ViewProjection));

        private void OnRenderModel()
        {
            if (Renderers.Count == 0)
                return;

            Context.BeginFrame();

            Context.Context.Rasterizer.SetViewport(new Viewport(0, 0, ClientSize.Width, ClientSize.Height, 0.0f, 1.0f));
            Context.Context.VertexShader.SetConstantBuffer(0, _matrixBuffer.Native);

            // Define the surface that always faces the camera.
            var billboardParameters = new BillboardParameters
            {
                Forward = Camera.Forward,
                Right = Camera.Right,
                Up = Camera.Up,
                InverseRotation = Matrix.Invert(Matrix.RotationYawPitchRoll(0, 0, 0))
            };

            foreach (var renderer in Renderers)
                renderer.Render(billboardParameters);

            Context.Dispatcher.ProcessFrame();

            Context.EndFrame();
        }

        private void OnClose(object sender, FormClosingEventArgs e)
        {
#if DEBUG
            Console.WriteLine("[DEBUG] Closing model viewer.");
#endif
            _renderTimer.Stop();

            foreach (var renderer in Renderers)
                renderer.Dispose();
            Renderers.Clear();
        }
    }
}
