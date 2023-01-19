#include <malloc.h>
#include <assert.h>
#include <math.h>
#include <memory.h>
#include "Shapes/PolygonShape.h"
#include "PolygonExtensionFields.h"
#include "Utils.h"

#pragma region Polygon2S
static inline void s_ConstructBasePolygon2S(struct BasePolygon2S* polygon, Vector2S* pointsBuffer, const Vector2S* points, unsigned count) {
	polygon->points = pointsBuffer;
	polygon->count = count;
	memcpy(polygon->points, points, sizeof(Vector2S) * count);
}

static inline void s_ConstructPolygon2S(Polygon2S polygon, Vector2S* pointsBuffer, const Vector2S* points, unsigned count) {
	s_ConstructBasePolygon2S((struct BasePolygon2S*)polygon, pointsBuffer, points, count);
	polygon->extensions = NULL;
}

Polygon2S Polygon2S_Create(const Vector2S* points, unsigned count) {
	Polygon2S polygon = (Polygon2S)malloc(sizeof(struct _Polygon2S) + sizeof(Vector2S) * count);
	s_ConstructPolygon2S(polygon, (Vector2S*)(polygon + 1), points, count);
	return polygon;
}

void Polygon2S_Delete(Polygon2S polygon) {
	Polygon2S_FreeExtensionFields(polygon);
	free(polygon);
}

Polygon2S Polygon2S_Copy(Polygon2S polygon) {
	Polygon2S copy = Polygon2S_Create(polygon->points, polygon->count);
	copy->extensions = Polygon2S_CopyExtensionFields(polygon);
	return copy;
}

Polygon2S CreateConvexHull2S(const Vector2S* points, unsigned count) {
	Polygon2S polygon = (Polygon2S)malloc(sizeof(struct _Polygon2S) + sizeof(Vector2S) * count);
	s_ConstructPolygon2S(polygon, (Vector2S*)(polygon + 1), NULL, 0);
	if (count > 10) {
		void* buffer = malloc(count * sizeof(Vector2S));
		ConstructConvexHullUsingBuf(points, count, (Vector2S*)buffer, polygon->points, &polygon->count);
		free(buffer);
	}
	else ConstructConvexHull(points, count, polygon->points, &polygon->count);
	return polygon;
}

bool Polygon2S_IsConvex(const struct BasePolygon2S* polygon) {
	if (polygon->count < 4) return true;

	const Vector2S* points = polygon->points;
	Vector2S a = SubstractedV2S(points[0], points[polygon->count - 1]),
		b = SubstractedV2S(points[1], points[0]);
	float sign = a.x * b.y - a.y * b.x;

	for (unsigned i = 2; i < polygon->count; ++i) {
		a = b;
		b = SubstractedV2S(points[i], points[i - 1]);
		if (sign * (a.x * b.y - a.y * b.x) < 0) return false;
	}
	return true;
}

bool Polygon2S_IsOrdered(const struct BasePolygon2S* pol) {
	unsigned i = UINT_MAX;
	Vector2S vec1, vec2, * points = pol->points;
	do {
		SubstractToV2S(points[++i % pol->count], points[(i + 1) % pol->count], &vec1);
		SubstractToV2S(points[(i + 1) % pol->count], points[(i + 2) % pol->count], &vec2);
	} while (Cross2S(vec1, vec2) == 0);
	return Cross2S(vec1, vec2) < 0;
}

void Polygon2S_OrderPoints(struct BasePolygon2S* pol) {
	assert(Polygon2S_IsConvex(pol) && "Polygon must be convex");
	if (Polygon2S_IsOrdered(pol)) return;
	Vector2S* points = pol->points, vec;
	for (unsigned i = 0; i < pol->count / 2; ++i) {
		vec = points[i];
		points[i] = points[pol->count - i - 1];
		points[pol->count - i - 1] = vec;
	}
}

