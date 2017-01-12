using System.Linq;
using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.IO.Files;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CharSections")]
    public sealed class CharSectionsEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] TextureFileDataID;
        public ushort Flags;
        public byte Race;
        public Genders Gender;
        public byte GenType;
        public byte Type;
        public byte Color;

        public uint GetTexFileDataID(int index)
        {
            var possibleValues =
                DBC.Get<TextureFileDataEntry>().Where(kv => kv.Value.TextureFileDataID == TextureFileDataID[index]);

            // TODO: Filter out based on race, class, and spec (??, probably gender)
            return possibleValues.FirstOrDefault().Value?.FileDataID.Key ?? 0;
        }
    }
}
