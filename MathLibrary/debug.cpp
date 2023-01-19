#include "debug.h"
#include <time.h>

extern "C" {
#include "PolygonShape.h"
#include "CircleShape.h"
#include "LinearEq.h"
#include "GJK.h"
}

#include <fstream>

void DebugEntryPoint(HMODULE hModule, DWORD  ul_reason_for_call, LPVOID lpReserved)
{
    std::ofstream log("MathLibrary.debuglogs.txt");
    Vector2S points1[7]{ {0,1},{0,3},{2,4},{4,3},{4,2},{3,1},{1,0} },
        points2[7]{ {1,5},{5,5},{5,1},{3.1,0},{3,0},{1.5,1.5}, {1,3} };

    Polygon2S p1 = CreatePolygon2S(points1, 7),
        p2 = CreatePolygon2S(points2, 6),
        ps[2]{ p1,p2 };
    
    log << "\tBefore \"OptimizedConvexPolygonSet2S\"\n";
    for (size_t i = 0; i < p1->count; ++i)
        log << "{ " << p1->points[i].x << ", " << p1->points[i].y << " },\n";
    log << '\n';
    for (size_t i = 0; i < p2->count; ++i)
        log << "{ " << p2->points[i].x << ", " << p2->points[i].y << " },\n";
    log << '\n';

    auto pset = CreateOptimizedPolygonSet2S(&ps[0], 2);
    
    log << "\tAfter \"OptimizedConvexPolygonSet2S\"\n";
    for (size_t j = 0; j < 2; ++j)
    {
        for (size_t i = 0; i < pset->polygons[j]->count; ++i)
            log << "{ " << pset->polygons[j]->points[i].x << ", " << pset->polygons[j]->points[i].y << " },\n";
        log << '\n';
    }

    log.close();
    return;
}