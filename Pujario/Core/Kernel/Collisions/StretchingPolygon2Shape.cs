using Microsoft.Xna.Framework;
using System;
using System.Security.Cryptography;
using static Pujario.Core.Kernel.Entry.StretchingPolygon2Shape;
using static Pujario.Core.Kernel.Entry;

namespace Pujario.Core.Kernel.Collisions;

public class StretchingPolygon2Shape : EShape, IProvideManagedObjectView<Polygon2>
{
    public StretchingPolygon2Shape(Vector2[] points) : base(Create(points, (uint)points.Length))
    { }
    public StretchingPolygon2Shape(EPolygon2 epol) : base(CreateFromPolygon(epol.P))
    { }
    /*public StretchingPolygon2Shape(IntPtr ptr) : base(CreateFromPolygon(ptr))
    { }*/

    protected override void Free()
    {
        Delete(P);
        base.Free();
    }

    public bool ProvideStretched = false;

    /// <summary>
    /// Writes internal polygon to <paramref name="result"/>;
    /// if <see cref="ProvideStretched"/> then writes last stretch result
    /// </summary>
    public unsafe bool TryGetObject(out Polygon2 result)
    {
        result = ProvideStretched ? GetStretchedPolygon(P) : Polygon2Marshaller.ConvertToManaged((IntPtr)Primitive);
        return true;
    }
}
