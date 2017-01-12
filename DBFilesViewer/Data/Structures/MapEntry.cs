using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("Map")]
    public sealed class MapEntry
    {
        public string Directory;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Flags;
        public float MinimapIconScale;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] CorpsePos;
        public string MapName;
        public string MapDescription0;                               // Horde
        public string MapDescription1;                               // Alliance
        public ForeignKey<AreaTableEntry> AreaTableID;
        public ForeignKey<LoadingScreensEntry> LoadingScreenID;
        public ForeignKey<MapEntry> CorpseMapID;
        public short TimeOfDayOverride;
        public ForeignKey<MapEntry> ParentMapID;
        public ForeignKey<MapEntry> CosmeticParentMapID;
        public ushort WindSettingsID;
        public byte InstanceType;
        public byte MapType;
        public byte ExpansionID;
        public byte MaxPlayers;
        public byte TimeOffset;

        public override string ToString() => MapName;
    }
}
