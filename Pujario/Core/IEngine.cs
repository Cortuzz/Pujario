using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Core
{
    public interface IEngine
    {
        public Game TargetGame { get; set; }
        public void Draw(GameTime gameTime);
        public void Update(GameTime gameTime);

        /// <summary>
        /// Adds your <see cref="IInstanceManager{TInstance}"/> 
        /// for calling <see cref="IDrawable.Draw(GameTime, SpriteBatch)"/>
        /// and <see cref="IUpdateable.Update(GameTime)"/> if implemented
        /// </summary>
        /// <param name="name">Access name</param>
        public void UseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, string name)
            where TInstance : class;

        public bool TryGetInstanceManager(string name, out object instanceManager);

        public void DisuseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, string name)
            where TInstance : class;
    }
}