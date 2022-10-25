#if DEBUG
#define OUTPUT_TPS
#define DRAW_TPS
#endif

using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using System.Diagnostics;
using Pujario.Core;
using Pujario.Core.Diagnostics;

namespace Pujario
{
    /// <summary>
    /// Can be used for attaching to <see cref="Game"/>, and auto updating/drawing
    /// </summary>
    public class EngineGameComponent : DrawableGameComponent
    {
        private int _lastUpdateGameSeconds = -1;
        private int _lastDrawGameSeconds = -1;
        private float _UPS, _DPS;
        private readonly TPS_HUD _TPS_HUD;

        private readonly EngineCameraSignHUD _engineCameraSignHUD;

        public bool OutputTPS { get; set; }

        public bool DrawTPS
        {
            get => _TPS_HUD.Visible;
            set => _TPS_HUD.Visible = value;
        }

        public bool DrawCameraPos
        {
            get => _engineCameraSignHUD.Visible; 
            set => _engineCameraSignHUD.Visible = value;
        }

        public EngineGameComponent() : base(Engine.Instance.TargetGame)
        {
            Enabled = true;
            Visible = true;
            UpdateOrder = int.MinValue;
            DrawOrder = int.MinValue;
#if OUTPUT_TPS
            OutputTPS = true;
#endif

            _TPS_HUD = new TPS_HUD(() => _DPS, () => _UPS);
            Engine.Instance.HUDsManager.RegisterInstance(_TPS_HUD);
#if DRAW_TPS
            _TPS_HUD.Visible = true;
#endif
            _engineCameraSignHUD = new EngineCameraSignHUD()
                { CameraSign = Engine.Instance.TargetGame.Content.Load<Texture2D>("debugCameraSign") };
            Engine.Instance.HUDsManager.RegisterInstance(_engineCameraSignHUD);
#if DEBUG
            _engineCameraSignHUD.Visible = true;
#endif
        }

        ~EngineGameComponent()
        {
            Engine.Instance.HUDsManager.UnregisterInstance(_TPS_HUD);
        }

        public override void Update(GameTime gameTime)
        {
            _UPS = (float)(1 / gameTime.ElapsedGameTime.TotalSeconds);
#if DEBUG
            if (OutputTPS && _lastUpdateGameSeconds != (_lastUpdateGameSeconds = gameTime.TotalGameTime.Seconds))
                Debug.WriteLine("Updates per seconds - " + _UPS.ToString());
#endif
            Engine.Instance.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            _DPS = (float)(1 / gameTime.ElapsedGameTime.TotalSeconds);
#if DEBUG
            if (OutputTPS &&_lastDrawGameSeconds != (_lastDrawGameSeconds = gameTime.TotalGameTime.Seconds))
                Debug.WriteLine("Draws per second - " + _DPS.ToString());
#endif

            Engine.Instance.Draw(gameTime);
        }
    }
}
