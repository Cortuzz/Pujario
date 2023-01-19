#pragma once
#include "Vectors.h"
#include "GJK.h"

struct _Transform
{
	Vector2S position, scale, origin;
	float rotation;
};

// move to anoter file 
typedef struct _Rectangle2S {
	float x, y, wight, height;
} Rectangle2S;

typedef struct _Collider2S {
	const struct Shape* shape;
	void(*applyTransform)(struct _Collider2S* self, struct _Transform* t);
	BOOLEAN preferBoundsCheck;
	Rectangle2S simplifyRect; 
	
}* Collider2S;

__declspec(dllexport) void ApplyTransform(Collider2S cl, struct _Transform* t);

__declspec(dllexport) BOOL CheckCollision(Collider2S c1, Collider2S c2);
__declspec(dllexport) struct Collision CalcCollision(Collider2S c1, Collider2S c2);

// TODO :	Streatching colliders 
//			Creation of colliders 
