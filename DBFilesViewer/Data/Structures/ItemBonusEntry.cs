using System.Runtime.InteropServices;
using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemBonus")]
    public class ItemBonusEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] Value;
        public ushort BonusListID;
        public byte Type;
        public byte Index;
    }
}
