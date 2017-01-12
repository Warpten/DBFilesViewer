using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("WorldSafeLocs")]
    public sealed class WorldSafeLocsEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Position;
        public float Orientation;
        public string AreaName;
        public ForeignKey<MapEntry> Map;
    }
}
