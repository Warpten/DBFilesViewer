using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using DBFilesViewer.Data.IO.CASC;
using DBFilesViewer.Data.IO.Files.Models;
using DBFilesViewer.Graphics.Files.Models.Animations;
using DBFilesViewer.Utils.Extensions;
using SharpDX;

namespace DBFilesViewer.Graphics.Files.Models
{
    public sealed class M2 : BinaryReader
    {
        public string Name => MD20.Name;

        public PFID PFID { get; }
        public MD20 MD20 { get; }
        public SFID SFID { get; }
        public AFID AFID { get; }

        #region Rendering data
        public ushort[] Indices { get; private set; } = new ushort[0];
        public ModelSubMeshInfo[] SubMeshes { get; private set; }

        public bool HasBlendPass { get; private set; }
        public bool HasOpaquePass { get; private set; }

        public List<ModelRenderPass> RenderPasses { get; } = new List<ModelRenderPass>();
        #endregion

        public M2(Stream fileStream) : base(fileStream)
        {
            // Try to read as an old non-chunked file.
            MD20 = new MD20(this);
            if (!MD20.Read())
                fileStream.Position = 0;

            var chunkedData = new ChunkData<M2>(this);

            foreach (var chunkInfo in chunkedData.Chunks)
            {
                switch (chunkInfo.Name)
                {
                    case "PFID":
                        PFID = new PFID(chunkInfo);
                        break;
                    case "MD21":
                        MD20 = new MD21(chunkInfo).MD20;
                        break;
                    case "AFID":
                        AFID = new AFID(chunkInfo);
                        break;
                    case "SFID":
                        SFID = new SFID(chunkInfo);
                        if (SFID.SkinFileDataID.Length != 0)
                            LoadSkin(SFID.SkinFileDataID[0]);
                        break;
                    default:
                        Console.WriteLine($"[DEBUG] Unknown chunk '{chunkInfo.Name}' found!");
                        break;
                }
            }
        }

        public string GetTextureName(int textureIndex)
        {
            return MD20.Textures.Length <= textureIndex ? null : MD20.Textures[textureIndex].Name;
        }

