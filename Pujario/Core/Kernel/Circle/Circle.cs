using Microsoft.Xna.Framework;
using System.Runtime.InteropServices;

namespace Pujario.Core.Kernel;

[StructLayout(LayoutKind.Sequential)]
public struct Circle
{
    public Vector2 Origin;
    public float Radius;
}
