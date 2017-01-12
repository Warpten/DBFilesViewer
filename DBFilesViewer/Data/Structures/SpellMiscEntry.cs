using System.ComponentModel;
using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("SpellMisc")]
    public class SpellMiscEntry
    {
        [TypeConverter(typeof(ArrayConverter))]
        public uint[] Attributes;
        public float Speed;
        public float MultistrikeSpeedMod;
        public ushort CastingTimeIndex;
        public ushort DurationIndex;
        public ushort RangeIndex;
        public ushort SpellIconID;
        public ushort ActiveIconID;
        public byte SchoolMask;
    }
}
