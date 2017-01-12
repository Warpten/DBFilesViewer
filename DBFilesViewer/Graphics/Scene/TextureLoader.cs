using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Utils;
using DBFilesViewer.Utils.Extensions;
using SharpDX.Direct3D11;

// Mindlessly stolen from Neo, with adjustments and additions.

namespace DBFilesViewer.Graphics.Scene
{
    public class TextureLoadInfo
    {
        public SharpDX.DXGI.Format Format;
        public ResourceUsage Usage = ResourceUsage.Default;
        public int Width;
        public int Height;
        public int BlockSize;
        public bool GenerateMipMaps;
        public Dictionary<int, byte[]> Layers = new Dictionary<int, byte[]>();
        public Dictionary<int, int> RowPitchs = new Dictionary<int, int>();
    }

    [StructLayout(LayoutKind.Sequential)]
    public unsafe struct BlpHeader
    {
        public int Magic;
        public int Version;
        public byte Compression;
        public byte AlphaDepth;
        public byte AlphaCompression;
        public byte MipLevels;
        public int Width;
        public int Height;
        public fixed int Offsets[16];
        public fixed int Sizes[16];
    }

    public static class TextureReader
    {
        private static readonly byte[] AlphaLookup1 = { 0x00, 0xFF };

        private static readonly byte[] AlphaLookup4 =
        {
            0x00, 0x11, 0x22, 0x33, 0x44, 0x55, 0x66, 0x77, 0x88, 0x99, 0xAA,
            0xBB, 0xCC, 0xDD, 0xEE, 0xFF
        };

        public static TextureLoadInfo LoadToArgbImage(string file)
        {
            var loadInfo = LoadFirstLayer(file);
            if (loadInfo == null)
                return null;

            if (loadInfo.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm)
                return loadInfo;

            loadInfo.Layers[0] = DxtHelper.Decompress(loadInfo.Width, loadInfo.Height, loadInfo.Layers[0], loadInfo.Format);
            loadInfo.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

            return loadInfo;
        }

        public static Bitmap GetBitmap(Stream file, int layer = 0)
        {
            var loadInfo = LoadLayer(file, layer);
            if (loadInfo == null)
                return null;

            var bitmap = new Bitmap(loadInfo.Width, loadInfo.Height);

            if (loadInfo.Format != SharpDX.DXGI.Format.R8G8B8A8_UNorm)
                loadInfo.Layers[0] = DxtHelper.Decompress(loadInfo.Width, loadInfo.Height, loadInfo.Layers[0], loadInfo.Format);
            
            var bitmapData = bitmap.LockBits(new Rectangle(0, 0, loadInfo.Width, loadInfo.Height), ImageLockMode.WriteOnly, PixelFormat.Format32bppArgb);
            for (var i = 0; i < loadInfo.Layers[0].Length; i += 4)
            {
                var tmp = loadInfo.Layers[0][i];
                loadInfo.Layers[0][i] = loadInfo.Layers[0][i + 2]; // Write blue into red
                loadInfo.Layers[0][i + 2] = tmp; // write stored red into blue
            }

            Marshal.Copy(loadInfo.Layers[0], 0, bitmapData.Scan0, loadInfo.Layers[0].Length);
            bitmap.UnlockBits(bitmapData);
            return bitmap;
        }

        public static TextureLoadInfo LoadToArgbImage(Stream file)
        {
            var loadInfo = LoadFirstLayer(file);
            if (loadInfo == null)
                return null;

            if (loadInfo.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm)
                return loadInfo;

            loadInfo.Layers[0] = DxtHelper.Decompress(loadInfo.Width, loadInfo.Height, loadInfo.Layers[0], loadInfo.Format);
            loadInfo.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

            return loadInfo;
        }

        public static TextureLoadInfo LoadToBestMatchingImage(string file, int targetWidth, int targetHeight)
        {
            using (var strm = Manager.OpenFile(file))
                return strm == null ? null : LoadToBestMatchingImage(strm, targetWidth, targetHeight);
        }

