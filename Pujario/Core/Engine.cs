using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;

namespace Pujario.Core
{
    // todo: implement all the staff
    /// <summary>
    /// The God at your service
    /// </summary>
    public class Engine : IEngine
    {
        private Engine()
        {
        }

        public static Engine Instance { get; } = new Engine();


        public TEngineObject CreateObject<TEngineObject>()
        {
            throw new NotImplementedException();
        }

        public WeakReference<IActor> SpawnActor<TEngineActor>(Transform2D withTransform)
        {
            throw new NotImplementedException();
        }

        public IBaseObject FindObjectById(ulong id)
        {
            throw new NotImplementedException();
        }

        public WeakReference<IActor> FindActorById(ulong id)
        {
            throw new NotImplementedException();
        }

        public WeakReference<IActor> FindActorByClass<TActorClass>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WeakReference<IActor>> FindActorsByClass<TActorClass>()
        {
            throw new NotImplementedException();
        }

        public void DestroyActor(IActor actor)
        {
            throw new NotImplementedException();
        }

        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public bool CanUpdate { get; set; }
        public void TryUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void ForceUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }
    }
}