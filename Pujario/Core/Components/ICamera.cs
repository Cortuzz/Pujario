using Microsoft.Xna.Framework;
using Pujario.Utils;

namespace Pujario.Core.Components
{
    public interface ICamera : IComponent, ITransformable
    {
        Matrix TransformMatrix { get; } 
    }
}