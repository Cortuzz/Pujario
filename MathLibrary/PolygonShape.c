#include "PolygonShape.h"
#include "CombinedShape.h"
#include "Utils.h"
#include <malloc.h>
#include <assert.h>
#include <math.h>

Vector2S Polygon2S_Shape_extremePointInDirection(const struct Shape* self, Vector2S v, float* outDistance) {
	Polygon2S polygon = (Polygon2S)self;
	assert(IsConvex_Polygon2S(polygon));
	register float curDist, farestDist = DotV2S(*polygon->points, v);
    Vector2S extreme = *polygon->points;

	for (size_t i = 1; i < polygon->count; i++) {
		if (farestDist < (curDist = DotV2S(polygon->points[i], v))) {
			farestDist = curDist;
			extreme = polygon->points[i];
		}
	}
	if (outDistance) *outDistance = farestDist;
	return extreme;
}

const struct _Line2SMap __Line2SMapZero = { 0 };

Polygon2S CreatePolygon2S(const Vector2S* points, int count) {
	struct _Polygon2S* polygon = (struct _Polygon2S*)malloc(sizeof(struct _Polygon2S) + sizeof(Vector2S) * count);
	polygon->shape.extremePointInDirection = Polygon2S_Shape_extremePointInDirection;
	polygon->points = (Vector2S*)(polygon + 1);
	memcpy(polygon->points, points, sizeof(Vector2S) * count);
	polygon->count = count;
	CalcCentroid_Polygon2S(polygon);
	
	polygon->edges = NULL;
	polygon->lineMap = __Line2SMapZero;

	return (Polygon2S)polygon;
}

Polygon2S CopyPolygon2S(Polygon2S polygon) {
	Polygon2S copy = CreatePolygon2S(polygon->points, polygon->count);
	size_t size;
	int linesCount = 0, laysCount = polygon->lineMap.count;
	Line2S* pLine;
	struct _Line2SMapLayer* pLay;

	if (polygon->edges) {
		size = sizeof(Segment2S) * polygon->count;
		copy->edges = (Segment2S*)malloc(size);
		memcpy(copy->edges, polygon->edges, size);
	}

	if (laysCount != 0) {
		for (int i = 0; i < polygon->lineMap.count; ++i)
			linesCount += polygon->lineMap.layers[i].count;

		copy->lineMap.count = laysCount;
		size = sizeof(struct _Line2SMapLayer) * laysCount + sizeof(Line2S) * linesCount;
		copy->lineMap.layers = (struct _Line2SMapLayer*)malloc(size);
		pLay = copy->lineMap.layers;
		pLine = (Line2S*)(copy->lineMap.layers + laysCount);
		memcpy(pLine, polygon->lineMap.layers + laysCount, sizeof(Line2S) * linesCount);

		for (int i = 0; i < laysCount; ++i, ++pLay) {
			pLay->lines = pLine;
			pLine += pLay->count;
		}
		
		size = sizeof(float) * (copy->lineMap.count + 1);
		copy->lineMap.corners = (float*)malloc(size);
		memcpy(copy->lineMap.corners, polygon->lineMap.corners, size);
	}

	return copy;
}

void DestructPolygon2S(Polygon2S polygon) { 
	if (polygon->edges) free(polygon->edges);
	if (polygon->lineMap.count > 0) {
		free(polygon->lineMap.corners);
		free(polygon->lineMap.layers);
	}
	free(polygon); 
}

BOOL IsConvex_Polygon2S(Polygon2S polygon) {
	if (polygon->count < 4) return TRUE;

	const Vector2S* points = polygon->points;
	Vector2S a = SubstractedV2S(points[0], points[polygon->count - 1]),
		b = SubstractedV2S(points[1], points[0]);
	float sign = a.x * b.y - a.y * b.x;

	for (size_t i = 2; i < polygon->count; ++i) {
		a = b;
		b = SubstractedV2S(points[i], points[i - 1]);
		if (sign * (a.x * b.y - a.y * b.x) < 0) return FALSE;
	}
	return TRUE;
}

Vector2S GetVertex_Polygon2S(Polygon2S polygon, int index) { return polygon->points[index]; }

Vector2S GetCentroid_Polygon2S(Polygon2S polygon) { return polygon->centroid; }

void CalcCentroid_Polygon2S(struct _Polygon2S* polygon)
{
	register Vector2S* cur = polygon->points;
	polygon->centroid.x = cur->x;
	polygon->centroid.y = cur++->y;
	register int count = polygon->count;
	for (size_t i = 1; i < count; ++i, ++cur) {
		polygon->centroid.x += cur->x;
		polygon->centroid.y += cur->y;
	}
	polygon->centroid.x /= count;
	polygon->centroid.y /= count;
}

