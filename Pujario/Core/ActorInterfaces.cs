﻿using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;
using Pujario.Core.Components;

namespace Pujario.Core
{
    /// <summary>
    ///     This replaces <see cref="Microsoft.Xna.Framework.IDrawable"/>.
    /// </summary>
    public interface IDrawable
    {
        int DrawOrder { get; }
        bool Visible { get; }

        public void Draw(GameTime gameTime, SpriteBatch spriteBatch);

        event EventHandler<EventArgs> DrawOrderChanged;
        event EventHandler<EventArgs> VisibleChanged;
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
    }

    /// <summary>
    /// An instance builder, that can be used by engine in spawning  
    /// </summary>
    public interface IActorFabric
    {
        public IActor CreateActor();
    }
}