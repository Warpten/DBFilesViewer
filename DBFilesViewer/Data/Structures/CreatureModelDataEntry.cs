using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("CreatureModelData")]
    public sealed class CreatureModelDataEntry
    {
        public float ModelScale;
        public float FootprintTextureLength;
        public float FootprintTextureWidth;
        public float FootprintParticleScale;
        public float CollisionWidth;
        public float CollisionHeight;
        public float MountHeight;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public float[] GeoBox;
        public float WorldEffectScale;
        public float AttachedEffectScale;
        public float MissileCollisionRadius;
        public float MissileCollisionPush;
        public float MissileCollisionRaise;
        public float OverrideLootEffectScale;
        public float OverrideNameScale;
        public float OverrideSelectionRadius;
        public float TamedPetBaseScale;
        public float HoverHeight;
        public uint Flags;
        public ModelFile FileDataID; // Model (M2)
        public uint SizeClass;
        public uint BloodID;
        public uint FootprintTextureID;
        public uint FoleyMaterialID;
        public uint FootstepShakeSize;
        public uint DeathThudShakeSize;
        public uint SoundKitID;
        public uint CreatureGeosetDataID;
    }
}
