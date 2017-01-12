using System.ComponentModel;
using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.Utils;
using EnumConverter = DBFilesViewer.UI.Controls.EnumConverter;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("Talent")]
    public sealed class TalentEntry
    {
        public ForeignKey<SpellEntry> SpellID;
        public ForeignKey<SpellEntry> OverridesSpellID;
        public string Description;
        public ForeignKey<ChrSpecializationEntry> SpecID;
        public byte TierID;
        public byte ColumnIndex;
        public byte Flags;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] CategoryMask;
        [TypeConverter(typeof(EnumConverter))]
        public Classes ClassID;
    }
}
