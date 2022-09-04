#if DEBUG
#define OUTPUT_TPS
#define DRAW_TPS
#endif


using Microsoft.Xna.Framework;
using System.Diagnostics;
using Pujario.Core;
using Pujario.Core.HUDs;

namespace Pujario
{
    /// <summary>
    /// Can be used for attaching to <see cref="Game"/>, and auto updating/drawing
    /// </summary>
    public class EngineGameComponent : DrawableGameComponent
    {
        private readonly IEngine _engine;
        private int _lastUpdateGameSeconds = -1;
        private int _lastDrawGameSeconds = -1;
        private float _UPS, _DPS;

        private readonly TPS_HUD _TPS_HUD;

        public bool OutputTPS { get; set; }
#if DEBUG && OUTPUT_TPS
            = true;
#endif
        private bool _drawTPS
#if DEBUG && DRAW_TPS
            = true;
#endif
        public bool DrawTPS 
        { 
            get => _drawTPS;
            set 
            {
                _drawTPS = value;
                _TPS_HUD.Visible = value;
            }
        }

        public EngineGameComponent(IEngine engine, Game game) : base(game)
        {
            _engine = engine;
            Enabled = true;
            Visible = true;
            UpdateOrder = int.MinValue;
            DrawOrder = int.MinValue;

            _TPS_HUD = new TPS_HUD(() => _DPS, () => _UPS);
            Engine.Instance.HUDsManager.RegisterInstance(_TPS_HUD);
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
            _engine.Update(gameTime);
        }


        public override void Draw(GameTime gameTime)
        {
            _DPS = (float)(1 / gameTime.ElapsedGameTime.TotalSeconds);
#if DEBUG
            if (OutputTPS &&_lastDrawGameSeconds != (_lastDrawGameSeconds = gameTime.TotalGameTime.Seconds))
                Debug.WriteLine("Draws per second - " + _DPS.ToString());
#endif

            _engine.Draw(gameTime);
        }
    }
}
