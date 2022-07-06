using Microsoft.Xna.Framework;

namespace Pujario.Utils
{
    public struct Transform2D
    {
        /// <summary>
        /// origin  coords
        /// </summary>
        public Vector2 Location;
        public float Depth; // 0-1
        public Vector2 Scale;
        public float Rotation;

        public Transform2D(Vector2 location, Vector2 scale, float depth, float rotation)
        {
            Location = location;
            Depth = depth;
            Scale = scale;
            Rotation = rotation;
        }

        public static Transform2D Base => new Transform2D(Vector2.Zero, Vector2.One, 0, 0);

        public static Transform2D operator +(Transform2D t1, in Transform2D t2) =>
            new Transform2D(t1.Location + t2.Location, t1.Scale + t2.Scale, t1.Depth + t2.Depth,
                t1.Rotation + t2.Rotation);

        public static Transform2D operator -(Transform2D t1, in Transform2D t2) =>
            new Transform2D(t1.Location - t2.Location, t1.Scale - t2.Scale, t1.Depth - t2.Depth,
                t1.Rotation - t2.Rotation);
    }

    public interface ITransformable
    {
        /// <summary>
        /// Sets transform and propagates delta to attached objects 
        /// </summary>
        public Transform2D Transform { get; set; }

        /// <summary>
        /// Changes transform and propagates delta to attached objects 
        /// </summary>
        public void ApplyTransform(in Transform2D deltaTransform);
    }
}