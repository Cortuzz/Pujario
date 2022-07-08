using System;

namespace Pujario.Core
{
    public abstract class BaseObject : IBaseObject
    {
        protected static Func<int> IdGetter = Engine.GetNextId;

        public int Id { get; } = IdGetter();

        public BaseObject()
        {
        }

        public override int GetHashCode()
        {
            return Id;
        }
    }
}