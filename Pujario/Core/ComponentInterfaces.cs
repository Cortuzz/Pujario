using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core
{
    /// <summary>
    /// Component that could be set into IComponentProvider or attached to other Components
    /// Components aren't managed by engine, it is Actor's property
    /// Strong references are used in descending hierarchy, backward are weak
    ///  
    /// <see cref="IGameComponent.Initialize"/> will be called on attachment 
    /// </summary>
    public interface IComponent : IUpdateable, IGameComponent, IBaseObject
    {
        /// <summary>
        /// Parent component; could be null if it is RootComponent or if it haven't been attached
        /// </summary>
        public WeakReference<IComponent> ParentComponent { get; }

        public WeakReference<IActor> OwningActor { get; }
        public HashSet<IComponent> ChildComponents { get; }

        public void Attach(IComponent other);
        public void Detach(IComponent other);
    }

    public interface IComponentProvider
    {
        public IComponent RootComponent { get; set; }

        public WeakReference<IComponent> FindComponentByClass<TClass>();
        public IEnumerable<WeakReference<IComponent>> FindComponentsByClass<TClass>();

        public WeakReference<IComponent> FindComponentById(ulong id);
    }
}