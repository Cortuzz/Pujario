using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.Components;
using Pujario.Core.Input;
using Pujario.Core.WorldPresentation;
using Pujario.Utils;

namespace Pujario.Core
{
    /// <summary>
    /// The God at your service
    /// </summary>
    public class Engine : IEngine
    {
        public static Func<float, float, bool> FloatEquality;

        private int _idCounter = int.MinValue; // podlyanka v code )) only 2^32 ids can be generated 
        private Game _targetGame;
        private List<ITicking> _orderedInstanceManagers;
        private WeakReference<ICamera> _camera;
        private SpriteBatch _spriteBatch;
        private GraphicsDeviceManager _gManager;

        public Game TargetGame
        {
            get => _targetGame;
            set
            {
                _spriteBatch = new SpriteBatch(value.GraphicsDevice, Config.DefaultSpriteBatchSize);
                _targetGame = value;
            }
        }

        public EngineConfig Config { get; private set; }
        public Dictionary<string, ITicking> InstanceManagers { get; private set; }
        public WorldMapping WorldMapping { get; private set; }
        public InputManager InputManager { get; private set; }

        public int NextId => ++_idCounter;

        // todo: learn how drawing parameters works and how we will manage them

        public BlendState BlendState { get; set; } = null;
        public SamplerState SamplerState { get; set; } = null;
        public DepthStencilState DepthStencilState { get; set; } = null;
        public RasterizerState RasterizerState { get; set; } = null;
        public Effect Effect { get; set; } = null;

        /// <summary>
        /// Set camera, which will be used for <see cref="Draw"/>ing; it is week referenced  
        /// </summary>
        /// <exception cref="NullReferenceException">When camera haven't been set</exception>
        public ICamera Camera
        {
            get
            {
                if (_camera.TryGetTarget(out var camera)) return camera;
                throw new NullReferenceException("_camera was null");
            }
            set => _camera.SetTarget(value);
        }

        public static Engine Instance { get; } = new Engine();

        private Engine()
        {
            _camera = new WeakReference<ICamera>(null);
        }

        /// <summary>
        /// Necessary constructing of the Engine  
        /// </summary>
        public void Configure(in EngineConfig config, Game targetGame, GraphicsDeviceManager gManager, InputManager inputManager,
            Func<ITickBeaconSystem> tickBeaconSystemFabricMethod, ICamera camera = null)
        {
            Config = config;
            TargetGame = targetGame;
            InputManager = inputManager;
            _gManager = gManager;
            _camera.SetTarget(camera);
            InstanceManagers = new Dictionary<string, ITicking>(Config.DefaultBufferSize);
            _orderedInstanceManagers = new List<ITicking>(Config.DefaultBufferSize);
            WorldMapping = new WorldMapping(Config.WorldChunkSize, new Point(20, 20),
                    tickBeaconSystemFabricMethod) { Enabled = true };

            FloatEquality = (f, f1) => Math.Abs(f - f1) < Config.FloatTolerance;
            InputManager.FloatEquality = FloatEquality;
        }

        public void Draw(GameTime gameTime)
        {
#if DEBUG
            if (gameTime.TotalGameTime.Seconds % 2 == 1)
                Debug.WriteLine("Draw FPS - " + (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString());
#endif
            Matrix? transformMatrix = null;
            if (_camera.TryGetTarget(out var camera))
            {
                if (_gManager.PreferredBackBufferHeight != camera.Viewport.Height 
                    || _gManager.PreferredBackBufferWidth != camera.Viewport.Width)
                {
                    camera.Viewport = new Viewport(camera.Viewport.X, camera.Viewport.Y,
                        _gManager.PreferredBackBufferWidth, _gManager.PreferredBackBufferHeight);
                }

                transformMatrix = camera.TransformMatrix;
            }

            _gManager.GraphicsDevice.Clear(Color.DarkGray);
            _spriteBatch.Begin(
                Config.DrawingSortMode,
                BlendState,
                SamplerState,
                DepthStencilState,
                RasterizerState,
                Effect,
                transformMatrix);

            WorldMapping.Draw(gameTime, _spriteBatch);
            foreach (var node in InstanceManagers)
            {
                node.Value?.Draw(gameTime, _spriteBatch);
            }

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
#if DEBUG
            if (gameTime.TotalGameTime.Seconds % 2 == 1)
                Debug.WriteLine("Update FPS - " + (1 / gameTime.ElapsedGameTime.TotalSeconds).ToString());
#endif
            InputManager.RaiseEvents();
            if (WorldMapping.Enabled) WorldMapping.Update(gameTime);

            foreach (var node in InstanceManagers)
            {
                if (node.Value is { Enabled: true })
                    node.Value.Update(gameTime);
            }
        }

        public void UseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, in string name)
            where TInstance : class
        {
            InstanceManagers.Add(name, instanceManager);
            var index = _orderedInstanceManagers.BinarySearch(instanceManager, UpdateOrderComparer.Instance);
            index = index < 0 ? index ^ -1 : index;

            _orderedInstanceManagers.Insert(index, instanceManager);
        }

        public void DisuseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, in string name)
            where TInstance : class
        {
            if (InstanceManagers.Remove(name))
            {
                _orderedInstanceManagers.RemoveAt(
                    _orderedInstanceManagers.BinarySearch(instanceManager, UpdateOrderComparer.Instance));
            }
#if DEBUG
            else throw new KeyNotFoundException("Unknown instance manager");
#endif
        }

        private IActor _placeActor(IActor actor, in Transform2D transform)
        {
            actor.Transform = transform;
            WorldMapping[transform.Position].RegisterInstance(actor);
            return actor;
        }

        public void SpawnActor(Func<IActor> fabricDelegate, in Transform2D transform, out WeakReference<IActor> result) =>
            result = new WeakReference<IActor>(_placeActor(fabricDelegate(), transform));

        public void SpawnActor(Func<IActor> fabricDelegate, in Transform2D transform) => _placeActor(fabricDelegate(), transform);

        public void SpawnActor(IActorFabric fabric, in Transform2D transform, out WeakReference<IActor> result) =>
            result = new WeakReference<IActor>(_placeActor(fabric.CreateActor(), transform));

        public void SpawnActor(IActorFabric fabric, in Transform2D transform) => _placeActor(fabric.CreateActor(), transform);
    }

    /// <summary>
    /// Can be used for attaching to <see cref="Game"/>, and auto updating/drawing
    /// </summary>
    public class EngineGameComponent : DrawableGameComponent
    {
        private readonly IEngine _engine;

        public EngineGameComponent(IEngine engine, Game game) : base(game)
        {
            _engine = engine;
            Enabled = true;
            Visible = true;
            UpdateOrder = int.MinValue;
            DrawOrder = int.MinValue;
        }

        public override void Update(GameTime gameTime) => _engine.Update(gameTime);

        public override void Draw(GameTime gameTime) => _engine.Draw(gameTime);
    }
}