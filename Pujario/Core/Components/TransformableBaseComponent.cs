using System;
using Pujario.Utils;

namespace Pujario.Core.Components
{
    public abstract class TransformableBaseComponent : BaseComponent, ITransformable
    {
        protected Transform2D _transform = Transform2D.Base;

        public Transform2D Transform
        {
            get => _transform;
            set
            {
                if (_transform == value) return;
                var delta = value - _transform;
                _transform = value;
                TransformChanged?.Invoke(this, EventArgs.Empty);
                _propagateTransform(delta);
            }
            // TODO : why I managed to propagate delta transform ? mb just set to it
        }

        public Transform2D RelativeTransform
        {
            get
            {
                if (ComponentHelper.TryGetTransformableParent(this, out var parent))
                    return _transform - parent.Transform;
                return _transform;
            }
            set
            {
                if (_transform == value) return;
                _transform = value;
                TransformChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        protected virtual void _propagateTransform(in Transform2D delta)
        {
            foreach (var component in _chiildComponents)
                if (component is ITransformable t) t.Transform += delta;
        }

        public event EventHandler<EventArgs> TransformChanged;
    }
}
