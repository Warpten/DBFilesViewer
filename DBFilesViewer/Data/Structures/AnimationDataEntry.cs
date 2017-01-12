using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("AnimationData")]
    public sealed class AnimationDataEntry
    {
        public string Name;
        public int Flags;
        public ForeignKey<AnimationDataEntry> Fallback;
        public int BehaviorID;
        public int BehaviorTier;
    }

    public class AnimationComboBoxEntry
    {
        public uint ID;
        public string Name;

        public override string ToString() => Name;
    }
}
