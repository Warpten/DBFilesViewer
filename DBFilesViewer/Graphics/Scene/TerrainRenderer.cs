using System.Collections.Generic;
using DBFilesViewer.Graphics.Files.Terrain;

namespace DBFilesViewer.Graphics.Scene
{
    public sealed class TerrainRenderer
    {
        private ADT[,] TerrainChunks = new ADT[64, 64];
        public GxContext Context { get; }

        public TerrainRenderer(string directoryName, GxContext context)
        {
            Context = context;

            for (var i = 0; i < 64; ++i)
                for (var j =0; j < 64; ++j)
                    TerrainChunks[i, j] = new ADT(directoryName, i, j);
        }

        public void PrepareRender()
        {
        }

        public void Render()
        {
            
        }
    }
}
