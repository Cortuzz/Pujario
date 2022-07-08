namespace Pujario.Core
{
    // todo: implement all basics
    public abstract class Actor : BaseObject, IActor
    {
        protected Actor(IInstanceManager<IActor> manager)
        {
            manager.RegisterInstance(this);
        }
    }
}