using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System;

namespace Pujario.Utils
{
    public static class SpriteBatchExtentions
    {
        private static Texture2D _pixel;

        public static void DrawLine(this SpriteBatch batch, in Vector2 from, in Vector2 to, float thikness, Color color)
        {
            if (_pixel == null)
            {
                _pixel = new Texture2D(batch.GraphicsDevice, 1, 1, false, SurfaceFormat.Color);
                _pixel.SetData(new[] { Color.White });
            }
            
            float angle = (float)Math.Atan2(to.Y - from.Y, to.X - from.X),
                lenght = (to - from).Length();
            batch.Draw(
                _pixel,
                from, null, 
                color, 
                angle,
                new Vector2(thikness / (lenght * 2), .5f),
                new Vector2(lenght, thikness), 
                SpriteEffects.None, 
                .0f);
        }
    }
}
