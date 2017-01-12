using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemXBonusTree")]
    public class ItemXBonusTreeEntry
    {
        public ForeignKey<ItemSparseEntry> Item;
        public uint BonusTree;
    }
}
