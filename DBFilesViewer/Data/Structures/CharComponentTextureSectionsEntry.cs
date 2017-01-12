using System.ComponentModel;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using SharpDX;
using EnumConverter = DBFilesViewer.UI.Controls.EnumConverter;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CharComponentTextureSections")]
    public sealed class CharComponentTextureSectionsEntry
    {
        public uint X;
        public uint Y;
        public uint SectionWidth;
        public uint SectionHeight;
        public ForeignKey<CharComponentTextureLayoutsEntry> Layout;
        public SectionType Section;

        public RectangleF GetBounds()
        {
            var width = (float)Layout.Value.Width;
            var height = (float)Layout.Value.Height;

            return new RectangleF(X / width, Y / height, SectionWidth / width, SectionHeight / height);
        }
    }

    [TypeConverter(typeof(EnumConverter))]
    public enum SectionType : uint
    {
        TEXTURE_SECTION_ARMS_UPPER    = 0,
        TEXTURE_SECTION_ARMS_LOWER    = 1,
        TEXTURE_SECTION_HANDS         = 2,
        TEXTURE_SECTION_TORSO_UPPER   = 3,
        TEXTURE_SECTION_TORSO_LOWER   = 4,
        TEXTURE_SECTION_LEGS_UPPER    = 5,
        TEXTURE_SECTION_LEGS_LOWER    = 6,
        TEXTURE_SECTION_FEET          = 7,
        TEXTURE_SECTION_UNK8          = 8, // Only used in Layout 2 (1024x512)
        TEXTURE_SECTION_SCALP_UPPER   = 9,
        TEXTURE_SECTION_SCALP_LOWER   = 10,
        TEXTURE_SECTION_UNUSED_11     = 11,
        TEXTURE_SECTION_BASE          = 12
    }
}