        private static readonly uint[] BlendModes = { 0, 1, 2, 10, 3, 4, 5 };
        private void LoadSkin(uint skinFileDataID)
        {
            using (var skinReader = new BinaryReader(Manager.OpenFile(skinFileDataID)))
            {
                var mSkin = skinReader.ReadStruct<M2Skin>();
                if (mSkin.Magic != 0x4e494b53)
                    return;

                var indexLookup = ReadArrayOf<ushort>(skinReader, mSkin.ofsIndices, mSkin.nIndices);
                var triangles = ReadArrayOf<ushort>(skinReader, mSkin.ofsTriangles, mSkin.nTriangles);

                var skinSubMeshes = ReadArrayOf<M2SubMesh>(skinReader, mSkin.ofsSubmeshes, mSkin.nSubmeshes);

                Indices = triangles.Select(t => indexLookup[t]).ToArray();
                SubMeshes = skinSubMeshes.Select(submesh => new ModelSubMeshInfo
                {
                    BoundingSphere =
                        new BoundingSphere(submesh.CenterBoundingBox, submesh.Radius),
                    NumIndices = submesh.NumTriangles,
                    StartIndex = submesh.StartTriangle + (((submesh.Level & 1) != 0) ? (ushort.MaxValue + 1) : 0)
                }).ToArray();

                foreach (var texUnit in ReadArrayOf<M2TexUnit>(skinReader, mSkin.ofsTexUnits, mSkin.nTexUnits))
                {
                    var mesh = skinSubMeshes[texUnit.SubmeshIndex];

                    // UV Animation
                    int uvIndex;
                    if (texUnit.TextureAnimIndex >= MD20.TextureTransformsLookupTable.Length || MD20.TextureTransformsLookupTable[texUnit.TextureAnimIndex] < 0)
                        uvIndex = -1;
                    else
                        uvIndex = MD20.TextureTransformsLookupTable[texUnit.TextureAnimIndex];

                    var startTriangle = (int)mesh.StartTriangle;
                    if ((mesh.Level & 1) != 0)
                        startTriangle += ushort.MaxValue + 1;

                    var texIndices = new List<int>(texUnit.OpCount);
                    switch (texUnit.OpCount)
                    {
                        case 2:
                            texIndices.Add(MD20.TextureLookupTable[texUnit.Texture]);
                            texIndices.Add(MD20.TextureLookupTable[texUnit.Texture + 1]);
                            break;
                        case 3:
                            texIndices.Add(MD20.TextureLookupTable[texUnit.Texture]);
                            texIndices.Add(MD20.TextureLookupTable[texUnit.Texture + 1]);
                            texIndices.Add(MD20.TextureLookupTable[texUnit.Texture + 2]);
                            break;
                        default:
                            texIndices.Add(MD20.TextureLookupTable[texUnit.Texture]);
                            break;
                    }

                    var flags = MD20.Materials[texUnit.RenderFlags].RenderFlags;
                    var blendMode = (uint)MD20.Materials[texUnit.RenderFlags].BlendingMode;

                    if (MD20.BlendMapOverrides != null && texUnit.ShaderId < MD20.BlendMapOverrides.Length)
                        blendMode = (uint)MD20.BlendMapOverrides[texUnit.ShaderId];

                    blendMode %= (uint)BlendModes.Length;

                    if (blendMode != 0 && blendMode != 1)
                        HasBlendPass = true;
                    else
                        HasOpaquePass = true;

                    RenderPasses.Add(new ModelRenderPass
                    {
                        MeshID = mesh.MeshPartId,
                        TextureIndices = texIndices,
                        IndexCount = mesh.NumTriangles,
                        RenderFlag = flags,
                        BlendMode = BlendModes[blendMode],
                        StartIndex = startTriangle,
                        OpCount = texUnit.OpCount,
                        VertexShaderType = ModelShaders.GetVertexShaderType(texUnit.ShaderId, texUnit.OpCount),
                        PixelShaderType = ModelShaders.GetPixelShaderType(texUnit.ShaderId, texUnit.OpCount),

                        TexAnimIndex = uvIndex,
                        AlphaAnimIndex = texUnit.TransparencyIndex,
                        ColorAnimIndex = texUnit.ColorIndex
                    });
                }
            }

            Sort(RenderPasses, (e1, e2) =>
            {
                //! Blend modes to shader blend modes:
                //! {      0,        1,     2,         10,   3,   4,     5 }
                //! { Opaque, AlphaKey, Alpha, NoAlphaAdd, Add, Mod, Mod2x }

                if (e1.BlendMode == 0 && e2.BlendMode != 0)
                    return -1;

                if (e1.BlendMode != 0 && e2.BlendMode == 0)
                    return 1;

                if (e1.BlendMode == e2.BlendMode && e1.BlendMode == 0)
                    return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);

                if (e1.BlendMode == 2 && e2.BlendMode != 2)
                    return -1;

                if (e2.BlendMode == 2 && e1.BlendMode != 2)
                    return 1;

                var is1Additive = e1.BlendMode == 1 || e1.BlendMode == 6 || e1.BlendMode == 3;
                var is2Additive = e2.BlendMode == 1 || e2.BlendMode == 6 || e2.BlendMode == 3;

                if (is1Additive && !is2Additive)
                    return -1;

                if (is2Additive && !is1Additive)
                    return 1;

                return e1.TexUnitNumber.CompareTo(e2.TexUnitNumber);
            });
        }

        private static void Sort<T>(List<T> container, Comparison<T> comparer) => container.Sort(comparer);

