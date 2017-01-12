using System.Runtime.InteropServices;
using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CurrencyTypes")]
    public sealed class CurrencyTypesEntry
    {
        public string Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public string[] InventoryIcon;
        public uint MaxQuantity;
        public uint MaxEarnablePerWeek;
        public uint Flags;
        public string Description;
        public byte CategoryID;
        public byte SpellCategory;
        public byte Quality;
        public int SpellWeight;
    }
}
