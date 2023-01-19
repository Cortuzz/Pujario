#pragma once
#define MAX_SIMPLEX_SIZE 255
#include <stdbool.h>
#include "MathBasics.h" 
#include "Shapes/Shape.h"

// 2D simplex for GJK
struct Simplex {
	uint8_t count;
	Vector2S points[3];
};

/// <param name="outDirection">current direction vector of simplex (not null)</param>
/// <returns> 1 if simplex proofs intersection, else 0 </returns>
bool ProcessSimplex(struct Simplex* simplex, Vector2S* outDirection);

bool _GJK(Shape shape1, Shape shape2, struct Simplex* simplex);
/// <summary>
/// 2D version of algorithm for determining collisions
/// </summary>
__declspec(dllexport) bool GJK(Shape shape1, Shape shape2);

struct Collision {
	bool areCollided;
	Vector2S direction;
	float depth;
};

struct ExtendedSimplex {
	uint8_t count;
	Vector2S points[MAX_SIMPLEX_SIZE];
};

void _EPA(Shape shape1, Shape shape2, struct ExtendedSimplex* simplex, float tolerance, struct Collision* out);
__declspec(dllexport) struct Collision EPA(Shape shape1, Shape shape2, float tolerance);