        private static T[] ReadArrayOf<T>(BinaryReader reader, int offset, int count) where T : struct
        {
            if (count == 0)
                return new T[0];

            reader.BaseStream.Position = offset;
            return reader.ReadArray<T>(count);
        }
    }
    
    /// <summary>
    /// A simple chunk containing the FileDataID of the physics file associated to the current model.
    /// </summary>
    public class PFID : ChunkReader<M2>
    {
        public PFID(Chunk<M2> chunk, bool read = true) : base(chunk, read) { }

        public uint PhysFileDataID { get; private set; }

        public override void Read()
        {
            PhysFileDataID = Chunk.Reader.ReadUInt32();
        }
    }

    public class SFID : ChunkReader<M2>
    {
        public uint[] SkinFileDataID { get; private set; }

        public override void Read()
        {
            SkinFileDataID = Chunk.Reader.ReadArray<uint>(Chunk.Reader.MD20.NumSkinProfiles);
            // LOD file data id - ignore
        }

        public SFID(Chunk<M2> chunk, bool read = true) : base(chunk, read) { }
    }

    public class AFID : ChunkReader<M2>
    {
        public AFID(Chunk<M2> chunk, bool read = true) : base(chunk, read) { }

        public ushort AnimID { get; private set; }
        public ushort SubAnimID { get; private set; }
        public uint FileDataID { get; private set; }

        public override void Read()
        {
            AnimID = Chunk.Reader.ReadUInt16();
            SubAnimID = Chunk.Reader.ReadUInt16();
            FileDataID = Chunk.Reader.ReadUInt32();
        }
    }

    public class MD21 : ChunkReader<M2>
    {
        public MD21(Chunk<M2> chunk, uint headerSize, bool read = true) : base(chunk, headerSize, read) { }
        public MD21(Chunk<M2> chunk, bool read = true) : base(chunk, read) { }

        public MD20 MD20 { get; private set; }

        public override void Read()
        {
            MD20 = new MD20(Chunk.Reader)
            {
                StartOffset = (int) Chunk.Offset
            };
            MD20.Read();
        }
    }

    public class MD20
    {
        private M2 Reader;
        public int StartOffset { get; set; } = 0;

        public MD20(M2 fileStream)
        {
            Reader = fileStream;
        }

        public string Name { get; private set; }

        public short[] BlendMapOverrides { get; private set; }

        public bool Read()
        {
            var signature = new string(Reader.ReadChars(4));
            if (signature != "MD20")
                return false;

            var header = Reader.ReadStruct<M2Header>();
            if ((header.GlobalFlags & 0x08) != 0)
            {
                var nBlendMaps = Reader.ReadInt32();
                var ofsBlendMaps = Reader.ReadInt32();
                Reader.BaseStream.Position = ofsBlendMaps;
                BlendMapOverrides = Reader.ReadArray<short>(nBlendMaps);
            }

            if (header.Name.Count > 1)
            {
                Reader.BaseStream.Position = header.Name.Offset + StartOffset;
                Name = System.Text.Encoding.ASCII.GetString(Reader.ReadBytes(header.Name.Count - 1));
            }

            GlobalLoops = ReadArrayOf(Reader, ref header.GlobalLoops);
            Sequences = ReadArrayOf(Reader, ref header.Sequences).Select(seq => new AnimationSequence
            {
                AnimationID = seq.AnimationID,
                BlendTime = seq.BlendTime,
                BoundingRadius = seq.BoundingRadius,
                Bounds = seq.Bounds,
                Duration = seq.Duration,
                Flags = seq.Flags,
                MaximumRepetitions = seq.MaximumRepetitions,
                MinimumRepetitions = seq.MinimumRepetitions,
                NextAlias = seq.NextAlias,
                NextAnimation = seq.NextAnimation,
                SubAnimID = seq.SubAnimID,
                Probability = (float)seq.Probability / 0x7FFF,
                Speed = seq.Speed
            }).ToArray();
            SequenceLookups = ReadArrayOf(Reader, ref header.SequenceLookup);

            var boneIndex = 0;
            Bones = ReadArrayOf(Reader, ref header.Bones).Select(b => new AnimatedBone(Reader, this, ref b, boneIndex++)).ToArray();
            KeyBoneLookup = ReadArrayOf(Reader, ref header.KeyBoneLookup);

            Vertices = ReadArrayOf(Reader, ref header.Vertices);
            NumSkinProfiles = header.NumSkinProfiles;
            Textures = ReadArrayOf(Reader, ref header.Textures).Select(tex =>
            {
                string name = null;
                if (tex.Name.Count > 1)
                {
                    Reader.BaseStream.Position = tex.Name.Offset + StartOffset;
                    name = System.Text.Encoding.ASCII.GetString(Reader.ReadArray<byte>(tex.Name.Count - 1));
                }

                return new ModelTexture
                {
                    Name = name,
                    Type = tex.Type,
                    Flags = tex.Flags
                };
            }).ToArray();

            TextureTransforms = ReadArrayOf(Reader, ref header.TextureTransforms).Select(tt => new AnimatedTexture(Reader, this, ref tt)).ToArray();

            ReplaceableTextureLookup = ReadArrayOf(Reader, ref header.ReplaceableTextureLookup);

            Materials = ReadArrayOf(Reader, ref header.Materials);

            Colors = ReadArrayOf(Reader, ref header.Colors).Select(c => new AnimatedColor(Reader, this, ref c)).ToArray();

            Transparencies = ReadArrayOf(Reader, ref header.TextureWeights).Select(tw => new AnimatedTransparency(Reader, this, ref tw)).ToArray();

            BoneLookupTable = ReadArrayOf(Reader, ref header.BoneLookupTable);
            TextureLookupTable = ReadArrayOf(Reader, ref header.TextureLookupTable);
            // TexUnitLookupTable = ReadArrayOf(Reader, ref header.TexUnitLookupTable);
            TransparencyLookupTable = ReadArrayOf(Reader, ref header.TransparencyLookupTable);
            TextureTransformsLookupTable = ReadArrayOf(Reader, ref header.TextureTransformsLookupTable);
            BoundingBox = header.BoundingBox;
            BoundingSphereRadius = header.BoundingSphereRadius;
            CollisionBox = header.CollisionBox;
            CollisionSphereRadius = header.CollisionSphereRadius;
            CollisionTriangles = ReadArrayOf(Reader, ref header.CollisionTriangles);
            CollisionVertices = ReadArrayOf(Reader, ref header.CollisionVertices);
            CollisionNormals = ReadArrayOf(Reader, ref header.CollisionNormals);
            Attachments = ReadArrayOf(Reader, ref header.Attachments);
            AttachmentLookupTable = ReadArrayOf(Reader, ref header.AttachmentLookupTable);
            // Events = ...
            // Lights = ...
            // Cameras = ...
            // CameraLookupTable = ...
            // RibbonEmitters = ...
            // ParticleEmitters = ...
            return true;
        }

        private T[] ReadArrayOf<T>(BinaryReader reader, ref M2Array<T> arr) where T : struct
        {
            if (arr.Count == 0)
                return new T[0];

            reader.BaseStream.Position = arr.Offset + StartOffset;
            return reader.ReadArray<T>(arr.Count);
        }

        #region Types fully loaded from MDX (everything is either POD or correctly loaded).
        public uint[] GlobalLoops { get; private set; }
        public AnimationSequence[] Sequences { get; private set; } // From M2Array<M2Sequence>
        public short[] SequenceLookups { get; private set; }
        public AnimatedBone[] Bones { get; private set; }
        public ushort[] KeyBoneLookup { get; private set; }
        public M2Vertex[] Vertices { get; private set; }
        public int NumSkinProfiles { get; private set; }
        public AnimatedColor[] Colors { get; private set; } // From M2Array<M2Color>
        public ModelTexture[] Textures { get; private set; } // From M2Array<M2Texture>
        public AnimatedTransparency[] Transparencies { get;private set; } // From M2Array<M2TextureWeight>
        public AnimatedTexture[] TextureTransforms { get; private set; }
        public ushort[] ReplaceableTextureLookup { get; private set; }
        public M2Material[] Materials { get; private set; }
        public ushort[] BoneLookupTable { get; set; }
        public ushort[] TextureLookupTable { get; set; }
        // public ushort[] TexUnitLookupTable { get; set; }
        public ushort[] TransparencyLookupTable { get; set; }
        public short[] TextureTransformsLookupTable { get; set; }
        public BoundingBox BoundingBox { get; private set; }
        public float BoundingSphereRadius { get; private set; }
        public BoundingBox CollisionBox { get; private set; }
        public float CollisionSphereRadius { get; private set; }
        public ushort[] CollisionTriangles { get; private set; }
        public Vector3[] CollisionVertices { get; private set; }
        public Vector3[] CollisionNormals { get; private set; }
        public M2Attachment[] Attachments { get; private set; } // From M2Array<M2Attachment>
        public short[] AttachmentLookupTable { get; private set; }
        #endregion

        #region Types partially loaded from MDX - Needs to be completed to be used
        
        #endregion
    }
}
