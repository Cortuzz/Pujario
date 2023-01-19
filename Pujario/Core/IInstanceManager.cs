using System;
using System.Collections.Generic;
using Pujario.Exceptions.Core;

namespace Pujario.Core
{
    public interface IInstanceManager<TInstance> : IEnumerable<TInstance> where TInstance : class
    {
        /// <summary>
        /// Registers instance (strong ref used)  
        /// </summary>
        public void RegisterInstance(TInstance instance);

        /// <exception cref="UnknownIdentifier">When instance isn't registered</exception>
        public void UnregisterInstance(TInstance instance);

        /// <exception cref="UnknownIdentifier">When hashCode is unknown</exception>
        public void UnregisterInstance(int hashCode);

        public WeakReference<TInstance> FindInstance(int hashCode);
    }

    public interface IAliasInstanceManager<TInstance, in TAlias> : IInstanceManager<TInstance> where TInstance : class
    {
        /// <summary>
        /// Registers instance and associate it with some alias  
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="alias">Some not unique name</param>
        public void RegisterInstance(TInstance instance, TAlias alias);

        /// <summary>
        /// Unregisters all instances associated with this alias
        /// </summary>
        /// <param name="alias"></param>
        /// <exception cref="UnknownIdentifier"></exception>
        public void UnregisterAlias(TAlias alias);

        public IEnumerable<WeakReference<TInstance>> GetInstancesByAlias(TAlias alias);
    }
}