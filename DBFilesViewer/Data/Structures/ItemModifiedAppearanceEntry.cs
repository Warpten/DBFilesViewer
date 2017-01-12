using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemModifiedAppearance")]
    public sealed class ItemModifiedAppearanceEntry
    {
        public ForeignKey<ItemSparseEntry> ItemID;
        public ForeignKey<ItemAppearanceEntry> AppearanceID;
        public uint AppearanceModID;
        public byte Index;
        public byte SourceType;
        public uint ID;
    }
}
