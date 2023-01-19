#pragma once
#include "Shape.h"

const PEntity Shape_GetPrimitive(Shape shape) { return shape->primitive; }

void Shape_SetTransform(TransformableShape shape, const struct Transform2S* transform) {
	struct EvaluatedTransform2S evtr;
	EvaluateTransform2S(transform, &evtr); 
	shape->setTransform(shape, &evtr);
}

void Shape_FindBoundaryInDir(const Shape shape, Vector2S direction, _Out_ Line2S* boundary) {
	Vector2S point = shape->support(shape, direction, NULL);
	Normal(direction, &direction);
	Line2SFromDir_P(direction, point, boundary);
}