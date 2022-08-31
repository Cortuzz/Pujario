using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;

namespace Pujario.Core.Components
{
    public interface ICamera : IComponent, ITransformable
    {
        public Matrix TransformMatrix { get; }
        public Viewport Viewport { get; set; }
        public Vector2 ToWorldPosition(in Vector2 viewportPos);
        public Vector2 ToViewportPosition(in Vector2 worldPosition);
    }
}