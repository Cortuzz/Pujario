#include <math.h>
#include "CircleShape.h"

Vector2S CircleShape_Shape_extremePointInDirection(const struct Shape* self, Vector2S direction, float* outDistance) 
{
	CircleShape circle = (CircleShape)self;
	float cf = circle->radius / sqrtf(direction.x * direction.x + direction.y * direction.y);
	Vector2S res = {
		circle->origin.x + cf * direction.x,
		circle->origin.y + cf * direction.y
	};
	if (outDistance) *outDistance = circle->radius;
	return res;
}

CircleShape CreateCircle(Vector2S origin, float radius) {
	CircleShape circle = (CircleShape)malloc(sizeof(struct _CircleShape));
	circle->origin = origin;
	circle->radius = radius;
	circle->shape.extremePointInDirection = CircleShape_Shape_extremePointInDirection;
	return circle;
}

void DestructCircle(CircleShape circle) { free(circle); }

__declspec(dllexport) void SetRadius(CircleShape circle, float radius) { circle->radius = radius; }
__declspec(dllexport) float GetRadius(const CircleShape circle) { return circle->radius; }

__declspec(dllexport) void SetOrigin(CircleShape circle, Vector2S origin) { circle->origin = origin; }
__declspec(dllexport) Vector2S GetOrigin(const CircleShape circle) { return circle->origin; }