using System;
using System.Diagnostics;
using System.IO;
using System.IO.Compression;
using System.Linq;
using DBFilesViewer.Utils.Streams;

namespace DBFilesViewer.Data.IO.CASC
{
    public class BLTE : Stream
    {
        /// <summary>
        /// Stream for the actual BLTE file.
        /// </summary>
        private EndianBinaryReader _fileReader;
        
        /// <summary>
        /// Stream for data from the underlying file packed in a BLTE archive.
        /// </summary>
        private MemoryStream _dataStream = new MemoryStream();

        /// <summary>
        /// File chunks of the BLTE file.
        /// </summary>
        private Chunk[] Chunks { get; }

        /// <summary>
        /// Index of the next chunk to be processed.
        /// </summary>
        private int _currentChunk;

        /// <summary>
        /// Expected size of the BLTE archive.
        /// </summary>
        private int _compressedFileSize;

        /// <summary>
        /// Expected size of the file compressed in this BLTE archive.
        /// </summary>
        public override long Length => Chunks.Sum(chunk => chunk.Header.DecompressedSize);

        #region Stream override
        public override bool CanRead => _dataStream.CanRead;
        public override bool CanSeek => _dataStream.CanSeek;
        public override bool CanWrite => false;

        // /// <summary>
        // /// Length of the data that has been currently extracted from the BLTE archive.
        // /// </summary>
        // public override long Length => _dataStream.Length;

        /// <summary>
        /// Gets or set position in the compressed file.
        /// </summary>
        public override long Position
        {
            get { return _dataStream.Position; }
            set { Seek(value, SeekOrigin.Begin); }
        }

        public override void Flush()
        {
            throw new InvalidOperationException();
        }

        /// <summary>
        /// Seeks in the uncompressed file.
        /// </summary>
        public override long Seek(long offset, SeekOrigin origin)
        {
            // ReSharper disable once SwitchStatementMissingSomeCases
            switch (origin)
            {
                case SeekOrigin.Begin:
                    CheckReadNeeded(offset - _dataStream.Position);
                    break;
                case SeekOrigin.Current:
                    CheckReadNeeded(offset);
                    break;
                case SeekOrigin.End:
                    CheckReadNeeded(_dataStream.Length - offset);
                    break;
            }
            return _dataStream.Seek(offset, origin);
        }

        public override void SetLength(long value) => _dataStream.SetLength(value);

        public override void Write(byte[] buffer, int offset, int count)
        {
            throw new InvalidOperationException();
        }

        public override int Read(byte[] buffer, int offset, int count)
        {
            CheckReadNeeded(count);
            return _dataStream.Read(buffer, offset, count);
        }

        private void CheckReadNeeded(long count)
        {
            if (count <= 0)
                return;

            // Save current read position before we start writing to the stream again ...
            var readPosition = _dataStream.Position;

            // Make sure we are writing at the end of the data stream
            if (_dataStream.Length != 0)
                _dataStream.Position = _dataStream.Length;

            // Read chunks one by one until we are through reading, or when enough data has been decompressed.
            while (count > 0 && _dataStream.Length - _dataStream.Position < count)
            {
                var downloadedDataSize = ReadChunk((int)count);
                if (downloadedDataSize == 0)
                    break;

                count -= downloadedDataSize;
            }

            // .. And restore it
            _dataStream.Position = readPosition;
        }

        #endregion

