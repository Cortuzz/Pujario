using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace Pujario.Core.Collisions;

/// <summary>
/// Combines multiple convex <see cref="NativeShape"/>s for future coliisions
/// </summary>
/// <remarks>
/// <see cref="CombinedNativeShape"/> is always considered not convex 
/// </remarks>
public class CombinedNativeShape : NativeShape
{
    private readonly IntPtr _p;
    public override IntPtr P => _p;
    public override bool IsConvex => false;

    [DllImport("MathLibrary.dll")]
    private static extern IntPtr CreateCombinedShape([In]IntPtr[] shapes, int count);

    [DllImport("MathLibrary.dll")]
    private static extern IntPtr DestructCombinedShape(IntPtr cs);

    public CombinedNativeShape(IEnumerable<NativeShape> shapes)
    {
        var arr = shapes.Select((shape, _) => shape.P).ToArray();
        _p = CreateCombinedShape(arr, arr.Length);
    }

    ~CombinedNativeShape() => DestructCombinedShape(_p);
}