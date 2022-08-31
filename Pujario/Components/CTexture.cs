using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core;
using Pujario.Core.Components;

namespace Pujario.Components
{
    public class CTexture : TransformableBaseComponent, Pujario.Core.IDrawable
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

        public Texture2D Texture { get; set; }
        public Rectangle SourceRectangle { get; set; }
        public Color Color { get; set; }
        public SpriteEffects SpriteEffects { get; set; }


        public CTexture([NotNull] Texture2D texture)
        {
            Texture = texture;
            SourceRectangle = texture.Bounds;
        }

        public CTexture(string assetName) 
            : this(Engine.Instance.TargetGame.Content.Load<Texture2D>(assetName)) { }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var rnd = new Random();
            spriteBatch.Draw(
                Texture,
                _transform.Position,
                SourceRectangle,
                Color.White,
                _transform.Rotation,
                _transform.Origin,
                _transform.Scale,
                SpriteEffects,
                (float)rnd.NextDouble());
        }

        public override void Initialize()
        {
        }

        public override void Update(GameTime gameTime)
        {
        }

        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}
