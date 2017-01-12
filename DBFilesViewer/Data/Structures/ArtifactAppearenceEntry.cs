using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ArtifactAppearance")]
    class ArtifactAppearenceEntry
    {
        public string Name;
        public uint SwatchColor;
        public float ModelDesaturation;
        public float ModelAlpha;
        public uint ShapeshiftDisplayID;
        public ForeignKey<ArtifactAppearanceSetEntry> ArtifactAppearanceSet;
        public ForeignKey<PlayerConditionEntry> PlayerConditionID;
        public ushort Unknown;
        public byte DisplayIndex;
        public byte AppearanceModID;
        public byte Flags;
        public byte ModifiesShapeshiftFormDisplay;
        public uint ID;
        public ForeignKey<ItemAppearanceEntry> ItemAppearance;
        public ForeignKey<ItemAppearanceEntry> AltItemAppearance;
    }
}
