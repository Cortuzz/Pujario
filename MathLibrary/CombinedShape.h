#pragma once
#include "GJK.h"

typedef const struct _CombinedShape {
	// hyeta; CombinedShape ne convex, so cant be checked by GJK, every part should be checked separately
	struct Shape shape;
	struct Shape* const* parts;
	int partsCount;
}* CombinedShape;

_declspec(dllexport) CombinedShape CreateCombinedShape(const struct Shape* const* parts, int partsCount);
_declspec(dllexport) void DestructCombinedShape(CombinedShape shape);