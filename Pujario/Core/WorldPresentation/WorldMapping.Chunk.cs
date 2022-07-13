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
    // you can find some duplicated code in methods in this class, that was made for the little performance reasons
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

            private void _onInstanceTransformChanged(object sender, EventArgs args)
            {
#if DEBUG
                Debug.Assert((IActor)sender != null, "Invalid sender");
#endif
                _locationChanged.Add((IActor)sender);
            }

            private void _handleLocationChange()
            {
                foreach (var instance in _locationChanged)
                {
                    var loc = instance.Transform.Location;
                    var point = new Point(
                        (int)Math.Floor(loc.X / _world.ChunkSize),
                        (int)Math.Floor(loc.Y / _world.ChunkSize));

                    if (GridPos != point)
                    {
                        var otherChunk = _world[point];

                        _located.Remove(instance);
                        instance.TransformChanged -= _onInstanceTransformChanged;

                        otherChunk._needSorting = instance.UpdateOrder < otherChunk._located.Last().UpdateOrder;
                        otherChunk._located.Add(instance);
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

            public void PreUpdate()
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
                {
                    actor.Draw(gameTime, spriteBatch);
                }
            }

            public IEnumerator<IActor> GetEnumerator()
            {
                if (_enumerator != null)
                {
                    _enumerator.Reset();
                }
                else
                {
                    _enumerator = _located.GetEnumerator();
                }

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

                _needSorting = instance.UpdateOrder < _located.Last().UpdateOrder;
                _located.Add(instance);

                instance.TransformChanged += _onInstanceTransformChanged;
            }

            public void UnregisterInstance(IActor instance)
            {
#if DEBUG
                Debug.Assert(Area.Contains(instance.Transform.Location) && _located.Contains(instance),
                    "This instance isn't associated with Chunk");
#endif
                _world._chunkActors.Remove(instance.Id);
                _located.Remove(instance);
                instance.TransformChanged -= _onInstanceTransformChanged;
            }

            public void UnregisterInstance(int hashCode)
            {
                _world._chunkActors.Remove(hashCode, out var instance);
#if DEBUG
                Debug.Assert(instance != null && Area.Contains(instance.Transform.Location),
                    "This instance isn't associated with Chunk");
#endif
                _located.Remove(instance);
                instance.TransformChanged -= _onInstanceTransformChanged;
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