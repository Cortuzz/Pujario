using System;
using System.Runtime.InteropServices;
using Microsoft.Xna.Framework;

namespace Pujario.Utils
{
    [StructLayout(LayoutKind.Sequential)]
    public struct Transform2D : IEquatable<Transform2D>
    {
        public static readonly Transform2D Zero = new Transform2D(Vector2.Zero, Vector2.Zero, Vector2.Zero, .0f);
        public static readonly Transform2D Base = new Transform2D(Vector2.Zero, Vector2.Zero, Vector2.One, .0f);

        public Vector2 Position;
        public Vector2 Scale;
        /// <summary>
        /// Origin point coords related to <see cref="Position"/>
        /// </summary>
        public Vector2 Origin;
        public float Rotation;

        public Transform2D(Vector2 position, Vector2 origin, Vector2 scale, float rotation)
        {
            Position = position;
            Origin = origin;
            Scale = scale;
            Rotation = rotation;
        }

        public static Transform2D operator +(in Transform2D t1, in Transform2D t2) =>
            new Transform2D(t1.Position + t2.Position, t1.Origin + t2.Origin, t1.Scale + t2.Scale,
                t1.Rotation + t2.Rotation);

        public static Transform2D operator -(in Transform2D t1, in Transform2D t2) =>
            new Transform2D(t1.Position - t2.Position, t1.Origin - t2.Origin, t1.Scale - t2.Scale,
                t1.Rotation - t2.Rotation);

        public static bool operator ==(in Transform2D t1, in Transform2D t2) => t1.Equals(t2);

        public static bool operator !=(in Transform2D t1, in Transform2D t2) => !t1.Equals(t2);

        public bool Equals(Transform2D other) =>
            Position.Equals(other.Position) && Scale.Equals(other.Scale) && Math.Abs(Rotation - other.Rotation) < 1e-5;

        public override bool Equals(object obj) => obj is Transform2D other && Equals(other);

        public override int GetHashCode() => HashCode.Combine(Position, Origin, Scale, Rotation);
    }

    public interface ITransformable
    {
        /// <summary>
        /// Sets transform and raises <see cref="TransformChanged"/>, then propagates delta to attached transformable objects 
        /// </summary>
        public Transform2D Transform { get; set; }

        /// <summary>
        /// Sets relative transform and raises <see cref="TransformChanged"/>
        /// </summary>
        public Transform2D RelativeTransform { get; set; }

        /// <summary>
        /// Must be called when transform changed
        /// </summary>
        public event EventHandler<EventArgs> TransformChanged;
    }
}