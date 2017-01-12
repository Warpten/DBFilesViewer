using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemAppearance")]
    public sealed class ItemAppearanceEntry
    {
        public ForeignKey<ItemDisplayInfoEntry> DisplayID;
        public TextureFile IconFileDataID;
        public int UIOrder;
        public byte ObjectComponentSlot;
    }
}
