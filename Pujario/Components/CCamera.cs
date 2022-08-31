using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core;
using Pujario.Core.Components;

namespace Pujario.Components
{
    public class CCamera : TransformableBaseComponent, ICamera
    {
        private bool _needRecalc;
        private Matrix _transformMatrix;
        public Matrix TransformMatrix
        {
            get
            {
                if (_needRecalc) _recalc();
                return _transformMatrix;
            }
        }

        private Matrix _invertedTransformMatrix;
        public Matrix InvertedTransformMatrix
        {
            get
            {
                if (_needRecalc) _recalc();
                return _invertedTransformMatrix;
            }
        }

        void _recalc()
        {
            _transformMatrix =
                Matrix.CreateTranslation(-(int)_transform.Position.X, -(int)_transform.Position.Y, 0)
                * Matrix.CreateRotationZ(_transform.Rotation)
                * Matrix.CreateScale(_transform.Scale.X, _transform.Scale.Y, 1)
                * Matrix.CreateTranslation(_viewport.Width / 2, _viewport.Height / 2, 0);
            Matrix.Invert(ref _transformMatrix, out _invertedTransformMatrix);
            _needRecalc = false;
        }

        private Viewport _viewport;
        public Viewport Viewport
        {
            get => _viewport;
            set
            {
                if (value.Width == _viewport.Width && value.Height == _viewport.Height) return;
                _viewport = value;
                _needRecalc = true;
            }
        }

        public override void Initialize()
        {
            _needRecalc = true;
            TransformChanged += _setNeedRecalc;
            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing) TransformChanged -= _setNeedRecalc;
            base.Dispose(disposing);
        }

        private void _setNeedRecalc(object sender, EventArgs e) => _needRecalc = true;

        public override void Update(GameTime gameTime)
        {
        }

        public Vector2 ToWorldPosition(in Vector2 viewportPos)
        {
            var worldPos = Vector2.Transform(viewportPos, InvertedTransformMatrix);
            return float.IsNaN(worldPos.X + worldPos.Y) ? Vector2.Zero : worldPos;
        }

        public Vector2 ToViewportPosition(in Vector2 worldPosition) => Vector2.Transform(worldPosition, TransformMatrix);
    }
}
