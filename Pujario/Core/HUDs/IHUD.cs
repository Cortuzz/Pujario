using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace Pujario.Core.HUDs
{
    public interface IHUD : IDrawable, IBaseObject
    {
        public Rectangle ViewportDest { get; }

        public event EventHandler ViewportDestChanged;
    }
}
