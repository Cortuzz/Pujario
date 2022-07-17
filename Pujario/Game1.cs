using System;
using System.Diagnostics;
using System.Linq;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Pujario.Core;
using Pujario.NoiseGeneration;
using Pujario.NoiseGeneration.Interfaces;

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
                    WorldChunkSize = 256
                },
                this
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