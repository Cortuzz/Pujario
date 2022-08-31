using System;
using System.Collections.Generic;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using Pujario.Components;
using Pujario.Core;
using Pujario.Core.Input;
using Pujario.Core.WorldPresentation;
using Pujario.Utils;

namespace Pujario
{
    public class Game1 : Game
    {
        private GraphicsDeviceManager _graphics;
        private CCamera _camera;

        public Game1()
        {
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
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
                _graphics,
                new InputManager(
                    new Dictionary<string, InputCombination>(new[]
                    {
                        KeyValuePair.Create("Forward", new InputCombination(null, new[] { Keys.W, Keys.Up }))
                    }),
                    new[] { typeof(BaseActor) }),
                () => new SimpleTickBeaconSystem()
            );

            var t = Transform2D.Base;
            t.Position = new Vector2(
                Engine.Instance.WorldMapping.Grid.GetLength(0) / 2 * Engine.Instance.WorldMapping.ChunkSize,
                Engine.Instance.WorldMapping.Grid.GetLength(1) / 2 * Engine.Instance.WorldMapping.ChunkSize);

            _camera = new CCamera();
            _camera.Transform = t;
            Engine.Instance.Camera = _camera;

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