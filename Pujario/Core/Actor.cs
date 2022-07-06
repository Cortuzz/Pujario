using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.Collisions;
using Pujario.Utils;

namespace Pujario.Core
{
    // todo: implement all of the staff
    public class Actor : BaseObject, IActor
    {
        public Actor(ulong id, Transform2D transform) : base(id)
        {
            CanUpdate = false;
            Transform = transform;
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

        public IComponent RootComponent { get; set; }
        public WeakReference<IComponent> FindComponentByClass<TClass>()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<WeakReference<IComponent>> FindComponentsByClass<TClass>()
        {
            throw new NotImplementedException();
        }

        public WeakReference<IComponent> FindComponentById(ulong id)
        {
            throw new NotImplementedException();
        }

        public Transform2D Transform { get; set; }
        public void ApplyTransform(in Transform2D deltaTransform)
        {
            throw new NotImplementedException();
        }
        
        public void Draw(SpriteBatch spriteBatch)
        {
            throw new NotImplementedException();
        }

        public void Kill()
        {
            throw new NotImplementedException();
        }

        public event OnOverlap BeginOverlap;
        public event OnOverlap EndOverlap;
    }
}