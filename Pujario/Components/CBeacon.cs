using System;
using Microsoft.Xna.Framework;
using Pujario.Core;
using Pujario.Utils;
using Pujario.Core.WorldPresentation;
using Pujario.Core.Components;
using Microsoft.Xna.Framework.Graphics;

namespace Pujario.Components
{
    public class CBeacon : TransformableBaseComponent
    {
        public ITickBeacon Beacon { get; set; } 
        public bool Visible { get; set; } = true;

        protected virtual void _onTransformChanged(object sender, EventArgs e)
        {
            if (sender is ITransformable ts && Beacon != null)
                Beacon.Position = ts.Transform.Position;
        }

        public override void Initialize()
        {
            if (Beacon != null)
                Engine.Instance.WorldMapping.TickBeaconSystem.UseBeacon(Beacon);
            TransformChanged += _onTransformChanged;
        }

        public override void Update(GameTime gameTime)
        {
        }

        protected override void Dispose(bool disposing)
        {
            if (Beacon != null)
                Engine.Instance.WorldMapping.TickBeaconSystem.DisuseBeacon(Beacon);
            TransformChanged -= _onTransformChanged;
        }
    }
}
