#include "NoiseGenFunctions.hpp"

extern "C" {
#include "Collisions.h"
#include "MathBasics.h"
#include "algorithms/SAT.h"
}

#if defined(_WINDLL) || defined(_USRDLL)      

#ifdef _WIN32
#include <Windows.h>

bool APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
	switch (ul_reason_for_call)
	{
	case DLL_PROCESS_ATTACH:
	case DLL_THREAD_ATTACH:
	case DLL_THREAD_DETACH:
	case DLL_PROCESS_DETACH:
		break;
	}

	return true;
}
#endif

#else
#include "debug.hpp"

int main(const char* argv[], int argc)
{
	DebugEntryPoint(argv, argc);
	return 0;
}
#endif