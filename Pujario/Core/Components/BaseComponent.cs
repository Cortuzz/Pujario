using System;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Pujario.Core.Components
{
    public abstract class BaseComponent : BaseObject, IComponent
    {
        private bool _enabled;
        public virtual bool Enabled
        {
            get => _enabled;
            set
            {
                if (_enabled == value) return;
                _enabled = value;
                EnabledChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _upateOrder;
        public virtual int UpdateOrder
        {
            get => _upateOrder;
            set
            {
                if (_upateOrder == value) return;
                _upateOrder = value;
                UpdateOrderChanged.Invoke(this, EventArgs.Empty);
            }
        }

        protected HashSet<IComponent> _chiildComponents = new HashSet<IComponent>();
        public virtual IEnumerable<IComponent> ChildComponents => _chiildComponents;

        public virtual WeakReference<IComponent> ParentComponent { get; set; } = new WeakReference<IComponent>(null);

        protected WeakReference<IComponentProvider> _owner = new WeakReference<IComponentProvider>(null);
        public virtual WeakReference<IComponentProvider> Owner
        {
            get => _owner;
            set
            {
                _owner = value;
                foreach (var component in _chiildComponents) component.Owner = value;
            }
        }

        public virtual void Attach(IComponent other)
        {
            if (other.ParentComponent.TryGetTarget(out var parent)) parent.Detach(other);
            _chiildComponents.Add(other);
            other.ParentComponent.SetTarget(this);
            if (Owner.TryGetTarget(out var thisOwner)) other.Owner.SetTarget(thisOwner);
            other.Initialize();
        }

        public virtual void Detach(IComponent other)
        {
            _chiildComponents.Remove(other);
            other.ParentComponent.SetTarget(null);
            other.Owner.SetTarget(null);
            other.Dispose();
        }

        public virtual void Initialize() => Attached?.Invoke(this, EventArgs.Empty);

        public abstract void Update(GameTime gameTime);

        protected virtual void Dispose(bool disposing)
        {
            if (disposing) Detached.Invoke(this, EventArgs.Empty);
        }
        
        public void Dispose() => Dispose(true);
        ~BaseComponent() => Dispose(false);

        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> Attached;
        public event EventHandler<EventArgs> Detached;
    }
}
