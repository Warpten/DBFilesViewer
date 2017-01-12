using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ZoneMusic")]
    public sealed class ZoneMusicEntry
    {
        public string SetName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] SilenceIntervalMin;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] SilenceIntervalMax;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ZoneMusicSoundKitKey[] RefID;
    }
}
