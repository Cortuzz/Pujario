using System.Collections;
using System.Collections.Generic;

namespace Pujario.Core.WorldPresentation
{
    /// <summary>
    /// Simple cascade of tick beacons; does not use ordered sequences
    /// </summary>
    public class SimpleTickBeaconSystem : ITickBeaconSystem
    {
        public class Enumerator : IEnumerator<IActor>
        {
            private readonly WorldMapping _world;
            private readonly List<ITickBeacon> _beacons;
            private readonly HashSet<int> _usedChunkIds;
            private List<ITickBeacon>.Enumerator _beaconEnumerator;
            private IEnumerator<WorldMapping.Chunk> _chunkEnumerator;
            private IEnumerator<IActor> _actorEnumerator;

            public bool IsDisposed { get; private set; }
            public TickBeaconMode Mode { get; set; }

            private bool _moveNextChunk()
            { 
                while (true)
                {
                    var isChunkEnumeratorValid = false;
                    if (_chunkEnumerator != null)
                        while ((isChunkEnumeratorValid = _chunkEnumerator.MoveNext()) &&
                               (_chunkEnumerator.Current == null || !_usedChunkIds.Add(_chunkEnumerator.Current.Id)))
                        {
                        }

                    if (isChunkEnumeratorValid) return true;
                    _chunkEnumerator?.Dispose();
                    do
                        if (!_beaconEnumerator.MoveNext())
                            return false;
                    while (((byte)_beaconEnumerator.Current!.Mode & (byte)Mode) == 0);
                    _chunkEnumerator = _beaconEnumerator.Current.GetSelector(_world);
                }
            }
            
            public Enumerator(WorldMapping world, List<ITickBeacon> beacons)
            {
                _world = world;
                _beacons = beacons;
                _usedChunkIds = new HashSet<int>();
                _beaconEnumerator = beacons.GetEnumerator();
                _actorEnumerator = _moveNextChunk() ? _chunkEnumerator.Current!.GetEnumerator() : null;
                IsDisposed = false;
            }

            ~Enumerator() => Dispose();

            public bool MoveNext()
            {
                if (_actorEnumerator == null) return false;
                while (true)
                {
                    if (_actorEnumerator.MoveNext()) return true;
                    _actorEnumerator.Dispose();
                    if (!_moveNextChunk()) return false;
                    _actorEnumerator = _chunkEnumerator.Current!.GetEnumerator();
                }
            }

            public void Reset()
            {
                Dispose();
                _beaconEnumerator = _beacons.GetEnumerator();
                _actorEnumerator = _moveNextChunk() ? _chunkEnumerator.Current!.GetEnumerator() : null;
                IsDisposed = false;
            }

            public IActor Current => _actorEnumerator?.Current;
            object IEnumerator.Current => Current;

            public void Dispose()
            {
                if (IsDisposed) return;
                _usedChunkIds.Clear();
                _beaconEnumerator.Dispose();
                _chunkEnumerator?.Dispose();
                _actorEnumerator?.Dispose();
                IsDisposed = true;
            }
        }

        private readonly List<ITickBeacon> _beacons = new List<ITickBeacon>();
        private readonly Enumerator _enumerator = null;

        public void UseBeacon(ITickBeacon beacon) => _beacons.Add(beacon);

        public void DisuseBeacon(ITickBeacon beacon) => _beacons.Remove(beacon);

        private Enumerator _getEnumerator(WorldMapping world, TickBeaconMode mode)
        {
            if (!(_enumerator is { IsDisposed: true })) return new Enumerator(world, _beacons) { Mode = mode };
            _enumerator.Reset();
            _enumerator.Mode = mode;
            return _enumerator;
        }

        public IEnumerator<IActor> GetDrawEnumerator(WorldMapping world) => _getEnumerator(world, TickBeaconMode.Draw);
        public IEnumerator<IActor> GetUpdateEnumerator(WorldMapping world) => _getEnumerator(world, TickBeaconMode.Update);
    }
}