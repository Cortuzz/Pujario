namespace Pujario.Core
{
    public abstract class BaseObject : IBaseObject
    {
        public int Id { get; } = Engine.Instance.NextId;

        public BaseObject()
        {
        }

        public override int GetHashCode() => Id;
    }
}