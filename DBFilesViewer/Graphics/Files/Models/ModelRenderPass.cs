using System.Collections.Generic;

namespace DBFilesViewer.Graphics.Files.Models
{
    public class ModelRenderPass
    {
        public bool Enabled { get; set; } = true;
        public int MeshID { get; set; }
        public int StartIndex { get; set; }
        public int IndexCount { get; set; }
        public List<int> TextureIndices { get; set; }
        public int TexAnimIndex { get; set; }
        public int TexUnitNumber { get; set; }
        public int ColorAnimIndex { get; set; }
        public int AlphaAnimIndex { get; set; }

        public M2MaterialRenderFlags RenderFlag { get; set; }
        public uint BlendMode { get; set; }

        public uint OpCount { get; set; }
        public M2VertexShaderType VertexShaderType { get; set; }
        public M2PixelShaderType PixelShaderType { get; set; }

        public ShaderProgram Program { get; set; }
    }
}
