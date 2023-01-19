using Pujario.Utils;
using static Pujario.Core.Kernel.Entry;

namespace Pujario.Core.Kernel.Collisions;

public class EShape : APIObjectEntry
{
    protected EShape(nint p) : base(p) { }

    public Transform2D Transform { set => Shape.SetTransform(P, value); }
    public unsafe void* Primitive => Shape.GetPrimitive(P);

    public virtual bool CheckCollision(EShape other) => Algorithms.GJK(P, other.P);
    public virtual Collision CalcCollision(EShape other, float tolerance = 0.0001f) => Algorithms.EPA(P, other.P, tolerance);
}
