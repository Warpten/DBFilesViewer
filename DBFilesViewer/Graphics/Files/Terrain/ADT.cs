using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Data.IO.Files.Models;
using DBFilesViewer.Graphics.Files.Models.Animations;
using DBFilesViewer.Utils;
using DBFilesViewer.Utils.Extensions;

namespace DBFilesViewer.Graphics.Files.Terrain
{
    public sealed class ADT
    {
        private Chunk[] _terrainChunks;

        public ADT(string directoryName)
        {
            var baseName = $"World/Maps/{directoryName}/{directoryName}";

            using (var rootADT = Manager.OpenFile($"{baseName}.adt"))
            using (var rootReader = new BinaryReader(rootADT))
                LoadRoot(rootReader);
        }

        private void LoadRoot(BinaryReader rootFile)
        {
            var chunkedFile = new ChunkData<BinaryReader>(rootFile);

            foreach (var chunk in chunkedFile.Chunks)
            {
                switch (chunk.Name)
                {
                    case "MHDR":
                        MHDR = new MHDR(chunk);
                        break;
                }
            }
        }

        public MHDR MHDR { get; private set; }

        public void Render(BillboardParameters billboard)
        {
            foreach (var chunk in _terrainChunks)
                chunk.Render(billboard);
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
