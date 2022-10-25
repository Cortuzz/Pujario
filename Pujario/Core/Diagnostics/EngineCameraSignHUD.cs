using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.HUDs;

namespace Pujario.Core.Diagnostics
{
    public class EngineCameraSignHUD : BaseHUD
    {
        Texture2D _cameraSign;
        public Texture2D CameraSign
        {
            get
            {
                if (_cameraSign == null)
                    _cameraSign = Engine.Instance.TargetGame.Content.Load<Texture2D>("debugCameraSign");
                return _cameraSign;
            }
            set
            {
                if (value == null) return;
                _cameraSign = value;
            }
        }

        public override void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            var position = Engine.Instance.Camera.ToViewportPosition(Engine.Instance.Camera.Transform.Position);
            position.X -= _cameraSign.Width / 2;
            position.Y -= _cameraSign.Height / 2;
            spriteBatch.Draw(
                _cameraSign,
                position,
                Color.White);
        }
    }
}
