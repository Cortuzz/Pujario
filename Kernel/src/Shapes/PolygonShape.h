#pragma once
#include <stdbool.h>
#include "Shapes/Shape.h"
#include "Utils.h"

struct BasePolygon2S {
	Vector2S* points;
	unsigned count;
};

struct _Polygon2SExtensionFields;

typedef struct _Polygon2S {
	struct BasePolygon2S;
	_Maybenull_ struct _Polygon2SExtensionFields* extensions;
}*Polygon2S;

typedef const struct _PolygonSet2S {
	Polygon2S* polygons;
	int count;
}*PolygonSet2S;

__declspec(dllexport) Polygon2S Polygon2S_Create(const Vector2S* points, unsigned count);
__declspec(dllexport) void Polygon2S_Delete(Polygon2S polygon);

// Copies only polygons which were created by "CreatePolygon2SShape", "CreatePolygon2SShape" function
__declspec(dllexport) Polygon2S Polygon2S_Copy(Polygon2S polygon);

inline unsigned ExtremePointIndInDir(
	const Vector2S* points,
	unsigned count,
	Vector2S dir,
	_Out_opt_ float* outDistance)
{
	float curDist, farestDist = DotV2S(*points, dir);
	unsigned extremeInd = 0;

	for (unsigned i = 1; i < count; ++i) {
		if (farestDist < (curDist = DotV2S(points[i], dir))) {
			farestDist = curDist;
			extremeInd = i;
		}
	}
	if (outDistance) *outDistance = farestDist;
	return extremeInd;
}

inline float Polygon2S_FindMinX(const struct BasePolygon2S* pol) {
	if (pol->count < 1) return 0;
	float temp = pol->points[0].x;
	for (unsigned i = 0; i < pol->count; ++i) {
		if (temp > pol->points[i].x) temp = pol->points[i].x;
	}
	return temp;
}
inline float Polygon2S_FindMaxX(const struct BasePolygon2S* pol) {
	if (pol->count < 1) return 0;
	float temp = pol->points[0].x;
	for (unsigned i = 0; i < pol->count; ++i) {
		if (temp < pol->points[i].x) temp = pol->points[i].x;
	}
	return temp;
}
inline float Polygon2S_FindMinY(const struct BasePolygon2S* pol) {
	if (pol->count < 1) return 0;
	float temp = pol->points[0].y;
	for (unsigned i = 0; i < pol->count; ++i) {
		if (temp > pol->points[i].y) temp = pol->points[i].y;
	}
	return temp;
}
inline float Polygon2S_FindMaxY(const struct BasePolygon2S* pol) {
	if (pol->count < 1) return 0;
	float temp = pol->points[0].y;
	for (unsigned i = 0; i < pol->count; ++i) {
		if (temp < pol->points[i].y) temp = pol->points[i].y;
	}
	return temp;
}
void Polygon2S_CalcAABB(const struct BasePolygon2S* pol, _Out_ struct Bounds2S* aabb);
__declspec(dllexport) struct Rectangle2S Polygon2S_CalcBoundRect(const struct BasePolygon2S* pol);

void Polygon2S_ProjectOnAxis(const struct BasePolygon2S* polygon, UnitVector2S axis, _Out_ struct Range* result);

/// <summary>
/// Ensures cloclwise order of convex polygon's points 
/// </summary>
__declspec(dllexport) void Polygon2S_OrderPoints(struct BasePolygon2S* pol);

/// <summary>
/// Put polygons bottom left boundary corner into {0,0}
/// </summary>
__declspec(dllexport) void Polygon2S_Normalize(struct BasePolygon2S* polygon);

__declspec(dllexport) bool Polygon2S_IsConvex(const struct BasePolygon2S* polygon);

/// <summary>
/// Checks if convex polygon's points are cloclwise ordered 
/// </summary>
__declspec(dllexport) bool Polygon2S_IsOrdered(const struct BasePolygon2S* pol);

/// <summary>
/// Checks if polygon has bottom left boundary corner in {0,0}
/// </summary>
__declspec(dllexport) bool Polygon2S_IsNormalized(const struct BasePolygon2S* polygon);

// creates edges of returns if already exist
Segment2S* Polygon2S_GetEdges(const Polygon2S polygon);

Vector2S* Polygon2S_GetNormals(const Polygon2S polygon);

// creates map or returns if already exists
struct Line2SMap* Polygon2S_GetLineMap(const Polygon2S polygon);

__declspec(dllexport) Vector2S Polygon2S_GetMassCenter(const Polygon2S pol);

// Uses lineMap
__declspec(dllexport) bool BelongsToPolygon2S(const Polygon2S polygon, Vector2S point);

