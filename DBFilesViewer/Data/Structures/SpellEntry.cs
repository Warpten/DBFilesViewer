using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("Spell")]
    public sealed class SpellEntry
    {
        public string Name;
        public string NameSubtext;
        public string Description;
        public string AuraDescription;
        public ForeignKey<SpellMiscEntry> MiscID;
        public uint ID;
        public ForeignKey<SpellDescriptionVariablesEntry> DescriptionVariablesID;
    }
}
