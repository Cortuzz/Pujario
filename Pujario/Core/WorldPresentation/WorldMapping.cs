using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;

namespace Pujario.Core.WorldPresentation
{
    public partial class WorldMapping : ITicking
    {
        private readonly SortingFilteredCollection<ActorWrap> _drawOrderedActors;

        private Point _zeroChunkPos;
        private int _updateOrder;
        private bool _enabled;

        private HashSet<Chunk> _prevDrawn;
        private HashSet<Chunk> _chunkBuffer;
        private HashSet<Chunk> _curDrawn;

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

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _drawOrder;
        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder == value) return;
                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

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
            ResizeGrid(worldSize, Rectangle.Empty, Point.Zero);

            _drawOrderedActors = new SortingFilteredCollection<ActorWrap>(
                ActorWrapDrawComparer.Instance,
                a => a.DrawFactor,
                (wrap, handler) => wrap.DrawFactorChanged += handler,
                (wrap, handler) => wrap.DrawFactorChanged -= handler,
                (wrap, handler) => wrap.Actor.DrawOrderChanged += handler,
                (wrap, handler) => wrap.Actor.DrawOrderChanged -= handler,
                Engine.Instance.Config.DefaultBufferSize);

            _prevDrawn = new HashSet<Chunk>(Engine.Instance.Config.DefaultBufferSize);
            _chunkBuffer = new HashSet<Chunk>(Engine.Instance.Config.DefaultBufferSize);
            _curDrawn = new HashSet<Chunk>(Engine.Instance.Config.DefaultBufferSize);

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

            for (; cur.X < newSize.X; ++cur.X, ++srcCur.X)
            {
                for (cur.Y = 0; cur.Y < newSize.Y; ++cur.Y, ++srcCur.Y)
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
            _chunkBuffer.Clear();
            using (var e = TickBeaconSystem.GetUpdateEnumerator(this))
                while (e.MoveNext())
                {
                    if (e.Current == null) continue;
                    e.Current.Update(gameTime);
                    _chunkBuffer.Add(e.Current);
                }

            foreach (var chunk in _curDrawn) chunk.Refresh(); 
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _chunkBuffer.Clear();
            _curDrawn.Clear();
            using (var e = TickBeaconSystem.GetDrawEnumerator(this))
                while (e.MoveNext() && e.Current != null)
                {
                    _curDrawn.Add(e.Current);
                    _chunkBuffer.Add(e.Current);
                }

            _chunkBuffer.ExceptWith(_prevDrawn);
            foreach (var chunk in _chunkBuffer) 
                chunk.Visible = true;

            _prevDrawn.ExceptWith(_curDrawn);
            foreach (var chunk in _prevDrawn) chunk.Visible = false;

            _drawOrderedActors.Mode = CollectionMode.Processed;
            foreach (var wrap in _drawOrderedActors) 
                wrap.Actor.Draw(gameTime, spriteBatch);

            foreach (var chunk in _curDrawn) chunk.HollowDraw(gameTime, spriteBatch);

            HashSet<Chunk> temp = _prevDrawn;
            _prevDrawn = _curDrawn;
            _prevDrawn = temp;
        }

        public override int GetHashCode() => HashCode.Combine(Grid.GetHashCode(), _zeroChunkPos.GetHashCode());

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}