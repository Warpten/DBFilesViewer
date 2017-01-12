using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Runtime.InteropServices;
using DBFilesViewer.Utils.Streams;

namespace DBFilesViewer.Data.IO.CASC
{
    public class IndexStore : Dictionary<Binary, IndexStore.Record>
    {
        public IndexStore() : base(50000) { }
        
        public void FromStream(Stream fileStream)
        {
            using (var reader = new EndianBinaryReader(EndianBitConverter.Little, fileStream))
            {
                reader.BaseStream.Position = (8 + reader.ReadInt32() + 0x0F) & 0xFFFFFFF0;

                var dataLength = reader.ReadInt32();
                var blockCount = dataLength / 18;
                reader.BaseStream.Position += 4; // data check

                // var blockEntries = reader.ReadArray<FastRecord>(blockCount);
                for (var i = 0; i < blockCount; ++i)
                {
                    // var idxLow = blockEntries[i].IndexLowBE >> 24;
                    // idxLow |= ((blockEntries[i].IndexLowBE >> 16) & 0xFF) << 8;
                    // idxLow |= ((blockEntries[i].IndexLowBE >> 8) & 0xFF) << 16;
                    // idxLow |= ((blockEntries[i].IndexLowBE >> 0) & 0xFF) << 24;
                    // 
                    // this[new Binary(new[]
                    // {
                    //     blockEntries[i].Key0, blockEntries[i].Key1, blockEntries[i].Key2, blockEntries[i].Key3,
                    //     blockEntries[i].Key4, blockEntries[i].Key5, blockEntries[i].Key6, blockEntries[i].Key7,
                    //     blockEntries[i].Key8
                    // })] = new Record
                    // {
                    //     ArchiveIndex = (blockEntries[i].IndexHigh << 2) | (byte)((idxLow & 0xC) >> 30),
                    //     Offset = (int)(idxLow & 0xFFFFFFF3),
                    //     Size = blockEntries[i].Size
                    // };


                    var key = reader.ReadBytes(9);
                    var entry = new Record();
                    
                    var indexHigh = reader.ReadByte();
                    reader.BitConverter = EndianBitConverter.Big;
                    var indexLow = reader.ReadUInt32();
                    reader.BitConverter = EndianBitConverter.Little;
                    
                    entry.ArchiveIndex = indexHigh << 2 | (byte)((indexLow & 0xC0000000) >> 30);
                    entry.Offset = (int)(indexLow & 0x3FFFFFFF);
                    entry.Size = reader.ReadInt32();
                    
                    var binaryKey = new Binary(key);
                    
                    if (!ContainsKey(binaryKey))
                        this[binaryKey] = entry;
                }
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        // private struct FastRecord
        // {
        //     public byte Key0, Key1, Key2, Key3, Key4, Key5, Key6, Key7, Key8;
        //     public byte IndexHigh;
        //     public int IndexLowBE;
        //     public int Size;
        // }

        public class Record
        {
            // public byte[] Hash { get; set; }
            public int ArchiveIndex { get; set; }
            public int Size { get; set; }
            public int Offset { get; set; }
        }
    }
}
