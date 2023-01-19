#ifndef _DEBUG
#define NDEBUG
#define SUPPRESS_TESTS
#endif 

#include <Windows.h>
#include "NoiseGenFunctions.h"

extern "C" {
#include "GJK.h"
#include "PolygonShape.h"
#include "CircleShape.h"
#include "Vectors.h"
}

#ifdef _DEBUG
#include "debug.h"
#endif

BOOL APIENTRY DllMain(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
#ifndef SUPPRESS_TESTS
    DebugEntryPoint(hModule, ul_reason_for_call, lpReserved);
#endif	
    return TRUE;
}
