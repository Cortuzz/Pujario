using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace Pujario.Core.Kernel.Collisions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct Collision
{
    readonly bool AreCollided;
    readonly Vector2 Direction;
    readonly float Depth;
};
