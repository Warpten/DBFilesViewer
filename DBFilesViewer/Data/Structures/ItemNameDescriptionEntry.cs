using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemNameDescription")]
    class ItemNameDescriptionEntry
    {
        public string Name;
        public Flags<uint> Flags;
    }
}
