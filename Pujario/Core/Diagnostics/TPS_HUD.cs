using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Pujario.Core.HUDs;

namespace Pujario.Core.Diagnostics
{
    public class TPS_HUD : BaseHUD
    {
        private Func<float> _getDPS;
        private Func<float> _getUPS;
        private SpriteFont _font;
        private int _prevUPS, _prevDPS; 

        public TPS_HUD(Func<float> getDPS, Func<float> getUPS)
        {
            _getDPS = getDPS;
            _getUPS = getUPS;
            _font = Engine.Instance.TargetGame.Content.Load<SpriteFont>("arial");
            var size = _font.MeasureString("TPS(U/D): 322/228").ToPoint();
            _viewportDest = new Rectangle(10, 10, size.X, size.Y); 
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            spriteBatch.DrawString(_font, 
                "TPS(U/D): " + (_prevUPS = (_prevUPS + (int)_getUPS()) / 2).ToString() + "/" 
                    + (_prevDPS = (_prevDPS + (int)_getDPS()) / 2).ToString(), 
                _viewportDest.Location.ToVector2(), 
                Color.Green);
        }
    }
}
