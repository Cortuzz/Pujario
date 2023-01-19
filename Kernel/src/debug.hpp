#pragma once
extern "C" {
#include "Collisions.h"
#include "Shapes/PolygonShape.h"
#include "Shapes/Polygon/PolygonExtensionFields.h"
#include "algorithms/SAT.h"
}

#define COUNT 100
#define TESTS_COUNT 2000000
#include <chrono>
#include <iostream>

static std::ostream& operator<<(std::ostream& stream, const struct BasePolygon2S& pol) {
	for (size_t i = 0; i < pol.count; ++i)
		stream << '{' << pol.points[i].x << ", " << pol.points[i].y << "}\n";
	return stream;
}

static void DebugEntryPoint(const char* argv[], int argc)
{
	std::chrono::high_resolution_clock clock;
	std::chrono::steady_clock::time_point t;
	std::chrono::milliseconds d1, d2;
	/*struct Transform2S tr = {
		.position = {5, 5},
		.scale = {1, 1},
		.origin = {0, 0},
		.rotation = 17.069
	};

	int count = 4;
	Vector2S* points = new Vector2S[count]{
		{0, 2},
		{2, 1},
		{2, 2},
		{1.5f, 3}
	};

	Vector2S* points1 = new Vector2S[count]{
		{3.5, 3.75},
		{3, 6},
		{2.2, 5.3},
		{1.75, 4.25}
	};

	Polygon2S pol = CreatePolygon2S(points, count);
	OrderPointsPolygon2S((BasePolygon2S*)pol);
	Polygon2S pol1 = CreatePolygon2S(points1, count);
	OrderPointsPolygon2S((BasePolygon2S*)pol1);

	StretchingPolygon2SShape shape = CreateStretchingPolygon2SShape(((BasePolygon2S*)pol1)->points, count);

	struct EvaluatedTransform2S evtr;
	EvaluateTransform2S(&tr, &evtr);

	shape->setTransform(shape, &evtr);

	BasePolygon2S hull = { new Vector2S[100], 0 };
	MergeToConvexHull((BasePolygon2S*)pol1, (BasePolygon2S*)shape->primitive, hull.points, &hull.count);

	std::cout << *(struct BasePolygon2S*)shape->primitive
		<< "\tStretched:\n" << GetStretchedPolygon(shape)
		<< "\tMerged hull:\n" << hull;*/

	Vector2S points[COUNT]
	{
		{1, 0},
		{1, -1},
		{1, 1},
		{-1, 1},
		{-1, -1},
		{-1, 0},
		{0, 1},
		{0, -1},
	}, hullBuffer[COUNT];
	//for (unsigned i = 0; i < COUNT; ++i)
	//	points[i] = { (float)rand() - RAND_MAX / 2, (float)rand() - RAND_MAX / 2 };

	Polygon2SShape shape1 = Polygon2SShape_Create(new Vector2S[4]{
			{-.5f, -1},
			{0,1.5f},
			{1, 1},
			{1.5f, 0}
		}, 4);
	Polygon2SShape shape2 = Polygon2SShape_Create(new Vector2S[4]{
			{-.5f, 10},
			{0, 15},
			{-.5f, 15},
			{-5, 12}
			/*{-.5f, -1},
			{0,1.5f},
			{1, 1},
			{1.5f, 0}*/
		}, 4);


	Polygon2SShape_Delete(shape1), Polygon2SShape_Delete(shape2);
	size_t sm1, sm2, pointsCount = 4;
	Polygon2S pol1, pol2;
	for (size_t j = 0; j < COUNT; ++j, ++pointsCount)
	{
		for (unsigned i = 0; i < pointsCount; ++i)
			points[i] = { (float)rand() - RAND_MAX / 2, (float)rand() - RAND_MAX / 2 };
		pol1 = CreateConvexHull2S(points, pointsCount);

		for (unsigned i = 0; i < pointsCount; ++i)
			points[i] = { (float)rand() - RAND_MAX / 2, (float)rand() - RAND_MAX / 2 };
		pol2 = CreateConvexHull2S(points, pointsCount);

		shape1 = Polygon2SShape_CreateFromPolygon((BasePolygon2S*)pol1);
		shape2 = Polygon2SShape_CreateFromPolygon((BasePolygon2S*)pol2);

		std::cout << "Tests count - " << TESTS_COUNT << ". Points count - " << pointsCount << std::endl;

		sm1 = 0;
		t = clock.now();
		for (size_t i = 0; i < TESTS_COUNT; ++i) {
			sm1 += GJK((Shape)shape1, (Shape)shape2);
		}
		std::cout << "GJK - " << (d1 = std::chrono::duration_cast<std::chrono::milliseconds>(clock.now() - t)) << ", " << sm1 << std::endl;

		sm2 = 0;
		t = clock.now();
		for (size_t i = 0; i < TESTS_COUNT; ++i) {
			sm2 += SAT(pol1, pol2);
		}
		std::cout << "SAT - " << (d2 = std::chrono::duration_cast<std::chrono::milliseconds>(clock.now() - t)) << ", " << sm2 << std::endl;

		if (d1 < d2)
			std::cout << "\tGJK better, difference - " << d2 - d1 << std::endl;
		else 
			std::cout << "\tSAT better, difference - " << d1 - d2 << std::endl;

		std::cout << "=============================================" << std::endl;

		if (sm1 != sm2)
		{
			std::cout << *(struct BasePolygon2S*)pol1 << "\n" << *(struct BasePolygon2S*)pol2;
			std::cout << std::endl;
		}

		Polygon2S_Delete(pol1), Polygon2S_Delete(pol2);
		Polygon2SShape_Delete(shape1), Polygon2SShape_Delete(shape2);
	}

	struct Pl2SView {
		Vector2S* points;
		unsigned count;
		_Polygon2SExtensionFields* extensions;
	};



	return;
}