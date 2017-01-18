using System.IO;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Data.IO.Files.Models;
using DBFilesViewer.Utils;
using DBFilesViewer.Utils.Extensions;
using SharpDX;

namespace DBFilesViewer.Graphics.Files.Terrain
{
    public sealed class ADT
    {
        private ChunkData<BinaryReader> _rootData, _texData, _objData;

        private MHDR MHDR { get; set; }
        private MCNK[] MCNK { get; } = new MCNK[256];

        public ADT(string directoryName, int i, int j)
        {
            var baseName = $"World/Maps/{directoryName}/{directoryName}_{i}_{j}";

            var rootADT = Manager.OpenFile($"{baseName}.adt");
            if (rootADT == null)
                return;

            _rootData = new ChunkData<BinaryReader>(new BinaryReader(rootADT));

            var textureADT = Manager.OpenFile($"{baseName}_tex0.adt");
            if (textureADT != null)
                _texData = new ChunkData<BinaryReader>(new BinaryReader(textureADT));

            var objectADT = Manager.OpenFile($"{baseName}_obj0.adt");
            if (objectADT != null)
                _objData = new ChunkData<BinaryReader>(new BinaryReader(objectADT));

            var mcnkIndex = 0;
            foreach (var rootChunk in _rootData.Chunks)
            {
                switch (rootChunk.Name)
                {
                    case "MHDR":
                        MHDR = new MHDR(rootChunk);
                        break;
                    case "MCNK":
                        MCNK[mcnkIndex] = new MCNK(rootChunk);
                        ++mcnkIndex;
                        break;
                }
            }
        }
    }

    public sealed class MCNK : ChunkReader<BinaryReader>
    {
        public MCNK(Chunk<BinaryReader> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MCNK(Chunk<BinaryReader> chunk, bool read = true) : base(chunk, read) { }

        public struct HeaderFlags
        {
            private BitSet<uint> _value;
            public uint Value => _value.Value;

            public bool HasMCSH => _value[0];
            public bool Impass => _value[1];
            public bool IsLiquidRiver => _value[2];
            public bool IsLiquidOcean => _value[3];
            public bool IsLiquidMagma => _value[4];
            public bool IsLiquidSlime => _value[5];
            public bool HasMCCV => _value[6];
            public bool DoNotFixMCAL => _value[15];
            public bool HighResolutionHoles => _value[16];
        }

        public HeaderFlags Flags { get; private set; }
        public uint X { get; private set; }
        public uint Y { get; private set; }
        public uint LayerCount { get; private set; }
        public uint DoodadRefCount { get; private set; }
        public ulong HighResolutionHoles { get; private set; }
        public uint AreaID { get; private set; }
        public uint MapObjRefCount { get; private set; }
        public ushort Holes { get; private set; }
        public ushort[] LowQualityTexturingMap { get; private set; }
        public Vector3 Position { get; private set; }
        public ulong EffectDoodadCount { get; private set; }

        public MCVT MCVT { get; private set; }
        public MCCV MCCV { get; private set; }
        public MCNR MCNR { get; private set; }
        public MCLY MCLY { get; private set; }

        public override void Read()
        {
            Flags = Chunk.Reader.ReadStruct<HeaderFlags>();
            X = Chunk.Reader.ReadUInt32();
            Y = Chunk.Reader.ReadUInt32();
            LayerCount = Chunk.Reader.ReadUInt32();
            DoodadRefCount = Chunk.Reader.ReadUInt32();
            HighResolutionHoles = Chunk.Reader.ReadUInt64();
            Chunk.Reader.BaseStream.Position += 6 * 4;
            AreaID = Chunk.Reader.ReadUInt32();
            MapObjRefCount = Chunk.Reader.ReadUInt32();
            Holes = Chunk.Reader.ReadUInt16();
            Chunk.Reader.BaseStream.Position += 2; // In alpha: padding
            LowQualityTexturingMap = Chunk.Reader.ReadArray<ushort>(8);
            EffectDoodadCount = Chunk.Reader.ReadUInt64();
            Chunk.Reader.BaseStream.Position += 4 * 4;
            Position = Chunk.Reader.ReadStruct<Vector3>();
            Chunk.Reader.BaseStream.Position += 4 * 4;

            var chunks = new ChunkData<BinaryReader>(Chunk.Reader, true, (uint)(Chunk.Size - Chunk.Reader.BaseStream.Position));
            foreach (var subChunk in chunks.Chunks)
            {
                switch (subChunk.Name)
                {
                    case "MCVT":
                        MCVT = new MCVT(subChunk);
                        break;
                    // case "MCLV":
                    //     MCLV = new MCLV(subChunk);
                    //     break;
                    case "MCCV":
                        MCCV = new MCCV(subChunk);
                        break;
                    case "MCNR":
                        MCNR = new MCNR(subChunk);
                        break;
                    case "MCLY":
                        MCLY = new MCLY(subChunk);
                        break;
                }
            }
        }
    }

