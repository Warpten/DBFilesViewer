using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using DBFilesViewer.Data.IO.Files;
using DBFilesViewer.Data.Structures;
using DBFilesViewer.Graphics;
using DBFilesViewer.Graphics.Files.Models;
using DBFilesViewer.Graphics.Scene;
using DBFilesViewer.Utils;

namespace DBFilesViewer.UI.Forms
{
    /// <summary>
    /// This form specializes in rendering creatures.
    /// </summary>
    public sealed class CreatureModelViewerForm : ModelRenderForm
    {
        private FlowLayoutPanel flowLayoutPanel1;
        private ComboBox _animationComboBox;

        public ModelRenderer CreatureRenderer { get; private set; }

        public uint ModelID { get; private set; }
        public CreatureDisplayInfoEntry DisplayInfo { get; private set; }
        public CreatureDisplayInfoExtraEntry ExtraDisplayInfo { get; private set; }

        // Attached items if any
        public ItemDisplayInfoEntry Gloves { get; private set; }
        public ItemDisplayInfoEntry Helmet { get; private set; }
        public ItemDisplayInfoEntry Shoulders { get; private set; }
        public ItemDisplayInfoEntry Shirt { get; private set; }
        public ItemDisplayInfoEntry Chest { get; private set; }
        public ItemDisplayInfoEntry Belt { get; private set; }
        public ItemDisplayInfoEntry Boots { get; private set; }
        public ItemDisplayInfoEntry Bracers { get; private set; }
        public ItemDisplayInfoEntry Tabard { get; private set; }
        public ItemDisplayInfoEntry Pants { get; private set; }
        public ItemDisplayInfoEntry Cape { get; private set; }

        public CharHairGeosetsEntry HairStyle { get; private set; }
        public CharHairGeosetsEntry HDHairStyle { get; private set; }

        public CharacterFacialHairStylesEntry FacialHairStyle { get; private set; }

        public void SetAnimationSource(IEnumerable<AnimationComboBoxEntry> animations)
        {
            _animationComboBox.BeginUpdate();
            _animationComboBox.Items.Clear();
            _animationComboBox.Items.AddRange(animations.Cast<object>().ToArray());
            _animationComboBox.EndUpdate();
        }

        public event Action<List<AnimationComboBoxEntry>> OnAnimationsLoaded;

        public CreatureModelViewerForm()
        {
            InitializeComponent();
        }

