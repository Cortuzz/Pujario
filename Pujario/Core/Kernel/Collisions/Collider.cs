using System.Collections.Immutable;
using System.Linq;
using static Pujario.Core.Kernel.Entry.Collider2;

namespace Pujario.Core.Kernel.Collisions;

public class ECollider2 : APIObjectEntry
{
    public readonly bool PreferBoundsCheck;
    public readonly ImmutableArray<EShape> Shapes;

    public ECollider2(EShape[] shapes, bool preferBoundsCheck) 
        : base(Create(shapes.Select(sh => sh.P).ToArray(), (uint)shapes.Length, preferBoundsCheck))
    {
        PreferBoundsCheck = preferBoundsCheck;
        Shapes = ImmutableArray.Create(shapes);
    }

    protected override void Free()
    {
        Delete(P);
        Shapes.Clear();
        base.Free();
    }

    public bool CheckCollision(EShape shape) => CheckCollisionWithShape(P, shape.P);
    public bool CheckCollision(ECollider2 collider) => Entry.Collider2.CheckCollision(P, collider.P);
    public Collision CalcCollision(EShape shape, float tolerance = 0.0001f) => CalcCollisionWithShape(P, shape.P, tolerance);
    public Collision CalcCollision(ECollider2 collider, float tolerance = 0.0001f) => Entry.Collider2.CalcCollision(P, collider.P, tolerance);
}
