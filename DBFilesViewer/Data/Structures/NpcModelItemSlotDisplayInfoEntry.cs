using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("NpcModelItemSlotDisplayInfo")]
    public sealed class NpcModelItemSlotDisplayInfoEntry
    {
        public ForeignKey<CreatureDisplayInfoExtraEntry> CreatureDisplayInfoExtra;
        public ForeignKey<ItemDisplayInfoEntry> ItemDisplayInfoID;
        public ItemType ItemType;
    }

    public enum ItemType
    {
        HEAD = 0,
        SHOULDER,
        SHIRT,
        CHEST,
        BELT,
        PANTS,
        BOOTS,
        BRACERS,
        GLOVES,
        TABARD,
        CAPE
    }
}
