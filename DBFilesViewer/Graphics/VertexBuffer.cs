namespace DBFilesViewer.Graphics
{
    /// <summary>
    /// A wrapper around DirectX's Buffer specifically designed to be used with vertices.
    /// </summary>
    public class VertexBuffer<T> : Buffer<T> where T : struct
    {
        public VertexBuffer(GxContext context) :
            base(context, SharpDX.Direct3D11.BindFlags.VertexBuffer)
        {

        }
    }
}
