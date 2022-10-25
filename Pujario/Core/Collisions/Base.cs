using Microsoft.Xna.Framework;
using System;
using System.Runtime.InteropServices;
using System.Diagnostics;

namespace Pujario.Core.Collisions;

[StructLayout(LayoutKind.Sequential)]
public readonly struct CollisionInfo2D
{
    public readonly bool Collided;
    /// <summary>
    /// Invalid if <see cref="Collided"/> is false
    /// </summary>
    public readonly Vector2 Direction;
    /// <summary>
    /// Invalid if <see cref="Collided"/> is false
    /// </summary>
    public readonly float Depth;
}

public interface IShape 
{
    bool IsConvex { get; }
}

public interface ICollideable<TCollider>
{
    /// <summary>
    /// Checks if shapes collides.
    /// </summary>
    bool Collide(TCollider other);
    /// <summary>
    /// Calculates shapes' collision information.
    /// </summary>
    /// <returns> <see cref="CollisionInfo2D.Collided"/> of <paramref name="collision"/> </returns>
    bool Collide(TCollider other, out CollisionInfo2D collision);
}

public abstract class NativeShape : ICollideable<NativeShape>, IShape
{
    [DllImport("MathLibrary.dll", EntryPoint = "GJK")]
    private static extern bool CheckCollision(IntPtr shape1, IntPtr shape2);

    [DllImport("MathLibrary.dll", EntryPoint = "EPA")]
    protected static extern CollisionInfo2D CalcCollision(IntPtr shape1, IntPtr shape2, float tolerance);

    public abstract IntPtr P { get; }
    public abstract bool IsConvex { get; }

    /// <remarks>
    /// Shapes must be convex set of points!
    /// </remarks>
    public virtual bool Collide(NativeShape other)
    {
        Debug.Assert(IsConvex && other.IsConvex, "Shapes must be convex");
        return CheckCollision(P, other.P);
    }

    /// <remarks>
    /// Shapes must be convex set of points!
    /// </remarks>
    public virtual bool Collide(NativeShape other, out CollisionInfo2D collision)
    {
        Debug.Assert(IsConvex && other.IsConvex, "Shapes must be convex");
        collision = CalcCollision(P, other.P, 0.0001f);
        return collision.Collided;
    }
}
