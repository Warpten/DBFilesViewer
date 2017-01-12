using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("AreaGroupMember")]
    public sealed class AreaGroupMemberEntry
    {
        public ushort AreaGroupId;
        public ForeignKey<AreaTableEntry> AreaId;
    }
}