        public static unsafe TextureLoadInfo LoadToBestMatchingImage(Stream file, int targetWidth, int targetHeight)
        {
            if (file == null)
                return null;

            var reader = new BinaryReader(file);
            var header = reader.ReadStruct<BlpHeader>();

            var layer = 0;
            if (header.MipLevels != 0)
            {
                for (; layer < 16; ++layer)
                {
                    var w = Math.Max(header.Width >> layer, 1);
                    var h = Math.Max(header.Height >> layer, 1);

                    var lw = Math.Max(header.Width >> (layer + 1), 1);
                    var lh = Math.Max(header.Height >> (layer + 1), 1);

                    if (w == h && lw == lh && lw == w && w == 1)
                        break;

                    if (w >= targetWidth && lw <= targetWidth && h >= targetHeight && lh <= targetHeight)
                        break;
                }
            }

            if (layer != 0 && (header.Sizes[layer] == 0 || header.Offsets[layer] == 0))
            {
                for (; layer > 0; --layer)
                {
                    if (header.Sizes[layer] != 0 && header.Offsets[layer] != 0)
                        break;
                }
            }

            file.Position = 0;
            var loadInfo = LoadLayer(file, layer);
            if (loadInfo == null)
                return null;

            loadInfo.Width = Math.Max(loadInfo.Width >> layer, 1);
            loadInfo.Height = Math.Max(loadInfo.Height >> layer, 1);

            if (loadInfo.Format == SharpDX.DXGI.Format.R8G8B8A8_UNorm)
                return loadInfo;

            loadInfo.Layers[0] = DxtHelper.Decompress(loadInfo.Width, loadInfo.Height, loadInfo.Layers[0], loadInfo.Format);
            loadInfo.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

            return loadInfo;
        }

        public static TextureLoadInfo LoadHeaderOnly(string file)
        {
            if (string.IsNullOrEmpty(file))
                return null;

            using (var strm = Manager.OpenFile(file))
            {
                if (strm == null)
                    return null;

                var reader = new BinaryReader(strm);
                var header = reader.ReadStruct<BlpHeader>();
                var loadInfo = ParseHeader(ref header);
                return loadInfo;
            }
        }

        public static TextureLoadInfo LoadFirstLayer(string file)
        {
            if (string.IsNullOrEmpty(file))
                return null;

            using (var strm = Manager.OpenFile(file))
                return LoadFirstLayer(strm);
        }

        public static TextureLoadInfo LoadFirstLayer(Stream strm)
        {
            if (strm == null)
                return null;

            var reader = new BinaryReader(strm);
            var header = reader.ReadStruct<BlpHeader>();
            var loadInfo = ParseHeader(ref header);
            var palette = new uint[0];
            ParseLayer(0, ref palette, reader, header, loadInfo);

            return loadInfo;
        }

        public static TextureLoadInfo LoadLayer(string file, int layer)
        {
            if (string.IsNullOrEmpty(file))
                return null;

            using (var strm = Manager.OpenFile(file))
                return LoadLayer(strm, layer);
        }

        public static unsafe TextureLoadInfo LoadLayer(Stream strm, int layer)
        {
            if (strm == null || layer >= 16)
                return null;

            var reader = new BinaryReader(strm);
            var header = reader.ReadStruct<BlpHeader>();
            if (header.Sizes[layer] == 0 || header.Offsets[layer] == 0)
                return null;

            var loadInfo = ParseHeader(ref header);
            var palette = new uint[0];
            ParseLayer(layer, ref palette, reader, header, loadInfo);
            return loadInfo;
        }

        public static TextureLoadInfo Load(string file)
        {
            if (string.IsNullOrEmpty(file))
                return null;

            using (var strm = Manager.OpenFile(file))
            {
                if (strm == null)
                    return null;

                var reader = new BinaryReader(strm);
                var header = reader.ReadStruct<BlpHeader>();
                var loadInfo = ParseHeader(ref header);
                var palette = new uint[0];
                for (var i = 0; i < 16; ++i)
                    ParseLayer(i, ref palette, reader, header, loadInfo);

                return loadInfo;
            }
        }

        public static TextureLoadInfo Load(uint fileDataID)
        {
            using (var strm = Manager.OpenFile(fileDataID))
            {
                if (strm == null)
                    return null;

                var reader = new BinaryReader(strm);
                var header = reader.ReadStruct<BlpHeader>();
                var loadInfo = ParseHeader(ref header);
                var palette = new uint[0];
                for (var i = 0; i < 16; ++i)
                    ParseLayer(i, ref palette, reader, header, loadInfo);

                return loadInfo;
            }
        }

        private static unsafe void ParseLayer(int layer, ref uint[] palette, BinaryReader reader, BlpHeader header, TextureLoadInfo loadInfo)
        {
            if (header.Sizes[layer] == 0)
                return;

            var w = Math.Max(1, header.Width >> layer);

            reader.BaseStream.Position = header.Offsets[layer];
            loadInfo.Layers[layer] = reader.ReadBytes(header.Sizes[layer]);
            if (loadInfo.Format != SharpDX.DXGI.Format.R8G8B8A8_UNorm)
                loadInfo.RowPitchs[layer] = (w + 3) / 4 * loadInfo.BlockSize;
            else
            {
                ParseUncompressedLayer(layer, ref palette, reader, ref header, loadInfo);
                loadInfo.RowPitchs[layer] = w * 4;
            }
        }

