using System.Collections.Generic;
using DBFilesViewer.Properties;
using SharpDX.Direct3D11;

namespace DBFilesViewer.Graphics.Files.Models
{
    public enum M2HullShaderType
    {
        HS_T1,
        HS_T1_T2,
        HS_T1_T2_T3,
        HS_T1_T2_T3_T4,
    };

    public enum M2DomainShaderType
    {
        DS_T1,
        DS_T1_T2,
        DS_T1_T2_T3,
        DS_T1_T2_T3_T4,
    };

    public enum M2VertexShaderType
    {
        VS_Diffuse_T1,
        VS_Diffuse_Env,
        VS_Diffuse_T1_T2,
        VS_Diffuse_T1_Env,
        VS_Diffuse_Env_T1,
        VS_Diffuse_Env_Env,
        VS_Diffuse_T1_Env_T1,
        VS_Diffuse_T1_T1,
        VS_Diffuse_T1_T1_T1,
        VS_Diffuse_EdgeFade_T1,
        VS_Diffuse_T2,
        VS_Diffuse_T1_Env_T2,
        VS_Diffuse_EdgeFade_T1_T2,
        VS_Diffuse_T1_T1_T1_T2,
        VS_Diffuse_EdgeFade_Env,
        VS_Diffuse_T1_T2_T1,
        VS_Diffuse_T1_T2_T3,
    };

    public enum M2PixelShaderType
    {
        PS_Combiners_Opaque,
        PS_Combiners_Mod,
        PS_Combiners_Opaque_Mod,
        PS_Combiners_Opaque_Mod2x,
        PS_Combiners_Opaque_Mod2xNA,
        PS_Combiners_Opaque_Opaque,
        PS_Combiners_Mod_Mod,
        PS_Combiners_Mod_Mod2x,
        PS_Combiners_Mod_Add,
        PS_Combiners_Mod_Mod2xNA,
        PS_Combiners_Mod_AddNA,
        PS_Combiners_Mod_Opaque,
        PS_Combiners_Opaque_Mod2xNA_Alpha,
        PS_Combiners_Opaque_AddAlpha,
        PS_Combiners_Opaque_AddAlpha_Alpha,
        PS_Combiners_Opaque_Mod2xNA_Alpha_Add,
        PS_Combiners_Mod_AddAlpha,
        PS_Combiners_Mod_AddAlpha_Alpha,
        PS_Combiners_Opaque_Alpha_Alpha,
        PS_Combiners_Opaque_Mod2xNA_Alpha_3s,
        PS_Combiners_Opaque_AddAlpha_Wgt,
        PS_Combiners_Mod_Add_Alpha,
        PS_Combiners_Opaque_ModNA_Alpha,
        PS_Combiners_Mod_AddAlpha_Wgt,
        PS_Combiners_Opaque_Mod_Add_Wgt,
        PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,
        PS_Combiners_Mod_Dual_Crossfade,
        PS_Combiners_Opaque_Mod2xNA_Alpha_Alpha,
        PS_Combiners_Mod_Masked_Dual_Crossfade,
        PS_Combiners_Opaque_Alpha,
        PS_Guild,
        PS_Guild_NoBorder,
        PS_Guild_Opaque,
        PS_Combiners_Mod_Depth,
        PS_Illum,
        PS_Combiners_Mod_Mod_Mod,
    };

    public class ModelShaders
    {
        private readonly List<PixelShader> _pixelShaders = new List<PixelShader>();
        private readonly List<PixelShader> _portraitPixelShaders = new List<PixelShader>();

        private readonly List<VertexShader> _vertexShaders = new List<VertexShader>();

        private struct M2ShaderEffect
        {
            public readonly M2PixelShaderType PixelShader;
            public readonly M2VertexShaderType VertexShader;
            public readonly M2HullShaderType HullShader;
            public readonly M2DomainShaderType DomainShader;
            public readonly uint ColorOperation;
            public readonly uint AlphaOperation;

            public M2ShaderEffect(M2PixelShaderType ps, M2VertexShaderType vs,
                M2HullShaderType hs, M2DomainShaderType ds, uint colorOp, uint alphaOp)
            {
                PixelShader = ps;
                VertexShader = vs;
                HullShader = hs;
                DomainShader = ds;
                ColorOperation = colorOp;
                AlphaOperation = alphaOp;
            }
        };

