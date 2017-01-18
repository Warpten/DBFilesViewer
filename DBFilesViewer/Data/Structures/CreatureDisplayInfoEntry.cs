using System.Runtime.InteropServices;
using System.Windows.Forms;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.UI.Forms;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CreatureDisplayInfo")]
    [ViewCreatureModelButton]
    public sealed class CreatureDisplayInfoEntry
    {
        public ForeignKey<CreatureDisplayInfoExtraEntry> ExtendedDisplayInfo;
        public float CreatureModelScale;
        public float PlayerModelScale;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] TextureVariation;
        public string PortraitTextureName;
        public uint PortraitCreatureDisplayInfoID;
        public uint CreatureGeosetData;
        public uint StateSpellVisualKitID;
        public float InstanceOtherPlayerPetScale;
        public ForeignKey<CreatureModelDataEntry> Model;
        public ushort SoundID;
        public ushort NPCSoundID;
        public ushort ParticleColorID;
        public ushort ObjectEffectPackageID;
        public ushort AnimReplacementSetID;
        public byte CreatureModelAlpha;
        public byte SizeClass;
        public byte BloodID;
        public byte Flags;
        public Genders Gender;
        public sbyte Unk700; // Always -1
    }
}
