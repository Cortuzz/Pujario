using Microsoft.Xna.Framework;
using System;
using static Pujario.Core.Kernel.Entry.Polygon2Shape;
using static Pujario.Core.Kernel.Entry;

namespace Pujario.Core.Kernel.Collisions;

public class EPolygon2Shape : EShape, IProvideManagedObjectView<Polygon2>
{
    public EPolygon2Shape(Vector2[] points) : base(Create(points, (uint)points.Length))
    { }
    public EPolygon2Shape(EPolygon2 epol) : base(CreateFromPolygon(epol.P))
    { }

    protected override void Free()
    {
        Delete(P);
        base.Free();
    }

#if USE_SAT
    public override bool CheckCollision(EShape other)
    {
        if (other.GetType() == typeof(EPolygon2))
            unsafe
            {
                return Algorithms.SAT((IntPtr)Primitive, (IntPtr)other.Primitive);
            }
        else return base.CheckCollision(other);
    }
#endif

    public unsafe bool TryGetObject(out Polygon2 result)
    {
        result = Polygon2Marshaller.ConvertToManaged((IntPtr)Primitive);
        return true;
    }
}
