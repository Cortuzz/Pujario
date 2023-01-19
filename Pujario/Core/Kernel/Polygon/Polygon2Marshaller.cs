using System;
using System.Runtime.InteropServices.Marshalling;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Pujario.Core.Kernel;

[CustomMarshaller(typeof(Polygon2), MarshalMode.ManagedToUnmanagedOut, typeof(Polygon2Marshaller))]
internal static unsafe class Polygon2Marshaller
{
    [StructLayout(LayoutKind.Sequential)]
    internal unsafe struct UnmanagedPolygon2
    {
        internal Vector2* points;
        internal uint count;
    }

    public static Polygon2 ConvertToManaged(UnmanagedPolygon2 unmanaged) =>
        new(new Span<Vector2>(unmanaged.points, (int)unmanaged.count).ToArray());
    public static Polygon2 ConvertToManaged(IntPtr unmanaged) => ConvertToManaged(*(UnmanagedPolygon2*)unmanaged);
}
