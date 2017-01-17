using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Graphics.Files;
using DBFilesViewer.Graphics.Files.Models;
using DBFilesViewer.Graphics.Files.Models.Animations;
using DBFilesViewer.Graphics.Files.Models.Attachments;
using DBFilesViewer.Resources;
using DBFilesViewer.Utils;
using SharpDX;
using SharpDX.Direct3D11;

namespace DBFilesViewer.Graphics.Scene
{
    public sealed class ModelRenderer : IDisposable
    {
        /// <summary>
        /// An instance of the actual model file.
        /// </summary>
        public M2 Model { get; }

        public List<ModelAttachment> Attachments { get; } = new List<ModelAttachment>(20);

        /// <summary>
        /// This event fires and allows you to load textures from texture
        /// type, using DBC data.
        /// </summary>
        public event Func<uint /* tex type */, Texture /* tex */> OnLoadTexture;

        /// <summary>
        /// When this event fires, it allows you to define wether or not a
        /// submesh (also called a render pass) should draw.
        /// </summary>
        public event Action<List<ModelRenderPass>> OnFilterMeshes;

        #region Rendering data
        private Dictionary<int, Texture> _textureCache = new Dictionary<int, Texture>();

        private ModelAnimator _animator;

        public VertexBuffer<M2Vertex> VertexBuffer { get; }
        public IndexBuffer IndexBuffer { get; }

        private Mesh<M2Vertex> _mesh;

        private Matrix[] _animationMatrices;
        private ConstantBuffer<Matrix> BonesAnimationMatrices { get; set; }

        private Sampler _samplerWrapU, _samplerWrapV, _samplerWrapUV, _samplerClamp;

        private BlendState[] _blendStates = new BlendState[8];

        private RasterState _noCullState, _cullState;

        private GxContext _drawContext;

        private PerModelPassBuffer _modelRenderData;
        private ConstantBuffer<PerModelPassBuffer> PerPassBuffer { get; set; }
        #endregion

        public ModelRenderer(uint fileDataID, GxContext context)
        {
            Model = new M2(Manager.OpenFile(fileDataID));

            _drawContext = context;

            VertexBuffer = new VertexBuffer<M2Vertex>(_drawContext);
            IndexBuffer = new IndexBuffer(_drawContext);

            VertexBuffer.UpdateData(Model.MD20.Vertices);
            IndexBuffer.UpdateData(Model.Indices);

            _animationMatrices = new Matrix[256];
            for (var i = 0; i < _animationMatrices.Length; ++i)
                _animationMatrices[i] = Matrix.Identity;

            BonesAnimationMatrices = new ConstantBuffer<Matrix>(_drawContext);
            BonesAnimationMatrices.UpdateData(_animationMatrices);

            PerPassBuffer = new ConstantBuffer<PerModelPassBuffer>(_drawContext);
            _modelRenderData = new PerModelPassBuffer
            {
                uvAnimMatrix1 = Matrix.Identity,
                AlphaRef = 1.0f / 255.0f,
                animatedColor = Vector4.One,
                modelPosition = Matrix.Identity
            };
            PerPassBuffer.UpdateData(_modelRenderData);

            _animator = new ModelAnimator(this);
            _animator.SetAnimation(0);

            Initialize();
        }

        /// <summary>
        /// Prepares rendering this model by filtering meshes if needed and loading textures.
        /// </summary>
        public void PrepareRender()
        {
            OnFilterMeshes?.Invoke(Model.RenderPasses);

            // var sectionMapping = new Dictionary<SectionType /* section */, CharComponentTextureSectionsEntry[] /* ld, hd */>();
            // foreach (var layout in DBC.Get<CharComponentTextureSectionsEntry>())
            // {
            //     CharComponentTextureSectionsEntry[] pair;
            //     if (!sectionMapping.TryGetValue(layout.Value.Section, out pair))
            //         sectionMapping[layout.Value.Section] = pair = new CharComponentTextureSectionsEntry[2];
            // 
            //     var hdLayout = layout.Value.Layout.Value.Height != layout.Value.Layout.Value.Width;
            //     pair[hdLayout ? 1 : 0] = layout.Value;
            // }

            foreach (var renderPass in Model.RenderPasses)
            {
                for (var i = 0; i < renderPass.OpCount && i < 4 && i < renderPass.TextureIndices.Count; ++i)
                {
                    if (_textureCache.ContainsKey(renderPass.TextureIndices[i]))
                        continue;

                    Texture tex = null;
                    if (Model.MD20.Textures[renderPass.TextureIndices[i]].Type != 0 && OnLoadTexture != null)
                        tex = OnLoadTexture(Model.MD20.Textures[renderPass.TextureIndices[i]].Type);

                    if (tex == null)
                        tex = _drawContext.TextureManager.GetTexture(Model.GetTextureName(renderPass.TextureIndices[i]));

                    _textureCache[renderPass.TextureIndices[i]] = tex;
                }
            }

            VertexBuffer.UpdateData(Model.MD20.Vertices);
        }

