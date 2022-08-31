using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core.Collisions;
using Pujario.Core.Components;
using Pujario.Utils;

namespace Pujario.Core
{
    public class BaseActor : BaseObject, IActor
    {
        private bool _enabled;
        private int _updateOrder;
        protected Transform2D _transform = Transform2D.Base;
        protected IComponent _rootComponent;
        private ComponentEnumerator _componentEnumerator;

        public BaseActor() => _componentEnumerator = new ComponentEnumerator(RootComponent);
        ~BaseActor() => Dispose(false);

        public virtual void Update(GameTime gameTime)
        {
            _componentEnumerator.Reset();
            while (_componentEnumerator.MoveNext())
            {
                if (_componentEnumerator.Current is { Enabled: true })
                    _componentEnumerator.Current.Update(gameTime);
            }
        }

        public virtual void Draw(GameTime gameTime, SpriteBatch spriteBatch)
        {
            _componentEnumerator.Reset();
            while (_componentEnumerator.MoveNext())
            {
                var temp = _componentEnumerator.Current as IDrawable;
                if (temp is { Visible: true }) temp.Draw(gameTime, spriteBatch);
            }
        }

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

        private bool _visible = true;
        public bool Visible
        {
            get => _visible;
            set
            {
                if (_visible == value) return;
                _visible = value;
                VisibleChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private int _drawOrder;
        public int DrawOrder
        {
            get => _drawOrder;
            set
            {
                if (_drawOrder == value) return;
                _drawOrder = value;
                DrawOrderChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual IComponent RootComponent
        { 
            get => _rootComponent;
            set
            {
                _rootComponent?.Dispose();
                value.Initialize();
                _componentEnumerator.Reset(value);
                _rootComponent = value;
            } 
        }

        public virtual Transform2D Transform
        {
            get => _transform;
            set
            {
                if (_transform == value) return;
                var delta = value - _transform;
                _transform = value;
                TransformChanged?.Invoke(this, EventArgs.Empty);
                if (RootComponent is ITransformable t) t.Transform += delta;
            }
        }

        public virtual Transform2D RelativeTransform
        {
            get => _transform;
            set
            {
                if (_transform == value) return;
                _transform = value;
                TransformChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public virtual WeakReference<TClass> FindComponentByClass<TClass>() where TClass : class
        {
            _componentEnumerator.Reset();
            while (_componentEnumerator.MoveNext())
            {
                var temp = _componentEnumerator.Current as TClass;
                if (temp != null) return new WeakReference<TClass>(temp);
            }

            return new WeakReference<TClass>(null);
        }

        public virtual IEnumerable<WeakReference<TClass>> FindComponentsByClass<TClass>() where TClass : class
        {
            _componentEnumerator.Reset();
            var result = new List<WeakReference<TClass>>();
            while (_componentEnumerator.MoveNext())
            {
                var temp = _componentEnumerator.Current as TClass;
                if (temp != null) result.Add(new WeakReference<TClass>(temp));
            }

            return result;
        }

        public virtual WeakReference<IComponent> FindComponentById(int id)
        {
            _componentEnumerator.Reset();
            while (_componentEnumerator.MoveNext())
            {
                if (_componentEnumerator.Current.Id == id) 
                    return new WeakReference<IComponent>(_componentEnumerator.Current);
            }

            return new WeakReference<IComponent>(null);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                Visible = false;
                Enabled = false;
                UpdateOrderChanged = null;
                TransformChanged = null;
                EnabledChanged = null;
                DrawOrderChanged = null;
                VisibleChanged = null;
            }
            RootComponent = null;
            _componentEnumerator.Dispose();
            _componentEnumerator = null;
            GC.SuppressFinalize(this);
        }

        public void Dispose() => Dispose(true);

        public event EventHandler<EventArgs> UpdateOrderChanged;
        public event EventHandler<EventArgs> TransformChanged;
        public event EventHandler<EventArgs> EnabledChanged;
        public event EventHandler<EventArgs> DrawOrderChanged;
        public event EventHandler<EventArgs> VisibleChanged;
    }
}