using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("AnimKit")]
    public class AnimKitEntry
    {
        public uint OneShotDuration;
        public ushort OneShotStopAnimKitID;
        public ushort LowDefAnimKitID;
    }
}
