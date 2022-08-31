using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core.Components
{
    /// <summary>
    /// Component that could be set into IComponentProvider or attached to other Components
    /// Components aren't managed by engine, it is Actor's property
    /// Strong references are used in descending hierarchy, backward are weak
    /// <see cref="IGameComponent.Initialize"/> will be called on attachment, must put component to working condition after disposing 
    /// <see cref="IDisposable.Dispose"/> will be called on detachment 
    /// </summary>
    public interface IComponent : IUpdateable, IGameComponent, IBaseObject, IDisposable
    {
        /// <summary>
        /// Parent component; could be null if it is RootComponent or if it haven't been attached
        /// </summary>
        public WeakReference<IComponent> ParentComponent { get; }

        /// <summary>
        /// Could be null if it haven't been attached
        /// </summary>
        public WeakReference<IComponentProvider> Owner { get; set; }

        public IEnumerable<IComponent> ChildComponents { get; }

        /// <summary>
        /// Attach other component to this
        /// </summary>
        public void Attach(IComponent other);

        /// <summary>
        /// Detach other component to this
        /// </summary>
        public void Detach(IComponent other);

        public event EventHandler<EventArgs> Attached;
        public event EventHandler<EventArgs> Detached;
    }

    public interface IComponentProvider
    {
        public IComponent RootComponent { get; set; }

        public WeakReference<TClass> FindComponentByClass<TClass>() where TClass : class;

        public IEnumerable<WeakReference<TClass>> FindComponentsByClass<TClass>() where TClass : class;

        public WeakReference<IComponent> FindComponentById(int id);
    }
}