    public class MCVT : ChunkReader<BinaryReader>
    {
        public MCVT(Chunk<BinaryReader> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MCVT(Chunk<BinaryReader> chunk, bool read = true) : base(chunk, read) { }

        public float[] Heights { get; private set; }

        public override void Read()
        {
            Heights = Chunk.Reader.ReadArray<float>(9 * 9 + 8 * 8);
        }
    }

    public class MCCV : ChunkReader<BinaryReader>
    {
        public MCCV(Chunk<BinaryReader> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MCCV(Chunk<BinaryReader> chunk, bool read = true) : base(chunk, read) { }

        public Vector4[] Components { get; } = new Vector4[9 * 9 + 8 * 8];

        public override void Read()
        {
            for (var i = 0; i < Components.Length; ++i)
                Components[i] = new Vector4(Chunk.Reader.ReadSByte() / 127.0f, Chunk.Reader.ReadSByte() / 127.0f, Chunk.Reader.ReadSByte() / 127.0f, Chunk.Reader.ReadSByte() / 127.0f);
        }
    }

    public class MCNR : ChunkReader<BinaryReader>
    {
        public MCNR(Chunk<BinaryReader> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MCNR(Chunk<BinaryReader> chunk, bool read = true) : base(chunk, read) { }

        public Vector3[] Normals { get; } = new Vector3[9 * 9 + 8 * 8];

        public override void Read()
        {
            for (var i = 0; i < Normals.Length; ++i)
                Normals[i] = new Vector3(Chunk.Reader.ReadSByte() / 127.0f, Chunk.Reader.ReadSByte() / 127.0f, Chunk.Reader.ReadSByte() / 127.0f);
        }
    }

    public class MCLY : ChunkReader<BinaryReader>
    {
        public MCLY(Chunk<BinaryReader> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MCLY(Chunk<BinaryReader> chunk, bool read = true) : base(chunk, read) { }

        public uint[] TextureID { get; } = new uint[4];
        public uint[] Flags { get; } = new uint[4];
        public uint[] MCAL { get; } = new uint[4]; // MCAL Offset
        public uint[] EffectID { get; } = new uint[4]; // Entry in GroundEffectTexture.db2

        public override void Read()
        {
            for (var i = 0; i < 4 && i < Chunk.Size / 16; ++i)
            {
                TextureID[i] = Chunk.Reader.ReadUInt32();
                Flags[i] = Chunk.Reader.ReadUInt32();
                MCAL[i] = Chunk.Reader.ReadUInt32();
                EffectID[i] = Chunk.Reader.ReadUInt32();
            }
        }
    }

    public class MHDR : ChunkReader<BinaryReader>
    {
        public MHDR(Chunk<BinaryReader> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MHDR(Chunk<BinaryReader> chunk, bool read = true) : base(chunk, read) { }

        public uint MCIN { get; private set; }
        public uint MTEX { get; private set; }
        public uint MMDX { get; private set; }
        public uint MMID { get; private set; }
        public uint MWMO { get; private set; }
        public uint MWID { get; private set; }
        public uint MDDF { get; private set; }
        public uint MODF { get; private set; }
        public uint MFBO { get; private set; }
        public uint MH2O { get; private set; }
        public uint MTXF { get; private set; }
        // unk and padding/unused

        public struct TerrainFlags
        {
            private BitSet<uint> _value;
            public uint Value => _value.Value;

            public bool HasMFBO => _value[0];
            public bool IsNorthrend => _value[1]; // Guessed name
        }

        public TerrainFlags Flags { get; private set; }

        public override void Read()
        {
            Flags = Chunk.Reader.ReadStruct<TerrainFlags>();
            MCIN = Chunk.Reader.ReadUInt32();
            MTEX = Chunk.Reader.ReadUInt32();
            MMDX = Chunk.Reader.ReadUInt32();
            MMID = Chunk.Reader.ReadUInt32();
            MWMO = Chunk.Reader.ReadUInt32();
            MWID = Chunk.Reader.ReadUInt32();
            MDDF = Chunk.Reader.ReadUInt32();
            MODF = Chunk.Reader.ReadUInt32();
            MFBO = Chunk.Reader.ReadUInt32();
            MH2O = Chunk.Reader.ReadUInt32();
            MTXF = Chunk.Reader.ReadUInt32();
        }
    }
}
