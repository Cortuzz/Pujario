using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Pujario.Core.Collisions;
using Pujario.Utils;

namespace Pujario.Core.Diagnostics
{
    public class PolygonShapeVisialiser : Core.IDrawable
    {
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

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;

        public Polygon Target { get; set; }

        float _coordsScaleFactor = 1;
        public float CoordsScaleFactor 
        { 
            get => _coordsScaleFactor;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _coordsScaleFactor = value;
            }
        }

        float _thickness = 1;
        public float Thickness
        {
            get => _thickness;
            set
            {
                if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
                _thickness = value;
            }
        }

        public Color Color { get; set; } = Color.White;

        public PolygonShapeVisialiser(Polygon polygon) => Target = polygon;

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            Vector2 prev, cur = Target[Target.VertexCount - 1] * CoordsScaleFactor;
            for (int i = 0; i < Target.VertexCount; ++i)
            {
                prev = cur;
                cur = Target[i] * CoordsScaleFactor;
                spriteBatch.DrawLine(prev, cur, Thickness, Color);
            }
        }
    }
}
