#pragma once
#include "GJK.h"

typedef struct _CircleShape {
	struct Shape shape;
	Vector2S origin;
	float radius;
}* CircleShape;

__declspec(dllexport) CircleShape CreateCircle(Vector2S origin, float radius);

__declspec(dllexport) void DestructCircle(CircleShape circle);

__declspec(dllexport) void SetRadius(CircleShape circle, float radius);
__declspec(dllexport) float GetRadius(const CircleShape circle);

__declspec(dllexport) void SetOrigin(CircleShape circle, Vector2S radius);
__declspec(dllexport) Vector2S GetOrigin(const CircleShape circle);