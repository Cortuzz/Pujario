using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.Components;
using Pujario.Core.WorldPresentation;
using Pujario.Utils;

namespace Pujario.Core
{
    /// <summary>
    /// The God at your service
    /// </summary>
    public class Engine : IEngine
    {
        private int _idCounter = Int32.MinValue; // podlyanka v code )) only 2^32 ids can be generated 
        private Game _targetGame;
        private List<ITicking> _orderedInstanceManagers;
        private WeakReference<ICamera> _camera;
        private SpriteBatch _spriteBatch;

        public EngineConfig Config = default;

        public Dictionary<string, ITicking> InstanceManagers { get; private set; }

        public Game TargetGame
        {
            get => _targetGame;
            set
            {
                _spriteBatch = new SpriteBatch(value.GraphicsDevice, Config.DefaultSpriteBatchSize);
                _targetGame = value;
            }
        }

        public WorldMapping WorldMapping { get; protected set; }

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

        protected Engine()
        {
            _camera = new WeakReference<ICamera>(null);
        }

        /// <summary>
        /// Necessary constructing of the Engine  
        /// </summary>
        public void Configure(in EngineConfig config, Game targetGame, ICamera camera = null)
        {
            Config = config;
            TargetGame = targetGame;
            _camera.SetTarget(camera);
            InstanceManagers = new Dictionary<string, ITicking>(Config.DefaultBufferSize);
            _orderedInstanceManagers = new List<ITicking>(Config.DefaultBufferSize);
            WorldMapping = new WorldMapping(Config.WorldChunkSize, Config.DefaultWorldSize);
        }

        public void Draw(GameTime gameTime)
        {
            Matrix? transformMatrix = null;
            if (_camera.TryGetTarget(out var camera))
                transformMatrix = camera.TransformMatrix;

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
            if (WorldMapping.Enabled) WorldMapping.Update(gameTime);

            foreach (var node in InstanceManagers)
            {
                if (node.Value is { Enabled: true })
                    node.Value.Draw(gameTime, _spriteBatch);
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
    }


    public class EngineGameComponent : GameComponent
    {
        private readonly IEngine _engine;

        public EngineGameComponent(IEngine engine, Game game) : base(game) => _engine = engine;

        public override void Update(GameTime gameTime)
        {
            _engine.Update(gameTime);
        }
    }
}