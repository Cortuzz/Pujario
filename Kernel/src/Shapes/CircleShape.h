#pragma once
#include <assert.h>
#include "Shapes/Shape.h"
#include "Utils.h"

typedef struct _Circle {
	Vector2S origin;
	float radius;
}*Circle;

typedef const union _CircleShape* CircleShape;
typedef Vector2S(*GJK_Circle_SupportFunc)(CircleShape shape, Vector2S direction, float* outDistance);
typedef void(*CircleShapeTransformer)(CircleShape shape, const struct EvaluatedTransform2S* transform);

union _CircleShape {
	struct _TransformableShape asTransformableShape;
	struct {
		Circle primitive;
		GJK_Circle_SupportFunc support;
		CircleShapeTransformer setTransform;
	};
};

__declspec(dllexport) CircleShape CircleShape_Create(Vector2S origin, float radius);
__declspec(dllexport) void CircleShape_Delete(CircleShape shape);

typedef CircleShape StretchingCircleShape;
__declspec(dllexport) StretchingCircleShape StretchingCircleShape_Create(Vector2S origin, float radius);
__declspec(dllexport) void StretchingCircleShape_Delete(StretchingCircleShape shape);

inline void Circle_ApplyEvTransform(Circle circle, const struct EvaluatedTransform2S* transform) {
	TransformPoint2S(&circle->origin, transform);
	assert(transform->scale.x == transform->scale.y && "Circles can only habe symmetric scale");
	circle->radius *= Len(transform->scale);
}
__declspec(dllexport) void Circle_ApplyTransform(Circle circle, const struct Transform2S* transform);