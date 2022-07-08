using System;
using System.Collections;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core
{
    // todo: implement all the staff
    /// <summary>
    /// The God at your service
    /// </summary>
    public class Engine : IEngine, IInstanceManager<IActor>, IInstanceManager<IBaseObject>
    {
        private static int _idCounter = Int32.MinValue;

        public static Engine Instance { get; } = new Engine();
        public static int GetNextId() => ++_idCounter;

        // private Dictionary<int, IActor> _actors;
        private Hashtable _actors;

        // private Dictionary<int, IBaseObject> _objects;
        private Hashtable _objects;
        private Dictionary<string, ITicking> _externalInstanceManagers;

        protected Engine()
        {
            _actors = new Hashtable();
            _objects = new Hashtable();
            _externalInstanceManagers = new Dictionary<string, ITicking>();
        }
    }

    public class EngineGameComponent : GameComponent
    {
        private IEngine _engine;

        public EngineGameComponent(IEngine engine, Game game) : base(game)
        {
            _engine = engine;
            _engine.TargetGame = game;
        }
    }
}