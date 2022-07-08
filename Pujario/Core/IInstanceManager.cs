using System;

namespace Pujario.Core
{
    public interface IInstanceManager<TInstance> : ITicking where TInstance : class
    {
        /// <summary>
        /// Registers instance (strong ref used)  
        /// </summary>
        /// <returns>instance's hash code</returns>
        public int RegisterInstance(TInstance instance);
        public void UnregisterInstance(TInstance instance);
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
        /// <returns>instance's hash code</returns>
        public int RegisterInstance(TInstance instance, TAlias alias);
        
        /// <summary>
        /// Unregisters all instances associated with this alias
        /// </summary>
        /// <param name="alias"></param>
        /// <exception cref=""></exception>
        public void UnregisterAlias(TAlias alias);
    }
}