using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Core.WorldPresentation
{
    public partial class WorldMapping : ITicking
    {
        public partial class Chunk : BaseObject, IInstanceManager<IActor>, IDisposable
        {
        }

        private readonly Dictionary<int, IActor> _chunkActors;
        // storage of Actors, which will be used by associated Chunks
        // that collection seems to be useless and could be removed in future 

        private Point _zeroChunkPos;
        private int _updateOrder;
        private bool _enabled;

        public readonly int ChunkSize;
        public readonly ITickBeaconSystem TickBeaconSystem;

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

        public bool Visible { get; set; }

        public Chunk[,] Grid { get; private set; }

        /// <summary>
        /// Chunk from grid position related to zero chunk
        /// </summary>
        /// <param name="y"></param>, <param name="x"></param> can be negative
        public Chunk this[int y, int x] => Grid[_zeroChunkPos.Y + y, _zeroChunkPos.X + x];

        /// <summary>
        /// same to <see cref="WorldMapping.this[int, int]"/>
        /// </summary>
        public Chunk this[in Point p] => Grid[_zeroChunkPos.Y + p.Y, _zeroChunkPos.X + p.X];

        /// <summary>
        /// Chunk from world location
        /// </summary>
        public Chunk this[in Vector2 location] => Grid[_zeroChunkPos.Y + (int)Math.Floor(location.Y / ChunkSize),
            _zeroChunkPos.X + (int)Math.Floor(location.X / ChunkSize)];

        /// <param name="chunkSize">size of the chunk</param>
        /// <param name="worldSize">Initial world size</param>
        /// <param name="tickBeaconSystemFabricMethod">An delegate that constructs new <see cref="ITickBeaconSystem"/></param>
        /// <param name="zeroChunkPos">Coordinate reference point</param>
        public WorldMapping(int chunkSize, Point worldSize, Func<ITickBeaconSystem> tickBeaconSystemFabricMethod,
            Point zeroChunkPos = default)
        {
            ChunkSize = chunkSize;
            _zeroChunkPos = zeroChunkPos;
            // Grid = new Chunk[worldSize.Y, worldSize.X];
            ResizeGrid(worldSize, Rectangle.Empty, Point.Zero);

            _chunkActors = new Dictionary<int, IActor>(Engine.Instance.Config.DefaultBufferSize);
            TickBeaconSystem = tickBeaconSystemFabricMethod();
        }

        ~WorldMapping()
        {
            foreach (var chunk in Grid) chunk.Dispose();
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
                    // ReSharper disable once PossiblyImpureMethodCallOnReadonlyVariable; stupid one .Contains() can't mutate it  
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
            using var e = TickBeaconSystem.GetUpdateEnumerator(this);
            while (e.MoveNext())
                e.Current?.Update(gameTime);
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            using var e = TickBeaconSystem.GetDrawEnumerator(this);
            while (e.MoveNext())
                e.Current?.Draw(gameTime, spriteBatch);
        }

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
    }
}