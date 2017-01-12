using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

// Shamelessly stolen from http://github.com/miceiken/WoWMap, with additions and tweaks

namespace DBFilesViewer.Data.IO.Files.Models
{
    public class ChunkData<T> where T : BinaryReader
    {
        public ChunkData(T stream, uint chunkSize = 0)
        {
            FromStream(stream, chunkSize);
        }
        
        private void FromStream(T reader, uint chunkSize = 0)
        {
            Stream = reader;
            Chunks = new List<Chunk<T>>();

            var maxRead = (uint)reader.BaseStream.Position + chunkSize;
            if (chunkSize == 0)
                maxRead = (uint)reader.BaseStream.Length;
            maxRead = Math.Min(maxRead, (uint)reader.BaseStream.Length);

            var baseOffset = (uint)reader.BaseStream.Position;
            var calcOffset = 0u;
            while ((calcOffset + baseOffset) < maxRead && (calcOffset < maxRead))
            {
                var header = new ChunkHeader(reader);
                calcOffset += 8; // Add 8 bytes as we read header name + size
                Chunks.Add(new Chunk<T>(header.Name, header.Size, calcOffset + baseOffset, reader));
                calcOffset += header.Size; // Move past the chunk

                // We seek from our current position to save some time
                if ((calcOffset + baseOffset) < maxRead && calcOffset < maxRead)
                    reader.BaseStream.Seek(header.Size, SeekOrigin.Current);
            }
        }

        public T Stream { get; private set; }
        public List<Chunk<T>> Chunks { get; private set; }

        public Chunk<T> GetChunkByName(string name)
        {
            return Chunks.FirstOrDefault(c => c.Name == name);
        }
    }

    public class Chunk<T> where T : BinaryReader
    {
        public string Name { get; }
        public uint Size { get; }
        public uint Offset { get; }

        public Chunk(string name, uint size, uint offset, T stream)
        {
            Name = name;
            Size = size;
            Offset = offset;
            Reader = stream;
        }

        public Chunk(ChunkHeader header, T stream)
            : this(header.Name, header.Size, (uint)stream.BaseStream.Position, stream)
        { }

        public T Reader { get; }

        public override string ToString()
        {
            return Name;
        }
    }

    public class ChunkHeader
    {
        public uint Size { get; private set; }
        public string Name { get; private set; }

        public ChunkHeader(char[] header, uint size)
        {
            // Array.Reverse(header);
            Name = new string(header, 0, 4);
            Size = size;
        }

        public ChunkHeader(BinaryReader br)
        {
            Read(br);
        }

        public override string ToString() => Name;

        public void Read(BinaryReader br)
        {
            var header = br.ReadChars(4);
            // Array.Reverse(header);
            Name = new string(header, 0, 4);
            Size = br.ReadUInt32();
        }
    }

    public abstract class ChunkReader<T> where T : BinaryReader
    {
        public ChunkReader(Chunk<T> chunk, uint headerSize, bool read = true)
        {
            Chunk = chunk;
            HeaderSize = headerSize;

            if (read)
            {
                var oldPosition = Chunk.Reader.BaseStream.Position;
                Chunk.Reader.BaseStream.Position = Chunk.Offset;
                Read();
                Chunk.Reader.BaseStream.Position = oldPosition;
            }
        }

        public ChunkReader(Chunk<T> chunk, bool read = true)
            : this(chunk, chunk.Size, read)
        { }

        public Chunk<T> Chunk { get; }
        public uint HeaderSize { get; }

        public abstract void Read();
    }
}
