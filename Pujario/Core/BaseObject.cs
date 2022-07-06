namespace Pujario.Core
{
    public abstract class BaseObject : IBaseObject
    {
        public ulong Id { get; }

        public BaseObject(ulong id)
        {
            Id = id;
        }
    }
}