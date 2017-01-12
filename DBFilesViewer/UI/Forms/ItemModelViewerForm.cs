using DBFilesViewer.Data.IO.Files;
using DBFilesViewer.Data.Structures;
using DBFilesViewer.Graphics.Scene;
using DBFilesViewer.Utils;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// A specialization of <see cref="ModelRenderForm"/> that only handles rendering items.
    /// </summary>
    public sealed class ItemModelViewerForm : ModelRenderForm
    {
        public override ModelRenderer LoadModel(uint itemDisplayInfoEntryID, bool clampCamera = true)
        {
            var itemDisplayInfo = DBC.Get<ItemDisplayInfoEntry>()[itemDisplayInfoEntryID];
            var renderer = base.LoadModel(DBC.GetModelFile(itemDisplayInfo.Model[0], Genders.Neutral, Classes.None, Races.None), clampCamera);

            renderer.OnLoadTexture += textureType =>
            {
                switch (textureType)
                {
                    case 2: // TEX_COMPONENT_OBJECT_SKIN 
                        return Context.TextureManager.GetTexture(itemDisplayInfo.Textures[0].Key);
                    case 3: // TEX_COMPONENT_WEAPON_BLADE (not used in client)
                        return Context.TextureManager.GetTexture(@"Item\ObjectComponents\Weapon\ArmorReflect4.BLP");
                }
                return null;
            };

            renderer.PrepareRender();
            return renderer;
        }
    }
}
