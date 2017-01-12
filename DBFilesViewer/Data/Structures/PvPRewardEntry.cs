using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("PvPReward")]
    public sealed class PvPRewardEntry
    {
        public uint HonorLevel;
        public uint Prestige;
        public ForeignKey<RewardPackEntry> RewardPack;
    }
}