        private static void ParseUncompressedLayer(int layer, ref uint[] palette, BinaryReader baseReader, ref BlpHeader header, TextureLoadInfo loadInfo)
        {
            if (header.Compression == 3)
                return;

            if (palette.Length == 0)
            {
                baseReader.BaseStream.Position = SizeCache<BlpHeader>.Size;
                palette = baseReader.ReadArray<uint>(256);
            }

            var w = Math.Max(header.Width >> layer, 1);
            var h = Math.Max(header.Height >> layer, 1);
            var indices = loadInfo.Layers[layer];
            var colors = new byte[w * h * 4];

            if (header.AlphaDepth == 8)
                DecompPaletteFastPath(ref palette, ref indices, ref colors);
            else
                DecompPaletteA8R8G8B8(header.AlphaDepth, ref palette, ref indices, ref colors);

            loadInfo.Layers[layer] = colors;
        }

        private static unsafe void DecompPaletteFastPath(ref uint[] palette, ref byte[] indices, ref byte[] colorBuffer)
        {
            var numEntries = colorBuffer.Length / 4;
            fixed (byte* outPtr = colorBuffer)
            {
                for (var i = 0; i < numEntries; ++i)
                {
                    var index = indices[i];
                    var alpha = indices[i + numEntries];
                    var color = palette[index];
                    color &= 0x00FFFFFF;
                    color |= ((uint)alpha) << 24;
                    *(uint*)(outPtr + i * 4) = color;
                }
            }
        }

        private static unsafe void DecompPaletteA8R8G8B8(int alphaSize, ref uint[] palette, ref byte[] indices, ref byte[] colorBuffer)
        {
            var numEntries = colorBuffer.Length / 4;
            fixed (byte* outPtr = colorBuffer)
            {
                for (var i = 0; i < numEntries; ++i)
                {
                    var index = indices[i];
                    var color = palette[index];
                    color &= 0x00FFFFFF;
                    color |= 0xFF000000;
                    *(uint*)(outPtr + i * 4) = color;
                }

                switch (alphaSize)
                {
                    case 0:
                        break;

                    case 1:
                        {
                            var colorIndex = 0;
                            for (var i = 0; i < (numEntries / 8); ++i)
                            {
                                var value = indices[i + numEntries];
                                for (var j = 0; j < 8; ++j, ++colorIndex)
                                {
                                    var color = (uint*)(outPtr + colorIndex * 4);
                                    *color &= 0x00FFFFFF;
                                    *color |= (uint)AlphaLookup1[((value & (1 << j)) != 0) ? 1 : 0] << 24;
                                }
                            }

                            if ((numEntries % 8) != 0)
                            {
                                var value = indices[numEntries + numEntries / 8];
                                for (var j = 0; j < (numEntries % 8); ++j)
                                {
                                    var color = (uint*)(outPtr + colorIndex * 4);
                                    *color &= 0x00FFFFFF;
                                    *color |= (uint)AlphaLookup1[((value & (1 << j)) != 0) ? 1 : 0] << 24;
                                }
                            }
                        }
                        break;

                    case 4:
                        {
                            var colorIndex = 0;
                            for (var i = 0; i < (numEntries / 2); ++i)
                            {
                                var value = indices[i + numEntries];
                                var alpha0 = AlphaLookup4[value & 0x0F];
                                var alpha1 = AlphaLookup4[value >> 4];
                                var color = (uint*)(outPtr + colorIndex++ * 4);
                                *color &= 0x00FFFFFF;
                                *color |= (uint)alpha0 << 24;
                                color = (uint*)(outPtr + colorIndex++ * 4);
                                *color &= 0x00FFFFFF;
                                *color |= (uint)alpha1 << 24;
                            }

                            if ((numEntries % 2) != 0)
                            {
                                var value = indices[numEntries + numEntries / 2];
                                var alpha0 = AlphaLookup4[value & 0x0F];
                                var color = (uint*)(outPtr + colorIndex * 4);
                                *color &= 0x00FFFFFF;
                                *color |= (uint)alpha0 << 24;
                            }
                        }
                        break;

                    case 8:
                        DecompPaletteFastPath(ref palette, ref indices, ref colorBuffer);
                        break;

                    default:
                        throw new InvalidOperationException("Invalid texture type: Wrong alpha size in paletted texture");
                }
            }
        }

        private static TextureLoadInfo ParseHeader(ref BlpHeader header)
        {
            var ret = new TextureLoadInfo
            {
                Height = header.Height,
                Width = header.Width
            };

            if (header.Compression == 2)
            {
                switch (header.AlphaCompression)
                {
                    case 0:
                        ret.Format = SharpDX.DXGI.Format.BC1_UNorm;
                        ret.BlockSize = 8;
                        break;

                    case 1:
                        ret.Format = SharpDX.DXGI.Format.BC2_UNorm;
                        ret.BlockSize = 16;
                        break;

                    case 7:
                        ret.Format = SharpDX.DXGI.Format.BC3_UNorm;
                        ret.BlockSize = 16;
                        break;
                }
            }
            else
                ret.Format = SharpDX.DXGI.Format.R8G8B8A8_UNorm;

            return ret;
        }
    }
}
