/*
 * That file should contain declarations of all resourcse provided from 'Kernel' native library 
 * and stably ready for linking to managed part.
 */

using Microsoft.Xna.Framework;
using Pujario.Core.Kernel.Collisions;
using Pujario.Utils;
using System;
using System.Runtime.InteropServices;
using System.Runtime.InteropServices.Marshalling;
using static Pujario.Core.Kernel.Entry;

[assembly: System.Runtime.CompilerServices.DisableRuntimeMarshalling]
namespace Pujario.Core.Kernel;

// TODO : provide Circles 
internal static partial class Entry
{
    private const string LibraryName = "Kernel.dll";

    internal static partial class Shape
    {
        [LibraryImport(LibraryName, EntryPoint = "Shape_GetPrimitive")]
        internal static unsafe partial void* GetPrimitive(IntPtr shape);
        [LibraryImport(LibraryName, EntryPoint = "Shape_SetTransform")]
        internal static partial void SetTransform(IntPtr shape, in Transform2D transform);
    }

    internal static partial class Polygon2
    {
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_Create")]
        internal static partial IntPtr Create(Vector2[] points, uint count);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_Delete")]
        internal static partial void Delete(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_Copy")]
        internal static partial IntPtr Copy(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_CalcBoundRect")]
        internal static partial Rectangle CalcBoundRect(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_OrderPoints")]
        internal static partial void OrderPoints(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_Normalize")]
        internal static partial void Normalize(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_IsConvex")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool IsConvex(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_IsOrdered")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool IsOrdered(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_IsNormalized")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool IsNormalized(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_GetMassCenter")]
        internal static partial Vector2 GetMassCenter(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "BelongsToPolygon2S")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool BelongsToPolygon(IntPtr polygon, Vector2 point);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2S_ApplyTransform")]
        internal static partial void ApplyTransform(IntPtr polygon, in Transform2D transform);
        [LibraryImport(LibraryName, EntryPoint = "CreateConvexHull2S")]
        internal static partial IntPtr CreateConvexHull(Vector2[] points, uint count);
    }

    internal static partial class Circle
    {
        [LibraryImport(LibraryName, EntryPoint = "Circle_ApplyTransform")]
        internal static unsafe partial void ApplyTransform(Kernel.Circle* circle, in Transform2D transform);
    }

    internal static partial class Polygon2Shape
    {
        [LibraryImport(LibraryName, EntryPoint = "Polygon2SShape_Create")]
        internal static partial IntPtr Create(Vector2[] points, uint count);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2SShape_CreateFromPolygon")]
        internal static partial IntPtr CreateFromPolygon(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "Polygon2SShape_Delete")]
        internal static partial void Delete(IntPtr shape);
    }

    internal static partial class StretchingPolygon2Shape
    {
        [LibraryImport(LibraryName, EntryPoint = "StretchingPolygon2SShape_Create")]
        internal static partial IntPtr Create(Vector2[] points, uint count);
        [LibraryImport(LibraryName, EntryPoint = "StretchingPolygon2SShape_CreateFromPolygon")]
        internal static partial IntPtr CreateFromPolygon(IntPtr polygon);
        [LibraryImport(LibraryName, EntryPoint = "StretchingPolygon2SShape_Delete")]
        internal static partial void Delete(IntPtr shape);
        [LibraryImport(LibraryName, EntryPoint = "StretchingPolygon2SShape_GetStretchedPolygon")]
        [return: MarshalUsing(typeof(Polygon2Marshaller))]
        internal static partial Kernel.Polygon2 GetStretchedPolygon(IntPtr shape);
    }

    internal static partial class CircleShape
    {
        [LibraryImport(LibraryName, EntryPoint = "CircleShape_Create")]
        internal static partial IntPtr Create(Vector2 origin, float radius);
        [LibraryImport(LibraryName, EntryPoint = "CircleShape_Delete")]
        internal static partial void Delete(IntPtr shape);
    }

    internal static partial class StretchingCircleShape
    {
        [LibraryImport(LibraryName, EntryPoint = "StretchingCircleShape_Create")]
        internal static partial IntPtr Create(Vector2 origin, float radius);
        [LibraryImport(LibraryName, EntryPoint = "StretchingCircleShape_Delete")]
        internal static partial void Delete(IntPtr shape);
    }

    internal static partial class Collider2
    {
        [LibraryImport(LibraryName, EntryPoint = "Collider2S_Create")]
        internal static partial IntPtr Create(IntPtr[] shapes, uint count, [MarshalAs(UnmanagedType.I1)] bool preferBoundsCheck);
        [LibraryImport(LibraryName, EntryPoint = "Collider2S_Delete")]
        internal static partial void Delete(IntPtr collider);
        [LibraryImport(LibraryName, EntryPoint = "Collider2S_SetTransform")]
        internal static partial void SetTransform(IntPtr collider, in Transform2D transform);

        [LibraryImport(LibraryName, EntryPoint = "CheckCollision_Collider2S_Shape")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool CheckCollisionWithShape(IntPtr collider, IntPtr shape);
        [LibraryImport(LibraryName, EntryPoint = "CalcCollision_Collider2S_Shape")]
        internal static partial Collision CalcCollisionWithShape(IntPtr collider, IntPtr shape, float tolerance);
        [LibraryImport(LibraryName, EntryPoint = "Collider2S_SetTransform")]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool CheckCollision(IntPtr cl1, IntPtr cl2);
        [LibraryImport(LibraryName, EntryPoint = "CalcCollision_Collider2S")]
        internal static partial Collision CalcCollision(IntPtr cl1, IntPtr cl2, float tolerance);
    }

    internal static partial class Algorithms
    {
        [LibraryImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool GJK(IntPtr shape1, IntPtr shape2);
        [LibraryImport(LibraryName)]
        internal static partial Collision EPA(IntPtr shape1, IntPtr shape2, float tolerance );
        [LibraryImport(LibraryName)]
        [return: MarshalAs(UnmanagedType.I1)]
        internal static partial bool SAT(IntPtr polygon1, IntPtr polygon2);
    }
}

