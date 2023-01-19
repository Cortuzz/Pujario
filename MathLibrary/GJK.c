#define MAX_SIMPLEX_SIZE 255

#include <assert.h>
#include <stdio.h>
#include <float.h>
#include <math.h>
#include "GJK.h"

Vector2S Support(const struct Shape* shape1, const struct Shape* shape2, Vector2S direction) {
    Vector2S extreme1 = shape1->extremePointInDirection(shape1, direction, NULL);
    InvertV2S(&direction);
    Vector2S extreme2 = shape2->extremePointInDirection(shape2, direction, NULL);
    SubstractV2S(&extreme1, extreme2);

    return extreme1;
}

BOOLEAN ProcessSimplex(Simplex* simplex, Vector2S* outDirection)
{
    assert((simplex->count >= 2 && "simplex must contain at leats 2 points"));
    if (simplex->count == 2) {
        Vector2S BC = SubstractedV2S(simplex->points[0], simplex->points[1]),
            BO = InvertedV2S(simplex->points[1]);
        *outDirection = TripleProductV2S(&BC, &BO, &BC);
        // TripleProductToV2S(simplex->points[0], simplex->points[1], outDirection);
        return FALSE;
    }

    Vector2S AC = SubstractedV2S(simplex->points[0], simplex->points[2]),
        AB = SubstractedV2S(simplex->points[1], simplex->points[2]),
        AO = InvertedV2S(simplex->points[2]);
    *outDirection = TripleProductV2S(&AC, &AB, &AB);
    // TripleProductToV2S(&AC, &AB, &AB, outDirection);
    if (DotV2S(*outDirection, AO) > 0) {
        simplex->points[0] = simplex->points[1];
        simplex->points[1] = simplex->points[2];
        simplex->count = 2;
        return FALSE;
    }

    *outDirection = TripleProductV2S(&AB, &AC, &AC);
    // TripleProductToV2S(&AB, &AC, &AC, outDirection);
    if (DotV2S(*outDirection, AO) > 0) {
        simplex->points[1] = simplex->points[2];
        simplex->count = 2;
        return FALSE;
    }
    return TRUE;
}

BOOLEAN __GJK(const struct Shape* shape1, const struct Shape* shape2, Simplex* simplex)
{
    simplex->points[0] = Support(shape1, shape2, i_Vector2S);
    simplex->points[1] = Support(shape1, shape2, Neg_i_Vector2S);
    simplex->count = 2;
    Vector2S direction = Neg_i_Vector2S;

    do {
        if (ProcessSimplex(simplex, &direction)) return TRUE;
        simplex->points[simplex->count++] = Support(shape1, shape2, direction);
    } while (DotV2S(simplex->points[2], direction) >= 0);
    return FALSE;
}

BOOL GJK(const struct Shape* shape1, const struct Shape* shape2) {
    Simplex simplex = {0};
    return __GJK(shape1, shape2, &simplex);
}

struct _ExtendedSimplex {
    uint8_t count;
    Vector2S points[MAX_SIMPLEX_SIZE];
};

uint8_t ClosestEdge(const struct _ExtendedSimplex* simplex, Vector2S* normal, float* distanse) {
    uint8_t index = 0;
    register float dist = FLT_MAX;
    *distanse = FLT_MAX;
    uint8_t simplexSize = simplex->count;
    Vector2S edge = {0}, tempNormal = {0};
    SubstractToV2S(simplex->points[0], simplex->points[simplexSize - 1], &edge);
    //TripleProductToV2S(&edge, &simplex->points[simplexSize - 1], &edge, &tempNormal);
    
    register uint8_t i = 0;
    while (TRUE)
    {
        TripleProductToV2S(&edge, &simplex->points[i], &edge, &tempNormal);
        Normalize(&tempNormal);
        dist = DotV2S(tempNormal, simplex->points[i]);
        if (dist < 0) dist *= -1.f;
        if (dist < *distanse) {
            *normal = tempNormal;
            *distanse = dist;
            index = i;
        }
        if (++i == simplexSize) return index;
        SubstractToV2S(simplex->points[i], simplex->points[i - 1], &edge);
        //TripleProductToV2S(&edge, &simplex->points[i - 1], &edge, &tempNormal);
    }
}

struct Collision EPA(const struct Shape* shape1, const struct Shape* shape2, float tolerance) 
{
    struct Collision result = { 0 };
    struct _ExtendedSimplex simplex = {0};
    if (!(result.areCollided = __GJK(shape1, shape2, (struct Simplex*)&simplex))) return result;
    register uint8_t edgeIndex;
    Vector2S sp;
    while (TRUE)
    {
        edgeIndex = ClosestEdge(&simplex, &result.direction, &result.depth);
        sp = Support(shape1, shape2, result.direction);

        float delta = result.depth - DotV2S(sp, result.direction);
        if ((delta > 0 ? delta : delta * -1.f) < tolerance
            || simplex.count++ == MAX_SIMPLEX_SIZE) return result;

        for (uint8_t i = simplex.count - 1; i > edgeIndex; --i)
            simplex.points[i] = simplex.points[i - 1];
        
        simplex.points[edgeIndex] = sp;
    }
}