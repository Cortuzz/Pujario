#include <memory.h>
#include "Shapes/Collider.h"
#include "algorithms/GJK.h"

struct s_Collider2SWithBounds {
	struct _Collider2S;
	struct Bounds2S bounds;
};

#define s_COLLIDER_EXTREME_IN_DIR(cl, extreme, dist, dir)						\
{																				\
	float curDist;																\
	Vector2S curPoint;															\
	extreme = cl->shapes[0]->support((Shape)cl->shapes[0], dir, &dist);			\
	for (unsigned i = 1; i < cl->count; ++i) {									\
		curPoint = cl->shapes[i]->support((Shape)cl->shapes[i], dir, &curDist);	\
		if (dist < curDist) extreme = curPoint, dist = curDist;					\
	}																			\
}

// Calculates bounds of collider using support function
static inline void s_CalcBoundsCollider2S(struct s_Collider2SWithBounds* cl) {
	float dist;
	Vector2S point;

	s_COLLIDER_EXTREME_IN_DIR(cl, point, dist, i_Vector2S);
	cl->bounds.x.sup = point.x;
	s_COLLIDER_EXTREME_IN_DIR(cl, point, dist, Neg_i_Vector2S);
	cl->bounds.x.inf = point.x;
	s_COLLIDER_EXTREME_IN_DIR(cl, point, dist, j_Vector2S);
	cl->bounds.y.sup = point.y;
	s_COLLIDER_EXTREME_IN_DIR(cl, point, dist, Neg_j_Vector2S);
	cl->bounds.y.inf = point.y;
}

Collider2S Collider2S_Create(const TransformableShape* shapes, unsigned count, bool preferBoundsCheck) {
	Collider2S cl;

	if (preferBoundsCheck) {
		cl = (Collider2S)malloc(sizeof(struct s_Collider2SWithBounds) + count * sizeof(TransformableShape));
		cl->shapes = (TransformableShape*)((struct s_Collider2SWithBounds*)cl + 1);
		s_CalcBoundsCollider2S((struct s_Collider2SWithBounds*)cl);
	}
	else {
		cl = (Collider2S)malloc(sizeof(struct _Collider2S) + count * sizeof(TransformableShape));
		cl->shapes = (TransformableShape*)(cl + 1);
	}

	cl->count = count;
	cl->preferBoundsCheck = preferBoundsCheck;
	memcpy(cl->shapes, shapes, count * sizeof(TransformableShape));

	return cl;
}

void Collider2S_Delete(Collider2S collider) { free((void*)collider); };

void Collider2S_SetTransform(Collider2S collider, const struct Transform2S* transform) {
	struct EvaluatedTransform2S evtr;
	EvaluateTransform2S(transform, &evtr);
	for (unsigned i = 0; i < collider->count; ++i)
		collider->shapes[i]->setTransform(collider->shapes[i], &evtr);
	if (collider->preferBoundsCheck) s_CalcBoundsCollider2S((struct s_Collider2SWithBounds*)collider);
}

bool CheckCollision_Collider2S_Shape(Collider2S cl, Shape shape) {
	struct Simplex simplex = { 0 };
	for (unsigned i = 0; i < cl->count; ++i)
		if (_GJK((Shape)cl->shapes[i], shape, &simplex)) return true;
	return false;
}

struct Collision CalcCollision_Collider2S_Shape(Collider2S cl, Shape shape, float tolerance) {
	struct Collision result = { 0 };
	struct ExtendedSimplex simplex = { 0 };
	for (unsigned i = 0; i < cl->count; ++i) {
		_EPA((Shape)cl->shapes[i], shape, &simplex, tolerance, &result);
		if (result.areCollided) break;
	}
	return result;
}

bool CheckCollision_Collider2S(Collider2S cl1, Collider2S cl2) {
	struct Simplex simplex = { 0 };
	if (cl1->preferBoundsCheck && cl2->preferBoundsCheck && !IsOverlapBounds2S(
		&((struct s_Collider2SWithBounds*)cl1)->bounds,
		&((struct s_Collider2SWithBounds*)cl2)->bounds)) return false;

	for (unsigned i = 0; i < cl1->count; ++i)
		for (unsigned j = 0; j < cl2->count; ++j)
			if (_GJK((Shape)cl1->shapes[i], (Shape)cl2->shapes[j], &simplex)) return true;
	return false;
}

struct Collision CalcCollision_Collider2S(Collider2S cl1, Collider2S cl2, float tolerance) {
	struct Collision result = { 0 };
	struct ExtendedSimplex simplex = { 0 };
	if (cl1->preferBoundsCheck && cl2->preferBoundsCheck && !IsOverlapBounds2S(
		&((struct s_Collider2SWithBounds*)cl1)->bounds,
		&((struct s_Collider2SWithBounds*)cl2)->bounds)) return result;

	for (unsigned i = 0; i < cl1->count; ++i)
		for (unsigned j = 0; j < cl2->count; ++j) {
			_EPA((Shape)cl1->shapes[i], (Shape)cl2->shapes[j], &simplex, tolerance, &result);
			if (result.areCollided) return result;
		}
	return result;
}
