using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemBonusTreeNode")]
    public class ItemBonusTreeNodeEntry
    {
        public ushort BonusTreeID;
        public ushort SubTreeID;
        public ushort BonusListID;
        public byte BonusTreeModID;
    }
}
