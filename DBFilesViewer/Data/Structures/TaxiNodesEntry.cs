using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("TaxiNodes")]
    public sealed class TaxiNodesEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Position;
        public string Name;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] MountCreatureID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public float[] MapOffset;
        public ForeignKey<MapEntry> MapID;
        public ForeignKey<PlayerConditionEntry> ConditionID;
        public ushort LearnableIndex;
        public byte Flags;
        public uint ID;
    }
}
