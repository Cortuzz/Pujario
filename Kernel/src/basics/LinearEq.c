#include "MathBasics.h"

bool IntersectLine2S(const Line2S* l1, const Line2S* l2, Vector2S* outPoint)
{
	outPoint->x = (l1->b * l2->c - l2->b * l1->c) / (l2->b * l1->a - l1->b * l2->a);

	if (isinf(outPoint->x)) return false;
	if (isnan(outPoint->x)) {
		outPoint->y = NAN;
		return true;
	}
	outPoint->y = -(l1->a * outPoint->x + l1->c) / l1->b;
	return true;
}

bool IntersectSegment2S(const Segment2S* s1, const Segment2S* s2, Vector2S* outPoint)
{
	outPoint->x = (s1->line.b * s2->line.c - s2->line.b * s1->line.c) /
		(s2->line.b * s1->line.a - s1->line.b * s2->line.a);

	if (isinf(outPoint->x)) return false;
	if (isnan(outPoint->x)) {
		outPoint->y = NAN;
		return
			s1->range.x.inf <= s2->range.x.inf && s1->range.x.sup >= s2->range.x.sup &&
			s1->range.y.inf <= s2->range.y.inf && s1->range.y.sup >= s2->range.y.sup
			||
			s1->range.x.inf > s2->range.x.inf && s1->range.x.sup < s2->range.x.sup&&
			s1->range.y.inf > s2->range.y.inf && s1->range.y.sup < s2->range.y.sup;
	}

	if (outPoint->x > s1->range.x.sup || outPoint->x < s1->range.x.inf
		|| outPoint->x > s2->range.x.sup || outPoint->x < s2->range.x.inf)
		return false;

	outPoint->y = -((s1->line.a - s2->line.a) * outPoint->x - s2->line.c + s1->line.c)
		/ (s1->line.b - s2->line.b);
	if (outPoint->y > s1->range.y.sup || outPoint->y < s1->range.y.inf
		|| outPoint->y > s2->range.y.sup || outPoint->y < s2->range.y.inf)
		return false;

	return true;
}
