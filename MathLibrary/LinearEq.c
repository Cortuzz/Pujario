#include "LinearEq.h"
#include <math.h>

void Line2SFromPoints_P(Vector2S p1, Vector2S p2, Line2S* out) {
	out->a = p2.y - p1.y;
	out->b = p1.x - p2.x;
	out->c = -(p1.x * out->a + p1.y * out->b);
}

Line2S Line2SFromPoints(Vector2S p1, Vector2S p2) {
	Line2S res = {
		p2.y - p1.y,
		p1.x - p2.x,
		-(p1.x * res.a + p1.y * res.b)
	};
	return res;
}

void Line2SFromDir_P(Vector2S direction, Vector2S point, Line2S* out) {
	out->a = 1 / direction.x;
	out->b = -1 / direction.y;
	out->c = -(point.y * out->b + point.x * out->a);
}

Line2S Line2SFromDir(Vector2S direction, Vector2S point) {
	Line2S res = {
		1 / direction.x,
		-1 / direction.y,
		-(point.y * res.b + point.x * res.a)
	};
	return res;
}

BOOLEAN IntersectLine2S(const Line2S* l1, const Line2S* l2, Vector2S* outPoint)
{
	outPoint->x = (l1->b * l2->c - l2->b * l1->c) / (l2->b * l1->a - l1->b * l2->a);

	if (isinf(outPoint->x)) return FALSE;
	if (isnan(outPoint->x)) {
		outPoint->y = NAN;
		return TRUE;
	}
	outPoint->y = -(l1->a * outPoint->x + l1->c) / l1->b;
	return TRUE;
}

BOOLEAN IntersectSegment2S(const Segment2S* s1, const Segment2S* s2, Vector2S* outPoint) 
{
	outPoint->x = (s1->line.b * s2->line.c - s2->line.b * s1->line.c) /
		(s2->line.b * s1->line.a - s1->line.b * s2->line.a);

	if (isinf(outPoint->x)) return FALSE;
	if (isnan(outPoint->x)) {
		outPoint->y = NAN;
		return
			s1->range.infX <= s2->range.infX && s1->range.supX >= s2->range.supX &&
			s1->range.infY <= s2->range.infY && s1->range.supY >= s2->range.supY
			||
			s1->range.infX > s2->range.infX && s1->range.supX < s2->range.supX &&
			s1->range.infY > s2->range.infY && s1->range.supY < s2->range.supY;
	}

	if (outPoint->x > s1->range.supX || outPoint->x < s1->range.infX
		|| outPoint->x > s2->range.supX || outPoint->x < s2->range.infX) 
		return FALSE;

	outPoint->y = -((s1->line.a - s2->line.a) * outPoint->x - s2->line.c + s1->line.c)
		/ (s1->line.b - s2->line.b);
	if (outPoint->y > s1->range.supY || outPoint->y < s1->range.infY
		|| outPoint->y > s2->range.supY || outPoint->y < s2->range.infY)
		return FALSE;

	return TRUE;
}

void ConstructSegment2S(Segment2S* seg, Vector2S p1, Vector2S p2) 
{
	Line2SFromPoints_P(p1, p2, seg);
	if (p1.x > p2.x) {
		seg->range.supX = p1.x;
		seg->range.infX = p2.x;
	}
	else {
		seg->range.infX = p1.x;
		seg->range.supX = p2.x;
	}
	if (p1.y > p2.y) {
		seg->range.supY = p1.y;
		seg->range.infY = p2.y;
	}
	else {
		seg->range.infY = p1.y;
		seg->range.supY = p2.y;
	}
}

float CalcXLine2S(Line2S* line, float y) { return -(line->b * y + line->c) / line->a; }
float CalcYLine2S(Line2S* line, float x) { return -(line->a * x + line->c) / line->b; }