void Polygon2S_Normalize(struct BasePolygon2S* polygon) {
	if (polygon->count < 3) return;
	Vector2S vec = {
		.x = Polygon2S_FindMinX(polygon), .y = Polygon2S_FindMinY(polygon)
	};
	for (unsigned i = 0; i < polygon->count; ++i) SubstractV2S(polygon->points + i, vec);
}

bool Polygon2S_IsNormalized(const struct BasePolygon2S* polygon) {
	return Polygon2S_FindMinX(polygon) == 0 && Polygon2S_FindMinY(polygon) == 0;
}

void Polygon2S_ApplyTransform(struct BasePolygon2S* polygon, const struct Transform2S* transform) {
	struct EvaluatedTransform2S evtr;
	EvaluateTransform2S(transform, &evtr);
	for (unsigned i = 0; i < polygon->count; ++i) TransformPoint2S(polygon->points + i, &evtr);
}
#pragma endregion

#pragma region Polygon2SShape
struct s_Polygon2SShape {
	union _Polygon2SShape;
	struct _Polygon2S polygon;
	// [basePoints][transformedPoints] = [Vector2S * polygon.count * 2]
	Vector2S pointsBuffer[];
};

static Vector2S s_GJK_Polygon2S_SupportFunc(Polygon2SShape shape, Vector2S v, _Out_opt_ float* outDistance) {
	struct s_Polygon2SShape* self = (struct s_Polygon2SShape*)shape;
	assert(Polygon2S_IsConvex((struct BasePolygon2S*)self->primitive));
	return self->polygon.points[
		ExtremePointIndInDir(self->polygon.points, self->polygon.count, v, outDistance)
	];
}

static void s_Polygon2SShapeTransformer(Polygon2SShape shape, const struct EvaluatedTransform2S* transform) {
	struct s_Polygon2SShape* self = (struct s_Polygon2SShape*)shape;
	int count = self->polygon.count;
	Vector2S* points = self->polygon.points;

	memcpy(points, self->pointsBuffer, sizeof(struct _Vector2S) * count);
	Polygon2S_FreeExtensionFields(&self->polygon);

	for (int i = 0; i < count; ++i) TransformPoint2S(points + i, transform);
}

Polygon2SShape Polygon2SShape_Create(const Vector2S* points, unsigned count) {
	struct s_Polygon2SShape* shape = (struct s_Polygon2SShape*)malloc(
		sizeof(struct s_Polygon2SShape) + (sizeof(Vector2S) * count) * 2);

	shape->support = s_GJK_Polygon2S_SupportFunc;
	shape->setTransform = s_Polygon2SShapeTransformer;
	shape->primitive = &shape->polygon;

	memcpy(shape->pointsBuffer, points, sizeof(Vector2S) * count);
	s_ConstructPolygon2S(&shape->polygon, &shape->pointsBuffer[count], points, count);

	return (Polygon2SShape)shape;
}

Polygon2SShape Polygon2SShape_CreateFromPolygon(const struct BasePolygon2S* polygon) {
	return Polygon2SShape_Create(polygon->points, polygon->count);
}

void Polygon2SShape_Delete(Polygon2SShape shape) {
	Polygon2S_FreeExtensionFields(shape->primitive);
	free((void*)shape);
}
#pragma endregion

#pragma region StretchingPolygon2SShape
struct s_StretchingPolygon2SShape {
	union _Polygon2SShape;
	struct _Polygon2S polygon, stretchResult;
	Vector2S* prevTransformedPoints;
	/*
	* [basePoints][transformedPoints1][transformedPoints2][stretchResultPoints] = [Vector2S * polygon.count * 3][Vector2S * stretchResult.count]
	* polygon.count <= stretchResult.count <= (polygon.count * 2)
	* transformedPoints[1,2] - data of points of currant and previous transform
	*/
	Vector2S pointsBuffer[];
};

