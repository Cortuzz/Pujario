using System;
using System.Diagnostics;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.Components;
using Pujario.Core.Input;
using Pujario.Core.WorldPresentation;
using Pujario.Utils;
using Pujario.Core.HUDs;

namespace Pujario.Core
{
    /// <summary>
    /// The God at your service
    /// </summary>
    public partial class Engine : IEngine
    {
        public static Func<float, float, bool> FloatEquality;

        private int _idCounter = int.MinValue; // podlyanka v code )) only 2^32 ids can be generated 
        private int _lastViewportHeight;
        private int _lastViewportWidth;

        private Game _targetGame;
        private GraphicsDeviceManager _gManager;
        private SpriteBatch _spriteBatch;

        private List<IUpdateable> _updateOrderedInstanceManagers;
        private List<IDrawable> _drawOrderedInstanceManagers;
        private Dictionary<string, object> _instanceManagers;

        private WeakReference<ICamera> _camera;

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
        public WorldMapping WorldMapping { get; private set; }
        public InputManager InputManager { get; private set; }
        public HUDsManager HUDsManager { get; private set; }

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
                return ZeroCamera.Instance;
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

            _instanceManagers = new Dictionary<string, object>(Config.DefaultBufferSize);
            _updateOrderedInstanceManagers = new List<IUpdateable>(Config.DefaultBufferSize);
            _drawOrderedInstanceManagers = new List<IDrawable>(Config.DefaultBufferSize);

            WorldMapping = new WorldMapping(Config.WorldChunkSize, Config.DefaultWorldSize,
                    tickBeaconSystemFabricMethod) { Enabled = true };
            HUDsManager = new HUDsManager();

            FloatEquality = (f, f1) => Math.Abs(f - f1) < Config.FloatTolerance;
            InputManager.FloatEquality = FloatEquality;

            _lastViewportWidth = _gManager.GraphicsDevice.Viewport.Width;
            _lastViewportHeight = _gManager.GraphicsDevice.Viewport.Height;
        }

        public void Draw(GameTime gameTime)
        {
            var viewport = _gManager.GraphicsDevice.Viewport;
            if (_lastViewportHeight != (_lastViewportHeight = viewport.Height)
                || _lastViewportWidth != (_lastViewportWidth = viewport.Width)) ViewportChanged?.Invoke(viewport);

            Matrix? transformMatrix = null;
            if (_camera.TryGetTarget(out var camera)) transformMatrix = camera.TransformMatrix;

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
            foreach (var drawable in _drawOrderedInstanceManagers) drawable.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();

            _spriteBatch.Begin(
                Config.DrawingSortMode,
                BlendState,
                SamplerState,
                DepthStencilState,
                RasterizerState,
                Effect,
                null);

            using (var e = HUDsManager.GetDrawEnumerator())
                while (e.MoveNext()) e.Current.Draw(gameTime, _spriteBatch);

            _spriteBatch.End();
        }

        public void Update(GameTime gameTime)
        {
            InputManager.RaiseEvents();
            if (WorldMapping.Enabled) WorldMapping.Update(gameTime);

            foreach (var updaetable in _updateOrderedInstanceManagers) updaetable.Update(gameTime);
        }

        public void UseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, string name)
            where TInstance : class
        {
            if (_instanceManagers.ContainsKey(name))
            {
#if DEBUG
                throw new ArgumentException("This key already exist");
#endif
#pragma warning disable CS0162 // Unreachable code detected
                return;
#pragma warning restore CS0162 // Unreachable code detected
            }
            _instanceManagers.Add(name, instanceManager);

            if (_instanceManagers is IUpdateable updateable)
            {
                var index = _updateOrderedInstanceManagers.BinarySearch(updateable, UpdateOrderComparer.Instance);
                index = index < 0 ? index ^ -1 : index;
                _updateOrderedInstanceManagers.Insert(index, updateable);
            }

            if (_instanceManagers is IDrawable drawable)
            {
                var index = _drawOrderedInstanceManagers.BinarySearch(drawable, DrawOrderComparer.Instance);
                index = index < 0 ? index ^ -1 : index;
                _drawOrderedInstanceManagers.Insert(index, drawable);
            }
        }

        public void DisuseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, string name)
            where TInstance : class
        {
            if (!_instanceManagers.Remove(name))
#if DEBUG
                    throw new KeyNotFoundException("Unknown instance manager");
#else
                    return;
#endif
            if (_instanceManagers is IUpdateable updateable)
                _updateOrderedInstanceManagers.RemoveAt(
                    _updateOrderedInstanceManagers.BinarySearch(updateable, UpdateOrderComparer.Instance));
            if (_instanceManagers is IDrawable drawable)
                _drawOrderedInstanceManagers.RemoveAt(
                    _drawOrderedInstanceManagers.BinarySearch(drawable, DrawOrderComparer.Instance));
        }

        public bool TryGetInstanceManager(string name, out object instanceManager) => 
            _instanceManagers.TryGetValue(name, out instanceManager);

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

        public event Action<Viewport> ViewportChanged;
    }
}