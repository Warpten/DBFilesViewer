using System.Runtime.InteropServices;
using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("RandPropPoints")]
    public sealed class ItemRandPropPointsEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] EpicPropertiesPoints;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] RarePropertiesPoints;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] UncommonPropertiesPoints;
    }
}