inline void Polygon2S_ApplyEvTransform(struct BasePolygon2S* polygon, const struct EvaluatedTransform2S* transform) {
	for (unsigned i = 0; i < polygon->count; ++i) TransformPoint2S(polygon->points + i, transform);
}
__declspec(dllexport) void Polygon2S_ApplyTransform(struct BasePolygon2S* polygon, const struct Transform2S* transform);

enum TangentType {
	TANGENT_UP = 0x00,
	TANGENT_DOWN = 0x11,
	TANGENT_UP_DOWN = 0x01,
	TANGENT_DOWN_UP = 0x10
};

/// <summary>
/// Finds tangent between 2 nonintersecting convex polygon
/// </summary>
/// <param name="type">type of tangent to find</param>
/// <param name="firstIndex">pol1's index of tangent point</param>
/// <param name="secondIndex">pol2's index of tangent point</param>
void FindTangent(
	const struct BasePolygon2S* pol1,
	const struct BasePolygon2S* pol2,
	enum TangentType type,
	_Out_ unsigned* firstIndex,
	_Out_ unsigned* secondIndex);

/// <summary>
/// Merges 2 convex polygons into 1 minimal convex hull
/// </summary>
/// <param name="pol1">An convex polygon</param>
/// <param name="pol2">Another convex polygon</param>
/// <param name="outHull">Polygon with necessary space to contain points</param>
void MergeToConvexHull(
	const struct BasePolygon2S* pol1,
	const struct BasePolygon2S* pol2,
	_Out_writes_(pol1->count + pol2->count) Vector2S* hullPoints,
	_Out_ unsigned* hullCount);

/// <summary>
/// Constructs minimum convex null. 'ConstructConvexHullUsingBuf' is preffered for medium and huge points amount;
/// </summary>
/// <param name="points">Set of points to wrap into hull</param>
/// <param name="count">Count of points</param>
void ConstructConvexHull(
	const Vector2S* points, 
	unsigned count,
	_Out_writes_(count) Vector2S* hullPoints,
	_Out_ unsigned* hullCount);

/// <summary>
/// Constructs minimum convex null. 'ConstructConvexHull' is preffered for tiny points amount;
/// </summary>
/// <param name="points">Set of points to wrap into hull</param>
/// <param name="count">Count of points</param>
void ConstructConvexHullUsingBuf(
	const Vector2S* points,
	unsigned count,
	_Writable_elements_(count) _Readable_elements_(count) Vector2S* buffer,
	_Out_writes_(count) Vector2S* hullPoints,
	_Out_ unsigned* hullCount);

__declspec(dllexport) Polygon2S CreateConvexHull2S(const Vector2S* points, unsigned count);

/*
* TODO :
*	Polygon set creation needs refactoring 
*/

/// <summary>
/// Creates new polygon set, and tries to optimize polygons intersections;
/// </summary>
__declspec(dllexport) PolygonSet2S CreateOptimizedPolygonSet2S(const Polygon2S* polygons, unsigned count);
__declspec(dllexport) void DestructPolygonSet2S(PolygonSet2S pset);

typedef const union _Polygon2SShape* Polygon2SShape;
typedef Vector2S(*GJK_Polygon2S_SupportFunc)(Polygon2SShape shape, Vector2S direction, float* outDistance);
typedef void(*Polygon2SShapeTransformer)(Polygon2SShape shape, const struct EvaluatedTransform2S* transform);

union _Polygon2SShape {
	struct _TransformableShape asTransformableShape;
	struct {
		Polygon2S primitive;
		GJK_Polygon2S_SupportFunc support;
		Polygon2SShapeTransformer setTransform;
	};
};

// Creates new polygon instance
__declspec(dllexport) Polygon2SShape Polygon2SShape_Create(const Vector2S* points, unsigned count);
__declspec(dllexport) Polygon2SShape Polygon2SShape_CreateFromPolygon(const struct BasePolygon2S* polygon);

// Frees memory of polygon created using "ConstructPolygon"
__declspec(dllexport) void Polygon2SShape_Delete(Polygon2SShape shape);

typedef Polygon2SShape StretchingPolygon2SShape;

__declspec(dllexport) StretchingPolygon2SShape StretchingPolygon2SShape_Create(const Vector2S* points, unsigned count);
__declspec(dllexport) StretchingPolygon2SShape StretchingPolygon2SShape_CreateFromPolygon(const struct BasePolygon2S* polygon);

__declspec(dllexport) void StretchingPolygon2SShape_Delete(StretchingPolygon2SShape shape);

__declspec(dllexport) struct BasePolygon2S StretchingPolygon2SShape_GetStretchedPolygon(StretchingPolygon2SShape shape);