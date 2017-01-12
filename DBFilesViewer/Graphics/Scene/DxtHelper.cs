﻿using System;
using System.IO;
using System.Threading;
using SharpDX.DXGI;

namespace DBFilesViewer.Graphics.Scene
{
    static class DxtHelper
    {
        public static byte[] Decompress(int width, int height, byte[] source, Format format)
        {
            using (var strm = new MemoryStream(source))
            {
                return Decompress(width, height, new BinaryReader(strm), format);
            }
        }

        public static byte[] Decompress(int width, int height, BinaryReader reader, Format format)
        {
            var retData = new byte[width * height * 4];
            switch (format)
            {
                case Format.BC1_UNorm:
                    LoadDxt1(reader, width, height, retData);
                    break;

                case Format.BC2_UNorm:
                    LoadDxt3(reader, width, height, retData);
                    break;

                case Format.BC3_UNorm:
                    LoadDxt5(reader, width, height, retData);
                    break;

                default:
                    throw new NotSupportedException("The given format is not supported for DXT decompression");
            }

            return retData;
        }

        private static void Rgb565ToRgb8Array(ushort input, byte[] output)
        {
            var r = (uint)(input & 0x1F);
            var g = (uint)((input >> 5) & 0x3F);
            var b = (uint)((input >> 11) & 0x1F);

            r = (r << 3) | (r >> 2);
            g = (g << 2) | (g >> 4);
            b = (b << 3) | (b >> 2);

            output[0] = (byte)r;
            output[1] = (byte)g;
            output[2] = (byte)b;
        }

        private static unsafe void LoadDxt1(BinaryReader strm, int w, int h, byte[] outData)
        {
            fixed (byte* ptr = outData)
            {
                var numBlocksFull = ((uint)Math.Ceiling(w / 4.0f) * (uint)Math.Ceiling(h / 4.0f));

                var blockData = new uint[numBlocksFull * 16];
                for (uint i = 0; i < numBlocksFull; ++i)
                    Dxt1GetBlock(strm, blockData, (int)i * 16);

                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
                    {
                        var bx = x / 4;
                        var by = y / 4;

                        var ibx = x % 4;
                        var iby = y % 4;

                        var blockIndex = by * (int)Math.Ceiling(w / 4.0f) + bx;
                        var innerIndex = iby * 4 + ibx;
                        *(uint*)(ptr + (y * w + x) * 4) = blockData[blockIndex * 16 + innerIndex];
                    }
                }
            }
        }

