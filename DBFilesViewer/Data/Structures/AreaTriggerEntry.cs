using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("AreaTrigger")]
    public class AreaTriggerEntry
    {
#pragma warning disable 169
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public float[] Position;
        public float Radius;
        public float BoxLength;
        public float BoxWidth;
        public float BoxHeight;
        public float BoxYaw;
        public ForeignKey<MapEntry> Map;
        public ushort PhaseID;
        public ushort PhaseGroupID;
        public ushort ShapeID;
        public ushort AreaTriggerActionSetID;
        public byte PhaseUseFlags;
        public byte ShapeType;
        public byte Flags;
        public int ID;
#pragma warning restore 169
    }
}
