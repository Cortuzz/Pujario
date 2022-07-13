using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.Collisions;
using Pujario.Core.Components;
using Pujario.Utils;

namespace Pujario.Core
{
    // todo: implement all basics
    public abstract class Actor : BaseObject, IActor
    {
        private bool _enabled;
        private int _updateOrder;
        protected Transform2D _transform;
        protected Actor(IInstanceManager<IActor> manager)
        {
            manager.RegisterInstance(this);
        }

        public abstract void Update(GameTime gameTime);
        public abstract void Draw(GameTime gameTime, SpriteBatch spriteBatch);
        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled != value)
                {
                    _enabled = value;
                    EnabledChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public virtual int UpdateOrder
        {
            get => _updateOrder;
            set
            {
                if (_updateOrder != value)
                {
                    _updateOrder = value;
                    UpdateOrderChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public Transform2D Transform
        {
            get => _transform;
            set
            {
                if (_transform != value)
                {
                    _transform = value;
                    TransformChanged?.Invoke(this, EventArgs.Empty);
                } 
            }
        }

        public abstract IComponent RootComponent { get; set; }
        public abstract void ApplyTransform(in Transform2D deltaTransform);
        public abstract WeakReference<TClass> FindComponentByClass<TClass>() where TClass : class;
        public abstract IEnumerable<WeakReference<TClass>> FindComponentsByClass<TClass>() where TClass : class;
        public abstract WeakReference<IComponent> FindComponentById(ulong id);
        public abstract void Dispose();
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> TransformChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public abstract event OnOverlap BeginOverlap;
        public abstract event OnOverlap EndOverlap;
    }
}