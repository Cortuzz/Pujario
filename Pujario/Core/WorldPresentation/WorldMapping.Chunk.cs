using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;
using System.Diagnostics;


namespace Pujario.Core.WorldPresentation
{
    public partial class WorldMapping
    {
        public class Chunk : BaseObject, IInstanceManager<IActor>, IDisposable
        {
            private readonly WorldMapping _world;
            private HashSet<ActorWrap> _locationChanged;
            private List<ActorWrap> _located;

            private bool _alreadyUpdated;
            private bool _needSorting;
            private bool _disposed;
            private readonly Vector2 _halfChunkSize;
#if DEBUG
            private readonly Texture2D _debugFrame;
            private readonly SpriteFont _font;
#endif

            private bool _enabled = true;
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    if (_enabled == value) return;
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            private int _updateOrder;
            public int UpdateOrder
            {
                get => _updateOrder;
                set
                {
                    if (_updateOrder == value) return;
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            private bool _visible = false;
            public bool Visible
            {
                get => _visible;
                set
                {
                    if (_visible == value) return;
                    _visible = value;
                    foreach (var item in _located) 
                        item.IsInVisibleChunk = value;
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

            public Point GridPos { get; }
            public Rectangle Area { get; }

            private void _addActor(ActorWrap wrap)
            {
                _located.Add(wrap);
                _needSorting = wrap.Actor.UpdateOrder < _located[^1].Actor.UpdateOrder;

                wrap.Actor.TransformChanged += _onInstanceTransformChanged;
                wrap.Actor.UpdateOrderChanged += _onUpdateOrderChanged;
            }

            private void _removeActor(ActorWrap wrap)
            {
                _located.Remove(wrap);

                wrap.Actor.TransformChanged -= _onInstanceTransformChanged;
                wrap.Actor.UpdateOrderChanged -= _onUpdateOrderChanged;
            }

            private void _onInstanceTransformChanged(object sender, EventArgs args)
            {
                var actor = sender as IActor;
                Debug.Assert(actor != null, "Invalid sender");
                Debug.Assert(_findLocatedActorWrapById(actor.Id, out var wrap), "This instance isn't associated with that Chunk");
                _locationChanged.Add(wrap);
            }

            private void _onUpdateOrderChanged(object sender, EventArgs args)
            {
                var actor = sender as IActor;
                Debug.Assert(actor != null, "Invalid sender");
                Debug.Assert(_findLocatedActorWrapById(actor.Id, out var wrap), "This instance isn't associated with that Chunk");
                var topBound = _located.Count;
                for (int i = 0; i < topBound; ++i)
                {
                    if (_located[i].Id != actor.Id) continue;
                    _needSorting = i != 0 && _located[i - 1].Actor.UpdateOrder > actor.UpdateOrder ||
                                   i != topBound && _located[i + 1].Actor.UpdateOrder < actor.UpdateOrder;
                    break;
                }
            }

            private void _handleLocationChange()
            {
                foreach (var wrap in _locationChanged)
                {
                    var anotherChunk = _world[wrap.Actor.Transform.Position];
                    if (GridPos != anotherChunk.GridPos)
                    {
                        _removeActor(wrap);
                        anotherChunk._addActor(wrap);
                    }
                }
            }

            public Chunk(WorldMapping world, Point gridPos)
            {
                _world = world;
                GridPos = gridPos;
                Area = new Rectangle(gridPos.X * _world.ChunkSize, gridPos.Y * _world.ChunkSize, _world.ChunkSize,
                    _world.ChunkSize);
                _halfChunkSize = new Vector2(_world.ChunkSize / 2, _world.ChunkSize / 2);

                _locationChanged = new HashSet<ActorWrap>(Engine.Instance.Config.DefaultBufferSize);
                _located = new List<ActorWrap>(Engine.Instance.Config.DefaultBufferSize);
#if DEBUG
                _font = Engine.Instance.TargetGame.Content.Load<SpriteFont>("arial");
                _debugFrame = Engine.Instance.TargetGame.Content.Load<Texture2D>("debugChunkFrameT");
#endif
            }

            ~Chunk() => Dispose();

            public void Refresh()
            {
                _alreadyUpdated = false;

                _handleLocationChange();

                _locationChanged.Clear();
                if (_needSorting) _located.Sort(ActorWrapUpdateComparer.Instance);
            }

            public void Update(GameTime gameTime)
            {
                if (_alreadyUpdated) return;
                foreach (var wrap in _located)
                {
                    if (wrap.Actor.Enabled) wrap.Actor.Update(gameTime);
                }

                _alreadyUpdated = true;
            }

            /// <summary>
            /// Draws only chunk's own stuff 
            /// </summary>
            public void HollowDraw(GameTime gameTime, SpriteBatch spriteBatch)
            {
#if DEBUG
                var fBounds = _debugFrame.Bounds;
                spriteBatch.Draw(
                    _debugFrame,
                    (GridPos - _world._zeroChunkPos).ToVector2() * _world.ChunkSize,
                    null,
                    Color.White,
                    0,
                    Vector2.Zero,
                    new Vector2(_world.ChunkSize / fBounds.Width, _world.ChunkSize / fBounds.Height),
                    SpriteEffects.None,
                    .0f);
                var posStr = GridPos.ToString();
                spriteBatch.DrawString(
                    _font,
                    posStr,
                    (GridPos - _world._zeroChunkPos).ToVector2() * _world.ChunkSize + _halfChunkSize - _font.MeasureString(posStr) / 2,
                    Color.Red,
                    0,
                    Vector2.Zero,
                    1,
                    SpriteEffects.None,
                    .0f);
#endif
            }

            /// <summary>
            /// Calls <see cref="HollowDraw(GameTime, SpriteBatch)"/> and draws all nested actors
            /// </summary>
            public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                HollowDraw(gameTime, spriteBatch);
                foreach (var actor in _located)
                    actor.Actor.Draw(gameTime, spriteBatch);
            }

            public IEnumerator<IActor> GetEnumerator() => new Enumerator(((IEnumerable<ActorWrap>)_located).GetEnumerator());
            IEnumerator IEnumerable.GetEnumerator() => new Enumerator(((IEnumerable<ActorWrap>)_located).GetEnumerator());

            public void RegisterInstance(IActor instance)
            {
                var wrap = new ActorWrap(instance);
                Debug.Assert(Area.Contains(instance.Transform.Position),
                    "This instance isn't associated with that Chunk");
                Debug.Assert(!_world._drawOrderedActors.Contains(wrap),
                    "This instance already exists in WorldMapping");

                wrap.InitEventListener();
                _world._drawOrderedActors.Add(wrap);
                _addActor(wrap);
            }

            private bool _findLocatedActorWrapById(int id, out ActorWrap wrap)
            {
                using var e = _world._drawOrderedActors.GetEnumerator(CollectionMode.Raw);
                while (e.MoveNext())
                {
                    if (e.Current.Id != id) continue;
                    wrap = e.Current;
                    return true;
                }

                wrap = null;
                return false;
            }

            private ActorWrap _findActorWrapById(int id)
            {
                using var e = _world._drawOrderedActors.GetEnumerator(CollectionMode.Raw);
                while (e.MoveNext())
                    if (e.Current.Id == id) return e.Current;

                return null;
            }

            public void UnregisterInstance(IActor instance) => UnregisterInstance(instance.Id);

            public void UnregisterInstance(int hashCode)
            {
                var wrap = _findActorWrapById(hashCode);
                if (wrap == null) return;

                Debug.Assert(Area.Contains(wrap.Actor.Transform.Position) && _located.Contains(wrap),
                    "This instance isn't associated with that Chunk");

                _world._drawOrderedActors.Remove(wrap);
                wrap.Dispose();
                _removeActor(wrap);
            }

            public WeakReference<IActor> FindInstance(int hashCode)
            {
                var wrap = _findActorWrapById(hashCode);
                return new WeakReference<IActor>(
                    wrap != null && Area.Contains(wrap.Actor.Transform.Position) ? wrap.Actor : null);
            }

            public void Dispose()
            {
                if (_disposed) return;

                _handleLocationChange();
                foreach (var instance in _located)
                {
                    var wrap = _findActorWrapById(instance.Id);
                    if (wrap != null)
                    {
                        _world._drawOrderedActors.Remove(wrap);
                        wrap.Dispose();
                    }
                    instance.Dispose();
                }

                _located = null;
                _locationChanged = null;
                _disposed = true;
                _enabled = false;

                GC.SuppressFinalize(this);
            }

            public event EventHandler<EventArgs> EnabledChanged;
            public event EventHandler<EventArgs> UpdateOrderChanged;
            public event EventHandler<EventArgs> DrawOrderChanged;
            public event EventHandler<EventArgs> VisibleChanged;
        }
    }
}