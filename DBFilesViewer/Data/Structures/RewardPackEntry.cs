using DBFilesClient.NET;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("RewardPack")]
    public sealed class RewardPackEntry
    {
        public uint Money;
        public float ArtifactPowerMultiplier;
        public byte ArtifactQuestXPIndex;
        public byte ArtifactCategoryID;
        public uint TitleID;
        public uint Unk;
    }
}