        public void Render(BillboardParameters billboard)
        {
            _mesh.InitLayout(_mesh.Program);
            _mesh.BlendState = null;
            _mesh.BeginDraw();
            _mesh.UpdateIndexBuffer(IndexBuffer);
            _mesh.UpdateVertexBuffer(VertexBuffer);

            if (_animator != null)
            {
                _animator.Update(billboard);
                if (_animator.GetBonesAnimationMatrix(_animationMatrices))
                    BonesAnimationMatrices.UpdateData(_animationMatrices);
            }

            _mesh.Program.SetVertexConstantBuffer(1, BonesAnimationMatrices);
            _mesh.Program.SetVertexConstantBuffer(2, PerPassBuffer);
            _mesh.Program.SetPixelConstantBuffer(0, PerPassBuffer);

            foreach (var renderPass in Model.RenderPasses)
            {
                if (!renderPass.Enabled)
                    continue;

                // var cullingDisabled = (renderPass.RenderFlag & 0x04) != 0;
                _mesh.UpdateRasterizerState(renderPass.RenderFlag.CullingDisabled ? _noCullState : _cullState);
                _mesh.UpdateBlendState(_blendStates[renderPass.BlendMode]);

                if (renderPass.Program == null)
                {
                    renderPass.Program = new ShaderProgram(_drawContext);
                    renderPass.Program.SetPixelShader(_drawContext.ModelShaders.GetPortraitPixelShader(renderPass.PixelShaderType));
                    renderPass.Program.SetVertexShader(Shaders.M2VertexPortrait);
                }

                _mesh.Program = renderPass.Program;
                _mesh.Program.Bind();

                _modelRenderData.LightCoefficient = renderPass.RenderFlag.Unlit ? 0.0f : 1.0f;
                _modelRenderData.FogCoefficient = renderPass.RenderFlag.Unfogged ? 0.0f : 1.0f;

                _mesh.StartVertex = 0;
                _mesh.StartIndex = renderPass.StartIndex;
                _mesh.IndexCount = renderPass.IndexCount;

                #region Animation
                if (_animator != null)
                {
                    _animator.GetTextureAnimationMatrix(renderPass.TexAnimIndex + 0, out _modelRenderData.uvAnimMatrix1);
                    _animator.GetTransparency(renderPass.AlphaAnimIndex, out _modelRenderData.transparency1);

                    if (renderPass.OpCount > 1)
                    {
                        _animator.GetTextureAnimationMatrix(renderPass.TexAnimIndex + 1, out _modelRenderData.uvAnimMatrix2);
                        _animator.GetTransparency(renderPass.AlphaAnimIndex + 1, out _modelRenderData.transparency2);
                    }
                    else
                    {
                        _modelRenderData.uvAnimMatrix2 = Matrix.Identity;
                        _modelRenderData.transparency2 = 1.0f;
                    }

                    if (renderPass.OpCount > 2)
                    {
                        _animator.GetTextureAnimationMatrix(renderPass.TexAnimIndex + 2, out _modelRenderData.uvAnimMatrix3);
                        _animator.GetTransparency(renderPass.AlphaAnimIndex + 2, out _modelRenderData.transparency3);
                    }
                    else
                    {
                        _modelRenderData.uvAnimMatrix3 = Matrix.Identity;
                        _modelRenderData.transparency3 = 1.0f;
                    }

                    if (renderPass.OpCount > 3)
                    {
                        _animator.GetTextureAnimationMatrix(renderPass.TexAnimIndex + 3, out _modelRenderData.uvAnimMatrix4);
                        _animator.GetTransparency(renderPass.AlphaAnimIndex + 3, out _modelRenderData.transparency4);
                    }
                    else
                    {
                        _modelRenderData.uvAnimMatrix4 = Matrix.Identity;
                        _modelRenderData.transparency4 = 1.0f;
                    }

                    _animator.GetColor(renderPass.ColorAnimIndex, out _modelRenderData.animatedColor);
                }
                #endregion

                // batch->textureWeight is the M2Batch animated texture weight track value (if present);
                // if batch->opCount is > 1, only the first textureWeight value is used
                //
                // For Blend_AlphaKey blending, the alphaRef value starts at a baseline, and is multiplied
                // by the M2Element->alpha.This multiplication prevents inappropriate pixel discards during
                // things like fades for distance culling. See the M2Element alpha section above for details
                // on what can modify M2Element alpha.
                // 
                // For all other blending modes (i.e. !(Blend_AlphaKey)), the alphaRef value is constant.
                if (renderPass.BlendMode != 1)
                    _modelRenderData.AlphaRef = 1.0f / 255.0f;
                else
                    _modelRenderData.AlphaRef = 128.0f / 255.0f  * _modelRenderData.transparency1;

                PerPassBuffer.UpdateData(_modelRenderData);
                for (var i = 0; i < renderPass.OpCount && i < 4 && i < renderPass.TextureIndices.Count; ++i)
                {
                    var texFlags = Model.MD20.Textures[renderPass.TextureIndices[i]].Flags;
                    if (texFlags.WrapBoth)
                        _mesh.Program.SetPixelSampler(i, _samplerWrapUV);
                    else if (texFlags.WrapU)
                        _mesh.Program.SetPixelSampler(i, _samplerWrapU);
                    else if (texFlags.WrapV)
                        _mesh.Program.SetPixelSampler(i, _samplerWrapV);
                    else
                        _mesh.Program.SetPixelSampler(i, _samplerClamp);

                    Texture tex;
                    _textureCache.TryGetValue(renderPass.TextureIndices[i], out tex);
                    _mesh.Program.SetPixelTexture(i, tex);
                }

                _mesh.Draw();
            }

            foreach (var attachment in Attachments)
                attachment.Render(billboard);
        }

