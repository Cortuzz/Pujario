using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core.WorldPresentation
{
    public class SquareBeacon : ITickBeacon
    {
        private int _version;
        private WorldMapping.Chunk[] _sequeance = null;

        public TickBeaconMode Mode { get; set; }

        public Vector2 Position { get; set; } = Vector2.Zero;
        public int R { get; set; }

        public IEnumerator<WorldMapping.Chunk> GetSelector(WorldMapping world)
        {
            var version = HashCode.Combine(R, Position.GetHashCode(), world.GetHashCode());
            if (_version != version || _sequeance == null)
            {
                _version = version;
                if (_sequeance == null) 
                    _sequeance = new WorldMapping.Chunk[(R * 2 + 1) * (R * 2 + 1)];
                int i = -1;
                var currentChunk = world[Position];
                for (int x = currentChunk.GridPos.X - R; x <= currentChunk.GridPos.X + R; x++)
                {
                    for (int y = currentChunk.GridPos.Y - R; y <= currentChunk.GridPos.Y + R; y++)
                    {
                        if (x < 0 || y < 0 || y >= world.Grid.GetLength(0) || x >= world.Grid.GetLength(1)) continue;
                        _sequeance[++i] = world.Grid[y, x];
                    }
                }
                for (; i + 1 < _sequeance.Length; ++i) _sequeance[i] = null;
            }

            return ((IEnumerable<WorldMapping.Chunk>)_sequeance).GetEnumerator();
        }
    }
}
