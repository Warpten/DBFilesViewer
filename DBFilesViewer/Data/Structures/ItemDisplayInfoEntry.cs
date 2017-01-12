using System.Runtime.InteropServices;
using System.Windows.Forms;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.UI.Forms;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("ItemDisplayInfo")]
    [ViewModelButton(typeof(ItemDisplayInfoEntry), "OpenModelViewer", typeof(uint))]
    public sealed class ItemDisplayInfoEntry
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public uint[] Model;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public TextureFileDataKey[] Textures;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public int[] GeosetGroups;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public uint[] UnkInt; // probably geosets
        public uint Flags;
        public int SpellVisualRef; // Maybe?
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public int[] HelmGeosetVis;
        public int ItemVisual;
        public int ParticleColorID;
        public int UnkInt3;
        public int UnkInt4;
        public int UnkInt5;
        public int UnkInt6;
        public int UnkInt7;
        public int UnkInt8;

        public static Form OpenModelViewer(uint recordKey)
        {
            var renderForm = new ItemModelViewerForm();
            renderForm.LoadModel(recordKey);
            return renderForm;
        }
    }
}
