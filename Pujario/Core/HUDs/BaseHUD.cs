using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Core.HUDs
{
    public abstract class BaseHUD : BaseObject, IHUD
    {
        protected Rectangle _viewportDest;
        public Rectangle ViewportDest
        {
            get => _viewportDest;
            private set
            {
                if (_viewportDest == value) return;
                _viewportDest = value;
                ViewportDestChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected bool _visible = true;
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

        protected int _drawOrder;
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

        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        public event EventHandler ViewportDestChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}
