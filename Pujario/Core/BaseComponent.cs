using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core
{
    // todo: implement base logic
    public abstract class BaseComponent : BaseObject, IComponent
    {
        public BaseComponent(ulong id) : base(id)
        {
        }

        public bool CanUpdate { get; set; }
        public void TryUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void ForceUpdate(GameTime gameTime)
        {
            throw new NotImplementedException();
        }

        public void Initialize()
        {
            throw new NotImplementedException();
        }

        public WeakReference<IComponent> ParentComponent { get; }
        public WeakReference<IActor> OwningActor { get; }
        public HashSet<IComponent> ChildComponents { get; }
        public void Attach(IComponent other)
        {
            throw new NotImplementedException();
        }

        public void Detach(IComponent other)
        {
            throw new NotImplementedException();
        }
    }
}