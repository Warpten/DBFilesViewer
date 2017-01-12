using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CharacterFacialHairStyles")]
    public sealed class CharacterFacialHairStylesEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 5)]
        public int[] Geosets;
        public Races RaceID;
        public Genders Gender;
        public int VariationType;
    }
}
