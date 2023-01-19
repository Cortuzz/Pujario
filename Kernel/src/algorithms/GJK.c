#include <assert.h>
#include <stdio.h>
#include <float.h>
#include <math.h>
#include "GJK.h"

static inline void s_Support(Shape shape1, Shape shape2, Vector2S direction, _Out_ Vector2S* result) {
	*result = shape1->support(shape1, direction, NULL);
	InvertV2S(&direction);
	SubstractV2S(result, shape2->support(shape2, direction, NULL));
}

bool ProcessSimplex(struct Simplex* simplex, Vector2S* outDirection)
{
	assert((simplex->count >= 2 && "simplex must contain at leats 2 points"));
	if (simplex->count == 2) {
		Vector2S BC, BO = simplex->points[1];
		SubstractToV2S(simplex->points[0], simplex->points[1], &BC);
		InvertV2S(&BO);
		TripleProductToV2S(BC, BO, BC, outDirection);
		return false;
	}

	Vector2S AC, AB, AO = simplex->points[2];
	SubstractToV2S(simplex->points[0], simplex->points[2], &AC);
	SubstractToV2S(simplex->points[1], simplex->points[2], &AB);
	InvertV2S(&AO);
	TripleProductToV2S(AC, AB, AB, outDirection);
	if (DotV2S(*outDirection, AO) > 0) {
		simplex->points[0] = simplex->points[1];
		simplex->points[1] = simplex->points[2];
		simplex->count = 2;
		return false;
	}

	TripleProductToV2S(AB, AC, AC, outDirection);
	if (DotV2S(*outDirection, AO) > 0) {
		simplex->points[1] = simplex->points[2];
		simplex->count = 2;
		return false;
	}
	return true;
}

static __forceinline bool s_GJK(Shape shape1, Shape shape2, struct Simplex* simplex)
{
	s_Support(shape1, shape2, i_Vector2S, simplex->points);
	s_Support(shape1, shape2, Neg_i_Vector2S, simplex->points + 1);
	simplex->count = 2;
	Vector2S direction = Neg_i_Vector2S;

	do {
		if (ProcessSimplex(simplex, &direction)) return true;
		s_Support(shape1, shape2, direction, simplex->points + simplex->count++);
	} while (DotV2S(simplex->points[2], direction) >= 0);
	return false;
}

bool _GJK(Shape shape1, Shape shape2, struct Simplex* simplex) { return s_GJK(shape1, shape2, simplex); }

bool GJK(Shape shape1, Shape shape2) {
	struct Simplex simplex = { 0 };
	return s_GJK(shape1, shape2, &simplex);
}

static __forceinline uint8_t s_ClosestEdge(const struct ExtendedSimplex* simplex, Vector2S* normal, float* distanse) {
	uint8_t index = 0;
	float dist = FLT_MAX;
	*distanse = FLT_MAX;
	uint8_t simplexSize = simplex->count;
	Vector2S edge = { 0 }, tempNormal = { 0 };
	SubstractToV2S(simplex->points[0], simplex->points[simplexSize - 1], &edge);

	uint8_t i = 0;
	while (true)
	{
		TripleProductToV2S(edge, simplex->points[i], edge, &tempNormal);
		ToUnitV2S(&tempNormal);
		dist = DotV2S(tempNormal, simplex->points[i]);
		if (dist < 0) dist *= -1.f;
		if (dist < *distanse) {
			*normal = tempNormal;
			*distanse = dist;
			index = i;
		}
		if (++i == simplexSize) return index;
		SubstractToV2S(simplex->points[i], simplex->points[i - 1], &edge);
	}
}

static __forceinline void s_EPA(Shape shape1, Shape shape2, struct ExtendedSimplex* simplex, float tolerance, struct Collision* out)
{
	if (!(out->areCollided = s_GJK(shape1, shape2, (struct Simplex*)simplex))) return;
	uint8_t edgeIndex;
	Vector2S sp;
	while (true)
	{
		edgeIndex = s_ClosestEdge(simplex, &out->direction, &out->depth);
		s_Support(shape1, shape2, out->direction, &sp);

		float delta = out->depth - DotV2S(sp, out->direction);
		if ((delta > 0 ? delta : delta * -1.f) < tolerance
			|| simplex->count++ == MAX_SIMPLEX_SIZE) return;

		for (uint8_t i = simplex->count - 1; i > edgeIndex; --i)
			simplex->points[i] = simplex->points[i - 1];

		simplex->points[edgeIndex] = sp;
	}
}

void _EPA(Shape shape1, Shape shape2, struct ExtendedSimplex* simplex, float tolerance, struct Collision* out) {
	s_EPA(shape1, shape2, simplex, tolerance, out);
}

struct Collision EPA(Shape shape1, Shape shape2, float tolerance)
{
	struct Collision result = { 0 };
	struct ExtendedSimplex simplex = { 0 };
	s_EPA(shape1, shape2, &simplex, tolerance, &result);
	return result;
}