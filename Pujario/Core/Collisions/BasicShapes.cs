using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Pujario.Core.Collisions;

public class Polygon : NativeShape
{
    private readonly IntPtr _p;
    public override IntPtr P => _p;

    [DllImport("MathLibrary.dll")] 
    private static extern IntPtr CreatePolygon2S([In]Vector2[] points, int count);

    [DllImport("MathLibrary.dll", EntryPoint = "GetVertex_Polygon2S")]
    private static extern Vector2 GetVertex(IntPtr pPolygon, int index);

    [DllImport("MathLibrary.dll", EntryPoint = "GetCentroid_Polygon2S")]
    private static extern Vector2 GetCentroid(IntPtr pPolygon);

    [DllImport("MathLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr DestructPolygon2S(IntPtr pPolygon);

    [DllImport("MathLibrary.dll", EntryPoint = "IsConvex_Polygon2S")]
    private static extern bool _IsConvex(IntPtr pPolygon);

    public Polygon(Vector2[] points)
    {
        VertexCount = points.Length;
        _p = CreatePolygon2S(points, VertexCount);
    }

    public readonly int VertexCount;
    public override bool IsConvex => _IsConvex(_p);
    public Vector2 Centroid => GetCentroid(_p);

    public Vector2 this[int index] => GetVertex(_p, index);

    ~Polygon() => DestructPolygon2S(_p);
}

public class Circle : NativeShape
{
    private readonly IntPtr _p;
    public override IntPtr P => _p;
    public override bool IsConvex => true;

    [DllImport("MathLibrary.dll", CallingConvention = CallingConvention.Cdecl)]
    private static extern IntPtr CreateCircle(Vector2 center, float radius);

    [DllImport("MathLibrary.dll")]
    private static extern IntPtr DestructCircle(IntPtr circle);

    [DllImport("MathLibrary.dll")]
    private static extern void SetRadius(IntPtr circle, float radius);
    [DllImport("MathLibrary.dll")]
    private static extern float GetRadius(IntPtr circle);

    [DllImport("MathLibrary.dll")]
    private static extern void SetOrigin(IntPtr circle, Vector2 radius);
    [DllImport("MathLibrary.dll")]
    private static extern Vector2 GetOrigin(IntPtr circle);

    public Circle(Vector2 center, float radius) => _p = CreateCircle(center, radius);

    public Vector2 Origin { get => GetOrigin(_p); set => SetOrigin(_p, value); }
    public float Radius { get => GetRadius(_p); set => SetRadius(_p, value); }

    ~Circle() => DestructCircle(_p);
}
