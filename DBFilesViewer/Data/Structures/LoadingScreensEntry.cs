using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("LoadingScreens")]
    public sealed class LoadingScreensEntry
    {
        public TextureFile DefaultTexture;
        public TextureFile UpscaledResolution;
        public TextureFile WideScreen;
    }
}
