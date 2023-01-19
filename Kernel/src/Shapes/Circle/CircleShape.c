#include <math.h>
#include "Shapes/CircleShape.h"

struct s_CircleShape {
	union _CircleShape;
	struct _Circle basePrimitive, transformedPrimitive;
};

static Vector2S s_GJK_Circle_SupportFunc(CircleShape shape, Vector2S direction, float* outDistance)
{
	Circle circle = shape->primitive;
	float cf = circle->radius / sqrtf(direction.x * direction.x + direction.y * direction.y);
	Vector2S res = {
		circle->origin.x + cf * direction.x,
		circle->origin.y + cf * direction.y
	};
	if (outDistance) *outDistance = DotV2S(direction, res);
	return res;
}

static void s_CircleShapeTransformer(CircleShape shape, const struct EvaluatedTransform2S* transform) {
	struct s_CircleShape* self = (struct s_CircleShape*)shape;
	self->transformedPrimitive = self->basePrimitive;
	Circle_ApplyEvTransform(&self->transformedPrimitive, transform);
}


CircleShape CircleShape_Create(Vector2S origin, float radius) {
	struct s_CircleShape* shape = (struct s_CircleShape*)malloc(sizeof(struct s_CircleShape));
	shape->primitive = &shape->transformedPrimitive;
	shape->basePrimitive.origin = origin, shape->basePrimitive.radius = radius;
	shape->transformedPrimitive = shape->basePrimitive;
	shape->support = s_GJK_Circle_SupportFunc;
	shape->setTransform = s_CircleShapeTransformer;
	return (CircleShape)shape;
}

struct s_StretchingCircleShape {
	struct s_CircleShape;
	struct _Circle prevTransformedPrimitive;
};

static void s_StretchingCircleShapeTransformer(CircleShape shape, const struct EvaluatedTransform2S* transform) {
	struct s_StretchingCircleShape* self = (struct s_StretchingCircleShape*)shape;
	self->prevTransformedPrimitive = self->transformedPrimitive;
	self->transformedPrimitive = self->basePrimitive;
	Circle_ApplyEvTransform(&self->transformedPrimitive, transform);
}

static Vector2S s_GJK_StretchingCircle_SupportFunc(CircleShape shape, Vector2S direction, float* outDistance)
{
	struct s_StretchingCircleShape* self = (struct s_StretchingCircleShape*)shape;
	float temp1 = self->transformedPrimitive.radius / sqrtf(direction.x * direction.x + direction.y * direction.y),
		temp2 = self->prevTransformedPrimitive.radius / sqrtf(direction.x * direction.x + direction.y * direction.y);

	Vector2S res1 = {
		self->transformedPrimitive.origin.x + temp1 * direction.x,
		self->transformedPrimitive.origin.y + temp1 * direction.y
	}, res2 = {
		self->prevTransformedPrimitive.origin.x + temp2 * direction.x,
		self->prevTransformedPrimitive.origin.y + temp2 * direction.y
	};

	temp1 = DotV2S(res1, direction), temp2 = DotV2S(res2, direction);
	if (temp1 > temp2) {
		if (outDistance) *outDistance = temp1;
		return res1;
	}
	if (outDistance) *outDistance = temp2;
	return res2;
}

StretchingCircleShape StretchingCircleShape_Create(Vector2S origin, float radius) {
	struct s_StretchingCircleShape* shape = (struct s_StretchingCircleShape*)malloc(sizeof(struct s_StretchingCircleShape));
	shape->primitive = &shape->transformedPrimitive;
	shape->basePrimitive.origin = origin, shape->basePrimitive.radius = radius;
	shape->transformedPrimitive = shape->basePrimitive;
	shape->prevTransformedPrimitive = shape->basePrimitive;
	shape->support = s_GJK_StretchingCircle_SupportFunc;
	shape->setTransform = s_StretchingCircleShapeTransformer;
	return (CircleShape)shape;
}

void CircleShape_Delete(CircleShape shape) { free((void*)shape); }
void StretchingCircleShape_Delete(StretchingCircleShape shape) { free((void*)shape); }

void Circle_ApplyTransform(Circle circle, const struct Transform2S* transform) {
	struct EvaluatedTransform2S evtr;
	EvaluateTransform2S(transform, &evtr);
	Circle_ApplyEvTransform(circle, &evtr);
}
