using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    public enum TextureSlot : int
    {
        UpperArm,
        LowerArm,
        Hands,
        UpperTorso,
        LowerTorso,
        UpperLeg,
        LowerLeg,
        Foot,
        Unk
    }

    [DBFileName("ItemDisplayInfoMaterialRes")]
    public sealed class ItemDisplayInfoMaterialResEntry
    {
        public ForeignKey<ItemDisplayInfoEntry> ItemDisplayInfo;
        public TextureFile TextureFileData;
        public TextureSlot TextureSlot;
    }
}
