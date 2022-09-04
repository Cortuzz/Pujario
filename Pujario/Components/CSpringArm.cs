using System;
using System.Diagnostics;
using Microsoft.Xna.Framework;
using Pujario.Core.Components;
using Pujario.Utils;

namespace Pujario.Components
{
    public class CSpringArm : BaseComponent, ITransformable
    {
        private Vector2 _destination;
        private Vector2 _direction;

        private Transform2D _transform = Transform2D.Base;
        public Transform2D Transform
        {
            get
            {
                var transform = _transform;
                transform.Position = _destination;
                return transform;
            }
            set
            {
                if (_transform == value) return;
                _transform.Rotation = value.Rotation;
                _transform.Scale = value.Scale;
                _destination = value.Position;
                _direction = value.Position - _transform.Position;
                _direction.Normalize();
                TransformChanged?.Invoke(this, EventArgs.Empty);
            }
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
                _destination += value.Position - _transform.Position;
                _transform = value;
                TransformChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        public event EventHandler<EventArgs> TransformChanged;

        /// <summary>Variation range of arm</summary>
        public float Variation = 500;
        /// <summary>0.0 - 1.0; part of variation area where oscillation will be ignored</summary>
        public float Latency = .25f;
        /// <summary>Speed of arm moving</summary>
        public float Rigidity = 200;

        public override void Update(GameTime gameTime)
        {
            var dist = (_destination - _transform.Position).Length() / Variation;
            if (dist < Latency) return;

            var d = Transform2D.Zero;

            var cf = (Latency - 1) / (dist - 1) - 1;
            if (cf > 0)
            {
                d.Position += _direction *
                     cf * (float)gameTime.ElapsedGameTime.TotalSeconds * Rigidity;
                _transform.Position += d.Position;
            }
            else
            {
                d.Position = _destination - _transform.Position;
                _transform.Position = _destination;
            }
            
            _propagateTransform(d);
        }

        protected virtual void _propagateTransform(in Transform2D delta)
        {
            foreach (var component in _chiildComponents)
                if (component is ITransformable t) t.Transform += delta;
        }
    }
}
