using System.Runtime.InteropServices;
using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemRandomSuffix")]
    public sealed class ItemRandomSuffixEntry
    {
        public string Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ushort[] Enchantment;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ushort[] AllocationPct;
    }
}
