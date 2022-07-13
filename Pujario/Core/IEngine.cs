﻿using Microsoft.Xna.Framework;

namespace Pujario.Core
{
    public interface IEngine
    {
        public Game TargetGame { get; set; }
        public void Draw(GameTime gameTime);
        public void Update(GameTime gameTime);

        public void UseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, in string name)
            where TInstance : class;

        public void DisuseInstanceManager<TInstance>(IInstanceManager<TInstance> instanceManager, in string name)
            where TInstance : class;
    }
}