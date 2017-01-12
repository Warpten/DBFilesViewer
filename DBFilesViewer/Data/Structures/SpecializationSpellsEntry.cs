using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("SpecializationSpells")]
    public sealed class SpecializationSpellsEntry
    {
        public ForeignKey<SpellEntry> SpellID;
        public ForeignKey<SpellEntry> OverridesSpellID;
        public string Description;
        public ForeignKey<ChrSpecializationEntry> SpecID;
        public byte OrderIndex;
        public uint ID;
    }
}
