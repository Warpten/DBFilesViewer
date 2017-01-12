using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using DBFilesViewer.Utils.Streams;

namespace DBFilesViewer.Data.IO.CASC
{
    public class Encoding : Dictionary<Binary, Encoding.Entry>
    {
        public Encoding() : base(50000) { }

        public bool FromStream(BLTE fileStream)
        {
            using (var reader = new EndianBinaryReader(EndianBitConverter.Little, fileStream))
            {
                if (reader.ReadInt16() != 0x4E45) // EN
                    return false;

                reader.BaseStream.Position += 1;
                var checksumSize = reader.ReadByte();
                reader.BaseStream.Position += 1 + 2 + 2; // Skip checksum size, flagsA, flagsB

                reader.BitConverter = EndianBitConverter.Big;
                var tableEntryCount = reader.ReadUInt32();
                reader.BaseStream.Position += 4 + 1; // skip entrycountB, unk
                var stringBlockSize = reader.ReadInt32(); // String block size (which we won't use)

                // Skip string block and hash headers
                reader.BaseStream.Position += stringBlockSize + (16 + 16) * tableEntryCount;

                reader.BitConverter = EndianBitConverter.Little;

                var chunkStart = reader.BaseStream.Position;

                for (var i = 0; i < tableEntryCount; ++i)
                {
                    ushort keyCount;
                    while ((keyCount = reader.ReadUInt16()) != 0)
                    {
                        reader.BitConverter = EndianBitConverter.Big;

                        // ReSharper disable once UseObjectOrCollectionInitializer
                        var encoding = new Entry();
                        encoding.Filesize = reader.ReadUInt32();
                        encoding.Keys = new Binary[keyCount];

                        var hash = new Binary(reader.ReadBytes(checksumSize));

                        var keys = reader.ReadBytes(keyCount * checksumSize);
                        for (var j = 0; j < keyCount; ++j)
                        {
                            var keyStart = j * checksumSize;

                            encoding.Keys[j] = new Binary(new[] {
                                keys[keyStart],     keys[keyStart + 1],  keys[keyStart + 2],  keys[keyStart + 3],
                                keys[keyStart + 4], keys[keyStart + 5],  keys[keyStart + 6],  keys[keyStart + 7],
                                keys[keyStart + 8]
                            });
                        }

                        this[hash] = encoding;

                        reader.BitConverter = EndianBitConverter.Little;
                    }

                    const int CHUNK_SIZE = 4096;
                    reader.Seek(CHUNK_SIZE - (int)((reader.BaseStream.Position - chunkStart) % CHUNK_SIZE), SeekOrigin.Current);
                }
            }

            return true;
        }

        public class Entry
        {
            public uint Filesize { get; set; }
            public Binary[] Keys { get; set; }
        }
    }
}
