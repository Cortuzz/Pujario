#nullable enable
using System;
using System.Collections.Generic;
using Pujario.Utils;

namespace Pujario.Core
{
    /// <summary>
    /// An manager for your objects
    /// </summary>
    public interface IEngine : IDrawable, IUpdateable
    {
        /// <summary>
        /// Creates object and weekly register it in engine 
        /// </summary>
        /// <typeparam name="TEngineObject">Type of your object; Must be inherited from base object class <see cref="BaseObject"/></typeparam>
        /// <returns>Your object</returns>
        public TEngineObject CreateObject<TEngineObject>();

        /// <summary>
        /// Creates actor and strongly register it in engine
        /// </summary>
        /// <param name="withTransform">Actor's transform for world placement</param>
        /// <typeparam name="TEngineActor">Type of your actor; Must be inherited from base actor class <see cref="Actor"/></typeparam>
        /// <returns>Your actor</returns>
        public WeakReference<IActor> SpawnActor<TEngineActor>(Transform2D withTransform);

        /// <summary>
        /// Try to find registered object inside;
        /// Performance overhead;
        /// </summary>
        /// <returns>Your object or null if not succeed</returns>
        public IBaseObject? FindObjectById(ulong id);

        /// <summary>
        /// Try to find registered actor inside;
        /// Performance overhead;
        /// </summary>
        /// <param name="id"></param>
        /// <returns>Your actor or null if not succeed</returns>
        public WeakReference<IActor> FindActorById(ulong id);

        /// <summary>
        /// Try to find registered actor inside;
        /// Performance overhead;
        /// </summary>
        /// <typeparam name="TActorClass">Finding actor class</typeparam>
        /// <returns>Your actor or null if not succeed</returns>
        public WeakReference<IActor> FindActorByClass<TActorClass>();

        /// <summary>
        /// Try to find all registered suitable actors inside;
        /// Performance overhead;
        /// </summary>
        /// <typeparam name="TActorClass">Finding actor class</typeparam>
        /// <returns>Array of actors; empty if not succeed</returns>
        public IEnumerable<WeakReference<IActor>> FindActorsByClass<TActorClass>();

        /// <summary>
        /// Unregister Actor in Engine
        /// If correct behavior Garbage Collector should start deconstructing an actor 
        /// </summary>
        /// <param name="actor"></param>
        public void DestroyActor(IActor actor);
    }
}