        public override ModelRenderer LoadModel(uint creatureModelID, bool clampCamera = true)
        {
            ModelID = creatureModelID;

            DisplayInfo = DBC.Get<CreatureDisplayInfoEntry>()[creatureModelID];
            var renderer = base.LoadModel(DisplayInfo.Model.Value.FileDataID.Key, clampCamera);
            if (clampCamera)
                CreatureRenderer = renderer;

            ExtraDisplayInfo = DisplayInfo.ExtendedDisplayInfo.Value;
            if (ExtraDisplayInfo != null)
            {
                #region Equipped items
                foreach (var equippedItem in DBC.GetEquippedItems(DisplayInfo.ExtendedDisplayInfo.Key))
                {
                    switch (equippedItem.ItemType)
                    {
                        case ItemType.HEAD:
                            Helmet = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.SHOULDER:
                            Shoulders = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.SHIRT:
                            Shirt = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.CHEST:
                            Chest = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.BELT:
                            Belt = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.PANTS:
                            Pants = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.BOOTS:
                            Boots = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.BRACERS:
                            Bracers = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.GLOVES:
                            Gloves = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.TABARD:
                            Tabard = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        case ItemType.CAPE:
                            Cape = equippedItem.ItemDisplayInfoID.Value;
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
                #endregion

                #region (Facial) Hair style

                var hairStyleGeoset = DBC.Get<CharHairGeosetsEntry>().Where(kv =>
                    kv.Value.Gender == DisplayInfo.Gender &&
                    kv.Value.RaceID == ExtraDisplayInfo.DisplayRaceID &&
                    kv.Value.VariationID == ExtraDisplayInfo.HairStyleID &&
                    (kv.Value.VariationType == 3 || kv.Value.VariationType == 8)).ToArray();
                if (hairStyleGeoset.Length != 0)
                {
                    HairStyle = hairStyleGeoset.FirstOrDefault(k => k.Value.VariationType == 3).Value;
                    HDHairStyle = hairStyleGeoset.FirstOrDefault(k => k.Value.VariationType == 8).Value;
                }

                var facialHairStyle = DBC.Get<CharacterFacialHairStylesEntry>().Where(kv =>
                    kv.Value.Gender == DisplayInfo.Gender &&
                    kv.Value.RaceID == ExtraDisplayInfo.DisplayRaceID &&
                    kv.Value.VariationType == ExtraDisplayInfo.FacialHairID).ToArray();
                if (facialHairStyle.Length != 0)
                    FacialHairStyle = facialHairStyle[0].Value;

                #endregion
            }

            renderer.OnLoadTexture += OnLoadTexture;
            renderer.OnFilterMeshes += OnFilterMeshes;

            if (ExtraDisplayInfo != null)
            {
                TryAttach(Helmet, 11);
                TryAttach(Shoulders, 5, 6);
                TryAttach(Shirt, 34);
                TryAttach(Chest, 34);
                TryAttach(Tabard, 34);
                TryAttach(Cape, 12);
            }

            renderer.PrepareRender();

            if (renderer.Model.MD20.SequenceLookups.Length != 0)
            {
                var availableAnimations = (from kv in DBC.Get<AnimationDataEntry>()
                    where kv.Key < renderer.Model.MD20.SequenceLookups.Length
                    where renderer.Model.MD20.SequenceLookups[kv.Key] >= 0
                    select new AnimationComboBoxEntry {
                        ID = kv.Key, Name = kv.Value.Name
                    }).ToList();
                OnAnimationsLoaded?.Invoke(availableAnimations);
            }

            return renderer;
        }

        /// <summary>
        /// Selects meshes based off CreatureDisplayInfoExtra.db2
        /// </summary>
        /// <param name="renderPasses"></param>
        private void OnFilterMeshes(List<ModelRenderPass> renderPasses)
        {
            // key is geoset group, value is subgroup
            // translates to meshId by i * 100 + [i] + 1
            var enabledGeosets = new int[26];
            for (var i = 0; i < 26; ++i)
                enabledGeosets[i] = i * 100 + 1;

            enabledGeosets[7] = 702; // Ears

            if (DisplayInfo.Model.Value.CreatureGeosetDataID != 0)
            {
                for (var i = 0; i <= 8; ++i)
                {
                    var value = (DisplayInfo.CreatureGeosetData >> (4 * i)) & 0xF;
                    if (value != 0)
                        enabledGeosets[i] = (int) (value + i * 100);
                }
            }

            if (ExtraDisplayInfo != null)
            {
                if (FacialHairStyle != null)
                {
                    var groups = new[] { 100, 300, 200, 1600, 1700 };
                    for (var i = 0; i < FacialHairStyle.Geosets.Length; ++i)
                        if (FacialHairStyle.Geosets[i] != 0)
                            enabledGeosets[groups[i] / 100] = groups[i] + FacialHairStyle.Geosets[i];
                }

                if (HairStyle != null && HairStyle.GeosetID != 0)
                    enabledGeosets[0] = HairStyle.GeosetID;

                // Glowy DK eyes
                if (ExtraDisplayInfo.DisplayClassID == Classes.DeathKnight)
                    enabledGeosets[17] = 1703;

                if (Gloves != null)
                {
                    if (Gloves.GeosetGroups[0] != 0)
                        enabledGeosets[4] = Gloves.GeosetGroups[0] + 401;
                }
                else if (Chest != null)
                {
                    if (Chest.GeosetGroups[0] != 0)
                        enabledGeosets[8] = Chest.GeosetGroups[0] + 801;
                }
                /*else if (Shirt != null)
                {
                    if (Shirt.GeosetGroups[0] != 0)
                        enabledGeosets[8] = Shirt.GeosetGroups[0] + 801;
                }*/

                var hasBulkyBelt = Belt != null && Belt.GeosetGroups[0] != 0 && (Belt.Flags & (1 << 9)) != 0;
                var dressPants = false;
                var dressChestpiece = false;

                if (Chest != null && Chest.GeosetGroups[2] != 0)
                {
                    dressChestpiece = true;
                    enabledGeosets[13] = Chest.GeosetGroups[2] + 1301;
                }
                else if (Pants != null && Pants.GeosetGroups[2] != 0 /* && characterComponent.field_20 & 0x08 */)
                {
                    dressPants = true;
                    enabledGeosets[13] = Pants.GeosetGroups[2] + 1301;
                }
                else if (Boots != null && Boots.GeosetGroups[0] != 0)
                {
                    // enabledGeosets[9] = 1;
                    enabledGeosets[5] = Boots.GeosetGroups[0] + 501;
                }
                else if (Pants != null)
                    enabledGeosets[9] = Pants.GeosetGroups[1] + 901;

                if (Boots != null)
                {
                    if (Boots.GeosetGroups[1] == 0)
                        enabledGeosets[20] = 2002;
                    else
                        enabledGeosets[20] = Boots.GeosetGroups[1] + 2000;
                }
                else
                    enabledGeosets[20] = 2001;

                // Tabard here
                var showTabard = false;
                var hasDress = (dressChestpiece || dressPants);
                if (!hasDress && !hasBulkyBelt)
                {
                    if (Tabard != null && Tabard.GeosetGroups[0] != 0)
                    {
                        showTabard = true;
                        enabledGeosets[12] = Tabard.GeosetGroups[0] + 1201;
                    }
                }

                if (!showTabard && !hasDress)
                {
                    if (Chest != null && Chest.GeosetGroups[1] != 0)
                        enabledGeosets[10] = Chest.GeosetGroups[1] + 1001;
                    else if (Shirt != null && Shirt.GeosetGroups[1] != 0)
                        enabledGeosets[10] = Shirt.GeosetGroups[1] + 1001;
                }

                if (!dressChestpiece)
                {
                    if (Pants != null && Pants.GeosetGroups[0] != 0)
                    {
                        var geosetId = Pants.GeosetGroups[0];
                        if (geosetId > 2)
                        {
                            enabledGeosets[13] = 1300;
                            enabledGeosets[11] = geosetId + 1101;
                        }
                        else if (showTabard)
                            enabledGeosets[11] = geosetId + 1101;
                    }
                }

                if (Cape != null && Cape.GeosetGroups[0] != 0)
                    enabledGeosets[15] = Cape.GeosetGroups[0] + 1501;

                if (Belt != null && Belt.GeosetGroups[0] != 0)
                    enabledGeosets[18] = Belt.GeosetGroups[0] + 1801;

                enabledGeosets[14] = showTabard && !hasDress ? 1401 : 1400;
            }

            foreach (var pass in renderPasses)
                pass.Enabled = pass.MeshID == 0 || enabledGeosets.Contains(pass.MeshID);

            Console.WriteLine("[DEBUG] {0} passes enabled out of {1}", renderPasses.Count(p => p.Enabled), renderPasses.Count);
        }

        /// <summary>
        /// Loads textures that are not of type 0.
        /// </summary>
        /// <param name="textureType"></param>
        /// <returns>The texture to render.</returns>
        private Texture OnLoadTexture(uint textureType)
        {
            Console.WriteLine("[DEBUG] Loading texture type {0}", textureType);

            switch (textureType)
            {
                case 1: // TEX_COMPONENT_SKIN
                    // TODO: HD Textures require proper offsetting ("chunks" of the texture)
                    // http://wowdev.wiki/DB/CharComponentTextureSections
                    return ExtraDisplayInfo?.GetTexture(Context.TextureManager, false);
                case 2: // TEX_COMPONENT_OBJECT_SKIN -- cape for NPCs
                {
                    if (Cape != null)
                        return Context.TextureManager.GetTexture(Cape.Textures[0].Key);
                    break;
                }
                case 6: // TEX_COMPONENT_CHAR_HAIR
                {
                    if (ExtraDisplayInfo == null)
                        return null;

                    var availableEntries = DBC.Get<CharSectionsEntry>()?.Where(
                        kv =>
                            (kv.Value.GenType == 8 || kv.Value.GenType == 3) &&
                            kv.Value.Gender == ExtraDisplayInfo.DisplaySexID &&
                            kv.Value.Color == ExtraDisplayInfo.HairColorID &&
                            kv.Value.Type == ExtraDisplayInfo.HairStyleID &&
                            kv.Value.Race == (byte) ExtraDisplayInfo.DisplayRaceID).Select(kv => kv.Value).ToArray();
                    if (availableEntries == null)
                        return null;
                    var entry = availableEntries.FirstOrDefault(kv => kv.GenType == 3) ?? availableEntries.FirstOrDefault(kv => kv.GenType == 8);
                    return entry == null ? null : Context.TextureManager.GetTexture(entry.GetTexFileDataID(0));
                }
                case 7: // TEX_COMPONENT_CHAR_FACIAL_HAIR
                {
                    if (ExtraDisplayInfo == null)
                        return null;

                    var availableEntries = DBC.Get<CharSectionsEntry>()?.Where(
                        kv =>
                            (kv.Value.GenType == 7 || kv.Value.GenType == 2) &&
                            kv.Value.Gender == ExtraDisplayInfo.DisplaySexID &&
                            kv.Value.Type == ExtraDisplayInfo.FacialHairID &&
                            kv.Value.Color == ExtraDisplayInfo.HairColorID &&
                            kv.Value.Race == (byte) ExtraDisplayInfo.DisplayRaceID).Select(kv => kv.Value).ToArray();
                    if (availableEntries == null)
                        return null;
                    var entry = availableEntries.FirstOrDefault(kv => kv.GenType == 2) ?? availableEntries.FirstOrDefault(kv => kv.GenType == 7);
                    return entry == null ? null : Context.TextureManager.GetTexture(entry.GetTexFileDataID(0));
                }
                case 8: // TEX_COMPONENT_SKIN_EXTRA
                {
                    if (ExtraDisplayInfo == null)
                        return null;

                    var availableEntries =
                        DBC.Get<CharSectionsEntry>()?
                            .Where(
                                kv =>
                                    (kv.Value.GenType == 0 || kv.Value.GenType == 5) &&
                                    kv.Value.Gender == ExtraDisplayInfo.DisplaySexID &&
                                    kv.Value.Color == ExtraDisplayInfo.SkinID &&
                                    kv.Value.Race == (byte) ExtraDisplayInfo.DisplayRaceID)
                            .Select(kv => kv.Value)
                            .ToArray();
                    if (availableEntries == null)
                        return null;
                    var entry = availableEntries.FirstOrDefault(kv => kv.GenType == 0) ?? availableEntries.FirstOrDefault(kv => kv.GenType == 5);
                    return entry == null ? null : Context.TextureManager.GetTexture(entry.GetTexFileDataID(0));
                }
                case 11: // TEX_COMPONENT_MONSTER_1
                case 12: // TEX_COMPONENT_MONSTER_2
                case 13: // TEX_COMPONENT_MONSTER_3
                    return Context.TextureManager.GetTexture(DisplayInfo.TextureVariation[textureType - 11]);
            }
            return null;
        }

        // Utility function used to attach equipment to the creature.
        private void TryAttach(ItemDisplayInfoEntry item, params int[] slots)
        {
            if (item == null)
                return;

            foreach (var slot in slots)
            {
                if (!CreatureRenderer.CanAttach(slot))
                    continue;

                var order = -1;
                if (slot == 5 || slot == 6) // Neat math trick
                    order = 6 - slot;

                var itemFile = DBC.GetModelFile(item.Model[0], DisplayInfo, order);
                if (itemFile == 0)
                    itemFile = DBC.GetModelFile(item.Model[1], DisplayInfo, order);

                if (itemFile == 0)
                    continue;

                var itemRenderer = new ModelRenderer(itemFile, Context);
                itemRenderer.OnLoadTexture += textureType =>
                {
                    switch (textureType)
                    {
                        case 2: // TEX_COMPONENT_OBJECT_SKIN 
                            return Context.TextureManager.GetTexture(item.Textures[0].Key);
                        case 3: // TEX_COMPONENT_WEAPON_BLADE (not used in client)
                            return Context.TextureManager.GetTexture(@"Item\ObjectComponents\Weapon\ArmorReflect4.BLP");
                    }
                    return null;
                };
                itemRenderer.PrepareRender();
                CreatureRenderer.Attach(slot, itemRenderer);
            }
        }

        private void InitializeComponent()
        {
            this._animationComboBox = new System.Windows.Forms.ComboBox();
            this.flowLayoutPanel1 = new System.Windows.Forms.FlowLayoutPanel();
            this.flowLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // _animationComboBox
            // 
            this._animationComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)((AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this._animationComboBox.FormattingEnabled = true;
            this._animationComboBox.Location = new System.Drawing.Point(3, 3);
            this._animationComboBox.Name = "_animationComboBox";
            this._animationComboBox.Size = new System.Drawing.Size(154, 21);
            this._animationComboBox.TabIndex = 0;
            this._animationComboBox.SelectionChangeCommitted += new System.EventHandler(this.OnAnimationSelected);
            // 
            // flowLayoutPanel1
            // 
            this.flowLayoutPanel1.Controls.Add(this._animationComboBox);
            this.flowLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.flowLayoutPanel1.Location = new System.Drawing.Point(0, 395);
            this.flowLayoutPanel1.Name = "flowLayoutPanel1";
            this.flowLayoutPanel1.Size = new System.Drawing.Size(784, 28);
            this.flowLayoutPanel1.TabIndex = 1;
            // 
            // CreatureModelViewerForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.ClientSize = new System.Drawing.Size(784, 423);
            this.Controls.Add(this.flowLayoutPanel1);
            this.Location = new System.Drawing.Point(0, 0);
            this.Name = "CreatureModelViewerForm";
            this.flowLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        private void OnAnimationSelected(object sender, EventArgs e)
        {
            var selectedItem = _animationComboBox.SelectedItem as AnimationComboBoxEntry;
            if (selectedItem != null)
                CreatureRenderer.SetAnimation(selectedItem.ID);
        }
    }
}
