using System.Runtime.InteropServices;
using DBFilesClient.NET;
using DBFilesViewer.Data.Structures.Types;

namespace DBFilesViewer.Data.Structures
{
    [DBFileName("LiquidType")]
    public sealed class LiquidTypeEntry
    {
        public string Name;
        public ForeignKey<SpellEntry> SpellID;
        public float MaxDarkenDepth;
        public float FogDarkenIntensity;
        public float AmbDarkenIntensity;
        public float DirDarkenIntensity;
        public float ParticleScale;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public string[] Texture;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public PackedColor[] Color;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 18)]
        public float[] Float;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public uint[] Int;
        public ushort Flags;
        public ushort LightID;
        public byte Type;
        public byte ParticleMovement;
        public byte ParticleTexSlots;
        public byte MaterialID;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] DepthTexCount;
        public uint SoundID;
    }
}
