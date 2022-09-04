using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Pujario.Utils;
using System;

namespace Pujario.Core
{
    public partial class Engine
    {
        private class ZeroCamera : ICamera
        {
            public Matrix TransformMatrix => Matrix.Identity;

            public Viewport Viewport { get; set; }
            public Transform2D Transform { get; set; } = Transform2D.Base;
            public Transform2D RelativeTransform { get; set; } = Transform2D.Base;

            public Vector2 ToViewportPosition(in Vector2 worldPosition) => worldPosition;

            public Vector2 ToWorldPosition(in Vector2 viewportPos) => viewportPos;

            public event EventHandler<EventArgs> TransformChanged;

            public static readonly ZeroCamera Instance = new ZeroCamera();
        }
    }
}
