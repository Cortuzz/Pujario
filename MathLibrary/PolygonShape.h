#pragma once
#include "GJK.h"
#include "LinearEq.h"
#include "CombinedShape.h"

struct _Line2SMap {
	struct _Line2SMapLayer {
		Line2S* lines;
		int count;
	}* layers;
	float* corners; // size : count + 1; conners[0] <= layer[0] <= conners[1] 
	int count;
};

typedef struct _Polygon2S {
	struct Shape shape;
	Vector2S* points;
	int /*points*/count;
	Vector2S centroid;
	Segment2S* edges; // NULL by default
	struct _Line2SMap lineMap; // {0} by default 
}* Polygon2S;

typedef const union _PolygonSet2S {
	struct {
		struct Shape shape;
		Polygon2S* polygons;
		int count;
		BOOLEAN isDataOwner;
	};
	struct _CombinedShape asCombinedShape;
}* PolygonSet2S;

// Creates new polygon instance
__declspec(dllexport) Polygon2S CreatePolygon2S(const Vector2S* points, int count);

__declspec(dllexport) Polygon2S CopyPolygon2S(Polygon2S polygon, Vector2S* points, int count);

// Frees memory of polygon created using "ConstructPolygon"
__declspec(dllexport) void DestructPolygon2S(Polygon2S polygon);

__declspec(dllexport) BOOL IsConvex_Polygon2S(const Polygon2S polygon);

__declspec(dllexport) Vector2S GetVertex_Polygon2S(Polygon2S polygon, int index);

__declspec(dllexport) Vector2S GetCentroid_Polygon2S(Polygon2S polygon);

// creates edges of returns if already exist
__declspec(dllexport) Segment2S* GetEdges_Polygon2S(Polygon2S polygon);

// Uses lineMap
__declspec(dllexport) BOOL BelongsToPolygon2S(Polygon2S polygon, Vector2S point);

// creates map or returns if already exists
struct _Line2SMap* GetLineMap_Polygon2S(Polygon2S polygon);

void CalcCentroid_Polygon2S(struct _Polygon2S* polygon);

/// <summary>
/// Creates new polygon set, and tries to optimize polygons intersections;
/// </summary>
__declspec(dllexport) PolygonSet2S CreateOptimizedPolygonSet2S(const Polygon2S* polygons, int count);

__declspec(dllexport) PolygonSet2S CreatePolygonSet2S(Polygon2S* polygons, int count);
__declspec(dllexport) void DestructPolygonSet2S(PolygonSet2S pset);