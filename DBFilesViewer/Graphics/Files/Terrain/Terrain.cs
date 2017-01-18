using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBFilesViewer.Graphics.Files.Terrain
{
    public sealed class Terrain
    {
        private ADT[,] _terrainChunks = new ADT[64, 64];

        public Terrain(string directoryName)
        {
            for (var i =0;i < 64; ++i)
                for (var j = 0; j < 64; ++j)
                    _terrainChunks[i, j] = new ADT(directoryName, i, j);
        }
    }
}
