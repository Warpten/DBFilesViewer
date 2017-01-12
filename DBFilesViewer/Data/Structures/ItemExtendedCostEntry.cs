using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemExtendedCost")]
    public class ItemExtendedCostEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ForeignKey<ItemSparseEntry>[] RequiredItem;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public uint[] RequiredCurrencyCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ushort[] RequiredItemCount;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ushort RequiredPersonalArenaRating;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public ForeignKey<CurrencyTypesEntry>[] RequiredCurrency;
        public byte RequiredArenaSlot;
        public byte RequiredFactionID;
        public byte RequiredFactionStanding;
        public byte RequirementFlags;
        public byte RequiredAchievement; // ???
    }
}
