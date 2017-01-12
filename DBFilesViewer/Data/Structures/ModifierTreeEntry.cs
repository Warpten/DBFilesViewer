using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ModifierTree")]
    public sealed class ModifierTreeEntry
    {
        public uint Asset1;
        public uint Asset2;
        public ForeignKey<ModifierTreeEntry> Parent;
        public uint Type;
        public byte AdditionalAsset;
        public byte Operator;
        public byte Amount;
    }
}
