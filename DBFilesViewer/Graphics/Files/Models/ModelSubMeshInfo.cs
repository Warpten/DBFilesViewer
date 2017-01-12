using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models
{
    public class ModelSubMeshInfo
    {
        public BoundingSphere BoundingSphere { get; set; }
        public uint NumIndices { get; set; }
        public int StartIndex { get; set; }
    }
}
