namespace Pujario.Core
{
    public abstract class BaseObject : IBaseObject
    {
        public int Id { get; } = Engine.Instance.NextId;

        public override int GetHashCode() => Id;
    }
}