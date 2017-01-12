using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ModelFileData")]
    public sealed class ModelFileDataEntry
    {
        public byte UnkByte;
        public ModelFile Model;
        public uint ModelID;
    }

    [DBFileName("MovieFileData")]
    public sealed class MovieFileDataEntry
    {
        public uint Resolution;
    }

    // Key is root's FileDataID
    [DBFileName("TextureFileData")]
    public sealed class TextureFileDataEntry
    {
        public int TextureFileDataID;
        public byte TextureType;
        public TextureFile FileDataID;
    }

    // Key is root's FileDataID
    [DBFileName("ComponentTextureFileData")]
    public sealed class ComponentTextureFileDataEntry
    {
        public BinaryFile UnkInt0;
        public BinaryFile UnkInt1;
        public BinaryFile UnkInt2;
    }

    // Key is root's FileDataID
    [DBFileName("ComponentModelFileData")]
    public sealed class ComponentModelFileDataEntry
    {
        public Genders GenderID;
        public Classes ClassID;
        public Races RaceID;
        public int Order; // 0 - left shoulder; 1 - right shoulder; -1 - everything else
    }
}
