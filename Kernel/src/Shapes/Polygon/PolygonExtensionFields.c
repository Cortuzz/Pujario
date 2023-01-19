#include <assert.h>
#include <memory.h>
#include <float.h>
#include "PolygonExtensionFields.h"

static int __cmpl2sp(const void* pel1, const void* pel2) {
	float el1 = ((struct __Line2Sp*)pel1)->cy, el2 = ((struct __Line2Sp*)pel2)->cy;
	return (el1 > el2) - (el1 < el2);
}

static const struct _Polygon2SExtensionFields s_EmptyExtensionFields = { {FLT_MAX, FLT_MAX}, NULL, NULL, NULL };

static inline _Ret_notnull_ struct _Polygon2SExtensionFields* EnsureExtentionsNotNull(const Polygon2S pol) {
	if (pol->extensions) return pol->extensions;
	pol->extensions = (struct _Polygon2SExtensionFields*)malloc(sizeof(struct _Polygon2SExtensionFields));
	*pol->extensions = s_EmptyExtensionFields;
	return pol->extensions;
}

void Polygon2S_FreeExtensionFields(_Post_invalid_ Polygon2S pol) {
	if (!pol->extensions) return;
	if (pol->extensions->edges)  free(pol->extensions->edges);
	if (pol->extensions->normals)  free(pol->extensions->normals);
	if (pol->extensions->lineMap)  free(pol->extensions->lineMap);
	free(pol->extensions);
	pol->extensions = NULL;
}

_Ret_maybenull_ struct  _Polygon2SExtensionFields* Polygon2S_CopyExtensionFields(const Polygon2S pol) {
	if (!pol->extensions) return NULL;
	size_t size = sizeof(struct _Polygon2SExtensionFields);
	struct _Polygon2SExtensionFields* copy = (struct _Polygon2SExtensionFields*)malloc(size);
	*copy = s_EmptyExtensionFields;

	copy->massCenter = pol->extensions->massCenter;

	if (pol->extensions->edges) {
		size = sizeof(Segment2S) * pol->count;
		copy->edges = (Segment2S*)malloc(size);
		memcpy(copy->edges, pol->extensions->edges, size);
	}

	if (pol->extensions->lineMap) {
		unsigned linesCount = 0, laysCount = pol->extensions->lineMap->count;
		for (unsigned i = 0; i < laysCount; ++i) linesCount += pol->extensions->lineMap->layers[i].count;

		size = sizeof(struct Line2SMap) + sizeof(float) * ((size_t)laysCount + 1) +
			sizeof(struct Line2SMapLayer) * laysCount + sizeof(Line2S) * linesCount;

		// TODO: test copying line map; ny wrode norm 
		copy->lineMap = (struct Line2SMap*)malloc(size);
		memcpy(copy->lineMap, pol->extensions->lineMap, size);

		copy->lineMap->corners = (float*)(copy->lineMap + 1);
		struct Line2SMapLayer* layers = (struct Line2SMapLayer*)(copy->lineMap->corners + (laysCount + 1));
		copy->lineMap->layers = layers;
		Line2S* lines = (Line2S*)(layers + laysCount);
		for (unsigned i = 0; i < laysCount; lines += layers[i++].count) {
			layers[i].lines = lines;
		}
	}

	if (pol->extensions->normals) {
		size = sizeof(Vector2S) * pol->count;
		pol->extensions->normals = (Vector2S*)malloc(size);
		memcpy(copy->normals, pol->extensions->normals, size);
	}

	return copy;
}

Segment2S* Polygon2S_GetEdges(const Polygon2S polygon) {
	struct _Polygon2SExtensionFields* ex = EnsureExtentionsNotNull(polygon);
	if (!ex->edges) {
		int count = polygon->count;
		ex->edges = (Segment2S*)malloc(sizeof(Segment2S) * count);

		ConstructSegment2S(ex->edges, polygon->points[count - 1], polygon->points[0]);
		for (int i = 1; i < count; ++i)
			ConstructSegment2S(ex->edges + i, polygon->points[i - 1], polygon->points[i]);
	}

	return ex->edges;
}

UnitVector2S* Polygon2S_GetNormals(const Polygon2S polygon) {
	struct _Polygon2SExtensionFields* ex = EnsureExtentionsNotNull(polygon);
	if (ex->normals) return ex->normals;
	ex->normals = (Vector2S*)calloc(polygon->count, sizeof(Vector2S));
	SubstractToV2S(*polygon->points, polygon->points[polygon->count - 1], ex->normals);
	Normal(*ex->normals, ex->normals);
	for (unsigned i = 1; i < polygon->count; ++i) {
		SubstractToV2S(polygon->points[i], polygon->points[i - 1], ex->normals + i);
		Normal(ex->normals[i], ex->normals + i);
		ToUnitV2S(ex->normals + i);
	}

	return ex->normals;
}

