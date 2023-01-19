#pragma once 
#include <stdlib.h>

const _CoreCrtNonSecureSearchSortCompareFunction cmpf; 

// shifts data in buffer of "size" in bytes
void memshift(void* data, int shift, size_t size);