void __OptimezePointsFor(const Polygon2S target, const Polygon2S intersector, Vector2S* pointsBuf, int* bufUsed)
{
	Segment2S* te = GetEdges_Polygon2S(target), * ie = GetEdges_Polygon2S(intersector), * ie1, * ie2;
	int beg = -1, end = -1, ipCount, bcount = 0;
	for (int i = 0; i < target->count; ++i)
	{
		if (BelongsToPolygon2S(intersector, target->points[i])) {
			if (beg < 0) { beg = i; end = i; }
			else end = i;
		}
		else {
			if ((ipCount = end - beg) > 2) {
				ie1 = te + beg, ie2 = te + end + 1;
				// TODO : use function for that loops
				for (int j = 0; j < intersector->count; ++j) {
					if (IntersectSegment2S(ie + j, ie1, pointsBuf + bcount)) {
						++bcount; break;
					}
				}
				for (int j = 0; j < intersector->count; ++j) {
					if (IntersectSegment2S(ie + j, ie2, pointsBuf + bcount)) {
						++bcount; break;
					}
				}
			}
			else if (ipCount > 0 )
				for (int j = beg; j <= end; ++j) 
					pointsBuf[bcount++] = target->points[j];

			pointsBuf[bcount++] = target->points[i];
			beg = -1; end = -1;
		}
	}
	if (end - beg > 0)
		for (int j = beg; j <= end; ++j)
			pointsBuf[bcount++] = target->points[j];
	*bufUsed = bcount;
}

/// <summary>
/// Tries to reduse points amount in 2 polygons combination, 
/// which are collide and reallocates/deletes them if needed
/// </summary>
/// <param name="p1">could be set to NULL</param>
/// <param name="p2">could be set to NULL</param>
void __OptimizeIntersection_Polygon2S(Polygon2S* p1, Polygon2S* p2)
{
	int pcount1 = (*p1)->count, pcount2 = (*p2)->count;
	Vector2S* pointsBuf = (Vector2S*)malloc(sizeof(Vector2S) * max(pcount1, pcount2));
	int size = 0;

	__OptimezePointsFor(*p1, *p2, pointsBuf, &size);
	if (size != pcount1) {
		DestructPolygon2S(*p1);
		if (size == 0) {
			*p1 = NULL; goto out;
		}
		*p1 = CreatePolygon2S(pointsBuf, size);
	}
	
	__OptimezePointsFor(*p2, *p1, pointsBuf, &size);
	if (size != pcount2) {
		DestructPolygon2S(*p2);
		if (size == 0) {
			*p2 = NULL; goto out;
		}
		*p2 = CreatePolygon2S(pointsBuf, size);
	}
	
out:
	free(pointsBuf);
}

PolygonSet2S CreateOptimizedPolygonSet2S(const Polygon2S* polygons, int count)
{
	Polygon2S* tempPolygons = (Polygon2S*)malloc(sizeof(Polygon2S) * count);
	for (int i = 0; i < count; ++i)
		tempPolygons[i] = CopyPolygon2S(polygons[i]);

	int pCount = 0;
	for (int i = 0; i < count; ++i) {
		for (int j = i + 1; j < count; ++j) {
			if (tempPolygons[j] == NULL || tempPolygons[i] == NULL) continue;
			__OptimizeIntersection_Polygon2S(tempPolygons + i, tempPolygons + j);
		}
		if (tempPolygons[i]) ++pCount;
	}

	union _PolygonSet2S* pset = (union _PolygonSet2S*)malloc(
		sizeof(union _PolygonSet2S) + sizeof(Polygon2S) * pCount);
	pset->polygons = (Polygon2S*)(pset + 1);
	pset->count = pCount;
	pset->isDataOwner = TRUE;
	for (int i = 0, j = 0; i < pCount; ++i) {
		if (tempPolygons[i])
			pset->polygons[j++] = tempPolygons[i];
	}
		
	free(tempPolygons);
	return pset;
}

PolygonSet2S CreatePolygonSet2S(Polygon2S* polygons, int count) {
	union _PolygonSet2S* pset = CreateCombinedShape((struct Shape**)polygons, count);
	pset->isDataOwner = FALSE;
	return pset;
}

void DestructPolygonSet2S(PolygonSet2S pset) {
	if (pset->isDataOwner) {
		for (int i = 0; i < pset->count; ++i)
			DestructPolygon2S(pset->polygons[i]);
	}
	free(pset);
}

