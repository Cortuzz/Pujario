using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Pujario.Core.Kernel;
using Pujario.Utils;
using Pujario.Core.Components;
using System.Reflection.Metadata;
using Pujario.Core.Kernel.Collisions;
using System.Collections.Generic;
using Pujario.Core.Input;
using System.Diagnostics;

namespace Pujario.Core.Diagnostics;

public class CEPolygon2Visialiser : TransformableBaseComponent, Core.IDrawable
{
    private bool _visible = true;
    public bool Visible
    {
        get => _visible;
        set
        {
            if (_visible == value) return;
            _visible = value;
            VisibleChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    private int _drawOrder;
    public int DrawOrder
    {
        get => _drawOrder;
        set
        {
            if (_drawOrder == value) return;
            _drawOrder = value;
            DrawOrderChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public event EventHandler<EventArgs> DrawOrderChanged;
    public event EventHandler<EventArgs> VisibleChanged;

    private List<IProvideManagedObjectView<Polygon2>> _polygonProviders;

    float _coordsScaleFactor = 1;
    public float CoordsScaleFactor
    {
        get => _coordsScaleFactor;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _coordsScaleFactor = value;
        }
    }

    float _thickness = 1;
    public float Thickness
    {
        get => _thickness;
        set
        {
            if (value < 0) throw new ArgumentOutOfRangeException(nameof(value));
            _thickness = value;
        }
    }

    public Color Color { get; set; } = Color.Red;

    private List<EPolygon2Shape> quads;

    public CEPolygon2Visialiser()
    {
        _polygonProviders = new();
    }

    public void Add(IProvideManagedObjectView<Polygon2> provider)
    {
        _polygonProviders.Add(provider);
    }
    public void Remove(IProvideManagedObjectView<Polygon2> provider)
    {
        _polygonProviders.Remove(provider);
    }

    public void Draw(GameTime gameTime, SpriteBatch spriteBatch)
    {
        for (int i = 0; i < _polygonProviders.Count; i++)
        {
            if (_polygonProviders[i].TryGetObject(out var polygon))
            {
                spriteBatch.DrawPolygon(in polygon, Thickness, Color, CoordsScaleFactor);
            }
        }
    }

    public override void Update(GameTime gameTime)
    { }
}
