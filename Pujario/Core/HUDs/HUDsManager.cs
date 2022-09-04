using System;
using System.Collections;
using System.Collections.Generic;
using Pujario.Utils;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Core.HUDs
{
    public class HUDsManager : IInstanceManager<IHUD>
    {
        private readonly SortingFilteredCollection<IHUD> _huds;
        private Rectangle _viewportBounds;

        public HUDsManager()
        {
            _huds = new SortingFilteredCollection<IHUD>(
                DrawOrderComparer.Instance,
                hud => hud.Visible && (_viewportBounds.Contains(hud.ViewportDest) || _viewportBounds.Intersects(hud.ViewportDest)),
                (hud, handler) => hud.DrawOrderChanged += handler,
                (hud, handler) => hud.DrawOrderChanged -= handler,
                (hud, handler) => hud.VisibleChanged += handler,
                (hud, handler) => hud.VisibleChanged -= handler);

            _viewportBounds = Engine.Instance.TargetGame.GraphicsDevice.Viewport.Bounds;
            Engine.Instance.ViewportChanged += _onViewportChanged;
        }

        ~HUDsManager() => Engine.Instance.ViewportChanged -= _onViewportChanged;

        private void _onViewportChanged(Viewport viewport)
        {
            _viewportBounds = viewport.Bounds;
            _huds.RefreshAll();
        }

        public IHUD _findInstance(int hashCode)
        {
            using var e = _huds.GetEnumerator(CollectionMode.Raw);
            while (e.MoveNext())
            {
                if (e.Current.Id != hashCode) continue;
                return e.Current;
            }

            return null;
        }

        public WeakReference<IHUD> FindInstance(int hashCode) => new WeakReference<IHUD>(_findInstance(hashCode));

        public IEnumerator<IHUD> GetEnumerator() => _huds.GetEnumerator(CollectionMode.Raw);
        public IEnumerator<IHUD> GetDrawEnumerator() => _huds.GetEnumerator(CollectionMode.Processed);

        public void RegisterInstance(IHUD instance) => _huds.Add(instance);

        public void UnregisterInstance(IHUD instance)
        {
            if (instance == null || !_huds.Remove(instance))
                throw new Exceptions.Core.UnknownIdentifier();
        }

        public void UnregisterInstance(int hashCode) => UnregisterInstance(_findInstance(hashCode));

        IEnumerator IEnumerable.GetEnumerator() => ((IEnumerable)_huds).GetEnumerator();
    }
}
