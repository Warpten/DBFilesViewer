using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("Holidays")]
    public sealed class HolidaysEntry
    {
        public uint ID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 16)]
        public TimeStamp[] Date;
        public string TextureFileName;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public ushort[] Duration;
        public ushort Region;
        public byte Looping;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 10)]
        public byte[] CalendarFlags;
        public ForeignKey<HolidayNamesEntry> HolidayNameID;
        public ForeignKey<HolidayDescriptionsEntry> HolidayDescriptionID;
        public byte Priority;
        public sbyte CalendarFilterType;
        public byte Flags;
    }
}
