using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Pujario.Core.WorldPresentation
{
    public partial class WorldMapping
    {
        private class ActorWrap : IDisposable, IBaseObject
        {
            private bool _disposed = true;
            public readonly IActor Actor;

            private void _onActorVisibleChanged(object sender, EventArgs args) => DrawFactorChanged?.Invoke(this, args);

            protected void Dispose(bool disposing)
            {
                if (_disposed) return;
                Actor.VisibleChanged -= _onActorVisibleChanged;
                if (disposing) GC.SuppressFinalize(this);
                _disposed = true;
            }

            public ActorWrap(IActor actor) => Actor = actor;

            public void InitEventListener()
            {
                if (!_disposed) return;
                Actor.VisibleChanged -= _onActorVisibleChanged;
                _disposed = false;
            }

            ~ActorWrap() => Dispose(false);
            public void Dispose() => Dispose(true);

            public bool DrawFactor => Actor.Visible && _isInVisibleChunk;

            private bool _isInVisibleChunk = false;
            public bool IsInVisibleChunk
            {
                get => _isInVisibleChunk;
                set
                {
                    if (_isInVisibleChunk == value) return;
                    _isInVisibleChunk = value;
                    DrawFactorChanged?.Invoke(this, EventArgs.Empty);
                }
            }

            public int Id => Actor.Id;

            public override int GetHashCode() => Id;

            public event EventHandler<EventArgs> DrawFactorChanged;
        }

        private class ActorWrapEqualityComparer : IEqualityComparer<ActorWrap>
        {
            public bool Equals([AllowNull] ActorWrap x, [AllowNull] ActorWrap y) => x.Actor.Id == y.Actor.Id;
            public int GetHashCode([DisallowNull] ActorWrap obj) => obj.Actor.Id;
        }

        private class ActorWrapDrawComparer : IComparer<ActorWrap>
        {
            public int Compare([AllowNull] ActorWrap x, [AllowNull] ActorWrap y) =>
                _intComparer.Compare(x?.Actor.DrawOrder ?? int.MinValue, y?.Actor.DrawOrder ?? int.MinValue);

            private Comparer<int> _intComparer = Comparer<int>.Default;
            protected ActorWrapDrawComparer() { }

            public static readonly ActorWrapDrawComparer Instance = new ActorWrapDrawComparer();
        }

        private class ActorWrapUpdateComparer : IComparer<ActorWrap>
        {
            public int Compare([AllowNull] ActorWrap x, [AllowNull] ActorWrap y) =>
                _intComparer.Compare(x?.Actor.UpdateOrder ?? int.MinValue, y?.Actor.UpdateOrder ?? int.MinValue);

            private Comparer<int> _intComparer = Comparer<int>.Default;
            protected ActorWrapUpdateComparer() { }

            public static readonly ActorWrapUpdateComparer Instance = new ActorWrapUpdateComparer();
        }

        private class Enumerator : IEnumerator<IActor>
        {
            private readonly IEnumerator<ActorWrap> _enumerator;

            IActor IEnumerator<IActor>.Current => _enumerator.Current?.Actor;

            object IEnumerator.Current => _enumerator.Current?.Actor;

            bool IEnumerator.MoveNext() => _enumerator.MoveNext();

            void IEnumerator.Reset() => _enumerator.Reset();

            protected virtual void Dispose(bool disposing) => _enumerator.Dispose();

            public Enumerator(IEnumerator<ActorWrap> enumerator) => _enumerator = enumerator;
            ~Enumerator() => Dispose(disposing: false);

            void IDisposable.Dispose()
            {
                Dispose(disposing: true);
                GC.SuppressFinalize(this);
            }
        }
    }
}
