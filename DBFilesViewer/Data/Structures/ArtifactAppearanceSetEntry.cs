using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ArtifactAppearanceSet")]
    public sealed class ArtifactAppearanceSetEntry
    {
        public string Name;
        public string Name2;
        public ushort UiCameraID;
        public ushort AltHandUICameraID;
        public byte ArtifactID;
        public byte DisplayIndex;
        public byte AttachmentPoint;
        public byte Flags;
        public uint ID;
    }
}
