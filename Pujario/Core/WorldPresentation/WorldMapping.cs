using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Core.WorldPresentation
{
    public partial class WorldMapping : ITicking
    {
        public partial class Chunk : IInstanceManager<IActor>, IDisposable
        {
        }

        private Dictionary<int, IActor> _chunkActors; // storage of Actors, which will be used by associated Chunks

        private Point _zeroChunkPos;
        private int _updateOrder;
        private bool _enabled;

        public readonly int ChunkSize;
        public List<ITickBeacon> TickBeacons;

        public bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Chunk[,] Grid { get; private set; }

        /// <summary>
        /// Chunk from grid position
        /// </summary>
        /// <param name="y"></param>, <param name="x"></param> could be negative, related to zero chunk
        public Chunk this[int y, int x] => Grid[_zeroChunkPos.Y + y, _zeroChunkPos.X + x];

        public Chunk this[in Point p] => Grid[_zeroChunkPos.Y + p.Y, _zeroChunkPos.X + p.X];

        /// <summary>
        /// Chunk from world location
        /// </summary>
        public Chunk this[in Vector2 location] => Grid[_zeroChunkPos.Y + (int)location.Y / ChunkSize,
            _zeroChunkPos.X + (int)location.X / ChunkSize];

        public WorldMapping(int chunkSize, Point worldSize, Point zeroChunkPos = default)
        {
            ChunkSize = chunkSize;
            _zeroChunkPos = zeroChunkPos;
            Grid = new Chunk[worldSize.Y, worldSize.X];
            _chunkActors = new Dictionary<int, IActor>(Engine.Instance.Config.DefaultBufferSize);
            TickBeacons = new List<ITickBeacon>(Engine.Instance.Config.DefaultBufferSize);
        }

        ~WorldMapping()
        {
            foreach (var chunk in Grid)
            {
                chunk.Dispose();
            }
        }

        public void ResizeGrid(in Point newSize, in Rectangle srcRect, in Point destPos)
        {
            var grid = new Chunk[newSize.Y, newSize.X];
            var cur = new Point();
            var srcCur = srcRect.Location - destPos;
            var destRect = new Rectangle(destPos, srcRect.Size);

            for (; cur.X < newSize.X; ++cur.X, ++cur.X, ++srcCur.X)
            {
                for (; cur.Y < newSize.Y; ++cur.Y, ++cur.Y, ++srcCur.Y)
                {
                    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable; stupid one .Contains can't mutate it  
                    if (destRect.Contains(cur) && srcRect.Contains(srcCur))
                        grid[cur.Y, cur.X] = Grid[srcCur.Y, srcCur.X];
                    else
                        grid[cur.Y, cur.X] = new Chunk(this, cur);
                }
            }

            _zeroChunkPos += destPos - srcRect.Location;
            Grid = grid;
        }

        public void Update(GameTime gameTime)
        {
            foreach (var beacon in TickBeacons)
            {
                if (!beacon.ForUpdating) continue;
                foreach (var chunk in beacon.Select(this))
                {
                    if (chunk.Enabled)
                        chunk.Update(gameTime);
                }
            }
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            foreach (var beacon in TickBeacons)
            {
                if (!beacon.ForDrawing) continue;
                foreach (var chunk in beacon.Select(this))
                {
                    chunk.Draw(gameTime, spriteBatch);
                }
            }
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}