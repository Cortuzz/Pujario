#pragma once
#include <Windows.h>
#include "Vectors.h"

// 2D shape for GJK
struct Shape {
    /// <summary Calculates extreme point </summary>
    /// <returns>point and distance to it if param outDistance is not NULL</returns>
    Vector2S(*extremePointInDirection)(const struct Shape* self, Vector2S direction, float* outDistance);
};

// 2D simplex for GJK
typedef struct Simplex
{
    uint8_t count;
    Vector2S points[3];
} Simplex;

/// <param name="outDirection">current direction vector of simplex (not null)</param>
/// <returns> 1 if simplex proofs intersection, else 0 </returns>
BOOLEAN ProcessSimplex(Simplex* simplex, Vector2S* outDirection);

Vector2S TripleProductV2S(const Vector2S* v1, const Vector2S* v2, const Vector2S* v3);
void TripleProductToV2S(const Vector2S* v1, const Vector2S* v2, const Vector2S* v3, Vector2S* outV);

Vector2S Support(const struct Shape* shape1, const struct Shape* shape2, Vector2S direction);

/// <summary>
/// 2D version of algorithm for determining collisions
/// </summary>
__declspec(dllexport) BOOL GJK(const struct Shape* shape1, const struct Shape* shape2);

struct Collision {
    BOOL areCollided;
    Vector2S direction;
    float depth;
};

__declspec(dllexport) struct Collision EPA(const struct Shape* shape1, const struct Shape* shape2, float tolerance);
