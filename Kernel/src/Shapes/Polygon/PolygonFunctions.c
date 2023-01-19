#include "PolygonExtensionFields.h"
#include <assert.h>
#include <memory.h>

bool BelongsToPolygon2S(const Polygon2S polygon, Vector2S point)
{
	struct Line2SMap* map = Polygon2S_GetLineMap(polygon);
	int layInd = map->count / 2, top = map->count, bot = 0;
	while (true) {
		if (map->corners[layInd] > point.x)
			top = layInd - 1;
		else if (map->corners[layInd + 1] < point.x)
			bot = layInd + 1;
		else break;
		if (bot > top) return false;
		layInd = (top + bot) / 2;
	}

	struct Line2SMapLayer* layer = map->layers + layInd;
	top = layer->count - 1, bot = 0;
	while (bot <= top)
	{
		unsigned lineInd = (top + bot) / 2;
		if (CalcYLine2S(layer->lines + lineInd, point.x) > point.y) {
			if (lineInd == 0) return false;
			top = lineInd - 1;
		}
		else if (lineInd + 1 < layer->count)
			if (CalcYLine2S(layer->lines + lineInd + 1, point.x) < point.y)
				bot = lineInd + 1;
			else return lineInd % 2 == 0;
		else return false;
	}

	return false;
}

void FindTangent(
	const struct BasePolygon2S* pol1,
	const struct BasePolygon2S* pol2,
	enum TangentType type,
	_Out_ unsigned* firstIndex,
	_Out_ unsigned* secondIndex) // TODO: do something with intersecting polygons
{
	unsigned ind1 = 0, ind2 = 0,
		prevInd1, prevInd2;
	char cf1 = (type & 0x10) ? -1 : 1, cf2 = (type & 0x01) ? -1 : 1;
	Vector2S vec, edge;

	do {
		prevInd1 = ind1, prevInd2 = ind2;

		for (unsigned i = 0; i < pol1->count; ++i)
		{
			SubstractToV2S(pol1->points[i], pol2->points[ind2], &vec);
			SubstractToV2S(pol1->points[cirind(i - 1, pol1->count)], pol1->points[i], &edge);
			if (cf1 * Cross2S(vec, edge) > 0) {
				SubstractToV2S(pol1->points[cirind(i + 1, pol1->count)], pol1->points[i], &edge);
				if (cf1 * Cross2S(vec, edge) > 0) {
					ind1 = i; break;
				}
			}
		}
		for (unsigned i = 0; i < pol2->count; ++i)
		{
			SubstractToV2S(pol1->points[ind1], pol2->points[i], &vec);
			SubstractToV2S(pol2->points[cirind(i - 1, pol2->count)], pol2->points[i], &edge);
			if (cf2 * Cross2S(vec, edge) > 0) {
				SubstractToV2S(pol2->points[cirind(i + 1, pol2->count)], pol2->points[i], &edge);
				if (cf2 * Cross2S(vec, edge) > 0) {
					ind2 = i; break;
				}
			}
		}
	} while (ind1 != prevInd1 || ind2 != prevInd2);

	*firstIndex = ind1; *secondIndex = ind2;
}

void MergeToConvexHull(
	const struct BasePolygon2S* pol1,
	const struct BasePolygon2S* pol2,
	_Out_writes_(pol1->count + pol2->count) Vector2S* hullPoints,
	_Out_ unsigned* hullCount)
{
	assert(pol1->count > 0 && pol2->count > 0 && "Polygons must contain at least 1 point");
	Vector2S vec, edge1, edge2;
	unsigned upInd1, downInd1, upInd2, downInd2, hullInd = 0;
	char bypassDir;

	FindTangent(pol1, pol2, TANGENT_UP, &upInd1, &upInd2);
	FindTangent(pol1, pol2, TANGENT_DOWN, &downInd1, &downInd2);

	hullPoints[hullInd] = pol2->points[upInd2];
	hullPoints[++hullInd] = pol1->points[upInd1];

	if (upInd1 != downInd1) {
		SubstractToV2S(pol1->points[upInd1], pol2->points[upInd2], &vec);
		SubstractToV2S(pol1->points[cirind(upInd1 + 1, pol1->count)], pol1->points[upInd1], &edge1);
		SubstractToV2S(pol1->points[cirind((int)upInd1 - 1, pol1->count)], pol1->points[upInd1], &edge2);
		bypassDir = DotV2S(vec, edge1) > DotV2S(vec, edge2) ? 1 : -1;

		for (unsigned i = cirind((int)upInd1 + bypassDir, pol1->count);
			i != downInd1;
			i = cirind((int)i + bypassDir, pol1->count))
		{
			hullPoints[++hullInd] = pol1->points[i];
		}
		hullPoints[++hullInd] = pol1->points[downInd1];
	}

	if (upInd2 != downInd2) {
		hullPoints[++hullInd] = pol2->points[downInd2];
		SubstractToV2S(pol2->points[downInd2], pol1->points[downInd1], &vec);
		SubstractToV2S(pol2->points[cirind(downInd2 + 1, pol2->count)], pol2->points[downInd2], &edge1);
		SubstractToV2S(pol2->points[cirind((int)downInd2 - 1, pol2->count)], pol2->points[downInd2], &edge2);
		bypassDir = DotV2S(vec, edge1) > DotV2S(vec, edge2) ? 1 : -1;

		for (int i = cirind((int)downInd2 + bypassDir, pol2->count);
			i != upInd2;
			i = cirind((int)i + bypassDir, pol2->count))
		{
			hullPoints[++hullInd] = pol2->points[i];
		}
	}

	*hullCount = hullInd + 1;
}

