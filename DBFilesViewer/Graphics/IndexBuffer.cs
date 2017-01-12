using SharpDX.DXGI;

namespace DBFilesViewer.Graphics
{
    /// <summary>
    /// A wrapper around DirectX's Buffer specifically for index.
    /// </summary>
    public class IndexBuffer : Buffer<ushort>
    {
        public IndexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.IndexBuffer)
        {
            IndexFormat = Format.R16_UInt;
        }

        public Format IndexFormat { get; set; }
    }
}
