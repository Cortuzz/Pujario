#include "CombinedShape.h"
#include <float.h>
#include <malloc.h>

// hyeta 
Vector2S CombinedShape_Shape_extremePointInDirextion(const struct Shape* self, Vector2S direction, float* outDistance) {
	CombinedShape comb = (CombinedShape)self;
	struct Shape* const* shapes = comb->parts;
	register int count = comb->partsCount;
	register Vector2S farest = {0}, curPoint;
	float farestDist = FLT_MIN, curDist = FLT_MIN;

	for (register int i = 0; i < count; ++i) {
		curPoint = shapes[i]->extremePointInDirection(shapes[i], direction, &curDist);
		if (curDist > farestDist) {
			farest = curPoint;
			farestDist = curDist;
		}
	}
	if (outDistance) *outDistance = farestDist;
	return farest;
}

CombinedShape CreateCombinedShape(const struct Shape* const* parts, int partsCount) {
	struct _CombinedShape* shape = (struct _CombinedShape*)malloc(
		sizeof(struct _CombinedShape*) + sizeof(struct Shape*) * partsCount);
	shape->partsCount = partsCount;
	shape->shape.extremePointInDirection = CombinedShape_Shape_extremePointInDirextion;
	shape->parts = (struct Shape**)(shape + 1);
	memcpy(shape->parts, parts, sizeof(struct Shape*) * partsCount);

	return (CombinedShape)shape;
}

void DestructCombinedShape(CombinedShape shape) { free(shape); }