#pragma once
#include <Windows.h>
#include "Vectors.h"
// ax + by + c = 0
typedef struct _Line2S {
	float a, b, c;
} Line2S;

typedef struct _Range2S {
	float infX, infY, supX, supY;
} Range2S;

typedef struct _Segment2S {
	Line2S line;
	Range2S range;
} Segment2S;

void Line2SFromPoints_P(Vector2S p1, Vector2S p2, Line2S* out);
Line2S Line2SFromPoints(Vector2S p1, Vector2S p2);

void Line2SFromDir_P(Vector2S direction, Vector2S point, Line2S* out);
Line2S Line2SFromDir(Vector2S direction, Vector2S point);

float CalcXLine2S(Line2S* line, float y);
float CalcYLine2S(Line2S* line, float x);

/// <param name="outPoint">{NaN, NaN} when nested</param>
/// <returns>false if not intersect, true otherwise</returns>
BOOLEAN IntersectLine2S(const Line2S* l1, const Line2S* l2, Vector2S* outPoint);

void ConstructSegment2S(Segment2S* seg, Vector2S p1, Vector2S p2);

/// <param name="outPoint">{NaN, NaN} when nested</param>
/// <returns>false if not intersect, true otherwise</returns>
BOOLEAN IntersectSegment2S(const Segment2S* s1, const Segment2S* s2, Vector2S* outPoint);
