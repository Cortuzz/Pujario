#include "Shapes/PolygonShape.h"

static void s_OptimezePointsFor(const Polygon2S target, const Polygon2S intersector, Vector2S* pointsBuf, int* bufUsed)
{
	Segment2S* te = Polygon2S_GetEdges(target), * ie = Polygon2S_GetEdges(intersector), * ie1, * ie2;
	int beg = -1, end = -1, ipCount, bcount = 0;
	for (unsigned i = 0; i < target->count; ++i)
	{
		if (BelongsToPolygon2S(intersector, target->points[i])) {
			if (beg < 0) { beg = i; end = i; }
			else end = i;
		}
		else {
			if ((ipCount = end - beg) > 2) {
				ie1 = te + beg, ie2 = te + end + 1;
				for (unsigned j = 0; j < intersector->count; ++j) {
					if (IntersectSegment2S(ie + j, ie1, pointsBuf + bcount)) {
						++bcount; break;
					}
				}
				for (unsigned j = 0; j < intersector->count; ++j) {
					if (IntersectSegment2S(ie + j, ie2, pointsBuf + bcount)) {
						++bcount; break;
					}
				}
			}
			else if (ipCount > 0)
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
static void s_OptimizeIntersection_Polygon2S(Polygon2S* p1, Polygon2S* p2)
{
	int pcount1 = (*p1)->count, pcount2 = (*p2)->count;
	Vector2S* pointsBuf = (Vector2S*)malloc(sizeof(Vector2S) * max(pcount1, pcount2));
	int size = 0;

	s_OptimezePointsFor(*p1, *p2, pointsBuf, &size);
	if (size != pcount1) {
		Polygon2SShape_Delete(*p1);
		if (size == 0) {
			*p1 = NULL; goto out;
		}
		*p1 = Polygon2SShape_Create(pointsBuf, size);
	}

	s_OptimezePointsFor(*p2, *p1, pointsBuf, &size);
	if (size != pcount2) {
		Polygon2SShape_Delete(*p2);
		if (size == 0) {
			*p2 = NULL; goto out;
		}
		*p2 = Polygon2SShape_Create(pointsBuf, size);
	}

out:
	free(pointsBuf);
}

PolygonSet2S CreateOptimizedPolygonSet2S(const Polygon2S* polygons, unsigned count)
{
	Polygon2S* tempPolygons = (Polygon2S*)malloc(sizeof(Polygon2S) * count);
	for (int i = 0; i < count; ++i)
		tempPolygons[i] = Polygon2S_Copy(polygons[i]);

	int pCount = 0;
	for (int i = 0; i < count; ++i) {
		for (int j = i + 1; j < count; ++j) {
			if (tempPolygons[j] == NULL || tempPolygons[i] == NULL) continue;
			s_OptimizeIntersection_Polygon2S(tempPolygons + i, tempPolygons + j);
		}
		if (tempPolygons[i]) ++pCount;
	}

	struct _PolygonSet2S* pset = (struct _PolygonSet2S*)malloc(
		sizeof(struct _PolygonSet2S) + sizeof(Polygon2S) * pCount);
	pset->polygons = (Polygon2S*)(pset + 1);
	pset->count = pCount;
	for (int i = 0, j = 0; i < pCount; ++i) {
		if (tempPolygons[i])
			pset->polygons[j++] = tempPolygons[i];
	}

	free(tempPolygons);
	return pset;
}

void DestructPolygonSet2S(PolygonSet2S pset) {
	for (int i = 0; i < pset->count; ++i)
		Polygon2SShape_Delete(pset->polygons[i]);
	free(pset);
}
