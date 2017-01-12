using DBFilesClient.NET;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CharHairGeosets")]
    public sealed class CharHairGeosetsEntry
    {
        public uint DemonHunterHDGeosetModelFileDataID;
        public Races RaceID;
        public Genders Gender;
        public uint VariationID;
        public uint VariationType; // 0 = skinColor, 1 = face, 2 = beardStyle, 3 = hairStyle & hairColor
        public int GeosetID;
        public uint GeosetType;
        public uint ShowScalp;
        public uint ColorIndex;
        public uint DemonHunterGeosetModelFileDataID;
    }
}