        /// <summary>
        /// Initialize universal objects
        /// </summary>
        private void Initialize()
        {
            _mesh = new Mesh<M2Vertex>(_drawContext)
            {
                Stride = SizeCache<M2Vertex>.Size,
                DepthState = { DepthEnabled = true }
            };

            _mesh.BlendState.Dispose();
            _mesh.IndexBuffer.Dispose();
            _mesh.VertexBuffer.Dispose();

            _mesh.BlendState = null;
            _mesh.IndexBuffer = null;
            _mesh.VertexBuffer = null;

            _mesh.AddElement("POSITION", 0, 3);
            _mesh.AddElement("BLENDWEIGHT", 0, 4, DataType.Byte, true);
            _mesh.AddElement("BLENDINDEX", 0, 4, DataType.Byte);
            _mesh.AddElement("NORMAL", 0, 3);
            _mesh.AddElement("TEXCOORD", 0, 2);
            _mesh.AddElement("TEXCOORD", 1, 2);

            // Random dummy program

            _samplerWrapU = new Sampler(_drawContext)
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            _samplerWrapV = new Sampler(_drawContext)
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Clamp,
                Filter = Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            _samplerWrapUV = new Sampler(_drawContext)
            {
                AddressU = TextureAddressMode.Wrap,
                AddressV = TextureAddressMode.Wrap,
                AddressW = TextureAddressMode.Clamp,
                Filter = Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            _samplerClamp = new Sampler(_drawContext)
            {
                AddressU = TextureAddressMode.Clamp,
                AddressV = TextureAddressMode.Clamp,
                AddressW = TextureAddressMode.Clamp,
                Filter = Filter.Anisotropic,
                MaximumAnisotropy = 16
            };

            #region Blend states
            for (var i = 0; i < _blendStates.Length; ++i)
                _blendStates[i] = new BlendState(_drawContext);

            // Blend states with key assigned through enum check out with the wiki
            _blendStates[(int)BlendMode.Opaque] = new BlendState(_drawContext)
            {
                BlendEnabled = false
            };

            _blendStates[(int)BlendMode.AlphaKey] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero
            };

            _blendStates[(int)BlendMode.Alpha] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = BlendOption.One, // SourceAlpha
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha
            };

            _blendStates[3] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceColor,
                DestinationBlend = BlendOption.DestinationColor,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.DestinationAlpha
            };

