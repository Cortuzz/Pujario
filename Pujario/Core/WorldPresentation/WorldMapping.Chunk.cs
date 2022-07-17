using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;

#if DEBUG
using System.Diagnostics;
#endif


namespace Pujario.Core.WorldPresentation
{
    // todo: needs debugging
    public partial class WorldMapping
    {
        public partial class Chunk
        {
            private WorldMapping _world;
            private HashSet<IActor> _locationChanged;
            private List<IActor> _located;
            private IEnumerator<IActor> _enumerator;

            private int _updateOrder;
            private bool _enabled = true;
            private bool _alreadyUpdated;
            private bool _needSorting;
            private bool _disposed;

            public virtual bool Enabled
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

            public virtual int UpdateOrder
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

            public Point GridPos { get; }
            public Rectangle Area { get; }

            private void _addActor(IActor actor)
            {
                _needSorting = actor.UpdateOrder < _located.Last().UpdateOrder;
                _located.Add(actor);

                actor.TransformChanged += _onInstanceTransformChanged;
                actor.UpdateOrderChanged += _onUpdateOrderChanged;
            }

            private void _removeActor(IActor actor)
            {
                _located.Remove(actor);

                actor.TransformChanged -= _onInstanceTransformChanged;
                actor.UpdateOrderChanged -= _onUpdateOrderChanged;
            }

            private void _onInstanceTransformChanged(object sender, EventArgs args)
            {
#if DEBUG
                Debug.Assert((IActor)sender != null && _located.Contains(sender), "Invalid sender");
#endif
                _locationChanged.Add((IActor)sender);
            }

            private void _onUpdateOrderChanged(object sender, EventArgs args)
            {
#if DEBUG
                Debug.Assert((IActor)sender != null && _located.Contains(sender), "Invalid sender");
#endif
                var aSender = (IActor)sender;
                var topBound = _located.Count;
                for (int i = 0; i < topBound; ++i)
                {
                    if (_located[i].Id != aSender.Id) continue;
                    _needSorting = i != 0 && _located[i - 1].UpdateOrder > aSender.UpdateOrder ||
                                   i != topBound && _located[i + 1].UpdateOrder < aSender.UpdateOrder;
                    break;
                }
            }

            private void _handleLocationChange()
            {
                foreach (var instance in _locationChanged)
                {
                    var anotherChunk = _world[instance.Transform.Location];
                    if (GridPos != anotherChunk.GridPos)
                    {
                        _removeActor(instance);
                        anotherChunk._addActor(instance);
                    }
                }
            }

            public Chunk(WorldMapping world, Point gridPos)
            {
                _world = world;
                GridPos = gridPos;
                Area = new Rectangle(gridPos.X * _world.ChunkSize, gridPos.Y * _world.ChunkSize, _world.ChunkSize,
                    _world.ChunkSize);

                _locationChanged = new HashSet<IActor>(Engine.Instance.Config.DefaultBufferSize);
                _located = new List<IActor>(Engine.Instance.Config.DefaultBufferSize);
            }

            ~Chunk() => Dispose();

            public void Refresh()
            {
                _alreadyUpdated = false;

                _handleLocationChange();

                _locationChanged.Clear();
                if (_needSorting) _located.Sort(UpdateOrderComparer.Instance);
            }

            public void Update(GameTime gameTime)
            {
                if (_alreadyUpdated) return;
                foreach (var actor in _located)
                {
                    if (actor.Enabled)
                    {
                        actor.Update(gameTime);
                    }
                }

                _alreadyUpdated = true;
            }

            public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
            {
                foreach (var actor in _located)
                    actor.Draw(gameTime, spriteBatch);
            }

            public IEnumerator<IActor> GetEnumerator()
            {
                if (_enumerator != null)
                    _enumerator.Reset();
                else
                    _enumerator = _located.GetEnumerator();

                return _enumerator;
            }

            IEnumerator IEnumerable.GetEnumerator() => GetEnumerator();

            public void RegisterInstance(IActor instance)
            {
#if DEBUG
                Debug.Assert(Area.Contains(instance.Transform.Location),
                    "This instance isn't associated with Chunk");
                Debug.Assert(!_world._chunkActors.ContainsKey(instance.Id),
                    "This instance already exists in WorldMapping");
#endif
                _world._chunkActors.Add(instance.Id, instance);
                _addActor(instance);
            }

            public void UnregisterInstance(IActor instance)
            {
#if DEBUG
                Debug.Assert(Area.Contains(instance.Transform.Location) && _located.Contains(instance),
                    "This instance isn't associated with Chunk");
#endif
                _world._chunkActors.Remove(instance.Id);
                _removeActor(instance);
            }

            public void UnregisterInstance(int hashCode)
            {
                _world._chunkActors.Remove(hashCode, out var instance);
#if DEBUG
                Debug.Assert(instance != null && Area.Contains(instance.Transform.Location),
                    "This instance isn't associated with Chunk");
#endif
                _removeActor(instance);
            }

            public WeakReference<IActor> FindInstance(int hashCode)
            {
                _world._chunkActors.TryGetValue(hashCode, out var result);
                return new WeakReference<IActor>(
                    result != null && Area.Contains(result.Transform.Location) ? result : null);
            }

            public void Dispose()
            {
                if (_disposed) return;

                _handleLocationChange();
                foreach (var instance in _located)
                {
                    _world._chunkActors.Remove(instance.Id);
                    instance.Dispose();
                }

                _located = null;
                _locationChanged = null;
                _enumerator.Dispose();
                _enumerator = null;
                _disposed = true;
                _enabled = false;

                GC.SuppressFinalize(this);
            }

            public event EventHandler<EventArgs> EnabledChanged;
            public event EventHandler<EventArgs> UpdateOrderChanged;
        }
    }
}