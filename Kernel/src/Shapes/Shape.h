#pragma once
#include "MathBasics.h"
#include "Utils.h"

typedef void* PEntity;
typedef const struct _Shape* Shape;

/// <summary Calculates extreme point </summary>
/// <returns>point and distance to it if param outDistance is not NULL</returns>
typedef Vector2S(*GJK_SupportFunc)(Shape self, Vector2S direction, _Out_opt_ float* outDistance);

// 2D shape for GJK
struct _Shape {
	PEntity primitive;
	GJK_SupportFunc support;
};

__declspec(dllexport) const PEntity Shape_GetPrimitive(Shape shape);

typedef const struct _TransformableShape* TransformableShape;
typedef void(*ShapeTransformer)(TransformableShape shape, const struct EvaluatedTransform2S* transform);

struct _TransformableShape {
	struct _Shape;
	ShapeTransformer setTransform;
};

__declspec(dllexport) void Shape_SetTransform(TransformableShape shape, const struct Transform2S* transform);

void Shape_FindBoundaryInDir(const Shape shape, Vector2S direction, _Out_ Line2S* boundary);