static Vector2S s_GJK_StretchingPolygon2S_SupportFunc(Polygon2SShape shape, Vector2S v, float* outDistance) {
	struct s_StretchingPolygon2SShape* self = (struct s_StretchingPolygon2SShape*)shape;
	return self->stretchResult.points[
		ExtremePointIndInDir(self->stretchResult.points, self->stretchResult.count, v, outDistance)
	];
}

static void s_StretchingPolygon2SShapeTransformer(Polygon2SShape shape, const struct EvaluatedTransform2S* transform)
{
	struct s_StretchingPolygon2SShape* self = (struct s_StretchingPolygon2SShape*)shape;
	const int count = self->polygon.count;
	Vector2S* points = self->prevTransformedPoints;

	memcpy(points, self->pointsBuffer, sizeof(struct _Vector2S) * count);
	for (int i = 0; i < count; ++i) TransformPoint2S(points + i, transform);

	self->prevTransformedPoints = self->polygon.points;
	self->polygon.points = points;
	Polygon2S_FreeExtensionFields(&self->polygon);

	struct BasePolygon2S prevPolygon = {
		self->prevTransformedPoints, count
	};
	MergeToConvexHull((struct BasePolygon2S*)&self->polygon, &prevPolygon, self->stretchResult.points, &self->stretchResult.count);
}

StretchingPolygon2SShape StretchingPolygon2SShape_Create(const Vector2S* points, unsigned count) {
	struct s_StretchingPolygon2SShape* shape = (struct s_StretchingPolygon2SShape*)malloc(
		sizeof(struct s_StretchingPolygon2SShape) + (sizeof(Vector2S) * count) * 5);

	shape->support = s_GJK_StretchingPolygon2S_SupportFunc;
	shape->setTransform = s_StretchingPolygon2SShapeTransformer;
	shape->primitive = &shape->polygon;

	memcpy(shape->pointsBuffer, points, sizeof(Vector2S) * count);
	shape->prevTransformedPoints = &shape->pointsBuffer[count];
	memcpy(shape->prevTransformedPoints, points, sizeof(Vector2S) * count);
	s_ConstructPolygon2S(shape->primitive, &shape->pointsBuffer[count * 2], points, count);
	s_ConstructPolygon2S(&shape->stretchResult, &shape->pointsBuffer[count * 3], points, 0);

	return (Polygon2SShape)shape;
}

StretchingPolygon2SShape StretchingPolygon2SShape_CreateFromPolygon(const struct BasePolygon2S* polygon) {
	return StretchingPolygon2SShape_Create(polygon->points, polygon->count);
}

void StretchingPolygon2SShape_Delete(StretchingPolygon2SShape shape) {
	Polygon2S_FreeExtensionFields(shape->primitive);
	Polygon2S_FreeExtensionFields(&((struct s_StretchingPolygon2SShape*)shape)->stretchResult);
	free((void*)shape);
}

struct BasePolygon2S StretchingPolygon2SShape_GetStretchedPolygon(StretchingPolygon2SShape shape) {
	return *(struct BasePolygon2S*)&((struct s_StretchingPolygon2SShape*)shape)->stretchResult;
}
#pragma endregion

#pragma region AABB
void Polygon2S_CalcAABB(const struct BasePolygon2S* pol, _Out_ struct Bounds2S* aabb) {
	aabb->x.inf = Polygon2S_FindMinX(pol);
	aabb->x.sup = Polygon2S_FindMaxX(pol);
	aabb->y.inf = Polygon2S_FindMinY(pol);
	aabb->y.sup = Polygon2S_FindMaxY(pol);
}

struct Rectangle2S Polygon2S_CalcBoundRect(const struct BasePolygon2S* pol) {
	struct Rectangle2S rect = {
		.x = Polygon2S_FindMinX(pol),
		.y = Polygon2S_FindMinY(pol),
		.width = Polygon2S_FindMaxX(pol) - rect.x,
		.height = Polygon2S_FindMaxY(pol) - rect.y
	};
	return rect;
}
#pragma endregion 