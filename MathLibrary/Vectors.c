#include <math.h>
#include "Vectors.h"

Vector3S InvertedV3S(Vector3S v) {
    Vector3S res = { -v.x, -v.y, -v.z };
    return res;
}

void InvertV3S(Vector3S* v) {
    v->x = -v->x;
    v->y = -v->y;
    v->z = -v->z;
}

Vector3S SubstractedV3S(Vector3S v1, Vector3S v2) {
    v1.x -= v2.x;
    v1.y -= v2.y;
    v1.z -= v2.z;
    return v1;
}

void SubstractV3S(Vector3S* v1, Vector3S v2) {
    v1->x -= v2.x;
    v1->y -= v2.y;
    v1->z -= v2.z;
}

const Vector2S Zero_Vector2S = { 0, 0 };
const Vector2S i_Vector2S = { 1, 0 };
const Vector2S Neg_i_Vector2S = { -1, 0 };
const Vector2S j_Vector2S = { 0, 1 };
const Vector2S Neg_j_Vector2S = { 0, -1 };

Vector2S InvertedV2S(Vector2S v) {
    Vector2S res = { -v.x, -v.y };
    return res;
}

void InvertV2S(Vector2S* v) {
    v->x = -v->x;
    v->y = -v->y;
}

Vector2S SubstractedV2S(Vector2S from, Vector2S val) {
    from.x -= val.x;
    from.y -= val.y;
    return from;
}

void SubstractToV2S(Vector2S from, Vector2S val, Vector2S* out) {
    out->x = from.x - val.x;
    out->y = from.y - val.y;
}

void SubstractV2S(Vector2S* from, Vector2S val) {
    from->x -= val.x;
    from->y -= val.y;
}

float DotV2S(Vector2S v1, Vector2S v2) {
    return v1.x * v2.x + v1.y * v2.y;
}

float Cross2S(Vector2S v1, Vector2S v2) {
    return v1.x * v2.y - v1.y * v2.x;
}

Vector2S TripleProductV2S(const Vector2S* v1, const Vector2S* v2, const Vector2S* v3) {
    float temp = v2->x * v1->y - v1->x * v2->y;
    Vector2S res = { v3->y * temp, -v3->x * temp };
    return res;
}

void TripleProductToV2S(const Vector2S* v1, const Vector2S* v2, const Vector2S* v3, Vector2S* outV) {
    float temp = v2->x * v1->y - v1->x * v2->y;
    outV->x = v3->y * temp;
    outV->y = -v3->x * temp;
}

float Len(Vector2S v) { return sqrtf(v.x * v.x + v.y * v.y); }

void Normalize(Vector2S* v) {
    float cf = 1.0f / Len(*v);
    v->x = v->x * cf;
    v->y = v->y * cf;
}

Vector2S Normalized(Vector2S v) {
    float cf = 1.0f / Len(v);
    v.x = v.x * cf;
    v.y = v.y * cf;
    return v;
}
