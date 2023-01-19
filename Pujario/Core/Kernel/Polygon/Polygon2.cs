using System;
using Microsoft.Xna.Framework;
using Pujario.Utils;
using static Pujario.Core.Kernel.Entry.Polygon2;

namespace Pujario.Core.Kernel;

public struct Polygon2
{
    public Vector2[] points;

    /// <remarks>copies input array</remarks>
    public Polygon2(Vector2[] points)
    {
        this.points = new Vector2[points.Length];
        Array.Copy(points, this.points, points.Length);
    }
}

public enum Polygon2CreationOptions : byte
{
    Order = 0x01,
    Normalize = 0x02,
    WrapToConvexHull = 0x04
}

public class EPolygon2 : APIObjectEntry, ICloneable, IProvideManagedObjectView<Polygon2>
{
    /// <summary>
    /// Creates new polygon on Kernel.dll side and return pointer to it
    /// </summary>
    /// <remarks>
    /// Returned pointer should be feed using <see cref="Entry.Polygon2.Delete(nint)"/>
    /// </remarks>
    internal static IntPtr RemoteCreate(Vector2[] points, Polygon2CreationOptions options)
    {
        var ppol = (options & Polygon2CreationOptions.WrapToConvexHull) != 0
            ? CreateConvexHull(points, (uint)points.Length)
            : Create(points, (uint)points.Length);
        ApplyCreationOptions(ppol, options);
        return ppol;
    }

    /// <param name="ptr">An pointer to polygon on Kernel.dll side</param>
    internal static void ApplyCreationOptions(IntPtr ptr, Polygon2CreationOptions options)
    {
        if ((options & Polygon2CreationOptions.Order) != 0) OrderPoints(ptr);
        if ((options & Polygon2CreationOptions.Normalize) != 0) Normalize(ptr);
    }

    public EPolygon2(Vector2[] points, Polygon2CreationOptions options = 0) : base(RemoteCreate(points, options)) { }
    protected EPolygon2(IntPtr ptr) : base(ptr) { }

    protected override void Free()
    {
        Delete(P);
        base.Free();
    }

    public object Clone() => new EPolygon2(Copy(P));
    public Rectangle Bounds => CalcBoundRect(P);
    public bool IsConvex => IsConvex(P);
    public bool IsOrdered => IsOrdered(P);
    public bool IsNormalized => IsNormalized(P);
    public Vector2 MassCenter => GetMassCenter(P);

    public void ApplyTransform(in Transform2D transform) => Entry.Polygon2.ApplyTransform(P, transform);
    public bool IsInside(Vector2 point) => BelongsToPolygon(P, point);

    public bool TryGetObject(out Polygon2 result)
    {
        result = Polygon2Marshaller.ConvertToManaged(P);
        return true;
    }
}

