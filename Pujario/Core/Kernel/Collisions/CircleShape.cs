using Microsoft.Xna.Framework;
using static Pujario.Core.Kernel.Entry.CircleShape;

namespace Pujario.Core.Kernel.Collisions;

public class ECircleShape : EShape, IProvideManagedObjectView<Circle>
{
    public ECircleShape(Vector2 origin, float radius) : base(Create(origin, radius))
    { }

    protected override void Free()
    {
        Delete(P);
        base.Free();
    }

    public unsafe bool TryGetObject(out Circle result)
    {
        result = *(Circle*)Primitive;
        return true;
    }
}
