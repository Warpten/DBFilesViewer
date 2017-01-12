namespace DBFilesViewer.Graphics
{
    public class ConstantBuffer<T> : Buffer<T> where T : struct
    {
        public ConstantBuffer(GxContext context)
            : base(context, SharpDX.Direct3D11.BindFlags.ConstantBuffer)
        {
        }
    }
}
