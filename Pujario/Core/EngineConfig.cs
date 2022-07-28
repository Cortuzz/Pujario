using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Core
{
    public struct EngineConfig
    {
        public int DefaultBufferSize;
        public int DefaultSpriteBatchSize;
        public SpriteSortMode DrawingSortMode;
        public int WorldChunkSize;
        public Point DefaultWorldSize;
        public float FloatTolerance;
    }
}