        /// <summary>
        /// Creates a new instance of the BLTE class.
        /// 
        /// This class is a stream wrapper around another stream, allowing you to read
        /// the underlying data buffer of a file compressed in a BLTE archive in a lazy
        /// manner: it does not read all the file if it doesn't need to.
        /// </summary>
        /// <param name="fileStream"></param>
        /// <param name="offset">Offset at which the data begins. This is mostly used for CASC, where BLTE files typically follow each other.</param>
        /// <param name="length">Length of the BLTE entry. This is specific to CASC, and makes sure the stream never tries to read out of the actual archive.</param>
        public BLTE(Stream fileStream, int offset = 0, int length = 0)
        {
            // Don't wrap in an using statement (BLTE implements IDisposable)
            _fileReader = new EndianBinaryReader(EndianBitConverter.Little, fileStream);
            _fileReader.BaseStream.Position = offset;

            _compressedFileSize = length == 0 ? -1 : length;

            // Header
            var signature = _fileReader.ReadInt32();
            if (signature != 0x45544c42)
                throw new InvalidOperationException("File is not a valid BTLE archive!");

            _fileReader.BitConverter = EndianBitConverter.Big;
            var headerSize = _fileReader.ReadInt32();

            var chunkCount = 1;
            if (headerSize > 0)
            {
                var flagsCount = _fileReader.ReadBytes(4);
                chunkCount = (flagsCount[1] << 16) | (flagsCount[2] << 8) | flagsCount[3];
            }

            if (chunkCount == 0)
                throw new InvalidOperationException($"Incorrect number of chunks in BLTE file.");

            if (headerSize > 0)
            {
                Chunks = new Chunk[chunkCount];
                for (var i = 0; i < chunkCount; ++i)
                {
                    Chunks[i] = new Chunk();
                    Chunks[i].Header.CompressedSize = _fileReader.ReadInt32() - 1;
                    Chunks[i].Header.DecompressedSize = _fileReader.ReadInt32();
                    Chunks[i].Header.Checksum = _fileReader.ReadBytes(16);
                }
            }
            else
            {
                Chunks = new Chunk[1];
                Chunks[0] = new Chunk();
                Chunks[0].Header.CompressedSize = (int)(length == 0 ? fileStream.Length : length) - 8;
                Chunks[0].Header.DecompressedSize = (int)((length == 0 ? fileStream.Length : length) - 8 - 1);
            }

            _currentChunk = 0;

            // Assign capacity
            _dataStream.Capacity = (int)Length;
        }

        /// <summary>
        /// Reads a chunk from the network stream.
        /// In the case of a large non-zlibbed nor recursive nor encrypted chunk,
        /// and if <see cref="maxDataRead"/> is larger than 0, the code will only
        /// query what is needed from the network stream.
        /// </summary>
        /// <param name="maxDataRead">Max amount of data to read.</param>
        /// <returns>The amount of bytes read.</returns>
        private int ReadChunk(int maxDataRead = -1)
        {
            if (Chunks == null || _currentChunk >= Chunks.Length)
                return 0;

            Debug.Assert(Chunks[_currentChunk].Header.CompressedSize != 0,
                $"(Chunks[{_currentChunk}].Header.CompressedSize = {Chunks[_currentChunk].Header.CompressedSize}) == 0");

            if (Chunks[_currentChunk].EncodingMode == 0xFF)
                Chunks[_currentChunk].EncodingMode = _fileReader.ReadByte();
            switch (Chunks[_currentChunk].EncodingMode)
            {
                case (byte)'N':
                {
                    // Compute the amount of bytes read. If maxDataRead = -1, read the whole block.
                    // If trying to read more than chunk size, cap to it (obviously)
                    var readSize = maxDataRead;
                    if (readSize <= 0 || readSize > Chunks[_currentChunk].Header.CompressedSize)
                        readSize = Chunks[_currentChunk].Header.CompressedSize;

                    var blockData = _fileReader.ReadBytes(readSize);
                    _dataStream.Write(blockData, 0, blockData.Length);

                    // Update the size of remaining data in header.
                    Chunks[_currentChunk].Header.CompressedSize -= blockData.Length;

                    // Move on to next chunk if we're done with this block.
                    if (Chunks[_currentChunk].Header.CompressedSize == 0)
                        _currentChunk += 1;
                    return blockData.Length;
                }
                case (byte)'Z':
                {
                    // Save old read position.
                    var oldPosition = _dataStream.Position;

                    var blockData = _fileReader.ReadBytes(Chunks[_currentChunk].Header.CompressedSize);
                    using (var memoryStream = new MemoryStream(blockData, 2, blockData.Length - 2))
                    using (var deflateStream = new DeflateStream(memoryStream, CompressionMode.Decompress))
                        deflateStream.CopyTo(_dataStream);

                    // Advance to next chunk
                    Chunks[_currentChunk].Header.CompressedSize = 0;
                    _currentChunk += 1;

                    // Return the amount of bytes actually written to the inflated stream
                    return (int)(_dataStream.Position - oldPosition);
                }
                case (byte)'E':
                    throw new NotImplementedException("Salsa20, ARC4 or RC4 encryptions are not implemented!");
                case (byte)'F':
                    throw new NotImplementedException("Recursive BLTE parsing is not implemented!");
                default:
                    throw new InvalidOperationException($"Encryption type {(char)Chunks[_currentChunk].EncodingMode} is not implemented!");
            }

            // Dead code here.
        }

        private class ChunkInfoEntry
        {
            public int DecompressedSize { get; set; }
            public int CompressedSize { get; set; }
            public byte[] Checksum { get; set; }
        }

        private class Chunk
        {
            public ChunkInfoEntry Header { get; } = new ChunkInfoEntry();
            public byte EncodingMode { get; set; } = 0xFF;
        }
    }
}