        private static readonly M2ShaderEffect[] M2ShaderEffects = {
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha,            M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Alpha,           M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_Add,        M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha,                 M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Alpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_3s,         M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Add_Alpha,                   M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_ModNA_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Wgt,                M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_AddAlpha_Wgt,             M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod_Add_Wgt,              M2VertexShaderType.VS_Diffuse_T1_Env,           M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T1,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Dual_Crossfade,              M2VertexShaderType.VS_Diffuse_T1_T1_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_T1,      M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_AddAlpha_Alpha,              M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Mod,                         M2VertexShaderType.VS_Diffuse_EdgeFade_T1_T2,   M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Masked_Dual_Crossfade,       M2VertexShaderType.VS_Diffuse_T1_T1_T1_T2,      M2HullShaderType.HS_T1_T2_T3_T4,    M2DomainShaderType.DS_T1_T2_T3_T4,  0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Alpha,                    M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha,  M2VertexShaderType.VS_Diffuse_T1_Env_T2,        M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2_T3,     0, 3),
            new M2ShaderEffect(M2PixelShaderType.PS_Combiners_Mod_Depth,                       M2VertexShaderType.VS_Diffuse_EdgeFade_Env,     M2HullShaderType.HS_T1,             M2DomainShaderType.DS_T1,           0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Guild,                                     M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Guild_NoBorder,                            M2VertexShaderType.VS_Diffuse_T1_T2,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2_T3,     0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Guild_Opaque,                              M2VertexShaderType.VS_Diffuse_T1_T2_T1,         M2HullShaderType.HS_T1_T2_T3,       M2DomainShaderType.DS_T1_T2,        0, 0),
            new M2ShaderEffect(M2PixelShaderType.PS_Illum,                                     M2VertexShaderType.VS_Diffuse_T1_T1,            M2HullShaderType.HS_T1_T2,          M2DomainShaderType.DS_T1_T2,        0, 0),
        };

        public static M2VertexShaderType GetVertexShaderType(ushort shaderId, ushort opCount)
        {
            if ((shaderId & 0x8000) != 0)
                return M2ShaderEffects[shaderId & 0x7FFF].VertexShader;

            if (opCount == 1)
            {
                if ((shaderId & 0x80) != 0)
                    return M2VertexShaderType.VS_Diffuse_Env;
                if ((shaderId & 0x4000) != 0)
                    return M2VertexShaderType.VS_Diffuse_T2;
                return M2VertexShaderType.VS_Diffuse_T1;
            }

            if ((shaderId & 0x80) != 0)
            {
                if ((shaderId & 0x8) != 0)
                    return M2VertexShaderType.VS_Diffuse_Env_Env;
                return M2VertexShaderType.VS_Diffuse_Env_T1;
            }

            if ((shaderId & 0x8) != 0)
                return M2VertexShaderType.VS_Diffuse_T1_Env;

            return ((shaderId & 0x4000) != 0)
                ? M2VertexShaderType.VS_Diffuse_T1_T2
                : M2VertexShaderType.VS_Diffuse_T1_T1;
        }

        // 7.0.3.21691 - 0x194C11
        public static M2PixelShaderType GetPixelShaderType(ushort shaderId, ushort opCount)
        {
            if ((shaderId & 0x8000) != 0)
                return M2ShaderEffects[shaderId & 0x7FFF].PixelShader;

            var hiHalfByte = (uint)(shaderId & 70);
            var loHalfByte = (uint)(shaderId & 7);

            if (opCount == 1)
            {
                if (hiHalfByte != 0)
                    return M2PixelShaderType.PS_Combiners_Mod;
                return M2PixelShaderType.PS_Combiners_Opaque;
            }

            if (hiHalfByte != 0)
            {
                switch (loHalfByte)
                {
                    case 0:  return M2PixelShaderType.PS_Combiners_Mod_Opaque;
                    case 3:  return M2PixelShaderType.PS_Combiners_Mod_Add;
                    case 4:  return M2PixelShaderType.PS_Combiners_Mod_Mod2x;
                    case 6:  return M2PixelShaderType.PS_Combiners_Mod_Mod2xNA;
                    case 7:  return M2PixelShaderType.PS_Combiners_Mod_AddNA;
                    default: return M2PixelShaderType.PS_Combiners_Mod_Mod;
                }
            }

            switch (loHalfByte)
            {
                case 0: return M2PixelShaderType.PS_Combiners_Opaque_Opaque;
                case 3: return M2PixelShaderType.PS_Combiners_Opaque_AddAlpha;
                case 4: return M2PixelShaderType.PS_Combiners_Opaque_Mod2x;
                case 6: return M2PixelShaderType.PS_Combiners_Opaque_Mod2xNA;
                case 7: return M2PixelShaderType.PS_Combiners_Opaque_AddAlpha;
                default: return M2PixelShaderType.PS_Combiners_Opaque_Mod;
            }
        }

