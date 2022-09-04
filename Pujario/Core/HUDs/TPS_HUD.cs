using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Pujario.Core.HUDs
{
    public class TPS_HUD : IHUD
    {
        private Rectangle _viewportDest;
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

        private Func<float> _getDPS;
        private Func<float> _getUPS;
        private SpriteFont _font;

        public int Id { get; private set; }
        
        public TPS_HUD(Func<float> getDPS, Func<float> getUPS)
        {
            _getDPS = getDPS;
            _getUPS = getUPS;
            _font = Engine.Instance.TargetGame.Content.Load<SpriteFont>("arial");
            var size = _font.MeasureString("TPS(U/D): 322/228").ToPoint();
            _viewportDest = new Rectangle(10, 10, size.X, size.Y); 
            Id = Engine.Instance.NextId;
        }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, 
                "TPS(U/D): " + _getUPS().ToString() + "/" + _getDPS().ToString(), 
                _viewportDest.Location.ToVector2(), 
                Color.Green);
        }

        public event EventHandler ViewportDestChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}
