#pragma once 
#include <stdlib.h>
#include "MathBasics.h"
#include "utils/WorkerContainer.h"

extern const _CoreCrtNonSecureSearchSortCompareFunction cmpf; 

static inline unsigned cirind(int ind, unsigned mod) { return ind < 0 ? mod - (-ind) % mod : ind % mod; }

// shifts data in buffer of "size" in bytes
void memshift(void* data, int shift, size_t size);
void cirmemshift(void* data, int shift, size_t size);

struct Transform2S {
	Vector2S position, scale, /*relative rotation*/origin;
	float rotation;
};

struct EvaluatedRotation2S {
	float sin, cos;
};

struct EvaluatedTransform2S {
	Vector2S pivot, scale, rotationArm;
	struct EvaluatedRotation2S rotation;
};

// Computes intermediates for points transformation;
void EvaluateTransform2S(const struct Transform2S* transform, struct EvaluatedTransform2S* out);

static inline void TransformPoint2S(Vector2S* point, const struct EvaluatedTransform2S* transform) {
	point->x = point->x * transform->scale.x + transform->rotationArm.x;
	point->y = point->y * transform->scale.y + transform->rotationArm.y;

	float x = point->x;
	point->x = point->x * transform->rotation.cos + point->y * transform->rotation.sin;
	point->y = point->y * transform->rotation.cos - x * transform->rotation.sin;

	point->x += transform->pivot.x;
	point->y += transform->pivot.y;
}

// There is problem with comparing collinear vectors 
void SortClockwisePoints2S(Vector2S* points, unsigned count);
_declspec(dllexport) void SortClockwiseOriginPoints2S(Vector2S* points, unsigned count, Vector2S origin);