void ConstructConvexHull(
	const Vector2S* points,
	unsigned count,
	_Out_writes_(count) Vector2S* hullPoints,
	_Out_ unsigned* hullCount)
{
	unsigned curInd, nextInd;
	memcpy(hullPoints, points, count * sizeof(Vector2S));
	Vector2S vec1 = hullPoints[0], vec2;

	if (count < 4) { *hullCount = count; return; }

	for (unsigned i = 1; i < count; ++i) {
		if (hullPoints[i].x > vec1.x) continue;
		hullPoints[0] = hullPoints[i];
		hullPoints[i] = vec1;
		vec1 = hullPoints[0];
	}
	SortClockwiseOriginPoints2S(hullPoints + 1, count - 1, vec1);

	curInd = 1;
	for (unsigned i = 2; i < count; ++i)
	{
		nextInd = curInd + 1;
		do {
			SubstractToV2S(hullPoints[curInd], hullPoints[curInd - 1], &vec1);
			SubstractToV2S(hullPoints[nextInd], hullPoints[curInd], &vec2);
		} while (Cross2S(vec1, vec2) > 0 && --curInd > 0);

		if (nextInd - ++curInd == 0) continue;
		memshift(hullPoints + curInd, ((int)curInd - nextInd) * sizeof(Vector2S), (count - curInd) * sizeof(Vector2S));
	}

	*hullCount = curInd + 1;
}

void ConstructConvexHullUsingBuf(
	const Vector2S* points,
	unsigned count,
	_Writable_elements_(count) _Readable_elements_(count) Vector2S* buffer,
	_Out_writes_(count) Vector2S* hullPoints,
	_Out_ unsigned* hullCount)
{
	if (count < 4) {
		memcpy(hullPoints, points, (*hullCount = count) * sizeof(Vector2S));
		return;
	}

	unsigned curInd;
	memcpy(buffer, points, count * sizeof(Vector2S));
	Vector2S vec1 = buffer[0], vec2;

	for (unsigned i = 1; i < count; ++i) {
		if (buffer[i].x > vec1.x) continue;
		buffer[0] = buffer[i];
		buffer[i] = vec1;
		vec1 = buffer[0];
	}
	SortClockwiseOriginPoints2S(buffer + 1, count - 1, vec1);

	curInd = 1;
	hullPoints[0] = buffer[0], hullPoints[1] = buffer[1];
	for (unsigned i = 2; i < count; ++i) {
		do {
			SubstractToV2S(hullPoints[curInd], hullPoints[curInd - 1], &vec1);
			SubstractToV2S(buffer[i], hullPoints[curInd], &vec2);
		} while (Cross2S(vec1, vec2) > 0 && --curInd > 0);
		hullPoints[++curInd] = buffer[i];
	}

	*hullCount = curInd + 1;
}

void Polygon2S_ProjectOnAxis(const struct BasePolygon2S* polygon, UnitVector2S axis, _Out_ struct Range* result)
{
	float pos;
	result->inf = DotV2S(axis, polygon->points[0]), result->sup = result->inf;
	for (unsigned i = 1; i < polygon->count; ++i) {
		if ((pos = DotV2S(axis, polygon->points[i])) < result->inf) result->inf = pos;
		else if (pos > result->sup) result->sup = pos;
	}
}