using System.Runtime.InteropServices;
using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemRandomProperties")]
    public sealed class ItemRandomPropertiesEntry
    {
        public string Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ushort[] Enchantment;
    }
}
