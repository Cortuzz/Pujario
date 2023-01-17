#pragma once
#include "Shapes/PolygonShape.h"

struct _Polygon2SExtensionFields
{
	Vector2S massCenter;
	Segment2S* edges;
	UnitVector2S* normals;
	struct Line2SMap* lineMap;
};

struct Line2SMap {
	struct Line2SMapLayer {
		Line2S* lines;
		unsigned  count;
	}*layers;
	float* corners; // size : count + 1; conners[0] <= layer[0] <= conners[1] 
	unsigned  count;
};

struct __Line2Sp {
	Line2S line;
	float cy;
};

void Polygon2S_FreeExtensionFields(_Post_invalid_ Polygon2S pol);

_Ret_maybenull_ struct _Polygon2SExtensionFields* Polygon2S_CopyExtensionFields(const Polygon2S pol);