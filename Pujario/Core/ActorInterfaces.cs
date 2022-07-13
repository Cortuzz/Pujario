using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;
using Pujario.Core.Collisions;
using Pujario.Core.Components;

namespace Pujario.Core
{
    public interface IDrawable
    {
        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);
    }
    
    public interface ITicking : IUpdateable, IDrawable
    {
    }

    /// <summary>
    /// Lowest object that can be registered in Engine 
    /// </summary>
    public interface IBaseObject
    {
        public int Id { get; }
    }

    /// <summary>
    /// Object that can be placed in world and contain <see cref="IComponent"/>
    /// 
    /// All Actor interacting should use <see cref="WeakReference{IActor}"/> for correct destroying by Engine 
    /// </summary>
    public interface IActor : ITicking, IBaseObject, IComponentProvider, ITransformable, IDisposable
    {
        public event OnOverlap BeginOverlap;
        public event OnOverlap EndOverlap;
    }
}