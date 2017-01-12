using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("AreaTable")]
    public sealed class AreaTableEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Flags;
        public string ZoneName;
        public float AmbientMultiplier;
        public string AreaName;
        public ForeignKey<MapEntry> MapID;
        public ForeignKey<AreaTableEntry> ParentAreaID;
        public short AreaBit;
        public ushort AmbienceID;
        public ForeignKey<ZoneMusicEntry> ZoneMusic;
        public ushort IntroSound;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public ForeignKey<LiquidTypeEntry>[] LiquidTypeID;
        public ForeignKey<ZoneMusicEntry> UWZoneMusic;
        public ushort UWAmbience;
        public ushort PvPCombatWorldStateID;
        public byte SoundProviderPref;
        public byte SoundProviderPrefUnderwater;
        public byte ExplorationLevel;
        public byte FactionGroupMask;
        public byte MountFlags;
        public byte WildBattlePetLevelMin;
        public byte WildBattlePetLevelMax;
        public byte WindSettingsID;
        public uint UWIntroSound;
    }
}
