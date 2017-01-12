using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("Artifact")]
    public sealed class ArtifactEntry
    {
        public string Name;
        public PackedColor BarConnectedColor;
        public PackedColor BarDisconnectedColor;
        public PackedColor TitleColor;
        public ushort ClassUiTextureKitID;
        public ForeignKey<ChrSpecializationEntry> SpecID;
        public byte ArtifactCategoryID;
        public byte Flags;
    }
}