        private static unsafe void LoadDxt3(BinaryReader strm, int w, int h, byte[] outData)
        {
            fixed (byte* ptr = outData)
            {
                var numBlocksFull = ((uint)Math.Ceiling(w / 4.0f) * (uint)Math.Ceiling(h / 4.0f));

                var blockData = new uint[numBlocksFull * 16];
                for (uint i = 0; i < numBlocksFull; ++i)
                    Dxt3GetBlock(strm, blockData, (int)i * 16);

                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
                    {
                        var bx = x / 4;
                        var by = y / 4;

                        var ibx = x % 4;
                        var iby = y % 4;

                        var blockIndex = by * (int)Math.Ceiling(w / 4.0f) + bx;
                        var innerIndex = iby * 4 + ibx;
                        *(uint*)(ptr + (y * w + x) * 4) = blockData[blockIndex * 16 + innerIndex];
                    }
                }
            }
        }

        private static unsafe void LoadDxt5(BinaryReader strm, int w, int h, byte[] outData)
        {
            fixed (byte* ptr = outData)
            {
                var numBlocksFull = ((uint)Math.Ceiling(w / 4.0f) * (uint)Math.Ceiling(h / 4.0f));

                var blockData = new uint[numBlocksFull * 16];
                for (uint i = 0; i < numBlocksFull; ++i)
                    Dxt5GetBlock(strm, blockData, (int)i * 16);

                for (var y = 0; y < h; ++y)
                {
                    for (var x = 0; x < w; ++x)
                    {
                        var bx = x / 4;
                        var by = y / 4;

                        var ibx = x % 4;
                        var iby = y % 4;

                        var blockIndex = by * (int)Math.Ceiling(w / 4.0f) + bx;
                        var innerIndex = iby * 4 + ibx;
                        *(uint*)(ptr + (y * w + x) * 4) = blockData[blockIndex * 16 + innerIndex];
                    }
                }
            }
        }

        private static void Dxt1GetBlock(BinaryReader stream, uint[] colors, int baseIndex)
        {
            var color1 = stream.ReadUInt16();
            var color2 = stream.ReadUInt16();

            Rgb565ToRgb8Array(color1, RgbTmpArray.Value[0]);
            Rgb565ToRgb8Array(color2, RgbTmpArray.Value[1]);

            var clr1 = RgbTmpArray.Value[0];
            var clr2 = RgbTmpArray.Value[1];
            var clr3 = RgbTmpArray.Value[2];
            var clr4 = RgbTmpArray.Value[3];

            if (color1 > color2)
            {
                for (var i = 0; i < 3; ++i)
                {
                    clr4[i] = (byte)((clr1[i] + 2 * clr2[i]) / 3);
                    clr3[i] = (byte)((2 * clr1[i] + clr2[i]) / 3);
                }
            }
            else
            {
                for (var i = 0; i < 3; ++i)
                {
                    clr3[i] = (byte)((clr1[i] + clr2[i]) / 2);
                    clr4[i] = 0;
                }
            }

            var indices = stream.ReadUInt32();
            var tableIndices = TableIndices.Value;
            for (var i = 0; i < 16; ++i)
                tableIndices[i] = (ushort)((indices >> (2 * i)) & 3);

            var arrays = RgbTmpArray.Value;
            var colorVals = new[]
            {
                BitConverter.ToUInt32(arrays[0], 0),
                BitConverter.ToUInt32(arrays[1], 0),
                BitConverter.ToUInt32(arrays[2], 0),
                BitConverter.ToUInt32(arrays[3], 0)
            };

            for (var y = 0; y < 4; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    var index = y * 4 + x;
                    colors[baseIndex + index] = colorVals[tableIndices[index]];
                }
            }
        }

        private static void Dxt3GetBlock(BinaryReader stream, uint[] colors, int baseIndex)
        {
            var alphaValues = AlphaValues.Value;
            var alpha = stream.ReadUInt64();
            for (var i = 0; i < 16; ++i)
                alphaValues[i] = (byte)((((alpha >> (4 * i)) & 0x0F) / 15.0f) * 255.0f);

            var color1 = stream.ReadUInt16();
            var color2 = stream.ReadUInt16();

            Rgb565ToRgb8Array(color1, RgbTmpArray.Value[0]);
            Rgb565ToRgb8Array(color2, RgbTmpArray.Value[1]);

            var clr1 = RgbTmpArray.Value[0];
            var clr2 = RgbTmpArray.Value[1];
            var clr3 = RgbTmpArray.Value[2];
            var clr4 = RgbTmpArray.Value[3];

            if (color1 > color2)
            {
                for (var i = 0; i < 3; ++i)
                {
                    clr4[i] = (byte)((clr1[i] + 2 * clr2[i]) / 3);
                    clr3[i] = (byte)((2 * clr1[i] + clr2[i]) / 3);
                }
            }
            else
            {
                for (var i = 0; i < 3; ++i)
                {
                    clr3[i] = (byte)((clr1[i] + clr2[i]) / 2);
                    clr4[i] = 0;
                }
            }

            var indices = stream.ReadUInt32();
            var tableIndices = TableIndices.Value;
            for (var i = 0; i < 16; ++i)
                tableIndices[i] = (ushort)((indices >> (2 * i)) & 3);

            var arrays = RgbTmpArray.Value;
            var colorVals = new[]
            {
                BitConverter.ToUInt32(arrays[0], 0),
                BitConverter.ToUInt32(arrays[1], 0),
                BitConverter.ToUInt32(arrays[2], 0),
                BitConverter.ToUInt32(arrays[3], 0)
            };

            for (var y = 0; y < 4; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    var index = y * 4 + x;
                    colors[baseIndex + index] = colorVals[tableIndices[index]];
                    colors[baseIndex + index] &= 0x00FFFFFF;
                    colors[baseIndex + index] |= alphaValues[index] << 24;
                }
            }
        }

        private static void Dxt5GetBlock(BinaryReader stream, uint[] colors, int baseIndex)
        {
            var alphaValues = AlphaValues.Value;
            var alphaLookup = AlphaLookup.Value;

            uint alpha1 = stream.ReadByte();
            uint alpha2 = stream.ReadByte();

            alphaValues[0] = alpha1;
            alphaValues[1] = alpha2;

            if (alpha1 > alpha2)
                for (var i = 0; i < 6; ++i)
                    alphaValues[i + 2] = (byte)(((6.0f - i) * alpha1 + (1.0f + i) * alpha2) / 7.0f);
            else
            {
                for (var i = 0; i < 4; ++i)
                    alphaValues[i + 2] = (byte)(((4.0f - i) * alpha1 + (1.0f + i) * alpha2) / 5.0f);

                alphaValues[6] = 0;
                alphaValues[7] = 255;
            }

            var lu1 = stream.ReadUInt32();
            ulong lu2 = stream.ReadUInt16();
            ulong lookupValue = lu1;
            lookupValue |= lu2 << 32;

            for (var i = 0; i < 16; ++i)
                alphaLookup[i] = (uint)((lookupValue >> (i * 3)) & 7);

            var color1 = stream.ReadUInt16();
            var color2 = stream.ReadUInt16();

            Rgb565ToRgb8Array(color1, RgbTmpArray.Value[0]);
            Rgb565ToRgb8Array(color2, RgbTmpArray.Value[1]);

            var clr1 = RgbTmpArray.Value[0];
            var clr2 = RgbTmpArray.Value[1];
            var clr3 = RgbTmpArray.Value[2];
            var clr4 = RgbTmpArray.Value[3];

            if (color1 > color2)
            {
                for (var i = 0; i < 3; ++i)
                {
                    clr4[i] = (byte)((clr1[i] + 2 * clr2[i]) / 3);
                    clr3[i] = (byte)((2 * clr1[i] + clr2[i]) / 3);
                }
            }
            else
            {
                for (var i = 0; i < 3; ++i)
                {
                    clr3[i] = (byte)((clr1[i] + clr2[i]) / 2);
                    clr4[i] = 0;
                }
            }

            var indices = stream.ReadUInt32();
            var tableIndices = TableIndices.Value;
            for (var i = 0; i < 16; ++i)
                tableIndices[i] = (ushort)((indices >> (2 * i)) & 3);

            var arrays = RgbTmpArray.Value;
            var colorVals = new[]
            {
                BitConverter.ToUInt32(arrays[0], 0),
                BitConverter.ToUInt32(arrays[1], 0),
                BitConverter.ToUInt32(arrays[2], 0),
                BitConverter.ToUInt32(arrays[3], 0)
            };

            for (var y = 0; y < 4; ++y)
            {
                for (var x = 0; x < 4; ++x)
                {
                    var index = y * 4 + x;
                    colors[baseIndex + index] = colorVals[tableIndices[index]];
                    colors[baseIndex + index] &= 0x00FFFFFF;
                    colors[baseIndex + index] |= alphaValues[alphaLookup[index]] << 24;
                }
            }
        }

        private static readonly ThreadLocal<byte[][]> RgbTmpArray = new ThreadLocal<byte[][]>(() => new[]
        {
            new byte[] {0, 0, 0, 255},
            new byte[] {0, 0, 0, 255}, new byte[] {0, 0, 0, 255}, new byte[] {0, 0, 0, 255}
        });
        private static readonly ThreadLocal<ushort[]> TableIndices = new ThreadLocal<ushort[]>(() => new ushort[16]);
        private static readonly ThreadLocal<uint[]> AlphaValues = new ThreadLocal<uint[]>(() => new uint[16]);
        private static readonly ThreadLocal<uint[]> AlphaLookup = new ThreadLocal<uint[]>(() => new uint[16]);
    }
}