        public void Initialize(DeviceContext ctx)
        {
            InitializePixelShaders(ctx);
            InitializePixelShaders_Portrait(ctx);
            InitializeVertexShaders(ctx);
        }

        private void InitializePixelShaders(DeviceContext ctx)
        {
            // enum order
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2x));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Opaque));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Mod));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Mod2x));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Add));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Mod2xNA));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddNA));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Opaque));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_AddAlpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_AddAlpha_Alpha));
            _pixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_Add));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddAlpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddAlpha_Alpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Alpha_Alpha));
            _pixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_3s));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_AddAlpha_Wgt));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Add_Alpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_ModNA_Alpha));
            _pixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_AddAlpha_Wgt));
            _pixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod_Add_Wgt));
            _pixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Dual_Crossfade));
            _pixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Mod2xNA_Alpha_Alpha));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Masked_Dual_Crossfade));
            _pixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Opaque_Alpha));
            _pixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Guild));
            _pixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Guild_NoBorder));
            _pixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Guild_Opaque));
            _pixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Combiners_Mod_Depth));
            _pixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2Pixel_PS_Illum));
        }

        private void InitializePixelShaders_Portrait(DeviceContext ctx)
        {
            // enum order
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2x));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2xNA));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Opaque));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Mod));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Mod2x));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Add));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Mod2xNA));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_AddNA));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Opaque));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2xNA_Alpha));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_AddAlpha));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_AddAlpha_Alpha));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2xNA_Alpha_Add));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_AddAlpha));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_AddAlpha_Alpha));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Alpha_Alpha));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2xNA_Alpha_3s));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_AddAlpha_Wgt));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Add_Alpha));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_ModNA_Alpha));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_AddAlpha_Wgt));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod_Add_Wgt));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2xNA_Alpha_UnshAlpha));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Dual_Crossfade));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Mod2xNA_Alpha_Alpha));
            _portraitPixelShaders.Add(null);//PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Masked_Dual_Crossfade));
            _portraitPixelShaders.Add(new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Opaque_Alpha));
            _portraitPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Guild));
            _portraitPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Guild_NoBorder));
            _portraitPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Guild_Opaque));
            _portraitPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Combiners_Mod_Depth));
            _portraitPixelShaders.Add(null);//new PixelShader(ctx.Device, Resources.Shaders.M2PixelPortrait_PS_Illum));
        }

        private void InitializeVertexShaders(DeviceContext ctx)
        {
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T2));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env_T1));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_Env_Env));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env_T1));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1_T1));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_T1));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T2));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_Env_T2));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_T1_T2));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T1_T1_T2));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_EdgeFade_Env));
            _vertexShaders.Add(new VertexShader(ctx.Device, Resources.Shaders.M2VertexInstanced_VS_Diffuse_T1_T2_T1));
        }

        public VertexShader GetVertexShader(M2VertexShaderType VertexShaderType)
        {
            var vertexShaderType = (int)VertexShaderType;
            if (vertexShaderType < _vertexShaders.Count)
            {
                var vs = _vertexShaders[vertexShaderType];
                if (vs != null) return vs;
            }

            return _vertexShaders[(int)M2VertexShaderType.VS_Diffuse_T1];
        }
        
        public PixelShader GetPixelShader(M2PixelShaderType PixelShaderType)
        {
            var pixelShaderType = (int)PixelShaderType;
            if (pixelShaderType < _pixelShaders.Count)
            {
                var ps = _pixelShaders[pixelShaderType];
                if (ps != null) return ps;
            }

            return _pixelShaders[(int)M2PixelShaderType.PS_Combiners_Opaque];
        }

        public PixelShader GetPortraitPixelShader(M2PixelShaderType PixelShaderType)
        {
            var pixelShaderType = (int)PixelShaderType;
            if (pixelShaderType < _portraitPixelShaders.Count)
            {
                var ps = _portraitPixelShaders[pixelShaderType];
                if (ps != null) return ps;
            }

            return _portraitPixelShaders[(int)M2PixelShaderType.PS_Combiners_Opaque];
        }
    }
}
