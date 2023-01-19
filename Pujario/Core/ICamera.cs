using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;

namespace Pujario.Core
{
    public interface ICamera : ITransformable
    {
        public Matrix TransformMatrix { get; }
        public Viewport Viewport { get; }
        public Vector2 ToWorldPosition(in Vector2 viewportPos);
        public Vector2 ToViewportPosition(in Vector2 worldPosition);
    }
}