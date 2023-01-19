#include "SAT.h"

bool SAT(const Polygon2S pol1, const Polygon2S pol2) {
	Vector2S* normals;
	struct Range proj1, proj2;

	normals = Polygon2S_GetNormals(pol1);
	for (unsigned i = 0; i < pol1->count; ++i)
	{
		Polygon2S_ProjectOnAxis((struct BasePolygon2S*)pol1, normals[i], &proj1);
		Polygon2S_ProjectOnAxis((struct BasePolygon2S*)pol2, normals[i], &proj2);
		if (!IsOverlapRange(proj1, proj2)) return false;
	}
	normals = Polygon2S_GetNormals(pol2);
	for (unsigned i = 0; i < pol2->count; ++i)
	{
		Polygon2S_ProjectOnAxis((struct BasePolygon2S*)pol1, normals[i], &proj1);
		Polygon2S_ProjectOnAxis((struct BasePolygon2S*)pol2, normals[i], &proj2);
		if (!IsOverlapRange(proj1, proj2))
			return false;
	}

	return true;
}