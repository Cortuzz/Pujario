using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Core;
using Pujario.Core.Components;

namespace Pujario.Components
{
    public class CCamera : TransformableBaseComponent, ICamera, IComponent
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
                if (value.Height == _viewport.Height && value.Width == _viewport.Width) return;
                _viewport = value;
                _needRecalc = true;
            }
        }

        public CCamera() => _viewport = Engine.Instance.TargetGame.GraphicsDevice.Viewport;

        public override void Initialize()
        {
            _needRecalc = true;
            Engine.Instance.ViewportChanged += _setViewport;
            TransformChanged += _setNeedRecalc;
            base.Initialize();
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                TransformChanged -= _setNeedRecalc;
                Engine.Instance.ViewportChanged -= _setViewport;
            }

            base.Dispose(disposing);
        }

        private void _setNeedRecalc(object sender, EventArgs e) => _needRecalc = true;
        private void _setViewport(Viewport viewport)
        {
            _viewport = viewport;
            _needRecalc = true;
        }

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
