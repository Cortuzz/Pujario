#pragma once
#include <stdbool.h>
#include <stdint.h>
#include <math.h>

typedef struct _Vector3S {
	float x, y, z;
} Vector3S;

inline Vector3S InvertedV3S(Vector3S v) {
	Vector3S res = { -v.x, -v.y, -v.z };
	return res;
}
inline void InvertV3S(Vector3S* v) {
	v->x = -v->x;
	v->y = -v->y;
	v->z = -v->z;
}

inline Vector3S SubstractedV3S(Vector3S v1, Vector3S v2) {
	v1.x -= v2.x;
	v1.y -= v2.y;
	v1.z -= v2.z;
	return v1;
}
inline void SubstractV3S(Vector3S* v1, Vector3S v2) {
	v1->x -= v2.x;
	v1->y -= v2.y;
	v1->z -= v2.z;
}

typedef struct _Vector2S {
	float x, y;
}
Vector2S,
UnitVector2S; // vectors declared as UnitVector2S are trusted to fulfill condition '|UnitVector2S| == 1'

static const Vector2S Zero_Vector2S = { 0, 0 };
static const Vector2S i_Vector2S = { 1, 0 };
static const Vector2S Neg_i_Vector2S = { -1, 0 };
static const Vector2S j_Vector2S = { 0, 1 };
static const Vector2S Neg_j_Vector2S = { 0, -1 };

inline Vector2S InvertedV2S(Vector2S v) {
	Vector2S res = { -v.x, -v.y };
	return res;
}

inline void InvertV2S(Vector2S* v) {
	v->x = -v->x;
	v->y = -v->y;
}

inline void AddV2S(Vector2S* to, Vector2S val) {
	to->x += val.x;
	to->y += val.y;
}

inline void AddToV2S(Vector2S v1, Vector2S v2, Vector2S* out) {
	out->x = v1.x + v2.x;
	out->y = v1.y + v2.y;
}

inline Vector2S SubstractedV2S(Vector2S from, Vector2S val) {
	from.x -= val.x;
	from.y -= val.y;
	return from;
}

inline void SubstractToV2S(Vector2S from, Vector2S val, _Out_ Vector2S* out) {
	out->x = from.x - val.x;
	out->y = from.y - val.y;
}

inline void SubstractV2S(Vector2S* from, Vector2S val) {
	from->x -= val.x;
	from->y -= val.y;
}

inline float DotV2S(Vector2S v1, Vector2S v2) { return v1.x * v2.x + v1.y * v2.y; }

inline float Cross2S(Vector2S v1, Vector2S v2) {
	return v1.x * v2.y - v1.y * v2.x;
}

inline void Normal(Vector2S v, _Out_ Vector2S* out) {
	out->x = v.y, out->y = -v.x;
}

inline Vector2S TripleProductV2S(Vector2S v1, Vector2S v2, Vector2S v3) {
	float temp = Cross2S(v1, v2);
	Vector2S res = { -v3.y * temp, v3.x * temp };
	return res;
}
inline void TripleProductToV2S(Vector2S v1, Vector2S v2, Vector2S v3, _Out_ Vector2S* result) {
	float temp = Cross2S(v1, v2);
	result->x = -v3.y * temp, result->y = v3.x * temp;
}

inline float Len(Vector2S v) { return sqrtf(v.x * v.x + v.y * v.y); }

inline void ToUnitV2S(Vector2S* v) {
	float cf = 1.0f / Len(*v);
	v->x = v->x * cf;
	v->y = v->y * cf;
}

inline UnitVector2S UnitV2S(Vector2S v) {
	float len = Len(v);
	v.x /= len, v.y /= len;
	return v;
}

// ax + by + c = 0
typedef struct _Line2S {
	float a, b, c;
} Line2S;

struct Range { float inf, sup; };

inline bool IsOverlapRange(struct Range r1, struct Range r2) {
	return r1.inf >= r2.inf && r1.inf <= r2.sup || r2.inf >= r1.inf && r2.inf <= r1.sup;
}

struct Bounds2S {
	struct Range x, y;
};

inline bool IsOverlapBounds2S(const struct Bounds2S* b1, const struct Bounds2S* b2) {
	return
		(b1->x.inf >= b2->x.inf && b1->x.inf <= b2->x.sup
			|| b2->x.inf >= b1->x.inf && b2->x.inf <= b1->x.sup)
		&&
		(b1->y.inf >= b2->y.inf && b1->y.inf <= b2->y.sup
			|| b2->y.inf >= b1->y.inf && b2->y.inf <= b1->y.sup);
}

typedef struct _Segment2S {
	Line2S line;
 struct Bounds2S range;
} Segment2S;

inline void Line2SFromPoints_P(Vector2S p1, Vector2S p2, Line2S* out) {
	out->a = p2.y - p1.y;
	out->b = p1.x - p2.x;
	out->c = -(p1.x * out->a + p1.y * out->b);
}
inline Line2S Line2SFromPoints(Vector2S p1, Vector2S p2) {
	Line2S res = {
		p2.y - p1.y,
		p1.x - p2.x,
		-(p1.x * res.a + p1.y * res.b)
	};
	return res;
}

inline void Line2SFromDir_P(Vector2S direction, Vector2S point, Line2S* out) {
	out->a = 1 / direction.x;
	out->b = -1 / direction.y;
	out->c = -(point.y * out->b + point.x * out->a);
}
inline Line2S Line2SFromDir(Vector2S direction, Vector2S point) {
	Line2S res = {
		1 / direction.x,
		-1 / direction.y,
		-(point.y * res.b + point.x * res.a)
	};
	return res;
}

inline float CalcXLine2S(Line2S* line, float y) { return -(line->b * y + line->c) / line->a; }
inline float CalcYLine2S(Line2S* line, float x) { return -(line->a * x + line->c) / line->b; }

/// <param name="outPoint">{NaN, NaN} when nested</param>
/// <returns>false if not intersect, true otherwise</returns>
bool IntersectLine2S(const Line2S* l1, const Line2S* l2, Vector2S* outPoint);

inline void ConstructSegment2S(_Post_valid_ _Inout_ Segment2S* seg, Vector2S p1, Vector2S p2)
{
	Line2SFromPoints_P(p1, p2, (Line2S*)seg);
	if (p1.x > p2.x) {
		seg->range.x.sup = p1.x;
		seg->range.x.inf = p2.x;
	}
	else {
		seg->range.x.inf = p1.x;
		seg->range.x.sup = p2.x;
	}
	if (p1.y > p2.y) {
		seg->range.y.sup = p1.y;
		seg->range.y.inf = p2.y;
	}
	else {
		seg->range.y.inf = p1.y;
		seg->range.y.sup = p2.y;
	}
}

/// <param name="outPoint">{NaN, NaN} when nested</param>
/// <returns>false if not intersect, true otherwise</returns>
bool IntersectSegment2S(const Segment2S* s1, const Segment2S* s2, Vector2S* outPoint);

struct Rectangle2S {
	float x, y, width, height;
};