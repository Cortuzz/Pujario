using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pujario.Core;
using Pujario.Core.Input;
using Pujario.Core.WorldPresentation;

namespace Pujario
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
        }

        protected override void Initialize()
        {
            Engine.Instance.Configure(
                new EngineConfig
                {
                    DefaultBufferSize = 10,
                    DefaultWorldSize = new Point(10, 10),
                    WorldChunkSize = 256,
                    FloatTolerance = 1e-5f,
                },
                this,
                new InputManager(
                    new Dictionary<string, InputCombination>(new[]
                    {
                        KeyValuePair.Create("Forward", new InputCombination(null, new[] { Keys.W, Keys.Up }))
                    }),
                    new[] { typeof(Actor) }),
                () => new SimpleTickBeaconSystem()
            );

            // if we use that, we don't need to call Engine in Update/Draw in the Game class 
            Components.Add(new EngineGameComponent(Engine.Instance, this));
            base.Initialize();
        }

        protected override void LoadContent()
        {
            // TODO: use this.Content to load your game content here
        }

        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed ||
                Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            base.Draw(gameTime);
        }
    }
}