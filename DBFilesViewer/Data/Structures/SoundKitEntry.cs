using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("SoundKitEntry")]
    public class SoundKitEntryEntry
    {
        public AudioFile FileDataID;
        public float UnkFloat1;
        public uint UnkDword2;
        public uint ZoneMusicID;
    }
}
