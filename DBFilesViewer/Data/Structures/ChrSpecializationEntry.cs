using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ChrSpecialization")]
    public sealed class ChrSpecializationEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public ForeignKey<SpellEntry>[] MasterySpellID;
        public string Name;
        public string Name2;
        public string Description;
        public string BackgroundImage;
        public ushort SpellIconID;
        public Classes ClassID;
        public byte OrderIndex;
        public byte PetTalentType;
        public SpecRole Role;
        public byte PrimaryStatOrder;
        public int ID;
        public int Flags;
        public int AnimReplacementSetID;
    }
}
