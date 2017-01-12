using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("WorldMapArea")]
    public class WorldMapAreaEntry
    {
        public string AreaName;
        public float LocLeft;
        public float LocRight;
        public float LocTop;
        public float LocBottom;
        public ForeignKey<MapEntry> MapID;
        public ForeignKey<AreaTableEntry> AreaID;
        public ForeignKey<MapEntry> DisplayMapID;
        public ushort DefaultDungeonFloor;
        public ForeignKey<MapEntry> ParentWorldMapID;
        public Flags<ushort> Flags;
        public byte LevelRangeMin;
        public byte LevelRangeMax;
        public byte BountySetID;
        public byte BountyBoardLocation;
        public uint ID;
        public ForeignKey<PlayerConditionEntry> PlayerCondition;
    }
}
