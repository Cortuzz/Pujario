#pragma once
#include "stdint.h"

typedef struct Vector3S
{
    float x, y, z;
} Vector3S;

Vector3S InvertedV3S(Vector3S v);
void InvertV3S(Vector3S* v);

Vector3S SubstractedV3S(Vector3S v1, Vector3S v2);
void SubstractV3S(Vector3S* v1, Vector3S v2);


typedef struct Vector2S
{
    float x, y;
} Vector2S;

const Vector2S Zero_Vector2S;// = { 0, 0 };
const Vector2S i_Vector2S;// = { 1, 0 };
const Vector2S Neg_i_Vector2S;// = { -1, 0 };
const Vector2S j_Vector2S;// = { 0, 1 };
const Vector2S Neg_j_Vector2S;// = { 0, -1 };

Vector2S InvertedV2S(Vector2S v);
void InvertV2S(Vector2S* v);

Vector2S SubstractedV2S(Vector2S from, Vector2S val);
void SubstractToV2S(Vector2S from, Vector2S val, Vector2S* out);
void SubstractV2S(Vector2S* from, Vector2S val);

void Normalize(Vector2S* v);
Vector2S Normalized(Vector2S v);

float Len(Vector2S v);

float DotV2S(Vector2S v1, Vector2S v2);

float Cross2S(Vector2S v1, Vector2S v2);

Vector2S TripleProductV2S(const Vector2S* v1, const Vector2S* v2, const Vector2S* v3);

void TripleProductToV2S(const Vector2S* v1, const Vector2S* v2, const Vector2S* v3, Vector2S* outV);