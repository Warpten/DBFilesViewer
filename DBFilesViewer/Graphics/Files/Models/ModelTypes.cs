using System.Runtime.InteropServices;
using DBFilesViewer.Graphics.Files.Models.Animations;
using DBFilesViewer.Utils;
using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models
{
    public struct M2Array<T>
    {
        public int Count;
        public uint Offset;
    }

    public struct M2HeaderFlags
    {
        private BitSet<uint> _value;

        public bool TiltX => _value[0];
        public bool TiltY => _value[1];
        public bool UnkBit1 => _value[2];
        public bool HasBlendMaps => _value[3];
        public bool UnkBit2 => _value[4];
        public bool LoadPhysData => _value[5];
        public bool UnkBit3 => _value[6];
        public bool UnkBit4 => _value[7];
        public bool UnkBit5_Camera => _value[8];
    }

    #region Structures in files
    [StructLayout(LayoutKind.Sequential)]
    public struct M2Header
    {
        public int Version;

        public M2Array<char> Name;

        public M2HeaderFlags GlobalFlags;
        
        public M2Array<uint> GlobalLoops;
        public M2Array<AnimationSequence> Sequences;
        public M2Array<short> SequenceLookup;
        public M2Array<M2Bone> Bones;
        public M2Array<ushort> KeyBoneLookup;
        public M2Array<M2Vertex> Vertices;
        public int NumSkinProfiles;
        public M2Array<M2Color> Colors;
        public M2Array<M2Texture> Textures;
        public M2Array<M2TextureWeight> TextureWeights;
        public M2Array<M2TextureTransform> TextureTransforms;
        public M2Array<ushort> ReplaceableTextureLookup;
        public M2Array<M2Material> Materials;
        public M2Array<ushort> BoneLookupTable;
        public M2Array<ushort> TextureLookupTable;
        private M2Array<ushort> TextureUnitLookupTable; // Unused
        public M2Array<ushort> TransparencyLookupTable;
        public M2Array<short> TextureTransformsLookupTable;
        public BoundingBox BoundingBox;
        public float BoundingSphereRadius;
        public BoundingBox CollisionBox;
        public float CollisionSphereRadius;
        public M2Array<ushort> CollisionTriangles;
        public M2Array<Vector3> CollisionVertices;
        public M2Array<Vector3> CollisionNormals;
        public M2Array<M2Attachment> Attachments;
        public M2Array<short> AttachmentLookupTable;
        public M2Array<char /* M2Event */> Events;
        public M2Array<char /* M2Light */> Lights;
        public M2Array<char /* M2Camera */> Cameras;
        public M2Array<ushort> CameraLookupTable;
        public M2Array<char /* M2RibbonEmitters */> RibbonEmitters;
        public M2Array<char /* M2Particle */> ParticleEmitters;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct AnimationBlock<T>
    {
        public InterpolationType Interpolation;
        public short GlobalSequence;
        public M2Array<M2Array<uint>> Timestamps;
        public M2Array<M2Array<T>> Values;
    }

    public enum InterpolationType : short
    {
        None = 0,
        Linear,
        Hermite,
        Bezier
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2Bone
    {
        public int KeyBoneId;
        public uint Flags;
        public short ParentBone;
        public ushort SubMeshId;
        private ushort unk1, unk2;
        public AnimationBlock<Vector3> Translation;
        public AnimationBlock<Quaternion16> Rotation;
        public AnimationBlock<Vector3> Scaling;
        public Vector3 Pivot;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2Texture
    {
        public uint Type;
        public M2TextureFlags Flags;
        public M2Array<char> Name;
    }

    public struct M2TextureFlags
    {
        private BitSet<uint> _value;

        public bool WrapX => _value[0];
        public bool WrapY => _value[1];
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2Color
    {
        public AnimationBlock<Vector3> Colors;
        public AnimationBlock<short> Alpha;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2TextureWeight
    {
        public AnimationBlock<short> Weight;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2TextureTransform
    {
        public AnimationBlock<Vector3> Translation;
        public AnimationBlock<InvQuaternion16> Rotation;
        public AnimationBlock<Vector3> Scaling;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2Skin
    {
        public int Magic;
        public M2Array<ushort> Indices;
        public M2Array<ushort> Triangles;
        public M2Array<char /* ? */> Properties; 
        public M2Array<M2SkinSection> SubMeshes; 
        public M2Array<M2Batch> TexUnits;
        public int bones;
        public int nShadowBatches;
        public int ofsShadowBatches;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2SkinSection
    {
        public ushort MeshPartId;
        public ushort Level;
        public ushort StartVertex;
        public ushort VerticeCount;
        public ushort StartTriangle;
        public ushort NumTriangles;
        public ushort BoneCount;
        public ushort StartBone;
        private ushort unk2;
        public ushort RootBone;
        public Vector3 centerMass;
        public Vector3 CenterBoundingBox;
        public float Radius;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2Batch
    {
        public ushort Flags;
        public ushort ShaderId;
        public ushort SubmeshIndex;
        public ushort submeshIndex2;
        public short ColorIndex;
        public ushort RenderFlags;
        public ushort TexUnitNumber;
        public ushort OpCount;
        public ushort Texture;
        public ushort TexUnitNumber2;
        public short TransparencyIndex;
        public short TextureAnimIndex;
    }

    [StructLayout(LayoutKind.Sequential)]
    public struct M2Material
    {
        public M2MaterialRenderFlags RenderFlags; // ushort
        public ushort BlendingMode;
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct M2Vertex
    {
        public Vector3 Position;
        public fixed byte BoneWeights[4];
        public fixed byte BoneIndices[4];
        public Vector3 Normal;
        public Vector2 TexCoord1;
        public Vector2 TexCoord2;
    }

    public struct AnimationSequence
    {
        public ushort AnimationID;
        public ushort SubAnimID;
        public uint Duration; // Milliseconds
        public float Speed; // Speed at which the character moves.
        public AnimationFlags Flags;
        public ushort Probability;
        private ushort __padding;
        public uint MinimumRepetitions;
        public uint MaximumRepetitions;
        public uint BlendTime;
        public BoundingBox Bounds;
        public float BoundingRadius;
        public short NextAnimation;
        public short NextAlias;
    }

    public struct M2Attachment
    {
        public uint ID;
        public ushort Bone;
        private ushort unk0;
        public Vector3 Position;
        public AnimationBlock<char> AnimateAttached;
    }
    #endregion

    #region Structures fully constructed
    public struct AnimationFlags
    {
        private BitSet<uint> _value;

        public bool Looped => _value[5];
        public bool IsAlias => _value[6];
    }

    public struct ModelTexture
    {
        public uint Type { get; set; }
        public M2TextureFlags Flags { get; set; }
        public string Name { get; set; }
    }

    public struct M2MaterialRenderFlags
    {
        private BitSet<ushort> _value;

        public bool Unlit => _value[0];
        public bool Unfogged => _value[1];
        public bool CullingDisabled => _value[2];
        public bool UnkFlag => _value[3];
    }
    #endregion
}
