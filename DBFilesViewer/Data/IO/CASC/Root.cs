using System.Collections.Generic;
using System.Data.Linq;
using System.IO;
using System.Linq;
using DBFilesViewer.Utils.Extensions;
using DBFilesViewer.Utils.Streams;

namespace DBFilesViewer.Data.IO.CASC
{
    public class Root : Dictionary<ulong, List<Root.Record>>
    {
        public bool FromStream(BLTE fileStream, IndexStore indexTable, Encoding encodingFile)
        {
            using (var fileReader = new EndianBinaryReader(EndianBitConverter.Little, fileStream))
            {
                try
                {
                    while (fileReader.BaseStream.Position != fileReader.BaseStream.Length)
                    {
                        var recordCount = fileReader.ReadInt32();
                        fileReader.BaseStream.Position += 8;

                        var fileDataIndex = 0u;

                        var fileDataIndices = fileReader.ReadArray<uint>(recordCount);
                        for (var i = 0; i < recordCount; ++i)
                        {
                            fileDataIndices[i] = fileDataIndex + fileDataIndices[i];
                            fileDataIndex = fileDataIndices[i] + 1;
                        }

                        var fastRecords = fileReader.ReadArray<FastRecord>(recordCount);
                        for (var i = 0; i < recordCount; ++i)
                        {
                            var fastRecord = fastRecords[i];
                            var record = new Record
                            {
                                MD5 = new Binary(new[] {
                                    fastRecord.V0,  fastRecord.V1,  fastRecord.V2,  fastRecord.V3,
                                    fastRecord.V4,  fastRecord.V5,  fastRecord.V6,  fastRecord.V7,
                                    fastRecord.V8,  fastRecord.V9,  fastRecord.V10, fastRecord.V11,
                                    fastRecord.V12, fastRecord.V13, fastRecord.V14, fastRecord.V15,
                                }),
                                FileDataID = fileDataIndices[i]
                            };
                            var hash = fastRecord.Hash;

                            // Ignore if encoding doesn't know about this file ...
                            if (!encodingFile.ContainsKey(record.MD5))
                                continue;

                            // ... or if index doesn't know about any of the keys in this encoding record.
                            if (!encodingFile[record.MD5].Keys.Any(indexTable.ContainsKey))
                                continue;

                            this[hash].Add(record);
                        }
                    }
                    return true;
                }
                catch (EndOfStreamException)
                {
                    return true;
                }
                catch
                {
                    return false;
                }
            }
        }

        public new List<Record> this[ulong hash]
        {
            get
            {
                List<Record> store;
                if (TryGetValue(hash, out store))
                    return store;

                return base[hash] = new List<Record>(20);
            }
        }

        private struct FastRecord
        {
            public byte V0, V1, V2, V3, V4, V5, V6, V7, V8, V9, V10, V11, V12, V13, V14, V15;
            public ulong Hash;
        }

        public class Record
        {
            public Binary MD5;
            public uint FileDataID;
        }
    }
}
