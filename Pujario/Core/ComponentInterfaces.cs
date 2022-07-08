using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core
{
    /// <summary>
    /// Component that could be set into IComponentProvider or attached to other Components
    /// Components aren't managed by engine, it is Actor's property
    /// Strong references are used in descending hierarchy, backward are weak
    /// <see cref="IGameComponent.Initialize"/> will be called on attachment 
    /// </summary>
    public interface IComponent : IUpdateable, IGameComponent, IBaseObject
    {
        /// <summary>
        /// Parent component; could be null if it is RootComponent or if it haven't been attached
        /// </summary>
        public WeakReference<IComponent> ParentComponent { get; }

        /// <summary>
        /// Could be null if it haven't been attached
        /// </summary>
        public WeakReference<IComponentProvider> Owner { get; }
        public HashSet<IComponent> ChildComponents { get; }

        /// <summary>
        /// Attach other component to this
        /// </summary>
        public void Attach(IComponent other);
        
        /// <summary>
        /// Detach other component to this
        /// </summary>
        public void Detach(IComponent other);
    }

    public interface IComponentProvider
    {
        public IComponent RootComponent { get; set; }

        public WeakReference<TClass> FindComponentByClass<TClass>() where TClass : class;
        public IEnumerable<WeakReference<TClass>> FindComponentsByClass<TClass>() where TClass : class;

        public WeakReference<IComponent> FindComponentById(ulong id);
    }
}