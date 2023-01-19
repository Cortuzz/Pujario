#pragma once
#include <stdbool.h>
#include "algorithms/GJK.h"
#include "Shape.h"
#include "Utils.h"

typedef struct _Collider2S {
	TransformableShape* shapes;
	unsigned count;
	bool preferBoundsCheck;
}*Collider2S;

__declspec(dllexport) Collider2S Collider2S_Create(const TransformableShape* shapes, unsigned count, bool preferBoundsCheck);
__declspec(dllexport) void Collider2S_Delete(Collider2S collider);
__declspec(dllexport) void Collider2S_SetTransform(Collider2S collider, const struct Transform2S* transform);

__declspec(dllexport) bool CheckCollision_Collider2S_Shape(Collider2S cl, Shape shape);
__declspec(dllexport) struct Collision CalcCollision_Collider2S_Shape(Collider2S cl, Shape shape, float tolerance);
__declspec(dllexport) bool CheckCollision_Collider2S(Collider2S cl1, Collider2S cl2);
__declspec(dllexport) struct Collision CalcCollision_Collider2S(Collider2S cl1, Collider2S cl2, float tolerance);

