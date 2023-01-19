#include "Utils.h"
#include <math.h>
#include <assert.h>

int __cmpf(const void* vpel1, const void* vpel2) {
	float el1 = *(float*)vpel1, el2 = *(float*)vpel2;
	return (el1 > el2) - (el1 < el2);
}

const _CoreCrtNonSecureSearchSortCompareFunction cmpf = __cmpf;

void memshift(void* data, int shift, size_t size)
{
	assert(size > abs(shift) && "shift must be less than size");
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
