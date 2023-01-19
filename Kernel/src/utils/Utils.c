#include "Utils.h"
#include <math.h>
#include <memory.h>
#include <assert.h>

static int s_cmpf(const void* vpel1, const void* vpel2) {
	float el1 = *(float*)vpel1, el2 = *(float*)vpel2;
	return (el1 > el2) - (el1 < el2);
}

const _CoreCrtNonSecureSearchSortCompareFunction cmpf = s_cmpf;

void memshift(void* data, int shift, size_t size) {
	assert(size > abs(shift) && "Shift must be less than size");
	char* bytes = (char*)data;
	if (shift > 0) {
		size_t i = size - shift;
		do {
			bytes[--i + shift] = bytes[i];
		} while (i != 0);
	}
	else if (shift < 0) {
		for (size_t i = -shift; i < size; ++i)
			bytes[i + shift] = bytes[i];
	}
}

void cirmemshift(void* data, int shift, size_t size) {
	if (!shift) return;
	bool isPositive = shift > 0;
	shift = (isPositive ? shift : -shift) % size;
	if (shift > size / 2) shift = (int)(size - shift), isPositive = !isPositive;
	size -= shift;
	char* bytes = (char*)data;
	char* buffer = (char*)malloc(shift);
	if (isPositive) {
		memcpy(buffer, bytes + size, shift);
		for (size_t i = size - 1; i != SIZE_MAX; --i)  bytes[i + shift] = bytes[i];
		memcpy(bytes, buffer, shift);
	}
	else {
		memcpy(buffer, bytes, shift);
		for (size_t i = 0; i < size; ++i) bytes[i] = bytes[i + shift];
		memcpy(bytes + size, buffer, shift);
	}

	free(buffer);
}

void EvaluateTransform2S(const struct Transform2S* transform, struct EvaluatedTransform2S* out) {
	out->pivot = transform->position;
	out->rotation.cos = cosf(transform->rotation);
	out->rotation.sin = sinf(transform->rotation);
	out->rotationArm.x = -transform->origin.x * transform->scale.x;
	out->rotationArm.y = -transform->origin.y * transform->scale.y;
	out->scale = transform->scale;
}

#define STACK_DEPTH 32

static inline void s_SortClockwisePoints2S(Vector2S* points, unsigned count) {
	struct {
		int beg, end;
	} frames[STACK_DEPTH], range;
	int i, j;
	unsigned char framesCount = 1;
	Vector2S tempV, pivot;

	frames[0].beg = 0; frames[0].end = count - 1;

	while (framesCount)
	{
		range = frames[--framesCount];
		i = range.beg, j = range.end;
		pivot = points[(i + j) / 2];

		while (i <= j) {
			while (Cross2S(pivot, points[i]) > 0) ++i;
			while (Cross2S(points[j], pivot) > 0) --j;

			if (i > j) break;
			tempV = points[i];
			points[i++] = points[j];
			points[j--] = tempV;
		}

		if (i < range.end)
			frames[framesCount].beg = i, frames[framesCount++].end = range.end;
		if (j > range.beg)
			frames[framesCount].beg = range.beg, frames[framesCount++].end = j;
	}
}

void SortClockwiseOriginPoints2S(Vector2S* points, unsigned count, Vector2S origin) {
	for (unsigned i = 0; i < count; ++i) SubstractV2S(points + i, origin);
	s_SortClockwisePoints2S(points, count);
	for (unsigned i = 0; i < count; ++i) AddV2S(points + i, origin);
}

void SortClockwisePoints2S(Vector2S* points, unsigned count) {
	s_SortClockwisePoints2S(points, count);
}