            _blendStates[4] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.One
            };

            _blendStates[5] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha
            };

            _blendStates[6] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.DestinationColor,
                DestinationBlend = BlendOption.SourceColor,
                SourceAlphaBlend = BlendOption.DestinationAlpha,
                DestinationAlphaBlend = BlendOption.SourceAlpha
            };

            _blendStates[7] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceColor,
                DestinationBlend = BlendOption.DestinationColor,
                SourceAlphaBlend = BlendOption.DestinationAlpha,
                DestinationAlphaBlend = BlendOption.SourceAlpha
            };

            /*_blendStates[0] = new BlendState(_drawContext)
            {
                BlendEnabled = false,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                Tag = "Blend_Opaque"
            };

            // only has alpha testing
            _blendStates[1] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                Tag = "Blend_AlphaKey"
            };

            _blendStates[2] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha,
                Tag = "Blend_Alpha"
            };

            _blendStates[3] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.One,
                Tag = "Blend_Add"
            };

            _blendStates[4] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.DestinationColor,
                DestinationBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.DestinationAlpha,
                DestinationAlphaBlend = BlendOption.Zero,
                Tag = "Blend_Mod"
            };

            _blendStates[5] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.DestinationColor,
                DestinationBlend = BlendOption.SourceColor,
                SourceAlphaBlend = BlendOption.DestinationAlpha,
                DestinationAlphaBlend = BlendOption.SourceAlpha,
                Tag = "Blend_Mod2x"
            };

            _blendStates[6] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.DestinationColor,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.DestinationAlpha,
                DestinationAlphaBlend = BlendOption.One,
                Tag = "Blend_ModAdd"
            };

            _blendStates[7] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.InverseSourceAlpha,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.InverseSourceAlpha,
                DestinationAlphaBlend = BlendOption.One,
                Tag = "Blend_InvSrcAlphaAdd"
            };

            _blendStates[8] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.InverseSourceAlpha,
                DestinationBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.InverseSourceAlpha,
                DestinationAlphaBlend = BlendOption.Zero,
                Tag = "Blend_InvSrcAlphaAddOpaque"
            };

            _blendStates[9] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.SourceAlpha,
                DestinationBlend = BlendOption.Zero,
                SourceAlphaBlend = BlendOption.SourceAlpha,
                DestinationAlphaBlend = BlendOption.Zero,
                Tag = "Blend_SrcAlphaOpaque"
            };

            _blendStates[10] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.Zero,
                DestinationAlphaBlend = BlendOption.One,
                Tag = "Blend_NoAlphaAdd"
            };

            _blendStates[11] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.BlendFactor,
                DestinationBlend = BlendOption.InverseBlendFactor,
                SourceAlphaBlend = BlendOption.BlendFactor,
                DestinationAlphaBlend = BlendOption.InverseBlendFactor,
                Tag = "Blend_ConstantAlpha"
            };

            _blendStates[12] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.InverseDestinationColor,
                DestinationBlend = BlendOption.One,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.Zero,
                Tag = "Blend_Screen"
            };

            _blendStates[13] = new BlendState(_drawContext)
            {
                BlendEnabled = true,
                SourceBlend = BlendOption.One,
                DestinationBlend = BlendOption.InverseSourceAlpha,
                SourceAlphaBlend = BlendOption.One,
                DestinationAlphaBlend = BlendOption.InverseSourceAlpha,
                Tag = "Blend_BlendAdd"
            };/**/
            #endregion

            _noCullState = new RasterState(_drawContext) { CullEnabled = false };
            _cullState = new RasterState(_drawContext) { CullEnabled = true };
        }

        public void Dispose()
        {
            _samplerWrapU?.Dispose();
            _samplerWrapU = null;

            _samplerWrapV?.Dispose();
            _samplerWrapV = null;

            _samplerWrapUV?.Dispose();
            _samplerWrapUV = null;

            _samplerClamp?.Dispose();
            _samplerClamp = null;

            PerPassBuffer?.Dispose();
            PerPassBuffer = null;

            BonesAnimationMatrices?.Dispose();
            BonesAnimationMatrices = null;

            _mesh.Dispose();
            _mesh = null;

            _noCullState?.Dispose();
            _noCullState = null;

            _cullState?.Dispose();
            _cullState = null;

            for (var i = 0; i < _blendStates.Length; ++i)
            {
                _blendStates[i].Dispose();
                _blendStates[i] = null;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct PerModelPassBuffer
        {
            public Matrix uvAnimMatrix1;
            public Matrix uvAnimMatrix2;
            public Matrix uvAnimMatrix3;
            public Matrix uvAnimMatrix4;

            // public Vector4 modelPassParams:
            public float LightCoefficient;
            public float FogCoefficient;
            private float __unused0;
            public float AlphaRef;

            public Vector4 animatedColor;
            public float transparency1;
            public float transparency2;
            public float transparency3;
            public float transparency4;

            // Used for attachments mostly
            public Matrix modelPosition;
        }

        public void UpdatePlacementMatrix(Matrix position)
        {
            _modelRenderData.modelPosition = position;
        }

        public void Attach(int attachmentSlot, ModelRenderer render)
        {
            if (CanAttach(attachmentSlot))
            {
                Attachments.Add(new ModelAttachment(this, render,
                    Model.MD20.Attachments[Model.MD20.AttachmentLookupTable[attachmentSlot]]));
            }
        }

        public bool CanAttach(int attachSlot)
        {
            return Model.MD20.AttachmentLookupTable[attachSlot] != -1;
        }

        public void SetAnimation(uint animID)
        {
            _animator?.SetAnimation(animID);
        }
    }
}