Segment2S* GetEdges_Polygon2S(Polygon2S polygon) {
	if (!polygon->edges) {
		register int count = polygon->count;
		polygon->edges = (Segment2S*)malloc(sizeof(Segment2S) * count);

		ConstructSegment2S(polygon->edges, polygon->points[count - 1], polygon->points[0]);
		for (int i = 1; i < count; ++i)
			ConstructSegment2S(polygon->edges + i, polygon->points[i - 1], polygon->points[i]);
	}

	return polygon->edges;
}

struct __Line2Sp {
	Line2S line;
	float cy;
};

int __cmpl2sp(const void* pel1, const void* pel2) { 
	float el1 = ((struct __Line2Sp*)pel1)->cy, el2 = ((struct __Line2Sp*)pel2)->cy;
	return (el1 > el2) - (el1 < el2); 
}

// self intersections ignored
struct _Line2SMap* GetLineMap_Polygon2S(Polygon2S polygon) 
{
	if (polygon->lineMap.count == 0) {
		Segment2S* edges = GetEdges_Polygon2S(polygon);
		const count = polygon->count;
		int lCount = 0, layCount = count - 1;
		float cx, cy;

		float* corners = (float*)malloc(sizeof(float) * count);

		void* mapTempMem = malloc(sizeof(int) * layCount + sizeof(struct __Line2Sp) * count * count / 2);
		int* sizes = (int*)mapTempMem;
		struct __Line2Sp* linesBuffer = (struct __Line2Sp*)(sizes + layCount);

		for (int i = 0; i < count; ++i)
			corners[i] = polygon->points[i].x;
		qsort((void*)corners, count, sizeof(float), cmpf);

		for (size_t i = 0; i < layCount;) {
			if (corners[i] == corners[i + 1])
				memshift((void*)(corners + i), -(int)sizeof(int), sizeof(int) * (layCount-- - i + 1));
			else ++i;
		}
		
		for (int i = 0; i < layCount; ++i)
		{
			cx = (corners[i] + corners[i + 1]) / 2;
			sizes[i] = 0;
			for (int j = 0; j < count; ++j)
			{
				if (edges[j].range.infX >= cx || edges[j].range.supX <= cx) 
					continue;
				if (edges[j].range.infY > (cy = CalcYLine2S((Line2S*)(edges + j), cx)) ||
					cy > edges[j].range.supY)
					continue;
				linesBuffer[lCount].cy = cy;
				linesBuffer[lCount].line = edges[j].line;
				++lCount; ++sizes[i];
			}
			qsort((void*)(linesBuffer + lCount - sizes[i]), sizes[i], sizeof(struct __Line2Sp), __cmpl2sp);
		}

		struct _Line2SMapLayer* layers = (struct _Line2SMapLayer*)malloc(
			sizeof(struct _Line2SMapLayer) * layCount + sizeof(Line2S) * lCount);

		Line2S* lines = (Line2S*)(layers + layCount), * dCur = lines;
		struct __Line2Sp* bCur = linesBuffer;
		for (int i = 0; i < layCount; ++i) {
			layers[i].lines = dCur;
			layers[i].count = sizes[i];
			for (int j = 0; j < sizes[i]; ++j, ++bCur, ++dCur) {
				*dCur = bCur->line;
			}
		}

		free(mapTempMem);
		polygon->lineMap.corners = corners;
		polygon->lineMap.layers = layers;
		polygon->lineMap.count = layCount;
	}

	return &polygon->lineMap;
}

BOOL BelongsToPolygon2S(Polygon2S polygon, Vector2S point) 
{
	struct _Line2SMap* map = GetLineMap_Polygon2S(polygon);
	int layInd = map->count / 2, top = map->count, bot = 0;
	while (1) {
		if (map->corners[layInd] > point.x) 
			top = layInd - 1;
		else if (map->corners[layInd + 1] < point.x) 
			bot = layInd + 1;
		else break;
		if (bot > top) return FALSE;
		layInd = (top + bot) / 2;
	}

	struct _Line2SMapLayer* layer = map->layers + layInd;
	top = layer->count - 1, bot = 0;
	int lineInd;
	while (bot <= top)
	{
		lineInd = (top + bot) / 2;
		if (CalcYLine2S(layer->lines + lineInd, point.x) > point.y) {
			if (lineInd == 0) return FALSE;
			top = lineInd - 1;
		}
		else if (lineInd + 1 < layer->count)
			if (CalcYLine2S(layer->lines + lineInd + 1, point.x) < point.y)
				bot = lineInd + 1;
			else return lineInd % 2 == 0;
		else return FALSE;
	}

	return FALSE;
}