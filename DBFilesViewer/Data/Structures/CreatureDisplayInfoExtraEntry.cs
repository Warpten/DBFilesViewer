using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;
using DBFilesViewer.Graphics;
using DBFilesViewer.Graphics.Scene;
using DBFilesViewer.Utils;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CreatureDisplayInfoExtra")]
    public sealed class CreatureDisplayInfoExtraEntry
    {
        // <Warpten_> or remap TextureFileData.db2 with TextureFileDataID as key
        // <Simca> but how when TextureFileDataID is not unique(many rows have same value for that)
        // <Warpten_> well, sounds like a "fuck me"
        // <Simca> I feel your pain; my solution was not elegant.
        // <Warpten_> assuming i got you right, i need to map to the first value, while the db2 is keyed on the last here: http://i.imgur.com/J1Stpfu.png
        // <Simca> yes
        // <Warpten_> "fuck me" indeed
        // <Simca> and you need the values from another DB2 to decide which entry to pick when there are multiple with same TextureFileDataID
        // <Simca> ComponentTextureFileData.db2
        // <Warpten_> bloody hell
        // <Warpten_> ok, this is a todo.
        // <Warpten_> skip!
        public CreatureDisplayInfoTextureKey TextureFileDataID;
        public CreatureDisplayInfoTextureKey HDTextureFileDataID;
        public Races DisplayRaceID;
        public Genders DisplaySexID;
        public Classes DisplayClassID;
        public byte SkinID;
        public byte FaceID;
        public byte HairStyleID;
        public byte HairColorID;
        public byte FacialHairID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] CustomDisplayOption;
        public byte Flags;

        public Texture GetTexture(TextureManager manager, bool forceHighDefinition = true)
        {
            if (forceHighDefinition)
                return manager.GetTexture(HDTextureFileDataID.Value?.FileDataID.Key ?? 0u) ??
                       manager.GetTexture(TextureFileDataID.Value?.FileDataID.Key ?? 0u);
            return manager.GetTexture(TextureFileDataID.Value?.FileDataID.Key ?? 0u) ??
                   manager.GetTexture(HDTextureFileDataID.Value?.FileDataID.Key ?? 0u);
        }
    }
}