// memory layout : [Line2SMap][Line2SMap.corners][Line2SMap.layers][Line2SMap.layers[0...Line2SMap.count].lines]
// self intersections ignored
struct Line2SMap* Polygon2S_GetLineMap(const Polygon2S polygon)
{
	struct _Polygon2SExtensionFields* ex = EnsureExtentionsNotNull(polygon);
	if (!ex->lineMap) {
		Segment2S* edges = Polygon2S_GetEdges(polygon);
		const unsigned count = polygon->count;
		unsigned lCount = 0, layCount = count - 1;
		float cx, cy;

		void* mapTempMem = malloc(sizeof(float) * count + sizeof(unsigned) * layCount + sizeof(struct __Line2Sp) * count * count / 2);
		float* corners = (float*)mapTempMem;
		unsigned* sizes = (unsigned*)(corners + count);
		struct __Line2Sp* linesBuffer = (struct __Line2Sp*)(sizes + layCount);

		for (unsigned i = 0; i < count; ++i)
			corners[i] = polygon->points[i].x;
		qsort((void*)corners, count, sizeof(float), cmpf);

		for (unsigned i = 0; i < layCount;) {
			if (corners[i] == corners[i + 1])
				memshift((void*)(corners + i), -(int)sizeof(unsigned), sizeof(unsigned) * ((size_t)layCount-- - i + 1));
			else ++i;
		}

		for (unsigned i = 0; i < layCount; ++i)
		{
			cx = (corners[i] + corners[i + 1]) / 2;
			sizes[i] = 0;
			for (unsigned j = 0; j < count; ++j)
			{
				if (edges[j].range.x.inf >= cx || edges[j].range.x.sup <= cx)
					continue;
				if (edges[j].range.y.inf > (cy = CalcYLine2S((Line2S*)(edges + j), cx)) ||
					cy > edges[j].range.y.sup)
					continue;
				linesBuffer[lCount].cy = cy;
				linesBuffer[lCount].line = edges[j].line;
				++lCount; ++sizes[i];
			}
			qsort((void*)(linesBuffer + lCount - sizes[i]), sizes[i], sizeof(struct __Line2Sp), __cmpl2sp);
		}

		struct Line2SMap* map = (struct Line2SMap*)malloc(sizeof(struct Line2SMap) +
			sizeof(float) * (layCount + 1) + sizeof(struct Line2SMapLayer) * layCount + sizeof(Line2S) * lCount);

		map->corners = (float*)(map + 1);
		memcpy(map->corners, corners, sizeof(float) * (layCount + 1));

		struct Line2SMapLayer* layers = (struct Line2SMapLayer*)(map->corners + (layCount + 1));
		Line2S* lines = (Line2S*)(layers + layCount), * dCur = lines;
		struct __Line2Sp* bCur = linesBuffer;
		for (unsigned i = 0; i < layCount; ++i) {
			layers[i].lines = dCur;
			layers[i].count = sizes[i];
			for (unsigned j = 0; j < sizes[i]; ++j, ++bCur, ++dCur) {
				*dCur = bCur->line;
			}
		}

		map->layers = layers;
		map->count = layCount;
		ex->lineMap = map;

		free(mapTempMem);
	}

	return ex->lineMap;
}

Vector2S Polygon2S_GetMassCenter(const Polygon2S pol) {
	assert(pol->count > 0 && "Polygon must contain at least 1 point");
	struct _Polygon2SExtensionFields* ex = EnsureExtentionsNotNull(pol);

	if (ex->massCenter.x != FLT_MAX && ex->massCenter.y != FLT_MAX) return ex->massCenter;

	if (pol->count < 3) {
		if (pol->count == 1)
			ex->massCenter = *pol->points;
		else {
			AddToV2S(pol->points[0], pol->points[1], &ex->massCenter);
			ex->massCenter.x /= 2, ex->massCenter.y /= 2;
		}
		return ex->massCenter;
	}

	const int trianglesCount = pol->count - 2;
	float square = .0f, curSquare;
	Vector2S massPoint = Zero_Vector2S, vec1, vec2;
	SubstractToV2S(pol->points[0], pol->points[1], &vec2);

	for (int i = 0; i < trianglesCount; ++i)
	{
		vec1 = vec2;
		SubstractToV2S(pol->points[0], pol->points[i + 2], &vec2);
		curSquare = Cross2S(vec1, vec2) / 2;

		vec1 = pol->points[0];
		AddV2S(&vec1, pol->points[i + 1]);
		AddV2S(&vec1, pol->points[i + 2]);
		vec1.x *= curSquare / 3, vec1.y *= curSquare / 3;

		AddV2S(&massPoint, vec1);
		square += curSquare;
	}
	massPoint.x /= square, massPoint.y /= square;
	ex->massCenter = massPoint;

	return ex->massCenter;
}